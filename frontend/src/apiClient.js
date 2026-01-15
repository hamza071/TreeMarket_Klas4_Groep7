// Poort 7054 is je backend poort
const BACKEND_URL = "https://localhost:7054";

const BASE_URL = import.meta.env.VITE_API_URL || `${BACKEND_URL}/api`;

export const authFetch = async (endpoint, options = {}) => {
    // Haal token op uit localStorage
    const token = localStorage.getItem("token");

    // Maak standaard headers
    const headers = {
        "Content-Type": "application/json",
        ...options.headers,
    };

    // Voeg Authorization header toe als er een token is
    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    try {
        // Doe het verzoek naar de backend poort 7054
        const response = await fetch(`${BASE_URL}${endpoint}`, {
            ...options,
            headers: headers,
        });

        // Check op 401 Unauthorized
        if (response.status === 401) {
            throw new Error("Je hebt geen rechten of je sessie is verlopen.");
        }

        // Foutafhandeling voor andere statuscodes
        if (!response.ok) {
            const errorData = await response.json().catch(() => null);
            throw new Error(errorData?.message || `Server fout: ${response.status}`);
        }

        // Geef JSON terug tenzij de response leeg is
        if (response.status !== 204) {
            return await response.json();
        }
    } catch (err) {
        // Als de backend niet draait, krijg je deze melding
        console.error("Verbindingsfout:", err);
        throw new Error("Kan de API op poort 7054 niet bereiken. Staat je backend aan?");
    }
};

//test 