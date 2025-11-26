import { useEffect, useState } from 'react';
import PlaceBid from '../components/PlaceBid';
import '../assets/css/AuctionPage.css';

export default function DashboardPage({ lots }) {
    const [lotsState, setLotsState] = useState([]);

    // Zet initial lotsState bij props verandering
    useEffect(() => {
        const now = Date.now();
        setLotsState(
            lots.map(lot => ({
                ...lot,
                startPrice: lot.startPrijs ?? 0,
                minPrice: lot.startPrijs ?? 0,
                closingTime: Math.max(0, (new Date(lot.eindDatum) - now) / 1000),
                startTimestamp: now,
                currentPrice: lot.startPrijs ?? 0,
                closing: Math.max(0, Math.ceil((new Date(lot.eindDatum) - now) / 1000)),
            }))
        );
    }, [lots]);

    // Timer interval voor countdown en actuele prijs
    useEffect(() => {
        const interval = setInterval(() => {
            const now = Date.now();
            setLotsState(prev =>
                prev.map(lot => {
                    const remaining = Math.max(0, (new Date(lot.eindDatum) - now) / 1000);
                    const priceRange = lot.startPrice - lot.minPrice;
                    const currentPrice =
                        remaining > 0
                            ? Math.max(lot.minPrice, lot.startPrice - priceRange * (1 - remaining / lot.closingTime))
                            : lot.minPrice;

                    return {
                        ...lot,
                        closing: Math.ceil(remaining),
                        currentPrice,
                    };
                })
            );
        }, 1000);

        return () => clearInterval(interval);
    }, []);

    if (lotsState.length === 0) return <p>Geen lopende kavels.</p>;

    const featuredLot = lotsState[lotsState.length - 1];

    return (
        <div className="dashboard-page">
            <section className="dashboard-hero">
                {featuredLot && (
                    <article className="featured-card">
                        <img
                            src={featuredLot.image || '/default-image.jpg'}
                            alt={featuredLot.titel}
                            className="featured-media"
                        />
                        <div className="featured-body">
                            <div className="featured-meta" aria-live="polite">
                                <span className="badge badge-live">
                                    {featuredLot.closing > 0 ? `${featuredLot.closing}s` : 'Afgesloten'}
                                </span>
                                <span className="lot-number">#{featuredLot.id}</span>
                            </div>
                            <h2>{featuredLot.titel}</h2>
                            <p>{featuredLot.beschrijving}</p>
                            <div className="featured-footer">
                                <span className="featured-price">€{featuredLot.currentPrice?.toFixed(2)}</span>
                                <PlaceBid auctionId={featuredLot.id} />
                            </div>
                        </div>
                    </article>
                )}
            </section>

            <section className="dashboard-table">
                <h3>Lopende kavels</h3>
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>Kavel</th>
                            <th>Titel</th>
                            <th>Omschrijving</th>
                            <th>Huidige prijs (€)</th>
                            <th>Sluiting</th>
                        </tr>
                    </thead>
                    <tbody>
                        {lotsState.map(lot => (
                            <tr key={lot.id}>
                                <td>{lot.id}</td>
                                <td>{lot.titel}</td>
                                <td>{lot.beschrijving}</td>
                                <td>€{lot.currentPrice?.toFixed(2)}</td>
                                <td>{lot.closing > 0 ? `${lot.closing}s` : 'Afgesloten'}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </section>
        </div>
    );
}