import React from 'react';
import heroImage from '../assets/img/hero-blossoms.jpg';

export default function AboutPage() {
    return (
        <div className="about-page-wrapper" style={{ width: '100%' }}>

            <main className="container" style={{ maxWidth: '960px', margin: '0 auto', padding: '1rem' }}>
                <article className="main-article">
                    <h1>About Tree Market</h1>
                    <p className="subheading">

                    </p>

                    <figure className="hero-image" style={{ textAlign: 'center' }}>
                        {/* HIER IS DE WIJZIGING VOOR RESPONSIVENESS: */}
                        <img
                            src={heroImage}
                            alt="Roze kersenbloesems in volle bloei aan takken tegen een heldere lucht."
                            style={{ width: '100%', height: 'auto' }}
                        />
                    </figure>

                    <div className="article-text">
                        <p>Welcome to Tree Market, a preview of the new space for both Nature and people, where growers, suppliers, and buyers connect in real time. At Tree Market, we believe that every stem tells a story, and we are committed to creating a platform that makes procurement efficient, transparent, and fair.</p>
                        <p>Our mission is simple: to bring efficiency, transparency, and excitement to the flower trade. We provide a dynamic auction system that allows buyers to source fresh flowers directly from trusted growers, ensuring better prices and higher quality for everyone involved. Whether you are a florist looking for seasonal varieties, a wholesaler needing large quantities, or a grower seeking to reach new markets, Tree Market offers the tools you need to succeed.</p>
                        <p>What sets Tree Market apart is our focus on speed, trust, and sustainability. Our platform is designed to streamline transactions, minimize waste, and create fair opportunities for buyers and sellers alike. While the world is shifting around payments, and downstream logistics, we make the flower trade an accent on data-driven insights. Our commitment extends beyond the marketplace; we are dedicated to supporting our growers by ensuring diversity is great design, and helping the industry thrive in a digital era. Our team works tirelessly to ensure a seamless experience, from the first bid to the final delivery. Join Tree Market today and be part of a blooming revolution. Let's grow together, one flower at a time.</p>
                    </div>
                </article>
            </main>
        </div>
    );
}