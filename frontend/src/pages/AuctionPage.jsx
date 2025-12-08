import { useState, useEffect } from 'react';
import '../assets/css/AuctionPage.css';

function AuctionPage({ currentUser }) {
    const [lots, setLots] = useState([]); // kavels van backend
    const [veilingen, setVeilingen] = useState([]); // actieve veilingen
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchLots = async () => {
            try {
                const response = await fetch('https://localhost:7054/api/Product/pending');
                if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
                const data = await response.json();
                setLots(data);
            } catch (err) {
                console.error("Fout bij ophalen pending kavels:", err);
            } finally {
                setLoading(false);
            }
        };
        fetchLots();
    }, []);

    if (!currentUser) return <p>Even wachten, gebruiker wordt geladen...</p>;
    if (loading) return <p>Even wachten, kavels worden geladen...</p>;

    const pendingLots = lots.filter(lot => lot.status === 'pending');
    if (pendingLots.length === 0) return <p>Geen kavels beschikbaar om te publiceren.</p>;

    const createVeiling = async (lot) => {
        try {
            const response = await fetch('https://localhost:7054/api/Veiling/CreateVeiling', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    startPrijs: lot.minPrice + 1, // bv startprijs iets hoger dan minPrice
                    prijsStap: 1,
                    productID: lot.productID,
                    veilingsmeesterID: currentUser.id,
                    timerInSeconden: 3600
                })
            });

            if (!response.ok) {
                const errorText = await response.text();
                return alert("Fout bij het aanmaken van veiling: " + errorText);
            }

            const veiling = await response.json();
            setVeilingen(prev => [...prev, veiling]);

            // **Verwijder de kavel uit pending list**
            setLots(prev => prev.filter(l => l.productID !== lot.productID));
        } catch (error) {
            console.error("Fout bij het publiceren van kavel:", error);
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
                    <article key={lot.code} className="auction-card">
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