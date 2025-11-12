// frontend/src/api-tests/ProductenTest.jsx
import { useEffect, useState } from "react";

function ProductenTest() {
    const [producten, setProducten] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Haal producten op van de backend
    useEffect(() => {
        fetch("https://localhost:7054/api/Product")
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij ophalen producten");
                return res.json();
            })
            .then((data) => {
                setProducten(data);
                setLoading(false);
            })
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    }, []);

    if (loading) return <p>Even geduld, producten laden...</p>;
    if (error) return <p>Fout: {error}</p>;

    return (
        <div>
            <h2>Producten (API Test)</h2>
            {producten.length === 0 ? (
                <p>Geen producten gevonden.</p>
            ) : (
                <ul>
                    {producten.map((p) => (
                        <li key={p.productId}>
                            {p.artikelkenmerken} - {p.minimumPrijs} €
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}

export default ProductenTest;
