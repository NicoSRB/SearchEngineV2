#!/bin/bash
set -e

echo "[1/7] Creating namespace..."
kubectl apply -f k8s/namespace.yaml

echo "[2/7] Installing Helm dependencies (Redis, Mongo, Loki)..."

helm upgrade --install mongo bitnami/mongodb -n searchengine
helm upgrade --install loki grafana/loki-stack -n searchengine
helm upgrade --install postgres bitnami/postgresql --set auth.database=indexerdb --set auth.username=postgres -n searchengine
helm upgrade --install grafana grafana/grafana -n searchengine

#kubectl create secret docker-registry dockerhub-secret --docker-username=broloes2 --docker-password=dckr_pat_CIEn_VAI5ncWUd-gvQcLiaWgDdk --docker-email=broloes2@gmail.com -n searchengine

echo "[3/7] Creating Persistent Volume Claims..."
kubectl apply -f k8s/indexer/pvc.yaml
kubectl run --namespace searchengine redis-client --restart="Never" --env REDIS_PASSWORD=$REDIS_PASSWORD --image docker.io/bitnami/redis:8.0.1-debian-12-r2 --command -- sleep infinity

echo " [4/7] Deploying microservices..."
kubectl apply -f k8s/ --recursive

echo " [5/7] Waiting for Mongo & Redis to be ready..."
kubectl rollout status statefulset/mongodb -n searchengine
kubectl rollout status statefulset/redis-master -n searchengine

echo "[6/7] Running indexer job..."
kubectl apply -f k8s/indexer/job.yaml

echo "✅ [7/7] Deployment complete!"
