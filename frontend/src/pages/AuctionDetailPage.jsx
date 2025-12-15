import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import '../assets/css/UploadAuctionPage.css';

function AuctionDetailPage({ lots, updateLot }) {
    const { code } = useParams();
    const navigate = useNavigate();

    const lot = lots.find(l => l.code === code);

    const [startPrice, setStartPrice] = useState('');
    const [closingTime, setClosingTime] = useState('');

    useEffect(() => {
        if (!lot) {
            alert('Kavel niet gevonden');
            navigate('/veiling');
        }
    }, [lot, navigate]);

    if (!lot) return null;
    ////////j
    const handlePublish = () => {
        if (!startPrice || !closingTime) return alert('Vul alle velden in!');
        if (Number(startPrice) <= lot.minPrice) return alert('Beginprijs moet hoger zijn dan minimumprijs');

        const closingTimestamp = Date.now() + Number(closingTime) * 1000;

        const updatedLot = {
            ...lot,
            startPrice: Number(startPrice),
            closingTime: Number(closingTime),
            status: 'published',
            closingTimestamp,
            startTimestamp: Date.now(), // start van veiling nu
            currentPrice: Number(startPrice), // beginprijs voor dashboard
        };

        updateLot(updatedLot);
        alert('Kavel gepubliceerd!');
        navigate('/veiling');
    };

    return (
        <div className="upload-page">
            <header className="section-header">
                <h1>Kavel publiceren (veilingmeester)</h1>
                <p>
                    Alle leveranciervelden zijn ingevuld door de leverancier. Vul hier de beginprijs en sluitingstijd in om te publiceren.
                </p>
            </header>

            <form className="upload-form" onSubmit={e => e.preventDefault()}>
                <fieldset className="form-grid">
                    <legend className="sr-only">Kavelgegevens</legend>

                    <label className="form-field">
                        <span className="form-label">Productnaam</span>
                        <input value={lot.name} disabled />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Variëteit</span>
                        <input value={lot.specs} disabled />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Aantal stuks</span>
                        <input value={lot.lots} disabled />
                    </label>

                    <label className="form-field full-width">
                        <span className="form-label">Omschrijving</span>
                        <textarea value={lot.description} disabled rows="4" />
                    </label>

                    <label className="form-field full-width">
                        <span className="form-label">Upload afbeelding</span>
                        {lot.image && (
                            <img
                                src={lot.image}
                                alt={lot.name}
                                style={{ width: '100%', maxHeight: '200px', objectFit: 'cover' }}
                            />
                        )}
                    </label>

                    {/* Veilingmeester velden */}
                    <label className="form-field">
                        <span className="form-label">Beginprijs (€)</span>
                        <input
                            type="number"
                            value={startPrice}
                            onChange={e => setStartPrice(e.target.value)}
                            placeholder={`> ${lot.minPrice}`}
                        />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Sluitingstijd (seconden)</span>
                        <input
                            type="number"
                            value={closingTime}
                            onChange={e => setClosingTime(e.target.value)}
                            placeholder="Bijv. 60"
                        />
                    </label>
                </fieldset>

                <div className="form-actions">
                    <button type="button" className="primary-action" onClick={handlePublish}>
                        Publiceer kavel
                    </button>
                    <button type="button" className="link-button" onClick={() => navigate('/veiling')}>
                        Annuleer
                    </button>
                </div>
            </form>
        </div>
    );
}

export default AuctionDetailPage;