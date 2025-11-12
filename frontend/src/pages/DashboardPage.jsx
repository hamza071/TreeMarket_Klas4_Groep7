import tulpenveld from '../assets/img/tulpenveld.jpg'

const featuredLot = {
    id: 'A12345',
    title: "Tulpen Mix 'Lente'",
    price: '€24,50',
    quantity: '100 stelen per bos',
    timeLeft: '10 seconden',
}

const upcomingLots = [
    {
        code: 'B12346',
        name: 'Orchidee Phalaenopsis',
        specs: 'Wit, 2 takken',
        lots: '50 bossen',
        closing: '10:05',
    },
    {
        code: 'B12347',
        name: 'Monstera Deliciosa',
        specs: 'Middelgroot',
        lots: '40 planten',
        closing: '10:08',
    },
    {
        code: 'B12348',
        name: 'Tulpen Royal',
        specs: 'Roze mix',
        lots: '150 bossen',
        closing: '10:15',
    },
]

function DashboardPage() {
    return (
        <div className="dashboard-page">
            <section className="dashboard-hero">
                <div className="hero-copy">
                    <span className="eyebrow">TREE MARKET</span>
                    <h1>De toekomst van bloemen en plantenveilingen</h1>
                    <p>
                        Digitale Veilingklok 2025 brengt kopers en kwekers samen in een moderne, efficiënte online
                        veilingomgeving.
                    </p>
                    <button type="button" className="primary-action">
                        Komende kavels bekijken
                    </button>
                </div>
                <article aria-label="Uitgelichte kavel" className="featured-card">
                    <img src={tulpenveld} alt="Tulpenveld in bloei" className="featured-media" />
                    <div className="featured-body">
                        <div className="featured-meta" aria-live="polite">
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

            <section aria-labelledby="upcoming-lots-heading" className="dashboard-table">
                <header className="section-header">
                    <h3 id="upcoming-lots-heading">Komende kavels</h3>
                    <button type="button" className="link-button">
                        Alle kavels bekijken
                    </button>
                </header>
                <div className="table-wrapper" role="region" aria-live="polite">
                    <table className="data-table">
                        <caption className="sr-only">Overzicht van kavels die binnenkort sluiten</caption>
                        <thead>
                            <tr>
                                <th scope="col">Kavel</th>
                                <th scope="col">Naam</th>
                                <th scope="col">Specificaties</th>
                                <th scope="col">Aantal</th>
                                <th scope="col">Sluiting</th>
                            </tr>
                        </thead>
                        <tbody>
                            {upcomingLots.map((lot) => (
                                <tr key={lot.code}>
                                    <th scope="row">{lot.code}</th>
                                    <td>{lot.name}</td>
                                    <td>{lot.specs}</td>
                                    <td>{lot.lots}</td>
                                    <td>{lot.closing}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </section>
        </div>
    )
}

export default DashboardPage