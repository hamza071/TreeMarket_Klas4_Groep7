import { Link } from 'react-router-dom';
import '../assets/css/AuctionPage.css';

export default function AuctionPage({ lots }) {
    // Filter pending kavels
    const pendingLots = lots.filter(lot => lot.status === 'pending');

    if (pendingLots.length === 0) return <p>Geen kavels beschikbaar om te publiceren.</p>;

    return (
        <div className="auction-page">
            <header className="section-header">
                <h1>Te publiceren kavels (veilingmeester)</h1>
                <p>Bekijk de kavels die door leveranciers zijn toegevoegd en publiceer ze.</p>
            </header>

            <div className="auction-grid">
                {pendingLots.map(lot => (
                    <article key={lot.id} className="auction-card">
                        <h2>{lot.titel}</h2>
                        <p>{lot.beschrijving}</p>
                        {lot.image && (
                            <img src={lot.image} alt={lot.titel} className="auction-card-image" />
                        )}
                        <Link to={`/veiling/${lot.id}`} className="primary-action">
                            Bewerk / Publiceer
                        </Link>
                    </article>
                ))}
            </div>
        </div>
    );
}