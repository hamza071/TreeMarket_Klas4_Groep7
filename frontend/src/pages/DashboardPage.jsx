import { useEffect, useState } from 'react';

const AUTO_REMOVE_DELAY = 4000; // 4 seconden na afloop verwijderen

function DashboardPage() {
    const [lotsState, setLotsState] = useState([]);

    // 🚀 Fetch veilingen bij mount
    useEffect(() => {
        const fetchVeilingen = async () => {
            try {
                const response = await fetch('https://localhost:7054/api/Veiling/GetVeilingen', {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                        'Content-Type': 'application/json',
                    },
                });
                const data = await response.json();
                const now = Date.now();

                const lots = data
                    .filter(lot => lot.status === true)
                    .map(lot => {
                        // Correcte timestamp naar milliseconden
                        const startTimestamp = new Date(lot.startTimestamp.split('.')[0] + 'Z').getTime();
                        const timerInSeconden = Number(lot.timerInSeconden);
                        const startPrice = Number(lot.startPrijs ?? 0);
                        const minPrice = Number(lot.minPrijs ?? startPrice);

                        const elapsed = Math.max(0, (now - startTimestamp) / 1000);
                        const remainingTime = Math.max(0, timerInSeconden - elapsed);

                        const progress = Math.min(elapsed / timerInSeconden, 1);
                        const currentPrice =
                            remainingTime > 0
                                ? startPrice - (startPrice - minPrice) * progress
                                : minPrice;

                        const removeAt = remainingTime > 0 ? null : now + AUTO_REMOVE_DELAY;

                        return {
                            ...lot,
                            startTimestamp,
                            timerInSeconden,
                            startPrice,
                            minPrice,
                            closing: Math.ceil(remainingTime),
                            currentPrice,
                            status: remainingTime > 0 ? 'actief' : 'afgesloten',
                            removeAt,
                        };
                    })
                    .sort((a, b) => b.veilingID - a.veilingID);

                setLotsState(lots);
            } catch (err) {
                console.error('Fout bij ophalen van veilingen:', err);
            }
        };

        fetchVeilingen();
    }, []);

    // ⏱ Timer update (Zorgt dat elke PC exact gelijk loopt)
    useEffect(() => {
        const interval = setInterval(() => {
            const now = Date.now(); // Huidige tijd op de computer van de bezoeker

            setLotsState(prevLots =>
                prevLots
                    .map(lot => {
                        // Als data mist, doe niets
                        if (!lot.startTimestamp || !lot.timerInSeconden) return lot;

                        // 1. Bereken de harde eindtijd (Starttijd + Duur in ms)
                        // Dit punt in de tijd is voor iedereen op de wereld gelijk.
                        const startTimeMs = new Date(lot.startTimestamp).getTime();
                        const durationMs = lot.timerInSeconden * 1000;
                        const endTimeMs = startTimeMs + durationMs;

                        // 2. Bereken hoeveel tijd er nog over is (Eindtijd - NU)
                        const msLeft = endTimeMs - now;
                        const secondsLeft = Math.ceil(msLeft / 1000);

                        // 3. Bereken de prijs (Lineair)
                        // Verstreken tijd in seconden (mag niet kleiner dan 0 zijn)
                        const elapsedSeconds = Math.max(0, (now - startTimeMs) / 1000);
                        const progress = Math.min(elapsedSeconds / lot.timerInSeconden, 1);

                        const currentPrice =
                            secondsLeft > 0
                                ? lot.startPrice - (lot.startPrice - lot.minPrice) * progress
                                : lot.minPrice;

                        // 4. Logica voor verwijderen (zoals je het al had)
                        const removeAt = secondsLeft > 0 ? null : lot.removeAt ?? now + AUTO_REMOVE_DELAY;

                        return {
                            ...lot,
                            closing: Math.max(0, secondsLeft), // Toon nooit -1
                            currentPrice,
                            status: secondsLeft > 0 ? 'actief' : 'afgesloten',
                            removeAt,
                        };
                    })
                    // Je originele filter logica blijft behouden
                    .filter(lot => {
                        if (lot.removeAt && now >= lot.removeAt) {
                            // Je DELETE logica
                            fetch(`https://localhost:7054/api/Veiling/DeleteVeiling/${lot.veilingID}`, {
                                method: 'DELETE',
                                headers: {
                                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                                },
                            }).catch(err => console.error('Fout bij verwijderen:', err));

                            return false;
                        }
                        return true;
                    })
            );
        }, 100); // Tip: Zet deze op 100ms of 200ms. Dan verspringt de seconde precies op tijd.
//
        return () => clearInterval(interval);
    }, []);

    const featuredLot = lotsState[0];
    const featuredTime = featuredLot?.closing ?? 0;

    return (
        <div className="dashboard-page">
            <section className="dashboard-hero">
                <div className="hero-copy">
                    <span className="eyebrow">TREE MARKET</span>
                    <h1>De toekomst van bloemen en plantenveilingen</h1>
                    <p>
                        Digitale Veilingklok 2025 brengt kopers en kwekers samen in een moderne, efficiënte online veilingomgeving.
                    </p>
                </div>

                {featuredLot && (
                    <article className="featured-card">
                        <img
                            src={featuredLot.foto?.startsWith('http') ? featuredLot.foto : `https://localhost:7054${featuredLot.foto}`}
                            alt={featuredLot.productNaam || 'Productfoto'}
                            style={{
                                maxWidth: '600px',
                                maxHeight: '400px',
                                objectFit: 'cover',
                                width: '100%',
                                height: '100%',
                                borderRadius: '16px',
                                display: 'block',
                                overflow: 'hidden',
                                marginBottom: '1rem',
                            }}
                        />
                        <div className="featured-body">
                            <div className="featured-meta" aria-live="polite">
                                <span className="badge badge-live">
                                    {featuredTime > 0 ? `${featuredTime}s` : 'Afgesloten'}
                                </span>
                                <span className="lot-number">#{featuredLot.veilingID}</span>
                            </div>
                            <h2>{featuredLot.productNaam}</h2>
                            <p className="featured-quantity">{featuredLot.lots ?? 1} stuks</p>
                            <div className="featured-footer">
                                <span className="featured-price">€{featuredLot.currentPrice?.toFixed(2)}</span>
                                <button type="button" className="secondary-action" disabled={featuredTime <= 0}>
                                    Bieden
                                </button>
                            </div>
                        </div>
                    </article>
                )}
            </section>

            <section className="dashboard-table">
                <h3>Komende kavels</h3>
                <div className="table-wrapper" role="region" aria-live="polite">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Kavel</th>
                                <th>Naam</th>
                                <th>Specificaties</th>
                                <th>Aantal</th>
                                <th>Huidige prijs (€)</th>
                                <th>Sluiting</th>
                            </tr>
                        </thead>
                        <tbody>
                            {lotsState.map(lot => (
                                <tr key={lot.veilingID}>
                                    <td>{lot.veilingID}</td>
                                    <td>{lot.productNaam}</td>
                                    <td>{lot.specs ?? '-'}</td>
                                    <td>{lot.lots ?? 1}</td>
                                    <td>€{lot.currentPrice?.toFixed(2)}</td>
                                    <td>{lot.closing > 0 ? `${lot.closing}s` : 'Afgesloten'}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </section>
        </div>
    );
}

export default DashboardPage;