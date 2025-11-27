import { useState, useEffect } from 'react';
import '../assets/css/AuctionPage.css';

function AuctionPage({ currentUser }) { // currentUser bevat veilingsmeesterID
    const [lots, setLots] = useState([]); // kavels van backend
    const [veilingen, setVeilingen] = useState([]); // actieve veilingen
    const [timerInput, setTimerInput] = useState(3600); // standaard timer 1 uur

    // Haal kavels op bij mount
    useEffect(() => {
        fetch('/api/Veiling')
            .then(res => res.json())
            .then(data => setLots(data))
            .catch(err => console.error(err));
    }, []);

    // Filter alleen pending kavels
    const pendingLots = lots.filter(lot => lot.status === 'pending');

    const createVeiling = async (lot) => {
        const response = await fetch('/api/Veiling/CreateVeiling', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                startPrijs: lot.startPrijs,
                prijsStap: lot.prijsStap,
                productID: lot.productID,
                veilingsmeesterID: currentUser.id, // dynamisch
                timerInSeconden: timerInput
            })
        });

        if (response.ok) {
            const veiling = await response.json();
            setVeilingen(prev => [...prev, veiling]); // update UI
        } else {
            console.error('Fout bij het aanmaken van veiling');
        }
    };

    if (pendingLots.length === 0) {
        return <p>Geen kavels beschikbaar om te publiceren.</p>;
    }

    return (
        <div className="auction-page">
            <header className="section-header">
                <h1>Te publiceren kavels (veilingmeester)</h1>
                <p>Bekijk de kavels die door leveranciers zijn toegevoegd en publiceer ze.</p>
            </header>

            <div className="timer-input">
                <label>Timer (seconden): </label>
                <input
                    type="number"
                    value={timerInput}
                    onChange={(e) => setTimerInput(Number(e.target.value))}
                    min={60}
                />
            </div>

            <div className="auction-grid">
                {pendingLots.map(lot => (
                    <article key={lot.code} className="auction-card">
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