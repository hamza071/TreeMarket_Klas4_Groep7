import { useState } from "react";

function DeleteIdUser() {
    //De Id wordt verzameld om de juiste gebruiker te tonen.
    const [id, setId] = useState("");
    const [deleted, setDeleted] = useState(false);
    //De gebruiker wordt alleen getoond. Verder niks.
    const [error, setError] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(`${API_URL}/api/Gebruiker/${id}`, {
                method: "DELETE",
            });

            if (response.status === 204) {
                setDeleted(true);
                setError("");
            } else if (response.status === 404) {
                throw new Error("Gebruiker niet gevonden");
            } else {
                throw new Error("Er is iets misgegaan bij het verwijderen");
            }
        } catch (err) {
            setDeleted(false);
            setError(err.message);
        }
    };

    return (
        <div>
            <h2>Gebruiker Verwijderen</h2>
            <form onSubmit={handleSubmit}>
                <input
                    type="number"
                    value={id}
                    onChange={(e) => setId(e.target.value)}
                    placeholder="Voer Gebruiker ID in"
                    required
                />
                <button style={{ backgroundColor: "#C4290A", color:"#F5F5F5" }} type="submit">Verwijder</button>
            </form>

            {/*Wanneer het misgaat of de gebruikerID bestaat niet*/}
            {error && <p style={{ color: "red" }}>{error}</p>}

            {/*Wanneer het gelukt is om de gebruiker te verwijderen*/}
            {deleted && <p style={{ color: "green" }}>Gebruiker succesvol verwijderd!</p>}
        </div>
    );
}

export default DeleteIdUser;