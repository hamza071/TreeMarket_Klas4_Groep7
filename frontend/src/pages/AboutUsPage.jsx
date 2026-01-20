import React from 'react';
import heroImage from '../assets/img/hero-blossoms.jpg';
export default function AboutPage() {
    return (
        <div className="about-page-wrapper" style={{ width: '100%' }}>

            <main className="container" style={{ maxWidth: '960px', margin: '0 auto', padding: '1rem' }}>
                <article className="main-article">
                    <h1>Over Tree Market</h1>

                    <figure className="hero-image" style={{ textAlign: 'center' }}>
                        <img
                            src={heroImage}
                            alt="Roze kersenbloesems in volle bloei aan takken tegen een heldere lucht."
                            style={{ width: '100%', height: 'auto' }}
                        />
                    </figure>

                    <div className="article-text">
                                               <p>Welkom bij Tree Market, een voorproefje van de nieuwe ruimte voor zowel de natuur als mensen, waar telers, leveranciers en kopers in real time met elkaar verbonden zijn. Bij Tree Market geloven we dat elke steel een verhaal vertelt, en we zetten ons in om een platform te creeren dat inkoop efficient, transparant en eerlijk maakt.</p>
                        <p>Onze missie is eenvoudig: efficientie, transparantie en enthousiasme brengen in de bloemenhandel. We bieden een dynamisch veilingplatform waarmee kopers verse bloemen rechtstreeks bij betrouwbare telers kunnen inkopen, waardoor betere prijzen en hogere kwaliteit voor iedereen worden gegarandeerd. Of je nu een bloemist bent op zoek naar seizoensgebonden varieteiten, een groothandel die grote hoeveelheden nodig heeft, of een teler die nieuwe markten wil bereiken, Tree Market biedt de tools die je nodig hebt om succesvol te zijn.</p>
                        <p>Wat Tree Market onderscheidt, is onze focus op snelheid, vertrouwen en duurzaamheid. Ons platform is ontworpen om transacties te stroomlijnen, verspilling te minimaliseren en eerlijke kansen te creeren voor zowel kopers als verkopers. Terwijl de wereld verandert op het gebied van betalingen en logistiek downstream, zetten wij de bloemenhandel centraal met datagedreven inzichten. Onze inzet gaat verder dan alleen de marktplaats; we ondersteunen onze telers door diversiteit te waarborgen als goed ontwerp en helpen de sector te floreren in een digitaal tijdperk. Ons team werkt onvermoeibaar om een naadloze ervaring te garanderen, van het eerste bod tot de uiteindelijke levering. Word vandaag nog lid van Tree Market en maak deel uit van een bloeiende revolutie. Laten we samen groeien, een bloem tegelijk.</p>
                    </div>
                </article>
            </main>
        </div>
    );
}