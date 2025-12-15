import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "../assets/css/AuctionPage.css";

function AuctionPage() {
    const [lots, setLots] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const navigate = useNavigate();
    const API_URL = "https://localhost:7054/api/Product/vandaag";

    useEffect(() => {
        const fetchLots = async () => {
            try {
                const response = await fetch(API_URL, {
                    method: "GET",
                    headers: { Accept: "application/json" },
                });

                if (!response.ok) {
                    throw new Error(`Server error: ${response.status}`);
                }

                const data = await response.json();
                const list = Array.isArray(data) ? data : [data];
                setLots(list);
            } catch (err) {
                console.error("Error fetching lots:", err);
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchLots();
    }, []);

    if (loading) return <p>Kavels worden geladen…</p>;
    if (error) return <p style={{ color: "red" }}>{error}</p>;

    // Alleen pending kavels tonen
    const pendingLots = lots.filter(
        (lot) => lot.status?.toLowerCase() === "pending" || !lot.status
    );

    if (pendingLots.length === 0) {
        return <p>Geen kavels beschikbaar om te publiceren.</p>;
    }

    const handleStartVeiling = (lot) => {
        // Navigeren naar AuctionDetailPage met de lot data in state
        navigate(`/veiling/${lot.productId}`, { state: { lot } });
    };

    return (
        <div className="auction-page">
            <header className="section-header">
                <h1>Te publiceren kavels</h1>
                <p>Bekijk de kavels die door leveranciers zijn toegevoegd.</p>
            </header>

            <div className="auction-grid">
                {pendingLots.map((lot) => (
                    <article key={lot.productId} className="auction-card">
                        <h2>{lot.naam || "Geen naam"}</h2>
                        <p>{lot.omschrijving || "Geen beschrijving beschikbaar."}</p>
                        <p>{lot.hoeveelheid || 0} stuks</p>
                        {lot.varieteit && <p>Variëteit: {lot.varieteit}</p>}

                        <img
                            src={
                                lot.foto
                                    ? lot.foto.startsWith("http")
                                        ? lot.foto
                                        : `https://localhost:7054${lot.foto}`
                                    : "/images/default.png"
                            }
                            alt={lot.naam || "Productfoto"}
                            className="auction-card-image"
                        />

                        {lot.leverancierNaam && (
                            <p>Leverancier: {lot.leverancierNaam}</p>
                        )}

                        <button
                            className="primary-action"
                            onClick={() => handleStartVeiling(lot)}
                        >
                            Start veiling
                        </button>
                    </article>
                ))}
            </div>
        </div>
    );
}

export default AuctionPage;