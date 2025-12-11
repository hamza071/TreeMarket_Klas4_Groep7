const metrics = [
    { label: 'Totale omzet deze week', value: '€128.400', trend: '+12% ten opzichte van vorige week' },
    { label: 'Actieve kopers', value: '356', trend: '+24 nieuwe registraties' },
    { label: 'Gemiddelde verkooptijd', value: '06:45', trend: 'Sneller dan 86% van de kavels' },
]

const highlights = [
    {
        title: 'Populairste productcategorie',
        detail: 'Snijbloemen en tulpen',
    },
    {
        title: 'Beste presterende kweker',
        detail: 'GreenBloom Cooperative',
    },
    {
        title: 'Hoogste verkoopprijs',
        detail: '€86,00 voor Rozen Avalanche',
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

            <section aria-labelledby="metrics-heading" className="metrics-grid">
                <h2 id="metrics-heading" className="sr-only">
                    Kerncijfers van deze week
                </h2>
                {metrics.map((metric) => (
                    <article key={metric.label} className="metric-card" aria-label={metric.label}>
                        <dl>
                            <div>
                                <dt className="sr-only">Waarde</dt>
                                <dd aria-live="polite">{metric.value}</dd>
                            </div>
                            <div>
                                <dt className="metric-label">{metric.label}</dt>
                                <dd className="metric-trend">{metric.trend}</dd>
                            </div>
                        </dl>
                    </article>
                ))}
            </section>

            <section aria-labelledby="highlights-heading" className="highlights">
                <h2 id="highlights-heading">Belangrijkste inzichten</h2>
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
