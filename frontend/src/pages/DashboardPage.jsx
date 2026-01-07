import { useEffect, useState } from 'react';

const AUTO_REMOVE_DELAY = 4000; // 4 seconden na afloop verwijderen

function DashboardPage() {
    const [lotsState, setLotsState] = useState([]);

    // 1. State voor de pop-up (modal)
    const [showModal, setShowModal] = useState(false);
    const [transactionData, setTransactionData] = useState(null);

    // 🚀 Fetch veilingen bij mount
    useEffect(() => {
        const fetchVeilingen = async () => {
            try {
                const response = await fetch('https://localhost:7054/api/Veiling/GetVeilingen', {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                        'Content-Type': 'application/json',
                    },
                });
                const data = await response.json();
                const now = Date.now();

                const lots = data
                    .filter(lot => lot.status === true)
                    .map(lot => {
                        const startTimestamp = new Date(lot.startTimestamp.split('.')[0] + 'Z').getTime();
                        const timerInSeconden = Number(lot.timerInSeconden);
                        const startPrice = Number(lot.startPrijs ?? 0);
                        const minPrice = Number(lot.minPrijs ?? startPrice);

                        const elapsed = Math.max(0, (now - startTimestamp) / 1000);
                        const remainingTime = Math.max(0, timerInSeconden - elapsed);

                        const progress = Math.min(elapsed / timerInSeconden, 1);
                        const currentPrice =
                            remainingTime > 0
                                ? startPrice - (startPrice - minPrice) * progress
                                : minPrice;

                        const removeAt = remainingTime > 0 ? null : now + AUTO_REMOVE_DELAY;

                        return {
                            ...lot,
                            startTimestamp,
                            timerInSeconden,
                            startPrice,
                            minPrice,
                            closing: Math.ceil(remainingTime),
                            currentPrice,
                            status: remainingTime > 0 ? 'actief' : 'afgesloten',
                            removeAt,
                        };
                    })
                    .sort((a, b) => b.veilingID - a.veilingID);

                setLotsState(lots);
            } catch (err) {
                console.error('Fout bij ophalen van veilingen:', err);
            }
        };

        fetchVeilingen();
    }, []);

    // ⏱ Timer update & auto-delete
    useEffect(() => {
        const interval = setInterval(() => {
            const now = Date.now();

            setLotsState(prevLots =>
                prevLots
                    .map(lot => {
                        if (!lot.startTimestamp || !lot.timerInSeconden) return lot;

                        const elapsed = Math.max(0, (now - lot.startTimestamp) / 1000);
                        const remainingTime = Math.max(0, lot.timerInSeconden - elapsed);

                        const progress = Math.min(elapsed / lot.timerInSeconden, 1);
                        const currentPrice =
                            remainingTime > 0
                                ? lot.startPrice - (lot.startPrice - lot.minPrice) * progress
                                : lot.minPrice;

                        const removeAt = remainingTime > 0 ? null : lot.removeAt ?? now + AUTO_REMOVE_DELAY;

                        return {
                            ...lot,
                            closing: Math.ceil(remainingTime),
                            currentPrice,
                            status: remainingTime > 0 ? 'actief' : 'afgesloten',
                            removeAt,
                        };
                    })
                    .filter(lot => {
                        if (lot.removeAt && now >= lot.removeAt) {
                            fetch(`https://localhost:7054/api/Veiling/DeleteVeiling/${lot.veilingID}`, {
                                method: 'DELETE',
                                headers: {
                                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                                },
                            }).catch(err => console.error('Fout bij verwijderen veiling:', err));

                            return false;
                        }
                        return true;
                    })
            );
        }, 1000);

        return () => clearInterval(interval);
    }, []);

    const featuredLot = lotsState[0];
    const featuredTime = featuredLot?.closing ?? 0;

    // ==========================================================
    // LOGICA VOOR DE POP-UP (MODAL)
    // ==========================================================

    // Stap 1: Open Modal en zet de gegevens klaar (Standaard aantal = 1)
    const handleInitialClick = () => {
        if (!featuredLot) return;

        // HIER ZAT DE FOUT: We gebruiken nu 'hoeveelheid' uit je JSON
        const maxAvailable = featuredLot.hoeveelheid ?? 1;
        const initialAmount = 1;

        setTransactionData({
            veilingId: featuredLot.veilingID,
            productNaam: featuredLot.productNaam,
            prijsPerStuk: featuredLot.currentPrice,
            maxAantal: maxAvailable,
            aantal: initialAmount,
            totaalPrijs: featuredLot.currentPrice * initialAmount
        });

        setShowModal(true);
    };

    // Stap 2: Update functie voor het invulveld IN de modal
    const handleModalQuantityChange = (e) => {
        const inputValue = e.target.value;
        const max = transactionData.maxAantal;

        // Sta toe dat het veld leeg is (voor backspace)
        if (inputValue === "") {
            setTransactionData(prev => ({ ...prev, aantal: "" }));
            return;
        }

        let val = parseInt(inputValue);

        // Validatie: check of het een geldig getal is
        if (isNaN(val)) val = 1;

        if (val > max) val = max; // Niet hoger dan max
        if (val < 1) val = 1;     // Niet lager dan 1

        // Update state: nieuw aantal én nieuwe totaalprijs
        setTransactionData(prev => ({
            ...prev,
            aantal: val,
            totaalPrijs: val * prev.prijsPerStuk
        }));
    };

    // Stap 3: Bevestigen en versturen naar backend
    const handleConfirmPurchase = async () => {
        if (!transactionData || !transactionData.aantal) return;

        try {
            const response = await fetch('https://localhost:7054/api/Claim/PlaceClaim', {
                method: 'POST',
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    veilingId: transactionData.veilingId,
                    aantal: transactionData.aantal,
                    prijs: transactionData.prijsPerStuk
                })
            });

            if (response.ok) {
                console.log("Gekocht:", transactionData);

                // UPDATE STATE: Verminder aantal én verwijder als het 0 is
                setLotsState(currentLots => {
                    return currentLots.map(lot => {
                        // 1. Zoek de juiste veiling en update het aantal
                        if (lot.veilingID === transactionData.veilingId) {
                            return {
                                ...lot,
                                hoeveelheid: (lot.hoeveelheid ?? 0) - transactionData.aantal
                            };
                        }
                        return lot;
                    })
                        // 2. FILTER: Gooi alles weg wat 0 (of minder) stuks heeft
                        // Hierdoor springt hij direct naar de volgende veiling!
                        .filter(lot => lot.hoeveelheid > 0);
                });

                alert(`Gefeliciteerd! Je hebt ${transactionData.aantal}x ${transactionData.productNaam} gekocht.`);
            } else {
                const errorData = await response.json();
                alert("Fout bij aankoop: " + (errorData.message || "Onbekende fout"));
            }
        } catch (error) {
            console.error("Fout bij aankoop:", error);
            alert("Er ging iets mis met de verbinding.");
        }
        setShowModal(false);
    };

    return (
        <div className="dashboard-page">
            <section className="dashboard-hero">
                <div className="hero-copy">
                    <span className="eyebrow">TREE MARKET</span>
                    <h1>De toekomst van bloemen en plantenveilingen</h1>
                    <p>
                        Digitale Veilingklok 2025 brengt kopers en kwekers samen in een moderne, efficiënte online veilingomgeving.
                    </p>
                </div>

                {featuredLot && (
                    <article className="featured-card">
                        <img
                            src={featuredLot.foto?.startsWith('http') ? featuredLot.foto : `https://localhost:7054${featuredLot.foto}`}
                            alt={featuredLot.productNaam || 'Productfoto'}
                            style={{
                                maxWidth: '600px',
                                maxHeight: '400px',
                                objectFit: 'cover',
                                width: '100%',
                                height: '100%',
                                borderRadius: '16px',
                                display: 'block',
                                overflow: 'hidden',
                                marginBottom: '1rem',
                            }}
                        />
                        <div className="featured-body">
                            <div className="featured-meta" aria-live="polite">
                                <span className="badge badge-live">
                                    {featuredTime > 0 ? `${featuredTime}s` : 'Afgesloten'}
                                </span>
                                <span className="lot-number">#{featuredLot.veilingID}</span>
                            </div>
                            <h2>{featuredLot.productNaam}</h2>
                            {/* AANGEPAST: hoeveelheid ipv lots */}
                            <p className="featured-quantity">Beschikbaar: {featuredLot.hoeveelheid ?? 1} stuks</p>

                            <div className="featured-footer" style={{display: 'flex', flexDirection: 'column', gap: '10px'}}>

                                <div style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center', width: '100%', marginBottom: '5px'}}>
                                    <span style={{fontSize: '1.1rem', fontWeight: 'bold', color: '#333'}}>Huidige prijs:</span>
                                    <span className="featured-price">€{featuredLot.currentPrice?.toFixed(2)}</span>
                                </div>

                                <button
                                    type="button"
                                    className="secondary-action"
                                    disabled={featuredTime <= 0}
                                    onClick={handleInitialClick}
                                    style={{ width: '100%' }}
                                >
                                    Bieden
                                </button>
                            </div>
                        </div>
                    </article>
                )}
            </section>

            <section className="dashboard-table">
                <h3>Komende kavels</h3>
                <div className="table-wrapper" role="region" aria-live="polite">
                    <table className="data-table">
                        <thead>
                        <tr>
                            <th>Kavel</th>
                            <th>Naam</th>
                            <th>Specificaties</th>
                            <th>Aantal</th>
                            <th>Huidige prijs (€)</th>
                            <th>Sluiting</th>
                        </tr>
                        </thead>
                        <tbody>
                        {lotsState.map(lot => (
                            <tr key={lot.veilingID}>
                                <td>{lot.veilingID}</td>
                                <td>{lot.productNaam}</td>
                                <td>{lot.specs ?? '-'}</td>
                                {/* AANGEPAST: hoeveelheid ipv lots */}
                                <td>{lot.hoeveelheid ?? 1}</td>
                                <td>€{lot.currentPrice?.toFixed(2)}</td>
                                <td>{lot.closing > 0 ? `${lot.closing}s` : 'Afgesloten'}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            </section>

            {/* ========================================================== */}
            {/* POP-UP MODAL (Met input en totaalprijs)                    */}
            {/* ========================================================== */}
            {showModal && transactionData && (
                <div style={styles.overlay}>
                    <div style={styles.modal}>
                        <h2>Bevestig Aankoop</h2>

                        <div style={styles.summary}>
                            <p><strong>Product:</strong> {transactionData.productNaam}</p>
                            <p><strong>Prijs per stuk:</strong> €{transactionData.prijsPerStuk.toFixed(2)}</p>

                            <p style={{fontSize: '0.9rem', color: '#666'}}>
                                <em>(Maximaal beschikbaar: {transactionData.maxAantal})</em>
                            </p>

                            {/* Input veld in de modal */}
                            <div style={{margin: '15px 0'}}>
                                <label style={{marginRight: '10px', fontWeight: 'bold'}}>Aantal:</label>
                                <input
                                    type="number"
                                    value={transactionData.aantal}
                                    onChange={handleModalQuantityChange}
                                    style={{
                                        padding: '8px', borderRadius: '5px',
                                        border: '1px solid #333', width: '80px',
                                        textAlign: 'center', fontSize: '1rem'
                                    }}
                                />
                            </div>

                            <hr style={{margin: '15px 0'}}/>

                            {/* Dynamische Totaalprijs */}
                            <p style={{fontSize: '1.4rem', color: '#2ecc71'}}>
                                <strong>Totaal: €{(transactionData.totaalPrijs || 0).toFixed(2)}</strong>
                            </p>
                        </div>

                        <div style={styles.buttons}>
                            <button onClick={() => setShowModal(false)} style={styles.cancelBtn}>Annuleren</button>
                            <button onClick={handleConfirmPurchase} style={styles.confirmBtn}>Bevestigen</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

const styles = {
    overlay: {
        position: 'fixed', top: 0, left: 0, right: 0, bottom: 0,
        backgroundColor: 'rgba(0,0,0,0.7)', zIndex: 1000,
        display: 'flex', justifyContent: 'center', alignItems: 'center'
    },
    modal: {
        backgroundColor: 'white', padding: '2rem', borderRadius: '12px',
        width: '90%', maxWidth: '400px', textAlign: 'center',
        boxShadow: '0 4px 20px rgba(0,0,0,0.3)', color: '#333'
    },
    summary: { textAlign: 'left', margin: '1.5rem 0', lineHeight: '1.6' },
    buttons: { display: 'flex', gap: '1rem', justifyContent: 'center' },
    cancelBtn: { padding: '10px 20px', borderRadius: '8px', border: '1px solid #ccc', backgroundColor: '#f5f5f5', cursor: 'pointer', color: 'black' },
    confirmBtn: { padding: '10px 20px', borderRadius: '8px', border: 'none', backgroundColor: '#2ecc71', color: 'white', fontWeight: 'bold', cursor: 'pointer' }
};

export default DashboardPage;