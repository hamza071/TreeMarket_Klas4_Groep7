import tulpenveld from '../assets/tulpenveld.jpg';

const featuredLot = {
    id: 'A12345',
    title: "Tulpen Mix 'Lente'",
    price: '�24,50',
    quantity: '100 stuks per bos',
    timeLeft: '10s',
}

const upcomingLots = [
    {
        code: 'B12346',
        name: 'Orchidee Phalaenopsis',
        specs: 'Wit, 2 tak',
        lots: '50 stuks',
        closing: '10:05',
    },
    {
        code: 'B12347',
        name: 'Monstera Deliciosa',
        specs: 'Middelgroot',
        lots: '40 stuks',
        closing: '10:08',
    },
    {
        code: 'B12348',
        name: 'Tulpen Royal',
        specs: 'Roze mix',
        lots: '150 stuks',
        closing: '10:15',
    },
]

function DashboardPage() {
    return (
        <div className="dashboard-page">
            <section className="dashboard-hero">
                <div className="hero-copy">
                    <span className="eyebrow">TREE MARKET</span>
                    <h1>De toekomst van bloemen en planten veilingen</h1>
                    <p>
                        Digitale Veilingklok 2025 brengt kopers en kwekers samen in een moderne, effici�nte online
                        veilingomgeving.
                    </p>
                    <button type="button" className="primary-action">
                        Komende kavels bekijken
                    </button>
                </div>
                <article className="featured-card">
                    <img
                        src={tulpenveld}
                        alt="Tulpenveld"
                        className="featured-media"
                    />
                    <div className="featured-body">
                        <div className="featured-meta">
                            <span className="badge badge-live">{featuredLot.timeLeft}</span>
                            <span className="lot-number">#{featuredLot.id}</span>
                        </div>
                        <h2>{featuredLot.title}</h2>
                        <p className="featured-quantity">{featuredLot.quantity}</p>
                        <div className="featured-footer">
                            <span className="featured-price">{featuredLot.price}</span>
                            <button type="button" className="secondary-action">
                                Bieden
                            </button>
                        </div>
                    </div>
                </article>
            </section>

            <section className="dashboard-table">
                <header className="section-header">
                    <h3>Komende Kavels</h3>
                    <button type="button" className="link-button">
                        Alle kavels bekijken
                    </button>
                </header>
                <div className="table">
                    <div className="table-row table-head">
                        <span>Kavel</span>
                        <span>Naam</span>
                        <span>Specificaties</span>
                        <span>Aantal</span>
                        <span>Sluiting</span>
                    </div>
                    {upcomingLots.map((lot) => (
                        <div key={lot.code} className="table-row">
                            <span>{lot.code}</span>
                            <span>{lot.name}</span>
                            <span>{lot.specs}</span>
                            <span>{lot.lots}</span>
                            <span>{lot.closing}</span>
                        </div>
                    ))}
                </div>
            </section>
        </div>
    )
}
///
export default DashboardPage