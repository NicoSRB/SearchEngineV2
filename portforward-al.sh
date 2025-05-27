#!/bin/bash

NAMESPACE="searchengine"

echo "Starting port-forwards for your services in namespace: $NAMESPACE..."

# Frontend: localhost:8080
kubectl port-forward svc/frontend 8080:80 -n $NAMESPACE > /dev/null 2>&1 &
echo "✅ Frontend → http://localhost:8080"

# Loadbalancer: localhost:7122
kubectl port-forward svc/loadbalancer 7122:8080 -n $NAMESPACE > /dev/null 2>&1 &
echo "✅ Loadbalancer → http://localhost:7122/api/search"

# Grafana: localhost:3000
kubectl port-forward svc/grafana 3000:3000 -n $NAMESPACE > /dev/null 2>&1 &
echo "✅ Grafana → http://localhost:3000"

# Termnet API: localhost:5000
kubectl port-forward svc/termnetapi 5000:80 -n $NAMESPACE > /dev/null 2>&1 &
echo "✅ Termnet API → http://localhost:5000/termnet"

# Redis (optional for CLI/dev): localhost:6379
kubectl port-forward svc/redis-client 6379:6379 -n $NAMESPACE > /dev/null 2>&1 &
echo "✅ Redis (CLI) → localhost:6379"

echo ""
echo "All port-forwards are active. Press Ctrl+C to stop them."
wait
