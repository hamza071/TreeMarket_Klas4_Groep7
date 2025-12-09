import { useState } from "react";
import { useNavigate } from "react-router-dom";

function AuthPage() {
    const [activeTab, setActiveTab] = useState("register");

    const [formData, setFormData] = useState({
        naam: "",
        email: "",
        telefoonnummer: "",
        wachtwoord: "",
        herhaalWachtwoord: "",
        rol: "",
        bedrijf: "",
        KvKNummer: "",
        IBANnummer: "",
    });

    const [errors, setErrors] = useState({});
    const [serverError, setServerError] = useState("");
    const [serverSuccess, setServerSuccess] = useState("");

    const isRegister = activeTab === "register";
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;

        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));

        setErrors((prev) => ({
            ...prev,
            [name]: "",
        }));
    };

    // ===== VALIDATIE =====
    const validateForm = () => {
        const newErrors = {};

        // Email
        if (!formData.email.trim()) {
            newErrors.email = "Vul uw e-mailadres in.";
        } else {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(formData.email)) {
                newErrors.email = "Vul een geldig e-mailadres in.";
            }
        }

        // Wachtwoord
        if (!formData.wachtwoord) {
            newErrors.wachtwoord = "Vul een wachtwoord in.";
        } else if (formData.wachtwoord.length < 8) {
            newErrors.wachtwoord = "Wachtwoord moet minimaal 8 tekens bevatten.";
        }

        if (isRegister) {
            // Rol
            if (!formData.rol) {
                newErrors.rol = "Selecteer een rol.";
            }

            // Naam (hier zat jouw fout -> nu gefixt)
            if (!formData.naam.trim()) {
                newErrors.naam = "Vul uw naam in.";
            } else if (formData.naam.trim().length < 2) {
                newErrors.naam = "Naam is te kort.";
            }

            // Telefoon
            if (formData.telefoonnummer.trim()) {
                const phoneRegex = /^0[0-9]{9}$/;
                if (!phoneRegex.test(formData.telefoonnummer.trim())) {
                    newErrors.telefoonnummer = "Vul een geldig Nederlands telefoonnummer in.";
                }
            }

            // Bevestig wachtwoord
            if (!formData.herhaalWachtwoord) {
                newErrors.herhaalWachtwoord = "Bevestig uw wachtwoord.";
            } else if (formData.wachtwoord !== formData.herhaalWachtwoord) {
                newErrors.herhaalWachtwoord = "Wachtwoorden komen niet overeen.";
            }

            // Extra velden leverancier
            if (formData.rol === "leverancier") {
                if (!formData.bedrijf.trim()) {
                    newErrors.bedrijf = "Vul de bedrijfsnaam in.";
                }
                if (!formData.KvKNummer.trim()) {
                    newErrors.KvKNummer = "Vul een KvK nummer in.";
                }
                if (!formData.IBANnummer.trim()) {
                    newErrors.IBANnummer = "Vul een IBAN in.";
                }
            }
        }

        return newErrors;
    };

    // ===== SUBMIT FORM =====
    const handleSubmit = async (e) => {
        e.preventDefault();

        setErrors({});
        setServerError("");
        setServerSuccess("");

        const newErrors = validateForm();
        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            setServerError("Controleer de gemarkeerde velden.");
            return;
        }

        // ==================
        // REGISTREREN
        // ==================
        if (isRegister) {
            let endpoint = "";

            switch (formData.rol) {
                case "klant":
                    endpoint = "https://localhost:7054/api/Gebruiker/Klant";
                    break;
                case "leverancier":
                    endpoint = "https://localhost:7054/api/Gebruiker/Leverancier";
                    break;
                case "veilingsmeester":
                    endpoint = "https://localhost:7054/api/Gebruiker/Veilingsmeester";
                    break;
                default:
                    setErrors({ rol: "Selecteer een rol." });
                    return;
            }

            try {
                const response = await fetch(endpoint, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(formData),
                });

                if (!response.ok) {
                    const errorBody = await response.json().catch(() => null);

                    setServerError(
                        errorBody?.message ||
                        "Registratie mislukt. Probeer het later opnieuw."
                    );
                    return;
                }

                setServerSuccess("Registratie succesvol! U kunt nu inloggen.");

                setFormData({
                    naam: "",
                    email: "",
                    telefoonnummer: "",
                    wachtwoord: "",
                    herhaalWachtwoord: "",
                    rol: "",
                    bedrijf: "",
                    KvKNummer: "",
                    IBANnummer: "",
                });
            } catch (err) {
                setServerError("Server niet bereikbaar.");
            }

            return;
        }

        // ==================
        // INLOGGEN
        // ==================
        try {
            const response = await fetch("https://localhost:7054/api/Gebruiker/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    email: formData.email,
                    wachtwoord: formData.wachtwoord,
                }),
            });

            const data = await response.json().catch(() => null);

            if (!response.ok) {
                setServerError(data?.message || "E-mailadres of wachtwoord is onjuist.");
                return;
            }

            // ===== TOKEN OPSLAAN (BELANGRIJK VOOR NAVIGATIE) =====
            if (data.token) {
                localStorage.setItem("token", data.token);
            }
            if (data.rol) localStorage.setItem("rol", data.rol);
            if (data.gebruikerId) localStorage.setItem("gebruikerId", data.gebruikerId);
            if (data.naam) localStorage.setItem("naam", data.naam);
            setServerSuccess("Succesvol ingelogd!");

            // Ga naar Homepagina
            navigate("/home");

        } catch (err) {
            setServerError("Er ging iets mis bij het inloggen.");
        }
    };

    return (
        <div className="auth-page">
            <section className="auth-card">
                <div className="auth-card__form">
                    <header className="auth-header">
                        <h1>{isRegister ? "Registreren" : "Aanmelden"}</h1>
                        <p>Log in of registreer om verder te gaan.</p>
                    </header>

                    <div className="auth-tabs">
                        <button
                            type="button"
                            className={`auth-tab ${isRegister ? "is-inactive" : "is-active"}`}
                            onClick={() => setActiveTab("login")}
                        >
                            Aanmelden
                        </button>
                        <button
                            type="button"
                            className={`auth-tab ${isRegister ? "is-active" : "is-inactive"}`}
                            onClick={() => setActiveTab("register")}
                        >
                            Registreren
                        </button>
                    </div>

                    <form className="auth-form" onSubmit={handleSubmit}>
                        {isRegister && (
                            <>
                                <label className="form-field">
                                    <span>Rol</span>
                                    <select
                                        name="rol"
                                        value={formData.rol}
                                        onChange={handleChange}
                                        className="input-field"
                                    >
                                        <option value="">-- Selecteer rol --</option>
                                        <option value="klant">Klant</option>
                                        <option value="leverancier">Leverancier</option>
                                        <option value="veilingsmeester">Veilingsmeester</option>
                                    </select>
                                    {errors.rol && <small className="error-text">{errors.rol}</small>}
                                </label>

                                <label className="form-field">
                                    <span>Naam</span>
                                    <input
                                        name="naam"
                                        type="text"
                                        value={formData.naam}
                                        onChange={handleChange}
                                        placeholder="Voornaam en achternaam"
                                    />
                                    {errors.naam && <small className="error-text">{errors.naam}</small>}
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
                                    {errors.telefoonnummer && (
                                        <small className="error-text">{errors.telefoonnummer}</small>
                                    )}
                                </label>

                                {formData.rol === "leverancier" && (
                                    <>
                                        <label className="form-field">
                                            <span>Bedrijf</span>
                                            <input
                                                name="bedrijf"
                                                type="text"
                                                value={formData.bedrijf}
                                                onChange={handleChange}
                                                placeholder="Bedrijfsnaam"
                                            />
                                            {errors.bedrijf && (
                                                <small className="error-text">{errors.bedrijf}</small>
                                            )}
                                        </label>

                                        <label className="form-field">
                                            <span>KvK nummer</span>
                                            <input
                                                name="KvKNummer"
                                                type="text"
                                                value={formData.KvKNummer}
                                                onChange={handleChange}
                                                placeholder="KvK nummer"
                                            />
                                            {errors.KvKNummer && (
                                                <small className="error-text">{errors.KvKNummer}</small>
                                            )}
                                        </label>

                                        <label className="form-field">
                                            <span>IBAN</span>
                                            <input
                                                name="IBANnummer"
                                                type="text"
                                                value={formData.IBANnummer}
                                                onChange={handleChange}
                                                placeholder="IBAN nummer"
                                            />
                                            {errors.IBANnummer && (
                                                <small className="error-text">{errors.IBANnummer}</small>
                                            )}
                                        </label>
                                    </>
                                )}
                            </>
                        )}

                        <label className="form-field">
                            <span>E-mailadres</span>
                            <input
                                name="email"
                                type="email"
                                value={formData.email}
                                onChange={handleChange}
                                placeholder="naam@voorbeeld.nl"
                            />
                            {errors.email && <small className="error-text">{errors.email}</small>}
                        </label>

                        <label className="form-field">
                            <span>Wachtwoord</span>
                            <input
                                name="wachtwoord"
                                type="password"
                                value={formData.wachtwoord}
                                onChange={handleChange}
                                placeholder="Minimaal 8 tekens"
                            />
                            {errors.wachtwoord && (
                                <small className="error-text">{errors.wachtwoord}</small>
                            )}
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
                                />
                                {errors.herhaalWachtwoord && (
                                    <small className="error-text">{errors.herhaalWachtwoord}</small>
                                )}
                            </label>
                        )}

                        <button type="submit" className="primary-action full-width">
                            {isRegister ? "Registreren" : "Aanmelden"}
                        </button>

                        {serverError && <p className="form-message form-message--error">{serverError}</p>}
                        {serverSuccess && <p className="form-message form-message--success">{serverSuccess}</p>}
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
    );
}

export default AuthPage;