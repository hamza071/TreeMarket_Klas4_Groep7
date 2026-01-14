import { useEffect } from "react";
import { API_URL } from '../DeployLocal';

export default function Logout() {
    useEffect(() => {
        async function doLogout() {
            try {
                // (optioneel) server-side logout, alleen laten staan als je endpoint echt bestaat
                await fetch("/api/auth/logout", {
                    method: "POST",
                    credentials: "include",
                });
            } catch (err) {
                console.error("Server-side logout mislukt (optioneel):", err);
            }

            // Client-side: alles wat met inloggen te maken heeft verwijderen
            localStorage.removeItem("token");
            localStorage.removeItem("role");

            // (optioneel) event voor listeners zoals App.jsx
            window.dispatchEvent(new Event("app-auth-changed"));

            // BELANGRIJK: harde redirect zodat App opnieuw wordt geladen
            // en direct de anonieme navigatie (zonder Veiling) laat zien
            window.location.href = "/home"; // of "/auth" als je liever naar login gaat
        }

        doLogout();
    }, []);

    return (
        <div className="page">
            <h1>Uitloggen...</h1>
            <p>U wordt zo doorgestuurd naar de homepagina.</p>
        </div>
    );
}
