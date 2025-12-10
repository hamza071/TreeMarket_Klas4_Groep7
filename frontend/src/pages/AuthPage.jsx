import { useState } from "react";
import { useNavigate } from "react-router-dom";

// optioneel, alleen nodig als je ooit echte JWT met rollen krijgt
function parseJwt(token) {
    try {
        const base64Url = token.split(".")[1];
        const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
        const jsonPayload = decodeURIComponent(
            window
                .atob(base64)
                .split("")
                .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
                .join("")
        );
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
}

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

    const validateForm = () => {
        const newErrors = {};

        if (!formData.email.trim()) {
            newErrors.email = "Vul uw e-mailadres in.";
        } else {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(formData.email)) {
                newErrors.email = "Vul een geldig e-mailadres in.";
            }
        }

        if (!formData.wachtwoord) {
            newErrors.wachtwoord = "Vul een wachtwoord in.";
        } else if (formData.wachtwoord.length < 8) {
            newErrors.wachtwoord = "Wachtwoord moet minimaal 8 tekens bevatten.";
        }

        if (isRegister) {
            if (!formData.rol) newErrors.rol = "Selecteer een rol.";
            if (!formData.naam.trim()) newErrors.naam = "Vul uw naam in.";

            if (formData.telefoonnummer.trim()) {
                const phoneRegex = /^0[0-9]{9}$/;
                if (!phoneRegex.test(formData.telefoonnummer.trim())) {
                    newErrors.telefoonnummer =
                        "Vul een geldig Nederlands telefoonnummer in (10 cijfers).";
                }
            }

            if (!formData.herhaalWachtwoord) {
                newErrors.herhaalWachtwoord = "Bevestig uw wachtwoord.";
            } else if (formData.wachtwoord !== formData.herhaalWachtwoord) {
                newErrors.herhaalWachtwoord = "Wachtwoorden komen niet overeen.";
            }

            if (formData.rol === "leverancier") {
                if (!formData.bedrijf.trim())
                    newErrors.bedrijf = "Vul een bedrijfsnaam in.";
                if (!formData.KvKNummer.trim())
                    newErrors.KvKNummer = "Vul een KvK nummer in.";
                if (!formData.IBANnummer.trim())
                    newErrors.IBANnummer = "Vul een IBAN in.";
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

        // =====================
        // REGISTREREN
        // =====================
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

                // Mapping: e-mail → rol opslaan voor later inloggen
                localStorage.setItem(
                    "userRole:" + formData.email,
                    formData.rol
                );

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
                setServerError("Er ging iets mis bij het registreren.");
            }

            return;
        }

        // =====================
        // INLOGGEN
        // =====================
        try {
            const response = await fetch("https://localhost:7054/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    email: formData.email,
                    password: formData.wachtwoord,
                }),
            });

            const data = await response.json().catch(() => null);
            console.log("Login response:", response, data);

            if (!response.ok) {
                setServerError(
                    data?.detail || data?.message || "E-mailadres of wachtwoord onjuist."
                );
                return;
            }

            // 1. Token
            let accessToken = data?.accessToken || data?.token;
            if (!accessToken) {
                accessToken = "dummy-token"; // we hebben hem alleen als flag nodig
            }
            localStorage.setItem("token", accessToken);

            // 2. Rol bepalen
            let role =
                data?.role ||
                data?.rol ||
                data?.userRole ||
                null;

            if (!role && accessToken.includes(".")) {
                const decoded = parseJwt(accessToken);
                role =
                    decoded?.role ||
                    decoded?.rol ||
                    decoded?.[
                    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                    ] ||
                    null;
            }

            // 3. Als backend/token geen rol geeft → mapping via e-mail gebruiken
            if (!role) {
                const mapped = localStorage.getItem(
                    "userRole:" + formData.email
                );
                if (mapped) {
                    role = mapped; // bv. "veilingsmeester"
                }
            }

            // 4. Als we nog steeds niks hebben → standaard klant
            if (!role) {
                role = "klant";
            }

            localStorage.setItem("role", role);

            setServerSuccess("Succesvol ingelogd!");

            // hard redirect zodat App.jsx opnieuw inleest
            window.location.href = "/home";
        } catch (err) {
            console.error(err);
            setServerError("Er ging iets mis bij het inloggen.");
        }
    };

    return (
        <div className="auth-page">
            <section className="auth-card">
                <div className="auth-card__form">
                    <header className="auth-header">
                        <h1>
                            {isRegister
                                ? "Registreren bij TreeMarket"
                                : "Aanmelden bij TreeMarket"}
                        </h1>
                        <p>Log in of maak een nieuw account aan.</p>
                    </header>

                    <div className="auth-tabs">
                        <button
                            type="button"
                            className={`auth-tab ${isRegister ? "is-inactive" : "is-active"
                                }`}
                            onClick={() => setActiveTab("login")}
                        >
                            Aanmelden
                        </button>
                        <button
                            type="button"
                            className={`auth-tab ${isRegister ? "is-active" : "is-inactive"
                                }`}
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
                                        <option value="veilingsmeester">
                                            Veilingsmeester
                                        </option>
                                    </select>
                                    {errors.rol && (
                                        <small className="error-text">
                                            {errors.rol}
                                        </small>
                                    )}
                                </label>

                                <label className="form-field">
                                    <span>Naam</span>
                                    <input
                                        name="naam"
                                        type="text"
                                        value={formData.naam}
                                        onChange={handleChange}
                                        placeholder="Voornaam en Achternaam"
                                    />
                                    {errors.naam && (
                                        <small className="error-text">
                                            {errors.naam}
                                        </small>
                                    )}
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
                                        <small className="error-text">
                                            {errors.telefoonnummer}
                                        </small>
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
                                        </label>
                                        <label className="form-field">
                                            <span>KvK Nummer</span>
                                            <input
                                                name="KvKNummer"
                                                type="text"
                                                value={formData.KvKNummer}
                                                onChange={handleChange}
                                                placeholder="KvK nummer"
                                            />
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
                            {errors.email && (
                                <small className="error-text">
                                    {errors.email}
                                </small>
                            )}
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
                                <small className="error-text">
                                    {errors.wachtwoord}
                                </small>
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
                                    <small className="error-text">
                                        {errors.herhaalWachtwoord}
                                    </small>
                                )}
                            </label>
                        )}

                        <button type="submit" className="primary-action full-width">
                            {isRegister ? "Registreren" : "Aanmelden"}
                        </button>

                        {serverError && (
                            <p className="form-message form-message--error">
                                {serverError}
                            </p>
                        )}
                        {serverSuccess && (
                            <p className="form-message form-message--success">
                                {serverSuccess}
                            </p>
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
                        alt={isRegister ? "Registreren" : "Inloggen"}
                    />
                </div>
            </section>

            <footer className="auth-footer">© Royal Flora 2025</footer>
        </div>
    );
}

export default AuthPage;
