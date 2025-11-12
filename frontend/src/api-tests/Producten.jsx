import React, { useEffect, useState } from "react";
import { getData } from "../api/api";
import ProductenTest from "./api-tests/ProductenTest";

export default function Producten() {
    const [producten, setProducten] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getData("Product")
            .then((data) => {
                setProducten(data);
                setLoading(false);
            })
            .catch((err) => console.error(err));
    }, []);

    if (loading) return <p>Producten laden...</p>;

    return (
        <div className="p-6">
            <h1>🌿 Alle producten</h1>
            {producten.map((p) => (
                <div
                    key={p.productId}
                    style={{
                        border: "1px solid #ddd",
                        borderRadius: "8px",
                        padding: "10px",
                        marginBottom: "10px",
                    }}
                >
                    <p><strong>{p.artikelkenmerken}</strong></p>
                    <p>Prijs: €{p.minimumPrijs}</p>
                    <p>Hoeveelheid: {p.hoeveelheid}</p>
                    <p>Leverancier ID: {p.leverancierID}</p>
                </div>
            ))}
        </div>
    );
}