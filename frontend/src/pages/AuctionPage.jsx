import { useState, useEffect } from "react";
import "../assets/css/AuctionPage.css";

function AuctionPage({ currentUser }) {
    const [lots, setLots] = useState([]);
    const [veilingen, setVeilingen] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // TODO: Pas aan naar jullie echte endpoint
    const API_URL = "https://localhost:7054/api/Product/pending";

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

                // zorg dat het altijd een array is
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

    // ------------- RENDER STATES -------------

    if (!currentUser) return <p>Gebruiker wordt geladen…</p>;
    if (loading) return <p>Kavels worden geladen…</p>;
    if (error) return <p style={{ color: "red" }}>{error}</p>;

    // pending filter defensive
    const pendingLots = lots.filter(
        (lot) => lot.status?.toLowerCase() === "pending"
    );

    if (pendingLots.length === 0) {
        return <p>Geen kavels beschikbaar om te publiceren.</p>;
    }

    // ------------- VEILING AANMAKEN -------------

    const createVeiling = async (lot) => {
        if (currentUser.rol !== "veilingsmeester") {
            return alert("Alleen de veilingsmeester kan een veiling starten.");
        }

        const payload = {
            startPrijs: (lot.minPrice || 0) + 1,
            prijsStap: 1,
            productID: lot.productID,
            veilingsmeesterID: currentUser.id,
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

            // succes → toevoegen aan actieve veilingen
            setVeilingen((prev) => [...prev, data]);

            // verwijder lot uit pending lijst
            setLots((prev) =>
                prev.filter((l) => l.productID !== lot.productID)
            );
        } catch (err) {
            console.error("Error creating veiling:", err);
            alert("Er ging iets mis bij het aanmaken van de veiling.");
        }
    };

    // ------------- RENDER UI -------------

    return (
        <div className="auction-page">
            <header className="section-header">
                <h1>Te publiceren kavels</h1>
                <p>
                    Bekijk de kavels die door leveranciers zijn toegevoegd
                    {currentUser.rol === "veilingsmeester" && " en publiceer ze."}
                </p>
            </header>

            <div className="auction-grid">
                {pendingLots.map((lot) => (
                    <article key={lot.productID} className="auction-card">
                        <h2>{lot.name}</h2>
                        <p>{lot.description}</p>
                        <span>{lot.lots} stuks</span>

                        {lot.image && (
                            <img
                                src={lot.image}
                                alt={lot.name}
                                className="auction-card-image"
                            />
                        )}

                        {currentUser.rol === "veilingsmeester" && (
                            <button
                                className="primary-action"
                                onClick={() => createVeiling(lot)}
                            >
                                Start veiling
                            </button>
                        )}
                    </article>
                ))}
            </div>

            {veilingen.length > 0 && (
                <section>
                    <h2>Actieve veilingen</h2>
                    <ul>
                        {veilingen.map((v) => (
                            <li key={v.veilingID}>
                                Veiling {v.veilingID} – Startprijs: €{v.startPrijs} –
                                Timer: {v.timerInSeconden} sec
                            </li>
                        ))}
                    </ul>
                </section>
            )}
        </div>
    );
}

export default AuctionPage;