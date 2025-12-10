import { useEffect, useState } from 'react';

function DashboardPage({ lots }) {
    // Filter alleen gepubliceerde kavels
    const publishedLots = lots.filter(lot => lot.status === 'published');

    // Voeg dynamische velden toe
    const [lotsState, setLotsState] = useState(() =>
        publishedLots.map(lot => ({
            ...lot,
            startPrice: lot.startPrice ?? lot.price ?? 0,
            minPrice: lot.minPrice ?? lot.price ?? 0,
            closingTime: lot.closingTime ?? 0,
            startTimestamp: lot.startTimestamp ?? Date.now(),
            currentPrice: lot.startPrice ?? lot.price ?? 0,
            closing: lot.closingTimestamp
                ? Math.max(0, Math.ceil((lot.closingTimestamp - Date.now()) / 1000))
                : 0,
        }))
    );

    // Update lotsState bij prop verandering
    useEffect(() => {
        const updatedLots = publishedLots.map(lot => ({
            ...lot,
            startPrice: lot.startPrice ?? lot.price ?? 0,
            minPrice: lot.minPrice ?? lot.price ?? 0,
            closingTime: lot.closingTime ?? 0,
            startTimestamp: lot.startTimestamp ?? Date.now(),
            currentPrice: lot.startPrice ?? lot.price ?? 0,
            closing: lot.closingTimestamp
                ? Math.max(0, Math.ceil((lot.closingTimestamp - Date.now()) / 1000))
                : 0,
        }));
        setLotsState(updatedLots);
    }, [lots]);

    // Timer voor prijs en sluiting
    useEffect(() => {
        const interval = setInterval(() => {
            const now = Date.now();
            setLotsState(prevLots =>
                prevLots.map(lot => {
                    const elapsed = (now - lot.startTimestamp) / 1000;
                    const remainingTime = Math.max(0, lot.closingTime - elapsed);

                    const priceRange = lot.startPrice - lot.minPrice;
                    const currentPrice =
                        remainingTime > 0
                            ? Math.max(lot.minPrice, lot.startPrice - (priceRange * (elapsed / lot.closingTime)))
                            : lot.minPrice;

                    return {
                        ...lot,
                        closing: Math.ceil(remainingTime),
                        currentPrice,
                    };
                })
            );
        }, 1000);

        return () => clearInterval(interval);
    }, []);

    const featuredLot = lotsState[lotsState.length - 1];
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
                            src={featuredLot.image || '/default-image.jpg'}
                            alt={featuredLot.name}
                            className="featured-media"
                        />
                        <div className="featured-body">
                            <div className="featured-meta" aria-live="polite">
                                <span className="badge badge-live">{featuredTime > 0 ? `${featuredTime}s` : 'Afgesloten'}</span>
                                <span className="lot-number">#{featuredLot.code}</span>
                            </div>
                            <h2>{featuredLot.name}</h2>
                            <p className="featured-quantity">{featuredLot.lots} stuks</p>
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
                                <tr key={lot.code}>
                                    <td>{lot.code}</td>
                                    <td>{lot.name}</td>
                                    <td>{lot.specs}</td>
                                    <td>{lot.lots}</td>
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