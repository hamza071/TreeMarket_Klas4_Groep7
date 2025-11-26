import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import '../assets/css/UploadAuctionPage.css';
import PlaceBid from '../components/PlaceBid'; // Optioneel: als je bod wil toevoegen

export default function AuctionDetailPage() {
    const { code } = useParams();
    const navigate = useNavigate();

    const [lot, setLot] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const [startPrice, setStartPrice] = useState('');
    const [closingTime, setClosingTime] = useState('');

    // Haal veiling van backend
    useEffect(() => {
        fetch(`/api/auctions/${code}`)
            .then(res => {
                if (!res.ok) throw new Error('Veiling niet gevonden');
                return res.json();
            })
            .then(data => {
                setLot(data);
                setLoading(false);
            })
            .catch(err => {
                setError(err.message);
                setLoading(false);
            });
    }, [code]);

    if (loading) return <p>Loading...</p>;
    if (error) {
        alert(error);
        navigate('/veiling');
        return null;
    }

    const handlePublish = async () => {
        if (!startPrice || !closingTime) return alert('Vul alle velden in!');
        if (Number(startPrice) <= 0) return alert('Beginprijs moet groter zijn dan 0');

        const closingTimestamp = Date.now() + Number(closingTime) * 1000;

        // Update via backend
        const updatedLot = {
            ...lot,
            startPrijs: Number(startPrice),
            eindDatum: new Date(closingTimestamp).toISOString(),
            status: 'published',
        };

        try {
            const response = await fetch(`/api/auctions/${lot.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updatedLot),
            });

            if (!response.ok) throw new Error('Kon kavel niet publiceren');

            alert('Kavel gepubliceerd!');
            navigate('/veiling');
        } catch (err) {
            alert(err.message);
        }
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
                        <input value={lot.titel} disabled />
                    </label>

                    <label className="form-field full-width">
                        <span className="form-label">Omschrijving</span>
                        <textarea value={lot.beschrijving} disabled rows="4" />
                    </label>

                    {lot.image && (
                        <label className="form-field full-width">
                            <span className="form-label">Afbeelding</span>
                            <img
                                src={lot.image}
                                alt={lot.titel}
                                style={{ width: '100%', maxHeight: '200px', objectFit: 'cover' }}
                            />
                        </label>
                    )}

                    {/* Veilingmeester velden */}
                    <label className="form-field">
                        <span className="form-label">Beginprijs (€)</span>
                        <input
                            type="number"
                            value={startPrice}
                            onChange={e => setStartPrice(e.target.value)}
                            placeholder="Beginprijs"
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

                {/* Optioneel bodformulier als veiling live is */}
                {lot.status === 'published' && <PlaceBid auctionId={lot.id} />}
            </form>
        </div>
    );
}