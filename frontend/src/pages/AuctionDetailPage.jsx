import { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import '../assets/css/UploadAuctionPage.css';
import { API_URL } from '../DeployLocal';

function formatLocalDatetimeInput(date) {
    const pad = (n) => n.toString().padStart(2, '0');
    return (
        date.getFullYear() + '-' +
        pad(date.getMonth() + 1) + '-' +
        pad(date.getDate()) + 'T' +
        pad(date.getHours()) + ':' +
        pad(date.getMinutes())
    );
}

function AuctionDetailPage() {
    const { code } = useParams();
    const navigate = useNavigate();
    const location = useLocation();

    const [lot, setLot] = useState(null);
    const [startPrice, setStartPrice] = useState('0.00'); // string voor decimalen
    const [minPrice, setMinPrice] = useState(0);
    const [closingTime, setClosingTime] = useState(10);

    const [startMode, setStartMode] = useState('datetime');
    const [startDateTime, setStartDateTime] = useState(formatLocalDatetimeInput(new Date()));
    const [startInSeconds, setStartInSeconds] = useState(0);

    // Fetch product
    useEffect(() => {
        const fetchLot = async () => {
            try {
                const response = await fetch(`${API_URL}/api/Product/${code}`, {
                    headers: { Accept: "application/json" }
                });
                if (!response.ok) throw new Error('Kavel niet gevonden');
                const data = await response.json();
                setLot(data);
                const min = Number(data.minimumPrijs ?? 0);
                setMinPrice(min);
                setStartPrice((min + 1).toFixed(2));
                setClosingTime(10);
            } catch (err) {
                alert(err.message);
                navigate('/veiling');
            }
        };

        if (location.state?.lot) {
            const min = Number(location.state.lot.minimumPrijs ?? 0);
            setLot(location.state.lot);
            setMinPrice(min);
            setStartPrice((min + 1).toFixed(2));
            return;
        }

        fetchLot();
    }, [code, navigate, location.state]);

    if (!lot) return <p>Laden van kavel…</p>;

    // Nieuw: veilige decimal input
    const handleStartPriceChange = (e) => {
        const value = e.target.value;
        if (/^\d*\.?\d*$/.test(value)) { // alleen cijfers en maximaal 1 punt
            setStartPrice(value);
        }
    };

    const handlePublish = async () => {
        if (!startPrice || !closingTime) return alert('Vul alle velden in!');

        const startPriceNumber = parseFloat(startPrice);
        const minPriceNumber = parseFloat(minPrice);
        const closingTimeNumber = Number(closingTime);

        if (isNaN(startPriceNumber) || startPriceNumber <= minPriceNumber)
            return alert(`Startprijs moet hoger zijn dan minimale prijs (€${minPriceNumber.toFixed(2)})`);

        if (isNaN(closingTimeNumber) || closingTimeNumber < 1)
            return alert('Sluitingstijd moet minimaal 1 seconde zijn');

        const prijsStap = Math.max(0.01, parseFloat((startPriceNumber * 0.1).toFixed(2)));

        const veilingsmeesterID = localStorage.getItem("veilingsmeesterId") || "29685004-81a1-44b6-b7f3-973dd5f60fc0";
        const token = localStorage.getItem("token");
        if (!token) return alert("Je bent niet ingelogd.");

        const now = new Date();
        let startTimestamp;

        if (startMode === 'seconds') {
            const seconds = Number(startInSeconds) || 0;
            if (seconds < 0) return alert('Start in seconden moet >= 0 zijn');
            startTimestamp = new Date(now.getTime() + seconds * 1000);
        } else {
            startTimestamp = startDateTime ? new Date(startDateTime) : now;
            if (isNaN(startTimestamp.getTime()))
                return alert('Ongeldige startdatum/tijd');
            if (startTimestamp.getTime() < now.getTime())
                return alert('Startdatum moet in de toekomst liggen of leeg zijn voor direct starten');
        }

        const payload = {
            productID: lot.productId,
            startPrijs: startPriceNumber,
            minPrijs: minPriceNumber,
            prijsStap,
            timerInSeconden: closingTimeNumber,
            startTimestamp: startTimestamp.toISOString(),
            veilingsmeesterID
        };

        try {
            const response = await fetch(`${API_URL}/api/Veiling/CreateVeiling`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const text = await response.text();
                return alert("Fout bij aanmaken veiling: " + text);
            }

            const data = await response.json();
            console.log("Veiling aangemaakt:", data);

            setLot(null);
            alert(`Veiling succesvol gepubliceerd! VeilingID: ${data.veilingID}`);
            navigate('/veiling');
        } catch (err) {
            console.error("Error creating veiling:", err);
            alert("Er ging iets mis: " + err.message);
        }
    };

    return (
        <div className="upload-page">
            <header className="section-header">
                <h1>Veiling publiceren (veilingmeester)</h1>
            </header>

            <form className="upload-form" onSubmit={e => e.preventDefault()}>
                <fieldset className="form-grid">
                    <label className="form-field">
                        <span className="form-label">Productnaam</span>
                        <input value={lot.naam || lot.productNaam} disabled />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Beginprijs (€)</span>
                        <input
                            type="text"
                            value={startPrice}
                            onChange={handleStartPriceChange}
                            placeholder={`> ${minPrice.toFixed(2)}`}
                        />
                        <small>Minimale prijs: €{minPrice.toFixed(2)}</small>
                    </label>

                    <label className="form-field">
                        <span className="form-label">Sluitingstijd (seconden)</span>
                        <input
                            type="number"
                            min={1}
                            value={closingTime}
                            onChange={e => setClosingTime(e.target.value)}
                        />
                        <small>Standaard 10 seconden</small>
                    </label>

                    <div className="form-field">
                        <span className="form-label">Start tijd</span>
                        <div className="start-mode-toggle">
                            <button
                                type="button"
                                className={`start-mode-option ${startMode === 'datetime' ? 'active' : ''}`}
                                onClick={() => setStartMode('datetime')}
                            >
                                Datum & tijd
                            </button>
                            <button
                                type="button"
                                className={`start-mode-option ${startMode === 'seconds' ? 'active' : ''}`}
                                onClick={() => setStartMode('seconds')}
                            >
                                Over seconden
                            </button>
                        </div>

                        {startMode === 'datetime' ? (
                            <div className="start-mode-input">
                                <input
                                    type="datetime-local"
                                    value={startDateTime}
                                    onChange={e => setStartDateTime(e.target.value)}
                                    min={formatLocalDatetimeInput(new Date())}
                                />
                                <small>Laat leeg om direct te starten</small>
                            </div>
                        ) : (
                            <div className="start-mode-input">
                                <input
                                    type="number"
                                    min={0}
                                    value={startInSeconds}
                                    onChange={e => setStartInSeconds(e.target.value)}
                                />
                                <small>Aantal seconden tot start (0 = direct)</small>
                            </div>
                        )}
                    </div>
                </fieldset>

                <div className="form-actions">
                    <button type="button" className="primary-action" onClick={handlePublish}>
                        Publiceer veiling
                    </button>
                    <button type="button" className="link-button" onClick={() => navigate('/veiling')}>
                        Annuleer
                    </button>
                </div>
            </form>

            <div className="lot-image" style={{
                maxWidth: '600px',
                maxHeight: '400px',
                overflow: 'hidden',
                marginTop: '1rem',
                borderRadius: '16px'
            }}>
                <img
                    src={lot.foto ? (lot.foto.startsWith('http') ? lot.foto : `${API_URL}${lot.foto}`) : '/images/default.png'}
                    alt={lot.naam || 'Productfoto'}
                    style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }}
                />
            </div>

            <p>Leverancier: {lot.leverancierNaam || 'Onbekend'}</p>
        </div>
    );
}

export default AuctionDetailPage;