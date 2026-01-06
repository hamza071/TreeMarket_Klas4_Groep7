import { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import '../assets/css/UploadAuctionPage.css';

function AuctionDetailPage() {
    const { code } = useParams(); // dit is productId
    const navigate = useNavigate();
    const location = useLocation();

    const [lot, setLot] = useState(null);
    const [startPrice, setStartPrice] = useState('');
    const [closingTime, setClosingTime] = useState(10);

    // Haal kavel op via API
    useEffect(() => {
        const fetchLot = async () => {
            try {
                const response = await fetch(`https://localhost:7054/api/Product/${code}`, { headers: { Accept: "application/json" } });
                if (!response.ok) throw new Error('Kavel niet gevonden');
                const data = await response.json();
                setLot(data);
                setStartPrice((data.minimumPrijs || 0) + 1);
                setClosingTime(10);
            } catch (err) {
                alert(err.message);
                navigate('/veiling');
            }
        };

        fetchLot(); // vergeet dit niet!
    }, [code, navigate]);

    if (!lot) return <p>Loading...</p>; // of return null;

    ////////j
    const handlePublish = async () => {
        if (!startPrice || !closingTime) return alert('Vul alle velden in!');

        const prijsStap = Math.max(1, Math.ceil(startPrice * 0.1));
        const veilingsmeesterID = localStorage.getItem("veilingsmeesterId") || "29685004-81a1-44b6-b7f3-973dd5f60fc0";
        const token = localStorage.getItem("token");
        if (!token) return alert("Je bent niet ingelogd.");

        const payload = {
            productID: lot.productId,
            startPrijs: Number(startPrice),
            prijsStap,
            timerInSeconden: Number(closingTime),
            veilingsmeesterID
        };

        try {
            const response = await fetch("https://localhost:7054/api/Veiling/CreateVeiling", {
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

            // ✅ Verwijder de kavel eerst uit de view zodat het lijkt alsof hij weg is
            setLot(null);

            // ✅ Notificatie
            alert(`Veiling succesvol gepubliceerd! VeilingID: ${data.veilingID}`);

            // ✅ Daarna navigeren (optioneel)
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
                            placeholder={`> ${lot.minimumPrijs}`}
                        />
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
                    src={lot.foto ? (lot.foto.startsWith('http') ? lot.foto : `https://localhost:7054${lot.foto}`) : '/images/default.png'}
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