// frontend/src/DeployLocal.js

// Dit pakt de Vercel-link (via de environment variable), 
// OF als die leeg is (op je eigen PC), pakt hij localhost.
export const API_URL = import.meta.env.VITE_API_URL || '${API_URL}';