import { Link } from 'react-router-dom';
import '../assets/css/AuctionPage.css'; // je kunt dezelfde styling als UploadAuctionPage gebruiken of aanpassen

function AuctionPage({ lots }) {
    // Filter alleen pending kavels
    const pendingLots = lots.filter(lot => lot.status === 'pending');

    if (pendingLots.length === 0) {
        return <p>Geen kavels beschikbaar om te publiceren.</p>;
    }

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
                        {lot.image && (
                            <img
                                src={lot.image}
                                alt={lot.name}
                                className="auction-card-image"
                            />
                        )}
                        <Link to={`/veiling/${lot.code}`} className="primary-action">
                            Bewerk / Publiceer
                        </Link>
                    </article>
                ))}
            </div>
        </div>
    );
}

export default AuctionPage;