import { useState } from "react";
import { useNavigate } from "react-router-dom";
// Importeer de decoder (zorg dat je 'npm install jwt-decode' hebt gedaan)
import { jwtDecode } from "jwt-decode";

function AuthPage() {
    const [activeTab, setActiveTab] = useState("register");

    // 1. DIT STUK IS CRUCIAAL VOOR VERCEL/AZURE
    const BASE_URL = import.meta.env.VITE_API_URL
        ? import.meta.env.VITE_API_URL.replace('/api', '')
        : "https://localhost:7054";

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
        setFormData((prev) => ({ ...prev, [name]: value }));
        setErrors((prev) => ({ ...prev, [name]: "" }));
    };

    const validateForm = () => {
        const newErrors = {};
        if (!formData.email.trim()) newErrors.email = "Vul uw e-mailadres in.";
        if (!formData.wachtwoord) newErrors.wachtwoord = "Vul een wachtwoord in.";

        if (isRegister) {
            if (!formData.rol) newErrors.rol = "Selecteer een rol.";
            if (!formData.naam.trim()) newErrors.naam = "Vul uw naam in.";
            if (formData.wachtwoord !== formData.herhaalWachtwoord) {
                newErrors.herhaalWachtwoord = "Wachtwoorden komen niet overeen.";
            }
            // ... (rest van je validatie, verkort voor overzicht) ...
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
            return;
        }

        // =====================
        // REGISTREREN
        // =====================
        if (isRegister) {
            let endpoint = "";
            switch (formData.rol) {
                case "klant": endpoint = `${BASE_URL}/api/Gebruiker/Klant`; break;
                case "leverancier": endpoint = `${BASE_URL}/api/Gebruiker/Leverancier`; break;
                case "veilingsmeester": endpoint = `${BASE_URL}/api/Gebruiker/Veilingsmeester`; break;
                default: setErrors({ rol: "Selecteer een rol." }); return;
            }

            try {
                const response = await fetch(endpoint, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(formData),
                });

                if (!response.ok) {
                    const errorBody = await response.json().catch(() => null);
                    setServerError(errorBody?.message || "Registratie mislukt.");
                    return;
                }
                setServerSuccess("Registratie succesvol! U kunt nu inloggen.");
                // Reset form...
            } catch (err) {
                console.error(err);
                setServerError("Server niet bereikbaar.");
            }
        }

            // =====================
            // INLOGGEN (DE BELANGRIJKE FIX)
        // =====================
        else {
            try {
                // Gebruik BASE_URL, niet localhost!
                const response = await fetch(`${BASE_URL}/login`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({
                        email: formData.email,
                        password: formData.wachtwoord,
                    }),
                });

                const data = await response.json().catch(() => null);

                if (!response.ok) {
                    setServerError(data?.detail || "Inloggen mislukt.");
                    return;
                }

                if (data.accessToken) {
                    // 1. Sla token op
                    localStorage.setItem("token", data.accessToken);

                    // 2. HAAL DE ROL UIT DE TOKEN (Dit ontbrak in versie 1)
                    try {
                        const decoded = jwtDecode(data.accessToken);
                        // Microsoft stopt de rol in deze lange url key
                        const roleKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
                        const userRole = decoded[roleKey] || decoded["role"];

                        if (userRole) {
                            localStorage.setItem("role", userRole); // <-- DIT HEB JE NODIG VOOR JE MENU
                        }
                    } catch (decodeErr) {
                        console.error("Kon token niet decoderen:", decodeErr);
                    }
                }

                setServerSuccess("Succesvol ingelogd!");
                navigate("/dashboard"); // Of home

            } catch (err) {
                console.error(err);
                setServerError("Er ging iets mis bij het inloggen.");
            }
        }
    };

    return (
        // ... Hier plak je gewoon je bestaande return HTML (JSX) ...
        // ... Die was bij beide versies hetzelfde ...
        <div className="auth-page">
            <section className="auth-card">
                {/* ... je HTML code ... */}
                {/* Ik kort dit even in om ruimte te besparen, gebruik jouw eigen JSX hier */}
                <div className="auth-card__form">
                    <header className="auth-header">
                        <h1>{isRegister ? "Registreren" : "Aanmelden"}</h1>
                    </header>

                    <div className="auth-tabs">
                        <button type="button" className={`auth-tab ${!isRegister ? "is-active" : "is-inactive"}`} onClick={() => setActiveTab("login")}>Aanmelden</button>
                        <button type="button" className={`auth-tab ${isRegister ? "is-active" : "is-inactive"}`} onClick={() => setActiveTab("register")}>Registreren</button>
                    </div>

                    <form className="auth-form" onSubmit={handleSubmit}>
                        {/* ... Jouw formulier velden (Email, Wachtwoord, Rol, etc) ... */}
                        {/* ... Kopieer dit uit je vorige bestand ... */}

                        {isRegister && (
                            <label className="form-field">
                                <span>Rol</span>
                                <select name="rol" value={formData.rol} onChange={handleChange} className="input-field">
                                    <option value="">-- Selecteer rol --</option>
                                    <option value="klant">Klant</option>
                                    <option value="leverancier">Leverancier</option>
                                    <option value="veilingsmeester">Veilingsmeester</option>
                                </select>
                            </label>
                        )}

                        <label className="form-field">
                            <span>E-mailadres</span>
                            <input name="email" type="email" value={formData.email} onChange={handleChange} />
                        </label>
                        <label className="form-field">
                            <span>Wachtwoord</span>
                            <input name="wachtwoord" type="password" value={formData.wachtwoord} onChange={handleChange} />
                        </label>

                        {/* ... etc ... */}

                        <button type="submit" className="primary-action full-width">{isRegister ? "Registreren" : "Aanmelden"}</button>

                        {serverError && <p className="form-message form-message--error">{serverError}</p>}
                        {serverSuccess && <p className="form-message form-message--success">{serverSuccess}</p>}
                    </form>
                </div>
            </section>
        </div>
    );
}

export default AuthPage;