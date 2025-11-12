import { useState } from 'react'

const registerFields = [
    { id: 'first-name', label: 'Voornaam', placeholder: 'Voornaam', type: 'text', autoComplete: 'given-name' },
    { id: 'last-name', label: 'Achternaam', placeholder: 'Achternaam', type: 'text', autoComplete: 'family-name' },
    { id: 'email', label: 'E-mailadres', placeholder: 'naam@voorbeeld.nl', type: 'email', autoComplete: 'email' },
    { id: 'password', label: 'Wachtwoord', placeholder: 'Minimaal 8 tekens', type: 'password', autoComplete: 'new-password' },
    {
        id: 'confirm-password',
        label: 'Bevestig wachtwoord',
        placeholder: 'Nogmaals wachtwoord',
        type: 'password',
        autoComplete: 'new-password',
    },
]

const loginFields = [
    { id: 'login-email', label: 'E-mailadres', placeholder: 'naam@voorbeeld.nl', type: 'email', autoComplete: 'email' },
    { id: 'login-password', label: 'Wachtwoord', placeholder: 'Je wachtwoord', type: 'password', autoComplete: 'current-password' },
]

function AuthPage() {
    const [activeTab, setActiveTab] = useState('register')

    const isRegister = activeTab === 'register'
    const fields = isRegister ? registerFields : loginFields

    return (
        <div className="auth-page">
            <section className="auth-card" aria-labelledby="auth-heading">
                <div className="auth-card__form">
                    <header className="auth-header">
                        <h1 id="auth-heading">
                            {isRegister ? 'Registreren bij TreeMarket' : 'Aanmelden bij TreeMarket'}
                        </h1>
                        <p>
                            Voordat u doorgaat, moet u zich aanmelden of registreren als u nog geen account heeft.
                        </p>
                    </header>

                    <div className="auth-tabs" role="tablist" aria-label="Authenticatietypen">
                        <button
                            type="button"
                            className={`auth-tab ${isRegister ? 'is-inactive' : 'is-active'}`}
                            role="tab"
                            aria-selected={!isRegister}
                            aria-controls="login-panel"
                            id="login-tab"
                            onClick={() => setActiveTab('login')}
                        >
                            Aanmelden
                        </button>
                        <button
                            type="button"
                            className={`auth-tab ${isRegister ? 'is-active' : 'is-inactive'}`}
                            role="tab"
                            aria-selected={isRegister}
                            aria-controls="register-panel"
                            id="register-tab"
                            onClick={() => setActiveTab('register')}
                        >
                            Registreren
                        </button>
                    </div>

                    <form
                        className="auth-form"
                        aria-labelledby={isRegister ? 'register-tab' : 'login-tab'}
                        id={isRegister ? 'register-panel' : 'login-panel'}
                        role="tabpanel"
                        onSubmit={(event) => event.preventDefault()}
                    >
                        {fields.map((field) => (
                            <label key={field.id} className="form-field" htmlFor={field.id}>
                                <span className="form-label">{field.label}</span>
                                <input
                                    id={field.id}
                                    type={field.type}
                                    name={field.id}
                                    placeholder={field.placeholder}
                                    autoComplete={field.autoComplete}
                                    required
                                />
                            </label>
                        ))}

                        <label className="checkbox-field" htmlFor="stay-logged-in">
                            <input id="stay-logged-in" type="checkbox" defaultChecked />
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

                <div className="auth-card__media" aria-hidden="true">
                    <img
                        src={
                            isRegister
                                ? 'https://images.unsplash.com/photo-1545243424-0ce743321e11?auto=format&fit=crop&w=800&q=80'
                                : 'https://images.unsplash.com/photo-1517427294546-5aaefec2b2bb?auto=format&fit=crop&w=800&q=80'
                        }
                        alt=""
                    />
                </div>
            </section>
            <footer className="auth-footer">© Royal Flora 2025</footer>
        </div>
    )
}

export default AuthPage