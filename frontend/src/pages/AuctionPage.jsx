const auctionLots = [
    {
        id: 'C45610',
        title: 'Gerbera Sunshine',
        description: 'Frisse gele gerbera’s, 60cm stelen',
        price: '€18,20',
        bidders: 8,
    },
    {
        id: 'C45611',
        title: 'Lavendel Provence',
        description: 'Geurige lavendel, 40 stuks per krat',
        price: '€14,50',
        bidders: 5,
    },
    {
        id: 'C45612',
        title: 'Eucalyptus Silver Dollar',
        description: 'Vers gesneden groen, bundels van 25',
        price: '€19,95',
        bidders: 12,
    },
]

function AuctionPage() {
    return (
        <div className="auction-page">
            <header className="section-header">
                <div>
                    <h1>Veilingoverzicht</h1>
                    <p>Volg live kavels en bied mee op de populairste producten van vandaag.</p>
                </div>
                <button type="button" className="primary-action">
                    Live klok openen
                </button>
            </header>

            <div className="auction-grid">
                {auctionLots.map((lot) => (
                    <article key={lot.id} className="auction-card">
                        <div className="auction-card__header">
                            <span className="lot-number">#{lot.id}</span>
                            <span className="badge badge-muted">{lot.bidders} bieders</span>
                        </div>
                        <h2>{lot.title}</h2>
                        <p>{lot.description}</p>
                        <div className="auction-card__footer">
                            <span className="featured-price">{lot.price}</span>
                            <button type="button" className="secondary-action">
                                Bied mee
                            </button>
                        </div>
                    </article>
                ))}
            </div>
        </div>
    )
}

export default AuctionPage
