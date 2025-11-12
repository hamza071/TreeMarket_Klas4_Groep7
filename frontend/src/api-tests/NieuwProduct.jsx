import React, { useState } from "react";
import { postData } from "../api/api";
import NieuwProductTest from "./api-tests/NieuwProductTest";

export default function NieuwProduct() {
    const [form, setForm] = useState({
        foto: "",
        artikelkenmerken: "",
        hoeveelheid: 1,
        minimumPrijs: 0,
        dagdatum: new Date().toISOString(),
        leverancierID: 1,
    });

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await postData("Product", form);
            alert("✅ Product toegevoegd!");
        } catch (err) {
            alert("❌ Er is iets misgegaan");
            console.error(err);
        }
    };

    return (
        <form onSubmit={handleSubmit} style={{ padding: "20px" }}>
            <h2>Nieuw product toevoegen</h2>

            <input name="foto" placeholder="Foto URL" onChange={handleChange} />
            <input
                name="artikelkenmerken"
                placeholder="Beschrijving"
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
                step="0.01"
                placeholder="Minimumprijs"
                onChange={handleChange}
            />

            <button type="submit">Toevoegen</button>
        </form>
    );
}