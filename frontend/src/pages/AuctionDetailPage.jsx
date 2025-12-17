import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import '../assets/css/UploadAuctionPage.css';

function AuctionDetailPage({ lots }) {
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

    const handlePublish = async () => {
        if (!startPrice || !closingTime)
            return alert('Vul alle velden in!');

        const payload = {
            ProductID: lot.id || lot.ProductId,
            StartPrijs: Number(startPrice),
            PrijsStap: 1,
            TimerInSeconden: Number(closingTime)
        };

        const token = localStorage.getItem("token");
        if (!token) return alert("Je bent niet ingelogd.");

        try {
            const response = await fetch(
                "https://localhost:7054/api/Veiling/CreateVeiling",
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`
                    },
                    body: JSON.stringify(payload)
                }
            );

            if (!response.ok) {
                const text = await response.text();
                return alert("Fout bij aanmaken veiling: " + text);
            }

            const data = await response.json();
            console.log("Veiling aangemaakt:", data);
            alert("Veiling succesvol aangemaakt!");
            navigate('/veiling');

        } catch (err) {
            console.error("Error creating veiling:", err);
            alert("Er ging iets mis: " + err.message);
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
                        <input value={lot.name || lot.Artikelkenmerken} disabled />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Beginprijs (€)</span>
                        <input
                            type="number"
                            value={startPrice}
                            onChange={e => setStartPrice(e.target.value)}
                            placeholder={`> ${lot.minPrice || lot.MinimumPrijs}`}
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