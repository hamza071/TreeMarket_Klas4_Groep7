import { useEffect, useState } from 'react';

function DashboardPage({ upcomingLots }) {
    const [lots, setLots] = useState(() => {
        const stored = localStorage.getItem('lots');
        return stored ? JSON.parse(stored) : upcomingLots;
    });

    const [featuredLot, setFeaturedLot] = useState(null);
    const [featuredTime, setFeaturedTime] = useState(0);

    // Wanneer lots veranderen, update featured lot
    useEffect(() => {
        setLots(upcomingLots);
        if (upcomingLots.length > 0) {
            const newestLot = upcomingLots[upcomingLots.length - 1];
            setFeaturedLot(newestLot);
            setFeaturedTime(Math.max(0, Math.ceil((newestLot.closingTimestamp - Date.now()) / 1000)));
        }
    }, [upcomingLots]);

    // Timer voor featured lot
    useEffect(() => {
        if (!featuredLot) return;

        const interval = setInterval(() => {
            const timeLeft = Math.max(0, Math.ceil((featuredLot.closingTimestamp - Date.now()) / 1000));
            setFeaturedTime(timeLeft);
        }, 1000);

        return () => clearInterval(interval);
    }, [featuredLot]);

    // Timer voor alle kavels
    useEffect(() => {
        const interval = setInterval(() => {
            setLots(prevLots =>
                prevLots.map(lot => ({
                    ...lot,
                    closing: Math.max(0, Math.ceil((lot.closingTimestamp - Date.now()) / 1000)),
                }))
            );
        }, 1000);

        return () => clearInterval(interval);
    }, []);

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
                            src={featuredLot.image || '/default-image.jpg'}
                            alt={featuredLot.name}
                            className="featured-media"
                        />
                        <div className="featured-body">
                            <div className="featured-meta" aria-live="polite">
                                <span className="badge badge-live">
                                    {featuredTime > 0 ? `${featuredTime}s` : 'Afgesloten'}
                                </span>
                                <span className="lot-number">#{featuredLot.code}</span>
                            </div>
                            <h2>{featuredLot.name}</h2>
                            <p className="featured-quantity">{featuredLot.lots} stuks</p>
                            <div className="featured-footer">
                                <span className="featured-price">{featuredLot.price}</span>
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
                                <th>Sluiting</th>
                            </tr>
                        </thead>
                        <tbody>
                            {lots.map(lot => (
                                <tr key={lot.code}>
                                    <td>{lot.code}</td>
                                    <td>{lot.name}</td>
                                    <td>{lot.specs}</td>
                                    <td>{lot.lots}</td>
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