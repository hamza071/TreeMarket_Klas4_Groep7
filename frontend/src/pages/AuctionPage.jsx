import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "../assets/css/AuctionPage.css";
import { API_URL } from '../DeployLocal';

function AuctionPage() {
    const [lots, setLots] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [expanded, setExpanded] = useState({});

    const navigate = useNavigate();

    const truncateWords = (text, maxWords = 10) => {
        if (!text) return { text: '', truncated: false };
        const words = text.split(/\s+/).filter(Boolean);
        if (words.length <= maxWords) return { text, truncated: false };
        return { text: words.slice(0, maxWords).join(' '), truncated: true };
    };

    // ------------------------------
    // Fetch kavels bij mount
    // ------------------------------
    useEffect(() => {
        const fetchLots = async () => {
            try {
                const response = await fetch(`${API_URL}/api/Product/vandaag`, {
                    method: "GET",
                    headers: { Accept: "application/json" },
                });

                if (!response.ok) throw new Error(`Server error: ${response.status}`);

                const data = await response.json();
                const list = Array.isArray(data) ? data : [data];

                // 1️⃣ Filter eerst lokaal producten met hoeveelheid > 0
                const activeLots = list.filter(l => (l.hoeveelheid ?? 0) > 0);
                setLots(activeLots);

                // 2️⃣ Automatisch verwijderen van zero-quantity producten (async, geen rerender)
                const zeroLots = list.filter(l => (l.hoeveelheid ?? 0) <= 0);
                if (zeroLots.length > 0) {
                    removeZeroQuantityProducts(zeroLots); // ⚡ draait op de achtergrond
                }

            } catch (err) {
                console.error("Error fetching lots:", err);
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchLots();
    }, []);

    // ------------------------------
    // DELETE functie voor zero-quantity producten
    // ------------------------------
    const removeZeroQuantityProducts = async (productList) => {
        if (!Array.isArray(productList) || productList.length === 0) return;

        const token = localStorage.getItem('token');

        const deletions = productList.map(async p => {
            try {
                const headers = token ? { Authorization: `Bearer ${token}` } : {};
                const resp = await fetch(`${API_URL}/api/Product/${p.productId}`, {
                    method: 'DELETE',
                    headers
                });

                if (!resp.ok) {
                    const text = await resp.text();
                    console.warn(`Kon product ${p.productId} niet verwijderen:`, resp.status, text);
                    return { id: p.productId, ok: false };
                }

                console.log(`Product ${p.productId} verwijderd (hoeveelheid 0).`);
                return { id: p.productId, ok: true };
            } catch (err) {
                console.error(`Fout bij verwijderen product ${p.productId}:`, err);
                return { id: p.productId, ok: false };
            }
        });

        // Wacht tot alle DELETE requests klaar zijn, zonder UI te blokkeren
        const results = await Promise.allSettled(deletions);
        const succeededIds = results
            .filter(r => r.status === 'fulfilled' && r.value?.ok)
            .map(r => r.value.id);

        if (succeededIds.length > 0) console.log('Verwijderde producten:', succeededIds);
    };

    // ------------------------------
    // UI
    // ------------------------------
    if (loading) return <p>Kavels worden geladen…</p>;
    if (error) return <p style={{ color: "red" }}>{error}</p>;

    const pendingLots = lots.filter(
        (lot) => lot.status?.toLowerCase() === "pending" || !lot.status
    );

    if (pendingLots.length === 0) {
        return <p>Geen kavels beschikbaar om te publiceren.</p>;
    }

    const handleStartVeiling = (lot) => {
        navigate(`/veiling/${lot.productId}`, { state: { lot } });
    };

    const toggle = (id) => setExpanded(prev => ({ ...prev, [id]: !prev[id] }));

    return (
        <div className="auction-page">
            <header className="section-header">
                <h1>Te publiceren kavels</h1>
                <p>Bekijk de kavels die door leveranciers zijn toegevoegd.</p>
            </header>

            <div className="auction-grid">
                {pendingLots.map((lot) => {
                    const desc = lot.omschrijving || lot.Omschrijving || '';
                    const { text: truncatedText, truncated } = truncateWords(desc, 10);
                    const isExpanded = !!expanded[lot.productId];

                    return (
                        <article key={lot.productId} className="auction-card">
                            <h2>{lot.naam || "Geen naam"}</h2>
                            <p>
                                {truncated && !isExpanded ? `${truncatedText}...` : desc}
                                {truncated && (
                                    <button className="read-more-link" onClick={() => toggle(lot.productId)}>
                                        {isExpanded ? 'lees minder' : 'lees meer'}
                                    </button>
                                )}
                            </p>
                            <p>{lot.hoeveelheid || 0} stuks</p>
                            {lot.varieteit && <p>Variëteit: {lot.varieteit}</p>}

                            <img
                                src={
                                    lot.foto
                                        ? lot.foto.startsWith("http")
                                            ? lot.foto
                                            : `${API_URL}${lot.foto}`
                                        : "/images/default.png"
                                }
                                alt={lot.naam || "Productfoto"}
                                className="auction-card-image"
                            />

                            {lot.leverancierNaam && <p>Leverancier: {lot.leverancierNaam}</p>}

                            <button
                                className="primary-action"
                                onClick={() => handleStartVeiling(lot)}
                            >
                                Start veiling
                            </button>
                        </article>
                    );
                })}
            </div>
        </div>
    );
}

export default AuctionPage;