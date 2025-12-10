const BASE_URL = import.meta.env.VITE_API_URL || "https://localhost:7054/api";

export const authFetch = async (endpoint, options = {}) => {
    // 1. Haal token op (Globaal!)
    const token = localStorage.getItem("token");

    // 2. Maak standaard headers
    const headers = {
        "Content-Type": "application/json",
        ...options.headers, // Voeg eventuele extra headers toe die je meestuurt
    };

    // 3. Als er een token is, plak hem erbij!
    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    // 4. Doe het verzoek
    const response = await fetch(`${BASE_URL}${endpoint}`, {
        ...options, // method: 'POST', body: ...
        headers: headers,
    });

    // 5. Globale Foutafhandeling (Bijv. als token verlopen is)
    if (response.status === 401) {
        // Optioneel: Log de gebruiker automatisch uit
        // localStorage.removeItem("token");
        // window.location.href = "/auth";
        throw new Error("Je bent niet (meer) ingelogd.");
    }

    if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || "Er ging iets mis.");
    }

    // 6. Geef json terug (als er inhoud is)
    if (response.status !== 204) {
        return await response.json();
    }
};//