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
                    <article key={lot.id} className="auction-card">
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