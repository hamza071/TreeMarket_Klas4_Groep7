//import { useState, useEffect } from 'react';
//import { useParams, useNavigate } from 'react-router-dom';
//import '../assets/css/UploadAuctionPage.css';

//function AuctionDetailPage({ lots }) {
//    const { code } = useParams();
//    const navigate = useNavigate();
//    const lot = lots.find(l => l.code === code);

//    const [startPrice, setStartPrice] = useState('');
//    const [closingTime, setClosingTime] = useState('');

//    useEffect(() => {
//        if (!lot) {
//            alert('Kavel niet gevonden');
//            navigate('/veiling');
//        }
//    }, [lot, navigate]);

//    if (!lot) return null;

//    const handlePublish = async () => {
//        if (!startPrice || !closingTime)
//            return alert('Vul alle velden in!');

//        const payload = {
//            ProductID: lot.id || lot.ProductId,
//            StartPrijs: Number(startPrice),
//            PrijsStap: 1,
//            TimerInSeconden: Number(closingTime)
//        };

//        const token = localStorage.getItem("token");
//        if (!token) return alert("Je bent niet ingelogd.");

//        try {
//            const response = await fetch(
//                "https://localhost:7054/api/Veiling/CreateVeiling",
//                {
//                    method: "POST",
//                    headers: {
//                        "Content-Type": "application/json",
//                        "Authorization": `Bearer ${token}`
//                    },
//                    body: JSON.stringify(payload)
//                }
//            );

//            if (!response.ok) {
//                const text = await response.text();
//                return alert("Fout bij aanmaken veiling: " + text);
//            }

//            const data = await response.json();
//            console.log("Veiling aangemaakt:", data);
//            alert("Veiling succesvol aangemaakt!");
//            navigate('/veiling');

//        } catch (err) {
//            console.error("Error creating veiling:", err);
//            alert("Er ging iets mis: " + err.message);
//        }
//    };

//    return (
//        <div className="upload-page">
//            <header className="section-header">
//                <h1>Kavel publiceren (veilingmeester)</h1>
//            </header>

//            <form className="upload-form" onSubmit={e => e.preventDefault()}>
//                <fieldset className="form-grid">
//                    <label className="form-field">
//                        <span className="form-label">Productnaam</span>
//                        <input value={lot.name || lot.Artikelkenmerken} disabled />
//                    </label>

//                    <label className="form-field">
//                        <span className="form-label">Beginprijs (€)</span>
//                        <input
//                            type="number"
//                            value={startPrice}
//                            onChange={e => setStartPrice(e.target.value)}
//                            placeholder={`> ${lot.minPrice || lot.MinimumPrijs}`}
//                        />
//                    </label>

//                    <label className="form-field">
//                        <span className="form-label">Sluitingstijd (seconden)</span>
//                        <input
//                            type="number"
//                            value={closingTime}
//                            onChange={e => setClosingTime(e.target.value)}
//                            placeholder="Bijv. 60"
//                        />
//                    </label>
//                </fieldset>

//                <div className="form-actions">
//                    <button type="button" className="primary-action" onClick={handlePublish}>
//                        Publiceer kavel
//                    </button>
//                    <button type="button" className="link-button" onClick={() => navigate('/veiling')}>
//                        Annuleer
//                    </button>
//                </div>
//            </form>
//        </div>
//    );
//}

//export default AuctionDetailPage;

import { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import '../assets/css/UploadAuctionPage.css';

function AuctionDetailPage() {
    const { code } = useParams(); // productId
    const navigate = useNavigate();
    const location = useLocation();
    const [lot, setLot] = useState(location.state?.lot || null);
    const [closingTime, setClosingTime] = useState(3600); // default 1 uur
    const [loading, setLoading] = useState(!lot);

    useEffect(() => {
        // Als lot nog niet via state is meegegeven, ophalen via API
        const fetchLot = async () => {
            if (!lot) {
                try {
                    const response = await fetch(`https://localhost:7054/api/Product/${code}`);
                    if (!response.ok) throw new Error('Kavel niet gevonden');
                    const data = await response.json();
                    setLot(data);
                } catch (err) {
                    alert(err.message);
                    navigate('/veiling');
                } finally {
                    setLoading(false);
                }
            }
        };
        fetchLot();
    }, [lot, code, navigate]);

    if (loading) return <p>Kavel wordt geladen…</p>;
    if (!lot) return null;

    // Beginprijs automatisch = MinimumPrijs + 1
    const startPrice = lot.minimumPrijs + 1;

    const handlePublish = async () => {
        if (!closingTime || closingTime <= 0) {
            return alert('Vul een geldige sluitingstijd in.');
        }

        const token = localStorage.getItem('token');
        if (!token) return alert('Je bent niet ingelogd.');

        const payload = {
            productID: lot.productId,
            startPrijs: startPrice,
            timerInSeconden: Number(closingTime)
        };

        try {
            const response = await fetch('https://localhost:7054/api/Veiling/CreateVeiling', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const text = await response.text();
                return alert('Fout bij aanmaken veiling: ' + text);
            }

            const data = await response.json();
            console.log('Veiling aangemaakt:', data);
            alert('Veiling succesvol aangemaakt!');
            navigate('/veiling');
        } catch (err) {
            console.error('Error creating veiling:', err);
            alert('Er ging iets mis: ' + err.message);
        }
    };

    return (
        <div className="upload-page">
            <header className="section-header">
                <h1>Kavel publiceren (veilingmeester)</h1>
            </header>

            <form className="upload-form" onSubmit={e => e.preventDefault()}>
                <fieldset className="form-grid">
                    <label className="form-field">
                        <span className="form-label">Productnaam</span>
                        <input value={lot.naam} disabled />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Variëteit</span>
                        <input value={lot.varieteit || '-'} disabled />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Aantal stuks</span>
                        <input value={lot.hoeveelheid} disabled />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Omschrijving</span>
                        <textarea value={lot.omschrijving || '-'} disabled rows={3} />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Beginprijs (€)</span>
                        <input value={startPrice} disabled />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Sluitingstijd (seconden)</span>
                        <input
                            type="number"
                            value={closingTime}
                            onChange={e => setClosingTime(e.target.value)}
                            min="1"
                        />
                    </label>

                    {lot.foto && (
                        <img
                            src={lot.foto.startsWith('http') ? lot.foto : `https://localhost:7054${lot.foto}`}
                            alt={lot.naam}
                            className="auction-card-image"
                        />
                    )}
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