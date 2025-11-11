const metrics = [
    { label: 'Totale omzet deze week', value: '€ 128.400', trend: '+12% t.o.v. vorige week' },
    { label: 'Actieve kopers', value: '356', trend: '+24 nieuwe registraties' },
    { label: 'Gemiddelde verkooptijd', value: '06:45', trend: 'Sneller dan 86% van de kavels' },
]

const highlights = [
    {
        title: 'Populairste productcategorie',
        detail: 'Snijbloemen & Tulpen',
    },
    {
        title: 'Beste presterende kweker',
        detail: 'GreenBloom Cooperative',
    },
    {
        title: 'Hoogste verkoopprijs',
        detail: '€ 86,00 voor Rozen Avalanche',
    },
]

function ReportsPage() {
    return (
        <div className="reports-page">
            <header className="section-header">
                <div>
                    <h1>Rapporten</h1>
                    <p>Blijf op de hoogte van de prestaties van TreeMarket met realtime statistieken.</p>
                </div>
                <button type="button" className="primary-action">
                    Download rapport
                </button>
            </header>

            <section className="metrics-grid">
                {metrics.map((metric) => (
                    <article key={metric.label} className="metric-card">
                        <h2>{metric.value}</h2>
                        <p className="metric-label">{metric.label}</p>
                        <span className="metric-trend">{metric.trend}</span>
                    </article>
                ))}
            </section>

            <section className="highlights">
                <h2>Belangrijkste inzichten</h2>
                <div className="highlight-grid">
                    {highlights.map((highlight) => (
                        <article key={highlight.title} className="highlight-card">
                            <h3>{highlight.title}</h3>
                            <p>{highlight.detail}</p>
                        </article>
                    ))}
                </div>
            </section>
        </div>
    )
}

export default ReportsPage