// frontend/src/api-tests/NieuwProductTest.jsx
import { useState } from "react";

function NieuwProductTest() {
    const [formData, setFormData] = useState({
        foto: "",
        artikelkenmerken: "",
        hoeveelheid: 0,
        minimumPrijs: 0,
        dagdatum: new Date().toISOString().split("T")[0],
        leverancierID: 1, // zorg dat er leverancier 1 is
    });
    const [message, setMessage] = useState("");

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        fetch("https://localhost:7054/api/Product", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(formData),
        })
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij toevoegen product");
                return res.json();
            })
            .then(() => setMessage("Product toegevoegd!"))
            .catch((err) => setMessage(err.message));
    };

    return (
        <div>
            <h2>Nieuw Product Test (POST)</h2>
            <form onSubmit={handleSubmit}>
                <input name="foto" placeholder="Foto" onChange={handleChange} />
                <input
                    name="artikelkenmerken"
                    placeholder="Artikelkenmerken"
                    onChange={handleChange}
                />
                <input
                    name="hoeveelheid"
                    type="number"
                    placeholder="Hoeveelheid"
                    onChange={handleChange}
                />
                <input
                    name="minimumPrijs"
                    type="number"
                    placeholder="MinimumPrijs"
                    onChange={handleChange}
                />
                <input
                    name="dagdatum"
                    type="date"
                    value={formData.dagdatum}
                    onChange={handleChange}
                />
                <input
                    name="leverancierID"
                    type="number"
                    placeholder="LeverancierID"
                    onChange={handleChange}
                />
                <button type="submit">Toevoegen</button>
            </form>
            <p>{message}</p>
        </div>
    );
}

export default NieuwProductTest;