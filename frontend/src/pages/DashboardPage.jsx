import { useEffect, useState } from 'react';
import '../assets/css/DashboardPage.css';
import { API_URL } from '../DeployLocal';

const AUTO_REMOVE_DELAY = 4000; // 4 seconden na afloop verwijderen

function DashboardPage() {
    const [lotsState, setLotsState] = useState([]);
    const [expandedDescriptions, setExpandedDescriptions] = useState({});
    const [featuredIndex, setFeaturedIndex] = useState(0);
    const [showModal, setShowModal] = useState(false);
    const [transactionData, setTransactionData] = useState(null);

    const toggleExpanded = (id) => {
        setExpandedDescriptions(prev => ({ ...prev, [id]: !prev[id] }));
    };

    const truncateWords = (text, maxWords = 10) => {
        if (!text) return '';
        const words = text.split(/\s+/).filter(Boolean);
        if (words.length <= maxWords) return { text, truncated: false };
        return { text: words.slice(0, maxWords).join(' '), truncated: true };
    };

    const getDescription = (lot) => {
        if (!lot) return '';
        return (
            lot.omschrijving ||
            lot.Omschrijving ||
            lot.product?.omschrijving ||
            lot.product?.Omschrijving ||
            lot.productOmschrijving ||
            ''
        );
    };

    // 🚀 Fetch veilingen bij mount
    useEffect(() => {
        const fetchVeilingen = async () => {
            try {
                const response = await fetch(`${API_URL}/api/Veiling/GetVeilingen`, {
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
                        const currentPrice = remainingTime > 0
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
                            status: startTimestamp > now ? 'gepland' : (remainingTime > 0 ? 'actief' : 'afgesloten'),
                            removeAt,
                        };
                    })
                    .sort((a, b) => b.veilingID - a.veilingID);

                setLotsState(lots);

                // Fetch missing descriptions from Product endpoint
                const missing = lots.filter(l => !getDescription(l) && l.productID);
                if (missing.length > 0) {
                    const token = localStorage.getItem('token');
                    await Promise.all(missing.map(async m => {
                        try {
                            const resp = await fetch(`${API_URL}/api/Product/${m.productID}`, {
                                headers: { Accept: 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}) }
                            });
                            if (!resp.ok) return;
                            const prod = await resp.json();
                            const oms = prod.omschrijving || prod.Omschrijving || '';
                            if (oms) {
                                setLotsState(prev => prev.map(x => x.productID === m.productID ? { ...x, omschrijving: oms } : x));
                            }
                        } catch (e) {
                            console.warn('Kon product omschrijving niet ophalen voor', m.productID, e);
                        }
                    }));
                }
            } catch (err) {
                console.error('Fout bij ophalen van veilingen:', err);
            }
        };

        fetchVeilingen();
    }, []);

    // ⏱ Timer update & auto-delete zero-quantity veilingen
    useEffect(() => {
        const deletedVeilingen = new Set();

        const interval = setInterval(() => {
            const now = Date.now();

            setLotsState(prevLots =>
                prevLots
                    .map(lot => {
                        if (!lot.startTimestamp || !lot.timerInSeconden) return lot;

                        const elapsed = Math.max(0, (now - lot.startTimestamp) / 1000);
                        const remainingTime = Math.max(0, lot.timerInSeconden - elapsed);
                        const currentPrice = remainingTime > 0
                            ? lot.startPrice - (lot.startPrice - lot.minPrice) * (elapsed / lot.timerInSeconden)
                            : lot.minPrice;

                        return {
                            ...lot,
                            closing: Math.ceil(remainingTime),
                            currentPrice,
                            status: lot.startTimestamp > now ? 'gepland' : (remainingTime > 0 ? 'actief' : 'afgesloten'),
                            removeAt: remainingTime > 0 ? null : lot.removeAt ?? now + AUTO_REMOVE_DELAY,
                        };
                    })
                    .filter(lot => {
                        const shouldDelete = (lot.hoeveelheid ?? 0) <= 0 || (lot.removeAt && now >= lot.removeAt);

                        if (shouldDelete) {
                            if (!deletedVeilingen.has(lot.veilingID)) {
                                deletedVeilingen.add(lot.veilingID);

                                const token = localStorage.getItem('token');
                                const headers = token ? { Authorization: `Bearer ${token}` } : {};

                                fetch(`${API_URL}/api/Veiling/DeleteVeiling/${lot.veilingID}`, { method: 'DELETE', headers })
                                    .then(res => {
                                        if (!res.ok) return res.text().then(t => Promise.reject(new Error(t || res.statusText)));
                                        console.log(`Veiling ${lot.veilingID} verwijderd (auto).`);
                                    })
                                    .catch(err => console.error(`Fout bij verwijderen veiling ${lot.veilingID}:`, err));
                            }
                            return false;
                        }

                        return true;
                    })
            );
        }, 1000);

        return () => clearInterval(interval);
    }, []);

    // Derived lists (minimal change): running (started & active) and upcoming (planned)
    const runningLots = lotsState.filter(l => l.startTimestamp <= Date.now() && l.closing > 0);
    const upcomingLots = lotsState.filter(l => l.startTimestamp > Date.now() && l.status !== 'afgesloten');

    // Keep featuredIndex in range when runningLots length changes
    useEffect(() => {
        if (runningLots.length === 0) {
            setFeaturedIndex(0);
            return;
        }
        if (featuredIndex >= runningLots.length) {
            setFeaturedIndex(runningLots.length - 1);
        }
    }, [runningLots.length, featuredIndex]);

    // IMPORTANT: only show featuredLot when there are running (started) auctions
    const featuredLot = runningLots.length > 0 ? runningLots[featuredIndex] : null;
    const featuredTime = featuredLot?.closing ?? 0;

    // NAV buttons for featured carousel (minimal UI additions)
    const goPrev = () => setFeaturedIndex(i => Math.max(0, i - 1));
    const goNext = () => setFeaturedIndex(i => Math.min(runningLots.length - 1, i + 1));

    // ==========================================================
    // LOGICA VOOR DE POP-UP (MODAL)
    // ==========================================================

    // Stap 1: Open Modal en zet de gegevens klaar
    const handleInitialClick = async () => {
        if (!featuredLot) return;

        // Reset
        setTransactionData({
            veilingId    : featuredLot.veilingID,
            productNaam   : featuredLot.productNaam,
            prijsPerStuk  : featuredLot.currentPrice,
            maxAantal     : featuredLot.hoeveelheid ?? 1,
            aantal        : 1,
            totaalPrijs   : featuredLot.currentPrice,
            history        : null
        });

        setShowModal(true);

        try {
            // PROBEER DE LEVERANCIERNAAM TE VINDEN
            const levNaam = featuredLot.leverancierNaam
                || featuredLot.product?.leverancier?.bedrijf
                || featuredLot.product?.leverancier?.naam
                || "Onbekend";

            const prodNaam = featuredLot.productNaam;

            console.log("Ophalen historie voor:", prodNaam, "van", levNaam);

            const url = `${API_URL}/api/Claim/GetHistory?productNaam=${encodeURIComponent(prodNaam)}&leverancierNaam=${encodeURIComponent(levNaam)}`;

            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                    'Content-Type': 'application/json'
                }
            });

            if(response.ok) {
                const histData = await response.json();
                setTransactionData(prev => ({ ...prev, history: histData }));
            } else {
                console.warn("Historie ophalen mislukt:", response.status);
            }
        } catch (e) {
            console.error("Fout bij ophalen historie:", e);
        }
    };

    // Stap 2: Update functie voor het invulveld IN de modal
    const handleModalQuantityChange = (e) => {
        const inputValue = e.target.value;
        const max = transactionData.maxAantal;

        if (inputValue === "") {
            setTransactionData(prev => ({ ...prev, aantal: "" }));
            return;
        }

        let val = parseInt(inputValue);
        if (isNaN(val)) val = 1;
        if (val > max) val = max;
        if (val < 1) val = 1;

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
            const response = await fetch(`${API_URL}/api/Claim/PlaceClaim`, {
                method: 'POST',
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    veilingId: transactionData.veilingId,
                    aantal   : transactionData.aantal,
                    prijs    : transactionData.prijsPerStuk
                })
            });

            if (response.ok) {
                console.log("Gekocht:", transactionData);

                // UPDATE STATE: Verminder aantal én verwijder als het 0 is
                setLotsState(currentLots => {
                    return currentLots.map(lot => {
                        if (lot.veilingID === transactionData.veilingId) {
                            return {
                                ...lot,
                                hoeveelheid: (lot.hoeveelheid ?? 0) - transactionData.aantal
                            };
                        }
                        return lot;
                    })
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

    const renderDescription = (lot) => {
        const desc = getDescription(lot);
        if (!desc) return '-';
        const id = lot.veilingID ?? lot.productID ?? lot.id ?? JSON.stringify(lot);
        const { text: truncatedText, truncated } = truncateWords(desc, 10);
        const isExpanded = !!expandedDescriptions[id];

        if (!truncated) return desc;

        return (
            <>
                <span>{isExpanded ? desc : truncatedText + '...'}</span>
                <button className="read-more-link" onClick={() => toggleExpanded(id)}>
                    {isExpanded ? 'lees minder' : 'lees meer'}
                </button>
            </>
        );
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

                {/* Keep the same featured layout but add minimal prev/next controls */}
                {featuredLot && (
                    <div style={{ position: 'relative' }}>
                        {/* prev/next buttons - small and unobtrusive */}
                        {runningLots.length > 1 && (
                            <>
                                <button onClick={goPrev} disabled={featuredIndex === 0} style={{ position: 'absolute', left: '-40px', top: '40%', zIndex: 5 }}>◀</button>
                                <button onClick={goNext} disabled={featuredIndex >= runningLots.length - 1} style={{ position: 'absolute', right: '-40px', top: '40%', zIndex: 5 }}>▶</button>
                            </>
                        )}

                        <article className="featured-card">
                            <img
                                src={featuredLot.foto?.startsWith('http') ? featuredLot.foto : `${API_URL}${featuredLot.foto}`}
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
                                {getDescription(featuredLot) && <p className="featured-description">{renderDescription(featuredLot)}</p>}
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
                    </div>
                )}
            </section>

            <section className="dashboard-table">
                <h3>Komende veilingen</h3>
                <div className="table-wrapper" role="region" aria-live="polite">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Veiling</th>
                                <th>Naam</th>
                                <th>Aantal</th>
                                <th>Omschrijving</th>
                                <th>Huidige prijs (€)</th>
                                <th>Startdatum</th>
                                <th>Veiling start over</th> {/* kolomnaam aangepast */}
                                <th>Sluitingstijd</th>
                            </tr>
                        </thead>
                        <tbody>
                            {upcomingLots.map(lot => {
                                const now = Date.now();
                                const timeUntilStart = Math.max(0, Math.ceil((lot.startTimestamp - now) / 1000));
                                const startDateDisplay = lot.startTimestamp ? new Date(lot.startTimestamp).toLocaleString('nl-NL') : '-';

                                return (
                                    <tr key={lot.veilingID}>
                                        <td>{lot.veilingID}</td>
                                        <td>{lot.productNaam}</td>
                                        <td>{lot.hoeveelheid ?? 1}</td>
                                        <td>{renderDescription(lot) || '-'}</td>
                                        <td>€{lot.currentPrice?.toFixed(2)}</td>
                                        <td>{startDateDisplay}</td>
                                        <td>{timeUntilStart}s</td> {/* aftel timer tot start */}
                                        <td>{lot.closing > 0 ? `${lot.closing}s` : 'Afgesloten'}</td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            </section>

            {/* ========================================================== */}
            {/* POP-UP MODAL (Met input en totaalprijs)                    */}
            {/* ========================================================== */}
            {showModal && transactionData && (
                <div className="dashboard-overlay">
                    <div className="dashboard-modal">
                        <h2>Bevestig Aankoop</h2>

                        <div className="dashboard-summary">
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

                                                {/* --- HISTORISCHE DATA VISUALISATIE --- */}
                                                {transactionData.history && (
                                                    <div style={{textAlign: 'left', marginTop: '15px', borderTop: '1px solid #ddd', paddingTop: '10px', maxHeight: '250px', overflowY: 'auto'}}>
                                                        <small><strong>Historie (Laatste 10)</strong></small>
                    
                                                        {/* Eigen Historie */}
                                                        <div style={{marginBottom: '10px'}}>
                                                            <span style={{fontSize: '0.8rem', color: '#555'}}>
                                                                Deze aanbieder: <strong>{featuredLot?.leverancierNaam || "Onbekend"}</strong>
                                                            </span>
                                                            <table style={styles.historyTable}>
                                                                <tbody>
                                                                {transactionData.history.eigenHistorie.map((h, i) => (
                                                                    <tr key={i}>
                                                                        <td>{h.datum}</td>
                                                                        <td align="right">€{h.prijs.toFixed(2)}</td>
                                                                    </tr>
                                                                ))}
                                                                {transactionData.history.eigenHistorie.length === 0 && (
                                                                    <tr><td colSpan="2" style={{fontStyle:'italic'}}>Geen data</td></tr>
                                                                )}
                                                                </tbody>
                                                            </table>
                                                            <div style={{fontSize: '0.75rem', fontWeight: 'bold'}}>
                                                                Gemiddeld: €{transactionData.history.gemiddeldeEigen.toFixed(2)}
                                                            </div>
                                                        </div>

                                    {/* Markt Historie */}
                                    <div>
                                        <span style={{fontSize: '0.8rem', color: '#555'}}>Alle aanbieders:</span>
                                        <table className="dashboard-historyTable">
                                            <tbody>
                                            {transactionData.history.marktHistorie.map((h, i) => (
                                                <tr key={i}>
                                                    <td>{h.aanbieder}</td>
                                                    <td>{h.datum}</td>
                                                    <td align="right">€{h.prijs.toFixed(2)}</td>
                                                </tr>
                                            ))}
                                            {transactionData.history.marktHistorie.length === 0 && (
                                                <tr><td colSpan="3" style={{fontStyle:'italic'}}>Geen data</td></tr>
                                            )}
                                            </tbody>
                                        </table>
                                        <div style={{fontSize: '0.75rem', fontWeight: 'bold'}}>
                                            Markt Gemiddeld: €{transactionData.history.gemiddeldeMarkt.toFixed(2)}
                                        </div>
                                    </div>
                                </div>
                            )}
                            {/* -------------------------------------- */}

                        </div>

                        <div className="dashboard-buttons">
                            <button onClick={() => setShowModal(false)} className="dashboard-cancelBtn">Annuleren</button>
                            <button onClick={handleConfirmPurchase} className="dashboard-confirmBtn">Bevestigen</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

const styles = {
    historyTable: { width: '100%', fontSize: '0.75rem', marginBottom: '5px' }
};

export default DashboardPage;