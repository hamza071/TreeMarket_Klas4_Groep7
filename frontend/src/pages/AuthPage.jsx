import { useState } from 'react'

function AuthPage() {
    const [activeTab, setActiveTab] = useState('register')
    //Hier wordt de data met de DTO Tabel
    //---Pss DTO is toegepast .....-----
    const [formData, setFormData] = useState({
        naam: "",
        email: "",
        telefoonnummer: "",
        wachtwoord: "",
        herhaalWachtwoord: "",
    })

    const isRegister = activeTab === 'register'

    //const loginFields = [
    //    { id: 'login-email', label: 'E-mailadres', placeholder: 'naam@voorbeeld.nl', type: 'email', autoComplete: 'email' },
    //    { id: 'login-password', label: 'Wachtwoord', placeholder: 'Je wachtwoord', type: 'password', autoComplete: 'current-password' },
    //]


    //De formData en setFormData wordt gebruikt
    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    }

    // Formulier wordt verzonden
    const handleSubmit = async (e) => {
        e.preventDefault();

        //_____Registratie________
        //Controlleerd of het wachtwoord en herhaal wachtwoord overeenkomt
        if (isRegister && formData.wachtwoord !== formData.herhaalWachtwoord) {
            alert("Wachtwoorden komen niet overeen.");
            return;
        }

        //Wanneer de gebruiker registreerd
        if (isRegister) {
            try {
                //Voor nu maakt die alleen van de klant aan.
                //Fetch wordt gebruikt om gegevens op te halen of te versturen. In dit geval stuurt hij de gegevens op.
                const response = await fetch("https://localhost:7054/api/Gebruiker/Klant", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(formData),
                });

                if (response.ok) {
                    alert("Registratie succesvol!");
                    //Als het succesvol is gegaan, dan wordt de gegevens binnen dezelfde pagina gereset
                    setFormData({ naam: "", email: "", telefoonnummer: "", wachtwoord: "", herhaalWachtwoord: "" });

                } else {
                    const error = await response.json();
                    alert("Fout: " + (error.message || JSON.stringify(error)));
                }
            } catch (err) {
                console.error(err);
                alert("Er ging iets mis bij het registreren.");
            }
        }
        //______Einde van registratie______

        ////______Login_________
        else {
            // Aanmelden — hier kun je later een login endpoint gebruiken
            // TODO: maak een aparte DTO voor Login aan!
            //if (isLogin) {
            //}


            ////______Einde van login
            alert(`Inloggen met ${formData.email}`);
        }




    };

    ////Controlleerd of je register of login page gaat gebruiken.

    return (
        <div className="auth-page">
            <section className="auth-card">
                <div className="auth-card__form">
                    <header className="auth-header">
                        <h1>{isRegister ? 'Registreren bij TreeMarket' : 'Aanmelden bij TreeMarket'}</h1>
                        <p>
                            Voordat u doorgaat, moet u zich aanmelden of registreren als u nog geen account heeft.
                        </p>
                    </header>

                    {/*Deze knoppen zorgen ervoor dat je naar registratie of naar login kunt navigeren */}
                    <div className="auth-tabs">
                        <button
                            type="button"
                            className={`auth-tab ${isRegister ? 'is-inactive' : 'is-active'}`}
                            onClick={() => setActiveTab('login')}
                        >
                            Aanmelden
                        </button>
                        <button
                            type="button"
                            className={`auth-tab ${isRegister ? 'is-active' : 'is-inactive'}`}
                            onClick={() => setActiveTab('register')}
                        >
                            Registreren
                        </button>
                    </div>

                    {/*//Formulier om de data te sturen.*/}
                    <form className="auth-form" onSubmit={handleSubmit}>
                        {isRegister && (
                            <>
                                <label className="form-field">
                                    <span>Naam</span>
                                    <input
                                        name="naam"
                                        type="text"
                                        value={formData.naam}
                                        onChange={handleChange}
                                        placeholder="Voornaam en Achternaam"
                                        required
                                    />
                                </label>

                                <label className="form-field">
                                    <span>Telefoonnummer</span>
                                    <input
                                        name="telefoonnummer"
                                        type="tel"
                                        value={formData.telefoonnummer}
                                        onChange={handleChange}
                                        placeholder="0612345678"
                                    />
                                </label>
                            </>
                        )}

                        {/*Deze code wordt bij zowel aanmelden en register toegepast*/}
                        <label className="form-field">
                            <span>E-mailadres</span>
                            <input
                                name="email"
                                type="email"
                                value={formData.email}
                                onChange={handleChange}
                                placeholder="naam@voorbeeld.nl"
                                required
                            />
                        </label>

                        <label className="form-field">
                            <span>Wachtwoord</span>
                            <input
                                name="wachtwoord"
                                type="password"
                                value={formData.wachtwoord}
                                onChange={handleChange}
                                placeholder="Minimaal 8 tekens"
                                required
                            />
                        </label>

                        {isRegister && (
                            <label className="form-field">
                                <span>Bevestig wachtwoord</span>
                                <input
                                    name="herhaalWachtwoord"
                                    type="password"
                                    value={formData.herhaalWachtwoord}
                                    onChange={handleChange}
                                    placeholder="Nogmaals wachtwoord"
                                    required
                                />
                            </label>
                        )}

                        {/*Nadat je gegevens ingevuld hebt, kun je op deze knop drukken*/}
                        <button type="submit" className="primary-action full-width">
                            {isRegister ? "Registreren" : "Aanmelden"}
                        </button>
                    </form>
                </div>

                <div className="auth-card__media">
                    <img
                        src={
                            isRegister
                                ? "https://images.unsplash.com/photo-1545243424-0ce743321e11?auto=format&fit=crop&w=800&q=80"
                                : "https://images.unsplash.com/photo-1517427294546-5aaefec2b2bb?auto=format&fit=crop&w=800&q=80"
                        }
                        alt={isRegister ? "Bloemenkweker in het veld" : "Bloemenverzorger met tulpen"}
                    />
                </div>
            </section>
            <footer className="auth-footer">© Royal Flora 2025</footer>
        </div>
    )
}

export default AuthPage