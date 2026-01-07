import heroImage from '../assets/img/tulpenveld.jpg'
import toadImage from '../assets/img/MLG Toad.jpg'
import { useNavigate } from "react-router-dom";


const featuredCollections = [
    {
        id: 'collection-fresh',
        title: 'Voorjaarscollectie',
        description: 'Heldere kleuren en stevige stelen voor het nieuwe seizoen.',
        image: toadImage,
        imageAlt: 'Decoratieve bloemcompositie met frisse kleuren',
    },
    {
        id: 'collection-green',
        title: 'Groenblijvers',
        description: 'Sterke kamerplanten die weinig verzorging nodig hebben.',
        image: toadImage,
        imageAlt: 'Sierplant in moderne pot',
    },
]

function HomePage() {
    const navigate = useNavigate();

    return (
        <main className="home-page">
            <section className="home-hero" aria-labelledby="home-hero-heading">
                <div className="home-hero__content">
                    <h1 id="home-hero-heading">Welkom bij Tree Market</h1>
                    <p>
                        Vind duurzame bloemen- en plantenkavels voor elke gelegenheid. Dankzij duidelijke labels, toetsenbordnavigatie en
                        hoge contrasten is deze demo toegankelijk voor iedereen.
                    </p>
                    <div className="home-hero__actions">
                        <button type="button" className="primary-action" onClick={() => navigate("/veiling")}>Bekijk veilingen</button>
                        <button type="button" className="secondary-action" onClick={() => navigate("/about")}>Ontdek ons verhaal</button>
                    </div>
                </div>
                <img className="home-hero__image" src={heroImage} alt="Uitgestrekt tulpenveld onder een blauwe lucht" />
            </section>

            <section className="home-section" aria-labelledby="collections-heading">
                <div className="section-header">
                    <h2 id="collections-heading">Uitgelichte collecties</h2>
                    <p>Blader door onze populairste kavels en stel eenvoudig je favorietenlijst samen.</p>
                </div>
                <div className="home-grid">
                    {featuredCollections.map((collection) => (
                        <article key={collection.id} className="home-card">
                            <img src={collection.image} alt={collection.imageAlt} className="home-card__image" />
                            <div className="home-card__body">
                                <h3>{collection.title}</h3>
                                <p>{collection.description}</p>
                                <button type="button" className="link-button">
                                    Bekijk details
                                </button>
                            </div>
                        </article>
                    ))}
                </div>
            </section>

            <section className="home-section" aria-labelledby="why-heading">
                <div className="section-header">
                    <h2 id="why-heading">Waarom Tree Market?</h2>
                </div>
                <ul className="home-benefits">
                    <li>
                        <h3>Directe koppeling</h3>
                        <p>Kopers en kwekers doen zaken zonder omwegen en met transparante prijzen.</p>
                    </li>
                    <li>
                        <h3>Toegankelijk ontwerp</h3>
                        <p>Alt-teksten, labels en toetsenbordbediening maken de demo bruikbaar voor iedereen.</p>
                    </li>
                    <li>
                        <h3>Voorbereid op groei</h3>
                        <p>De mock-API en toekomstige C#-koppeling sluiten naadloos aan op dit ontwerp.</p>
                    </li>
                </ul>
            </section>
        </main>
    )
}

export default HomePage