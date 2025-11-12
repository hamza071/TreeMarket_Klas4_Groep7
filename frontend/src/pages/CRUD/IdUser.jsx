import { useEffect, useState } from "react";

function IdUser() {
    //De Id wordt verzameld om de juiste gebruiker te tonen.
    const [id, setId] = useState("");
    //De gebruiker wordt alleen getoond. Verder niks.
    const [user, setUser] = useState(null);
    const [error, setError] = useState("");

    const handleSubmit = (e) => {
        e.preventDefault();
        //De ID staat in ${id} zodat het niet statisch bij een waarde staat.
        fetch(`https://localhost:7054/api/Gebruiker/${id}`, {
            //Voor de zekerheid dat het naar de GET gaat.
            method: "GET",
        })
            // De gebruiker bestaat niet
            .then(res => {
                if (!res.ok) throw new Error("Gebruiker niet gevonden");
                return res.json();
            })
            //Toont de data van de gebruiker
            .then(data => {
                setUser(data);
                setError("");
            })
            //Wanneer het misgaat binnen de programma
            .catch(err => {
                setUser(null);
                setError(err.message);
            });
    };

    return (
        <div>
            <h2>Zoek gebruiker op ID</h2>
            {/*De formulier waar je een waarde van de Gebruiker ID vult*/}
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

            {/*Wanneer het goed gaat, wordt de data getoond*/}
            {user && (
                <table border="1" cellPadding="5">
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