import { useState, useEffect } from 'react';
import '../assets/css/AuctionPage.css';

function AuctionPage({ currentUser }) {
    const [lots, setLots] = useState([]); // kavels van backend
    const [veilingen, setVeilingen] = useState([]); // actieve veilingen
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchLots = async () => {
            const url = 'http://localhost:7054/api/Product/pending'; // HTTP voor lokaal dev
            console.log("Fetching pending kavels vanaf:", url);

            try {
                const response = await fetch(url);

                if (!response.ok) {
                    const text = await response.text();
                    console.error("Fetch error:", text);
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const data = await response.json();
                console.log("Fetched lots:", data);
                setLots(data);

            } catch (err) {
                console.error("Fout bij ophalen kavels:", err);
                setError("Kon pending kavels niet ophalen. Check console voor details.");
            } finally {
                setLoading(false);
            }
        };

        fetchLots();
    }, []);

    if (!currentUser) return <p>Even wachten, gebruiker wordt geladen...</p>;
    if (loading) return <p>Even wachten, kavels worden geladen...</p>;
    if (error) return <p>{error}</p>;

    // Case-insensitive filter op status
    const pendingLots = lots.filter(lot => lot.status?.toLowerCase() === 'pending');

    if (pendingLots.length === 0) return <p>Geen kavels beschikbaar om te publiceren.</p>;

    const createVeiling = async (lot) => {
        try {
            const response = await fetch('http://localhost:7054/api/Veiling/CreateVeiling', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    startPrijs: lot.minPrice + 1,
                    prijsStap: 1,
                    productID: lot.productID,
                    veilingsmeesterID: currentUser.id,
                    timerInSeconden: 3600
                })
            });

            const text = await response.text();
            console.log("Create veiling response:", text);

            if (!response.ok) {
                return alert("Fout bij het aanmaken van veiling: " + text);
            }

            const veiling = JSON.parse(text);
            setVeilingen(prev => [...prev, veiling]);

            // Verwijder de kavel uit pending list
            setLots(prev => prev.filter(l => l.productID !== lot.productID));

        } catch (error) {
            console.error("Fout bij publiceren van kavel:", error);
            alert("Er ging iets mis bij het publiceren van de kavel.");
        }
    };

    return (
        <div className="auction-page">
            <header className="section-header">
                <h1>Te publiceren kavels (veilingmeester)</h1>
                <p>Bekijk de kavels die door leveranciers zijn toegevoegd en publiceer ze.</p>
            </header>

            <div className="auction-grid">
                {pendingLots.map(lot => (
                    <article key={lot.productID} className="auction-card">
                        <h2>{lot.name}</h2>
                        <p>{lot.description}</p>
                        <span>{lot.lots} stuks</span>
                        {lot.image && <img src={lot.image} alt={lot.name} className="auction-card-image" />}
                        <button className="primary-action" onClick={() => createVeiling(lot)}>Start Veiling</button>
                    </article>
                ))}
            </div>

            {veilingen.length > 0 && (
                <section>
                    <h2>Actieve veilingen</h2>
                    <ul>
                        {veilingen.map(v => (
                            <li key={v.veilingID}>
                                Veiling {v.veilingID} - Startprijs: {v.startPrijs} - Timer: {v.timerInSeconden} seconden
                            </li>
                        ))}
                    </ul>
                </section>
            )}
        </div>
    );
}

export default AuctionPage;