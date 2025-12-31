import { useEffect, useState } from 'react';

function DashboardPage() {
    const [lotsState, setLotsState] = useState([]);

    // Fetch veilingen van backend
    useEffect(() => {
        const fetchVeilingen = async () => {
            try {
                const response = await fetch('https://localhost:7054/api/Veiling/GetVeilingen', {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`,
                        'Content-Type': 'application/json',
                    },
                });
                const data = await response.json();

                // Alleen actieve veilingen
                const activeLots = data
                    .filter(lot => lot.status === true)
                    .map(lot => ({
                        ...lot,
                        startTimestamp: new Date(lot.startTimestamp).getTime() || Date.now(),
                        closingTimestamp: new Date(lot.startTimestamp).getTime() + (lot.timerInSeconden * 1000) || Date.now() + (lot.timerInSeconden * 1000),
                        currentPrice: lot.startPrijs ?? 0,
                        minPrice: lot.startPrijs ?? 0,
                        closing: Math.max(0, Math.ceil(((new Date(lot.startTimestamp).getTime() + (lot.timerInSeconden * 1000)) - Date.now()) / 1000))
                    }))
                    .sort((a, b) => b.veilingID - a.veilingID); // nieuwste eerst

                setLotsState(activeLots);
            } catch (err) {
                console.error('Fout bij ophalen van veilingen:', err);
            }
        };

        fetchVeilingen();
    }, []);

    // Timer, prijsupdate en automatische verwijdering van afgeronde veilingen
    useEffect(() => {
        const interval = setInterval(() => {
            const now = Date.now();
            setLotsState(prevLots =>
                prevLots.map(lot => {
                    const remainingTime = Math.max(0, Math.ceil((lot.closingTimestamp - now) / 1000));
                    const elapsed = lot.timerInSeconden - remainingTime;
                    const priceRange = lot.startPrijs - lot.minPrice;
                    const currentPrice =
                        remainingTime > 0
                            ? Math.max(lot.minPrice, lot.startPrijs - (priceRange * (elapsed / lot.timerInSeconden)))
                            : lot.minPrice;

                    return {
                        ...lot,
                        closing: remainingTime,
                        currentPrice,
                        // ✅ geen verwijdering, lot blijft staan ook bij closing === 0
                    };
                })
            );
        }, 1000);

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
                        Digitale Veilingklok 2025 brengt kopers en kwekers samen in een moderne, efficiënte online
                        veilingomgeving.
                    </p>
                </div>

                {featuredLot && (
                    <article className="featured-card">
                        <img
                            src={featuredLot.foto?.startsWith('http') ? featuredLot.foto : `https://localhost:7054${featuredLot.foto}`}
                            alt={featuredLot.productNaam || 'Productfoto'}
                            style={{
                                maxWidth: '600px',      // maximale breedte
                                maxHeight: '400px',     // maximale hoogte
                                objectFit: 'cover',     // crop en schaal
                                width: '100%',
                                height: '100%',
                                borderRadius: '16px',
                                display: 'block',
                                overflow: 'hidden',
                                marginBottom: '1rem'
                            }}
                        />
                        <div className="featured-body">
                            <div className="featured-meta" aria-live="polite">
                                <span className="badge badge-live">{featuredTime > 0 ? `${featuredTime}s` : 'Afgesloten'}</span>
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