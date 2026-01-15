import { useState } from "react";
// Zorg dat het pad klopt (2 mappen omhoog)
import { authFetch } from "../../apiClient";

function IdUser() {
    const [id, setId] = useState("");
    const [user, setUser] = useState(null);
    const [error, setError] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        // Reset de state voordat we gaan zoeken
        setUser(null);
        setError("");

        try {
           

            // Roep authFetch aan. 
            // Die doet de fetch, checkt errors en doet de .json() conversie.
            const data = await authFetch(`/Gebruiker/${id}`, {
                method: "GET",
            });

            // We hebben direct de data, dus we kunnen het opslaan.
            // (De regels met 'if (!response.ok)' en 'await response.json()' zijn WEG)
            setUser(data);

        } catch (err) {
            // Als authFetch faalt (bijv. 404 of 401), komt hij hier.
            console.error(err);
            setError(err.message);
        }
    };

    return (
        <div>
            <h2>Zoek gebruiker op ID</h2>
            <form onSubmit={handleSubmit}>
                <input
                    type="number"
                    value={id}
                    onChange={(e) => setId(e.target.value)}
                    placeholder="Voer Gebruiker ID in"
                    required
                />
                <button type="submit">Zoek</button>
            </form>

            {error && <p style={{ color: "red" }}>{error}</p>}

            {user && (
                <table border="1" cellPadding="5" style={{ marginTop: "20px" }}>
                    <thead>
                    <tr>
                        <th>Gebruikers ID</th>
                        <th>Gebruikersnaam</th>
                        <th>Emailadres</th>
                        <th>Rol</th>
                    </tr>
                    </thead>
                    <tbody>
                    <tr>
                        <td>{user.gebruikerId}</td>
                        <td>{user.naam}</td>
                        <td>{user.email}</td>
                        <td>{user.rol}</td>
                    </tr>
                    </tbody>
                </table>
            )}
        </div>
    );
}

export default IdUser;