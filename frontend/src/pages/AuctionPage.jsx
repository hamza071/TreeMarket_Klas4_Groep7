import { useState, useEffect } from 'react';
import '../assets/css/AuctionPage.css';

function AuctionPage({ currentUser }) {
    const [lots, setLots] = useState([]); // kavels van backend
    const [veilingen, setVeilingen] = useState([]); // actieve veilingen
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchLots = async () => {
            const url = 'https://localhost:7054/api/Product/pending';
            console.log("Fetching pending kavels vanaf:", url);

            try {
                const response = await fetch(url, {
                    method: 'GET',
                    headers: { 'Accept': 'application/json' },
                    mode: 'cors'
                });

                const text = await response.text();
                console.log("Raw response:", text);

                let data;
                try {
                    data = JSON.parse(text);
                } catch (err) {
                    console.warn("Kan JSON niet parsen, response is waarschijnlijk HTML:", err);
                    setError("Kon kavels niet ophalen: server geeft geen JSON terug.");
                    setLoading(false);
                    return;
                }

                if (!response.ok) {
                    console.error("HTTP error:", response.status, data);
                    setError(`Kon kavels niet ophalen: ${response.status}`);
                    setLoading(false);
                    return;
                }

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

    const pendingLots = lots.filter(lot => lot.status?.toLowerCase() === 'pending');
    if (pendingLots.length === 0) return <p>Geen kavels beschikbaar om te publiceren.</p>;

    const createVeiling = async (lot) => {
        if (currentUser.rol !== 'veilingsmeester') {
            return alert("Alleen een veilingsmeester kan een veiling aanmaken.");
        }

        try {
            const response = await fetch('https://localhost:7054/api/Veiling/CreateVeiling', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
                body: JSON.stringify({
                    startPrijs: lot.minPrice + 1,
                    prijsStap: 1,
                    productID: lot.productID,
                    veilingsmeesterID: currentUser.id,
                    timerInSeconden: 3600
                })
            });

            const text = await response.text();
            console.log("Create veiling raw response:", text);

            let veiling;
            try {
                veiling = JSON.parse(text);
            } catch {
                console.warn("Response geen JSON, gebruik text:", text);
                veiling = { message: text };
            }

            if (!response.ok) {
                console.error("Fout bij aanmaken veiling:", veiling);
                return alert("Fout bij het aanmaken van veiling: " + (veiling.message || response.statusText));
            }

            setVeilingen(prev => [...prev, veiling]);
            setLots(prev => prev.filter(l => l.productID !== lot.productID));

        } catch (error) {
            console.error("Fout bij publiceren van kavel:", error);
            alert("Er ging iets mis bij het publiceren van de kavel. Check console voor details.");
        }
    };

    return (
        <div className="auction-page">
            <header className="section-header">
                <h1>Te publiceren kavels</h1>
                <p>Bekijk de kavels die door leveranciers zijn toegevoegd{currentUser.rol === 'veilingsmeester' ? ' en publiceer ze.' : '.'}</p>
            </header>

            <div className="auction-grid">
                {pendingLots.map(lot => (
                    <article key={lot.productID} className="auction-card">
                        <h2>{lot.name}</h2>
                        <p>{lot.description}</p>
                        <span>{lot.lots} stuks</span>
                        {lot.image && <img src={lot.image} alt={lot.name} className="auction-card-image" />}
                        {currentUser.rol === 'veilingsmeester' && (
                            <button className="primary-action" onClick={() => createVeiling(lot)}>Start Veiling</button>
                        )}
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