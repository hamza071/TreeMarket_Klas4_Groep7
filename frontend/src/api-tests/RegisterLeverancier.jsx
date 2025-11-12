import React, { useState } from "react";
import { postData } from "../api/api";
import LeveranceirTest from "./api-tests/LeverancierTest";

export default function RegisterLeverancier() {
    const [leverancier, setLeverancier] = useState({
        naam: "",
        email: "",
        bedrijf: "",
        kvkNummer: "",
        ibanNummer: "",
        wachtwoord: "",
    });

    const handleChange = (e) => {
        setLeverancier({ ...leverancier, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await postData("Leverancier", leverancier);
            alert("✅ Leverancier geregistreerd!");
        } catch (err) {
            alert("❌ Fout bij registratie");
            console.error(err);
        }
    };

    return (
        <form onSubmit={handleSubmit} style={{ padding: "20px" }}>
            <h2>Leverancier registreren</h2>
            <input name="naam" placeholder="Naam" onChange={handleChange} />
            <input name="email" placeholder="E-mail" onChange={handleChange} />
            <input name="bedrijf" placeholder="Bedrijf" onChange={handleChange} />
            <input name="kvkNummer" placeholder="KvK nummer" onChange={handleChange} />
            <input name="ibanNummer" placeholder="IBAN" onChange={handleChange} />
            <input
                name="wachtwoord"
                placeholder="Wachtwoord"
                type="password"
                onChange={handleChange}
            />
            <button type="submit">Registreren</button>
        </form>
    );
}