import { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import '../assets/css/UploadAuctionPage.css';
import { API_URL } from './DeployLocal';
function formatLocalDatetimeInput(date) {
    // returns 'YYYY-MM-DDTHH:MM' in local time for datetime-local input
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
    const { code } = useParams(); // productId
    const navigate = useNavigate();
    const location = useLocation();

    const [lot, setLot] = useState(null);
    const [startPrice, setStartPrice] = useState('');
    const [minPrice, setMinPrice] = useState(0);
    const [closingTime, setClosingTime] = useState(10);

    // Two options for scheduling: 'datetime' or 'seconds'
    const [startMode, setStartMode] = useState('datetime'); // 'datetime' | 'seconds'
    const [startDateTime, setStartDateTime] = useState(formatLocalDatetimeInput(new Date())); // default = now
    const [startInSeconds, setStartInSeconds] = useState(0);

    // Haal kavel op via API
    useEffect(() => {
        const fetchLot = async () => {
            try {
                const response = await fetch(`${API_URL}/api/Product/${code}`, {
                    headers: { Accept: "application/json" }
                });
                if (!response.ok) throw new Error('Kavel niet gevonden');
                const data = await response.json();

                setLot(data);
                setMinPrice(Number(data.minimumPrijs ?? 0));
                setStartPrice(Number(data.minimumPrijs ?? 0) + 1);
                setClosingTime(10);
            } catch (err) {
                alert(err.message);
                navigate('/veiling');
            }
        };

        if (location.state?.lot) {
            setLot(location.state.lot);
            setMinPrice(Number(location.state.lot.minimumPrijs ?? 0));
            setStartPrice(Number(location.state.lot.minimumPrijs ?? 0) + 1);
            return;
        }

        fetchLot();
    }, [code, navigate, location.state]);

    if (!lot) return <p>Laden van kavel…</p>;

    // Publiceer veiling
    const handlePublish = async () => {
        if (!startPrice || !closingTime) return alert('Vul alle velden in!');
        if (Number(startPrice) <= minPrice) return alert(`Startprijs moet hoger zijn dan minimale prijs (€${minPrice})`);

        const prijsStap = Math.max(1, Math.ceil(startPrice * 0.1));
        const veilingsmeesterID = localStorage.getItem("veilingsmeesterId") || "29685004-81a1-44b6-b7f3-973dd5f60fc0";
        const token = localStorage.getItem("token");
        if (!token) return alert("Je bent niet ingelogd.");

        // Bereken geplande starttijd afhankelijk van gekozen mode
        const now = new Date();
        let startTimestamp;

        if (startMode === 'seconds') {
            const seconds = Number(startInSeconds) || 0;
            if (seconds < 0) return alert('Start in seconden moet >= 0 zijn');
            startTimestamp = new Date(now.getTime() + seconds * 1000);
        } else {
            // datetime mode
            if (!startDateTime) {
                startTimestamp = now;
            } else {
                startTimestamp = new Date(startDateTime);
                if (isNaN(startTimestamp.getTime())) return alert('Ongeldige startdatum/tijd');
                if (startTimestamp.getTime() < now.getTime()) return alert('Startdatum moet in de toekomst liggen of leeg zijn voor direct starten');
            }
        }

        const payload = {
            productID: lot.productId,
            startPrijs: Number(startPrice),
            minPrijs: Number(minPrice),
            prijsStap,
            timerInSeconden: Number(closingTime),
            // stuur ISO-string zodat backend een correcte UTC timestamp ontvangt
            startTimestamp: startTimestamp.toISOString(),
            veilingsmeesterID
        };

        try {
            const response = await fetch("${API_URL}/api/Veiling/CreateVeiling", {
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
                            type="number"
                            value={startPrice}
                            onChange={e => setStartPrice(e.target.value)}
                            placeholder={`> ${minPrice}`}
                        />
                        <small>Minimale prijs: €{minPrice}</small>
                    </label>

                    <label className="form-field">
                        <span className="form-label">Sluitingstijd (seconden)</span>
                        <input
                            type="number"
                            value={closingTime}
                            onChange={e => setClosingTime(e.target.value)}
                        />
                        <small>Standaard 10 seconden</small>
                    </label>

                    {/* Start tijd keuze */}
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
                                Over (enkele) seconden
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

            <p>Variëteit: {lot.varieteit || 'Niet opgegeven'}</p>
            <p>Leverancier: {lot.leverancierNaam || 'Onbekend'}</p>
        </div>
    );
}

export default AuctionDetailPage;