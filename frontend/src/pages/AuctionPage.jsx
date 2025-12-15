import { useState, useEffect } from "react";
import "../assets/css/AuctionPage.css";

function AuctionPage() {
    const [lots, setLots] = useState([]);
    const [veilingen, setVeilingen] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

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

    const pendingLots = lots.filter(
        (lot) => lot.status?.toLowerCase() === "pending" || !lot.status
    );

    if (pendingLots.length === 0) {
        return <p>Geen kavels beschikbaar om te publiceren.</p>;
    }

    const createVeiling = async (lot) => {
        const payload = {
            startPrijs: (lot.minimumPrijs || 0) + 1,
            prijsStap: 1,
            productID: lot.productId,
            veilingsmeesterID: 1, // dummy ID, geen auth
            timerInSeconden: 3600,
        };

        try {
            const response = await fetch(
                "https://localhost:7054/api/Veiling/CreateVeiling",
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Accept: "application/json",
                    },
                    body: JSON.stringify(payload),
                }
            );

            const data = await response.json();

            if (!response.ok) {
                console.error("Veiling fout:", data);
                return alert("Kon veiling niet aanmaken.");
            }

            setVeilingen((prev) => [...prev, data]);
            setLots((prev) =>
                prev.filter((l) => l.productId !== lot.productId)
            );
        } catch (err) {
            console.error("Error creating veiling:", err);
            alert("Er ging iets mis bij het aanmaken van de veiling.");
        }
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

                        {lot.foto && (
                            <img
                                src={
                                    lot.foto.startsWith("http")
                                        ? lot.foto
                                        : `https://localhost:7054${lot.foto}`
                                }
                                alt={lot.naam || "Productfoto"}
                                className="auction-card-image"
                            />
                        )}

                        {lot.leverancierNaam && (
                            <p>Leverancier: {lot.leverancierNaam}</p>
                        )}

                        <button
                            className="primary-action"
                            onClick={() => createVeiling(lot)}
                        >
                            Start veiling
                        </button>
                    </article>
                ))}
            </div>

            {veilingen.length > 0 && (
                <section>
                    <h2>Actieve veilingen</h2>
                    <ul>
                        {veilingen.map((v) => (
                            <li key={v.veilingID}>
                                Veiling {v.veilingID} – Startprijs: €{v.startPrijs} – Timer: {v.timerInSeconden} sec
                            </li>
                        ))}
                    </ul>
                </section>
            )}
        </div>
    );
}

export default AuctionPage;