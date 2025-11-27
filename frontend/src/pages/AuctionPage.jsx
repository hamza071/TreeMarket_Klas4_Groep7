import { useState, useEffect } from 'react';
import '../assets/css/AuctionPage.css';

function AuctionPage({ currentUser }) {
    const [lots, setLots] = useState([]);
    const [veilingen, setVeilingen] = useState([]);

    // Kavels ophalen van backend
    useEffect(() => {
        fetch('/api/Product')
            .then(res => res.json())
            .then(data => setLots(data))
            .catch(err => console.error(err));
    }, []);

    const pendingLots = lots.filter(lot => lot.status === 'pending');

    const createVeiling = async (lot) => {
        try {
            const response = await fetch('/api/Veiling/CreateVeiling', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    startPrijs: lot.minPrice,
                    prijsStap: 1, // default prijsstap
                    productID: lot.id,
                    veilingsmeesterID: currentUser.id,
                    timerInSeconden: 3600 // standaard, kan later dynamisch
                })
            });

            if (response.ok) {
                const veiling = await response.json();
                setVeilingen(prev => [...prev, veiling]);
                // update frontend status van kavel
                setLots(prev => prev.map(l => l.id === lot.id ? { ...l, status: 'published' } : l));
            } else {
                console.error('Fout bij starten veiling');
            }
        } catch (err) {
            console.error(err);
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
                    <article key={lot.id} className="auction-card">
                        <h2>{lot.name}</h2>
                        <p>{lot.description}</p>
                        <span>{lot.quantity} stuks</span>
                        {lot.image && <img src={lot.image} alt={lot.name} className="auction-card-image" />}
                        <button
                            className="primary-action"
                            onClick={() => createVeiling(lot)}
                        >
                            Start Veiling
                        </button>
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