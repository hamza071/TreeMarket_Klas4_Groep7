import { useState } from "react";
//Importeren zodat je het kunt gebruiken om te navigeren.
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

    // Frontend fouten per veld
    const [errors, setErrors] = useState({});
    // Meldingen (zoals andere sites: algemene fout of succes)
    const [serverError, setServerError] = useState("");
    const [serverSuccess, setServerSuccess] = useState("");

    const isRegister = activeTab === "register";

    //Deze variabele wordt gebruikt om na het inloggen te navigeren naar de home pagina.
    const navigate = useNavigate();


    const handleChange = (e) => {
        const { name, value } = e.target;

        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));

        // zodra gebruiker typt, foutmelding van dat veld weghalen
        setErrors((prev) => ({
            ...prev,
            [name]: "",
        }));
    };

    // ===== LOGISCHE VALIDATIE ZOALS ANDERE SITES =====
    const validateForm = () => {
        const newErrors = {};

        // E-mail
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

            // Naam
            if (!formData.naam.trim()) {
                newErrors.naam = "Vul uw naam in.";
            } else if (formData.naam.trim().length < 2) {
                newErrors.naam = "Naam is te kort.";
            }

            // Telefoon (optioneel, maar als ingevuld dan check)
            if (formData.telefoonnummer.trim()) {
                const phoneRegex = /^0[0-9]{9}$/; // NL 10 cijfers beginnend met 0
                if (!phoneRegex.test(formData.telefoonnummer.trim())) {
                    newErrors.telefoonnummer = "Vul een geldig Nederlands telefoonnummer in (10 cijfers).";
                }
            }

            // Bevestig wachtwoord
            if (!formData.herhaalWachtwoord) {
                newErrors.herhaalWachtwoord = "Bevestig uw wachtwoord.";
            } else if (formData.wachtwoord !== formData.herhaalWachtwoord) {
                newErrors.herhaalWachtwoord = "Wachtwoorden komen niet overeen.";
            }

            // Extra checks voor leverancier
            if (formData.rol === "leverancier") {
                if (!formData.bedrijf.trim()) {
                    newErrors.bedrijf = "Vul de bedrijfsnaam in.";
                }
                if (!formData.KvKNummer.trim()) {
                    newErrors.KvKNummer = "Vul een KvK-nummer in.";
                }
                if (!formData.IBANnummer.trim()) {
                    newErrors.IBANnummer = "Vul een IBAN in.";
                }
            }
        }

        return newErrors;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setErrors({});
        setServerError("");
        setServerSuccess("");

        const newErrors = validateForm();

        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            setServerError("Controleer de gemarkeerde velden en probeer het opnieuw.");
            return;
        }

        if (isRegister) {
            // ========== REGISTREREN ==========
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

                    if (response.status === 400 && errorBody?.errors) {
                        setServerError("Registratie mislukt door ongeldige invoer.");
                    } else {
                        setServerError(
                            errorBody?.message ||
                            "Registratie mislukt. Probeer het later opnieuw."
                        );
                    }
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
                console.error(err);
                setServerError("Er ging iets mis bij het registreren (server niet bereikbaar).");
            }
        } else {
            // ========== INLOGGEN ==========
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
                console.log("Login response:", response, data);

                //Controlleert of de gegevens wel overeenkomen (wordt al in de gebruikerscontroller gedaan)
                if (!response.ok) {
                    setServerError(
                        data?.message || "E-mailadres of wachtwoord is onjuist."
                    );
                    return;
                }

                console.log("Login response:", data);
                setServerSuccess("Succesvol ingelogd!");

                // Navigeren naar home
                navigate("/home");

            } catch (err) {
                console.error(err);
                setServerError("Er ging iets mis bij het inloggen (server niet bereikbaar).");
            }
        }
    };

    return (
        <div className="auth-page">
            <section className="auth-card">
                <div className="auth-card__form">
                    <header className="auth-header">
                        <h1>{isRegister ? "Registreren bij TreeMarket" : "Aanmelden bij TreeMarket"}</h1>
                        <p>
                            Voordat u doorgaat, moet u zich aanmelden of registreren als u nog geen account heeft.
                        </p>
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
                                {/* Rol */}
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

                                {/* Naam */}
                                <label className="form-field">
                                    <span>Naam</span>
                                    <input
                                        name="naam"
                                        type="text"
                                        value={formData.naam}
                                        onChange={handleChange}
                                        placeholder="Voornaam en Achternaam"
                                    />
                                    {errors.naam && <small className="error-text" style={{ color: "red" }}>{errors.naam}</small>}
                                </label>

                                {/* Telefoonnummer */}
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
                                        <small className="error-text" style={{ color: "red" }}>{errors.telefoonnummer}</small>
                                    )}
                                </label>

                                {/* Extra velden voor Leverancier */}
                                {formData.rol === "leverancier" && (
                                    <>
                                        <label className="form-field">
                                            <span>Bedrijf</span>
                                            <input
                                                name="bedrijf"
                                                type="text"
                                                value={formData.bedrijf || ""}
                                                onChange={handleChange}
                                                placeholder="Bedrijfsnaam"
                                            />
                                            {errors.bedrijf && (
                                                <small className="error-text" style={{ color: "red" }}>{errors.bedrijf}</small>
                                            )}
                                        </label>

                                        <label className="form-field">
                                            <span>KvK Nummer</span>
                                            <input
                                                name="KvKNummer"
                                                type="text"
                                                value={formData.KvKNummer || ""}
                                                onChange={handleChange}
                                                placeholder="KvK nummer"
                                            />
                                            {errors.KvKNummer && (
                                                <small className="error-text" style={{ color: "red" }}>{errors.KvKNummer}</small>
                                            )}
                                        </label>

                                        <label className="form-field">
                                            <span>IBAN</span>
                                            <input
                                                name="IBANnummer"
                                                type="text"
                                                value={formData.IBANnummer || ""}
                                                onChange={handleChange}
                                                placeholder="IBAN nummer"
                                            />
                                            {errors.IBANnummer && (
                                                <small className="error-text" style={{ color: "red" }}>{errors.IBANnummer}</small>
                                            )}
                                        </label>
                                    </>
                                )}
                            </>
                        )}

                        {/* E-mail (voor login én registratie) */}
                        <label className="form-field">
                            <span>E-mailadres</span>
                            <input
                                name="email"
                                type="email"
                                value={formData.email}
                                onChange={handleChange}
                                placeholder="naam@voorbeeld.nl"
                            />
                            {errors.email && <small className="error-text" style={{ color: "red" }}>{errors.email}</small>}
                        </label>

                        {/* Wachtwoord */}
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
                                <small className="error-text" style={{ color: "red" }}>{errors.wachtwoord}</small>
                            )}
                        </label>

                        {/* Bevestig wachtwoord (alleen bij registratie) */}
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
                                    <small className="error-text" style={{ color: "red" }}>{errors.herhaalWachtwoord}</small>
                                )}
                            </label>
                        )}

                        <button type="submit" className="primary-action full-width">
                            {isRegister ? "Registreren" : "Aanmelden"}
                        </button>

                        {/* Algemene meldingen zoals op andere sites */}
                        {serverError && (
                            <p className="form-message form-message--error">{serverError}</p>
                        )}
                        {serverSuccess && (
                            <p className="form-message form-message--success">{serverSuccess}</p>
                        )}
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
