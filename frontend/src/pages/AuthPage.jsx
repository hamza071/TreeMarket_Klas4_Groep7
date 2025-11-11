import { useState } from 'react'

const registerFields = [
    { label: 'Naam', placeholder: 'Voornaam', type: 'text' },
    { label: 'Achternaam', placeholder: 'Achternaam', type: 'text' },
    { label: 'Email', placeholder: 'naam@voorbeeld.nl', type: 'email' },
    { label: 'Wachtwoord', placeholder: '●●●●●●●●', type: 'password' },
    { label: 'Bevestig wachtwoord', placeholder: '●●●●●●●●', type: 'password' },
]

const loginFields = [
    { label: 'Email', placeholder: 'naam@voorbeeld.nl', type: 'email' },
    { label: 'Wachtwoord', placeholder: '●●●●●●●●', type: 'password' },
]

function AuthPage() {
    const [activeTab, setActiveTab] = useState('register')

    const isRegister = activeTab === 'register'
    const fields = isRegister ? registerFields : loginFields

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

                    <form className="auth-form" onSubmit={(event) => event.preventDefault()}>
                        {fields.map((field) => (
                            <label key={field.label} className="form-field">
                                <span>{field.label}</span>
                                <input type={field.type} placeholder={field.placeholder} />
                            </label>
                        ))}

                        <label className="checkbox-field">
                            <input type="checkbox" defaultChecked />
                            <span>Aangemeld blijven</span>
                        </label>

                        {!isRegister && (
                            <button type="button" className="link-button align-right">
                                Wachtwoord vergeten?
                            </button>
                        )}

                        <button type="submit" className="primary-action full-width">
                            {isRegister ? 'Registreren' : 'Aanmelden'}
                        </button>
                    </form>
                </div>

                <div className="auth-card__media">
                    <img
                        src={
                            isRegister
                                ? 'https://images.unsplash.com/photo-1545243424-0ce743321e11?auto=format&fit=crop&w=800&q=80'
                                : 'https://images.unsplash.com/photo-1517427294546-5aaefec2b2bb?auto=format&fit=crop&w=800&q=80'
                        }
                        alt={isRegister ? 'Bloemenkweker in het veld' : 'Bloemenverzorger met tulpen'}
                    />
                </div>  
            </section>
            <footer className="auth-footer">© Royal Flora 2025</footer>
        </div>
    )
}

export default AuthPage