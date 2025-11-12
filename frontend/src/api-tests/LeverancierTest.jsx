// frontend/src/api-tests/LeverancierTest.jsx
import { useState } from "react";

function LeverancierTest() {
    const [formData, setFormData] = useState({
        bedrijf: "",
        kvkNummer: "",
        ibanNummer: "",
        wachtwoord: "",
    });
    const [message, setMessage] = useState("");

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        fetch("https://localhost:7054/api/Gebruiker/Leverancier", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(formData),
        })
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij toevoegen leverancier");
                return res.json();
            })
            .then(() => setMessage("Leverancier toegevoegd!"))
            .catch((err) => setMessage(err.message));
    };

    return (
        <div>
            <h2>Nieuwe Leverancier Test (POST)</h2>
            <form onSubmit={handleSubmit}>
                <input name="bedrijf" placeholder="Bedrijf" onChange={handleChange} />
                <input name="kvkNummer" placeholder="KvK Nummer" onChange={handleChange} />
                <input name="ibanNummer" placeholder="IBAN" onChange={handleChange} />
                <input name="wachtwoord" placeholder="Wachtwoord" onChange={handleChange} />
                <button type="submit">Toevoegen</button>
            </form>
            <p>{message}</p>
        </div>
    );
}

export default LeverancierTest;