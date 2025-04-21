import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api/": {
        target: "https://localhost:5001",  // Should be changed to load balancer later
        changeOrigin: true,
        secure: false, // Disable SSL certificate verification for local dev
        //rewrite: (path) => path.replace(/^\/api\/LoadBalancer/, "/api/LoadBalancer"), // Keep the path as is
      },
    },
  },
});
