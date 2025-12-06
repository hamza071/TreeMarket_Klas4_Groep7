import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        port: 55125, // frontend poort
        proxy: {
            '/api': {
                target: 'http://localhost:7054', // backend poort
                changeOrigin: true,
                secure: false,
            },
        },
    },
});