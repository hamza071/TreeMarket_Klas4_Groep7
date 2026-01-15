import { useState } from "react";
//import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
// We gebruiken de API_URL die we eerder gemaakt hebben
import { API_URL } from '../DeployLocal';

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
    //const navigate = useNavigate();

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
            if (formData.rol === "leverancier") {
                if (!formData.bedrijf.trim()) newErrors.bedrijf = "Vul een bedrijfsnaam in.";
                if (!formData.KvKNummer.trim()) newErrors.KvKNummer = "Vul een KvK nummer in.";
                if (!formData.IBANnummer.trim()) newErrors.IBANnummer = "Vul een IBAN in.";
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
            return;
        }

        // =====================
        // REGISTREREN
        // =====================
        if (isRegister) {
            let endpoint = "";
            // Let op: Hier gebruiken we nu API_URL in plaats van BASE_URL
            switch (formData.rol) {
                case "klant": endpoint = `${API_URL}/api/Gebruiker/Klant`; break;
                case "leverancier": endpoint = `${API_URL}/api/Gebruiker/Leverancier`; break;
                case "veilingsmeester": endpoint = `${API_URL}/api/Gebruiker/Veilingsmeester`; break;
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

                setFormData({ ...formData, wachtwoord: "", herhaalWachtwoord: "" });
                setActiveTab("login");

            } catch (err) {
                console.error(err);
                setServerError("Server niet bereikbaar.");
            }
        }

            // =====================
            // INLOGGEN
        // =====================
        else {
            try {
                // Let op: Hier gebruiken we nu API_URL
                // Controleer even of je login route '/login' is of '/api/login' in je backend!
                // Ik ga hier uit van /login zoals je het had staan.
                const response = await fetch(`${API_URL}/login`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({
                        email: formData.email,
                        password: formData.wachtwoord,
                    }),
                });

                const data = await response.json().catch(() => null);

                if (!response.ok) {
                    setServerError(data?.detail || "Inloggen mislukt. Controleer uw gegevens.");
                    return;
                }

                if (data.accessToken) {
                    localStorage.setItem("token", data.accessToken);

                    let userRole = null;
                    try {
                        const decoded = jwtDecode(data.accessToken);
                        const roleKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
                        userRole = decoded[roleKey] || decoded["role"];
                    } catch (decodeErr) {
                        console.error("Kon token niet decoderen:", decodeErr);
                    }

                    if (formData.email.toLowerCase() === "admin@treemarket.nl") {
                        userRole = "admin";
                    }

                    if (userRole) {
                        localStorage.setItem("role", userRole.toLowerCase());
                    } else {
                        try {
                            const roleResponse = await fetch(
                                `${API_URL}/api/Gebruiker/RoleByEmail?email=${encodeURIComponent(formData.email)}`
                            );

                            if (roleResponse.ok) {
                                const roleData = await roleResponse.json().catch(() => null);
                                if (roleData?.role) {
                                    localStorage.setItem("role", roleData.role.toLowerCase());
                                }
                            }
                        } catch (apiErr) {
                            console.error("Fout bij ophalen rol via API:", apiErr);
                        }
                    }
                }

                setServerSuccess("Succesvol ingelogd!");
                window.location.href = "/home";

            } catch (err) {
                console.error(err);
                setServerError("Er ging iets mis bij het inloggen.");
            }
        }
    };

    return (
        <div className="auth-page">
            <section className="auth-card">
                <div className="auth-card__form">
                    <header className="auth-header">
                        <h1>{isRegister ? "Registreren" : "Aanmelden"}</h1>
                    </header>

                    <div className="auth-tabs">
                        <button type="button" className={`auth-tab ${!isRegister ? "is-active" : "is-inactive"}`} onClick={() => setActiveTab("login")}>Aanmelden</button>
                        <button type="button" className={`auth-tab ${isRegister ? "is-active" : "is-inactive"}`} onClick={() => setActiveTab("register")}>Registreren</button>
                    </div>

                    <form className="auth-form" onSubmit={handleSubmit}>
                        {isRegister && (
                            <>
                                <label className="form-field">
                                    <span>Rol</span>
                                    <select name="rol" value={formData.rol} onChange={handleChange} className="input-field">
                                        <option value="">-- Selecteer rol --</option>
                                        <option value="klant">Klant</option>
                                        <option value="leverancier">Leverancier</option>
                                        <option value="veilingsmeester">Veilingsmeester</option>
                                    </select>
                                    {errors.rol && <small className="error-text">{errors.rol}</small>}
                                </label>

                                <label className="form-field">
                                    <span>Naam</span>
                                    <input name="naam" type="text" value={formData.naam} onChange={handleChange} placeholder="Voornaam en Achternaam" />
                                    {errors.naam && <small className="error-text">{errors.naam}</small>}
                                </label>

                                <label className="form-field">
                                    <span>Telefoonnummer</span>
                                    <input name="telefoonnummer" type="tel" value={formData.telefoonnummer} onChange={handleChange} placeholder="0612345678" />
                                    {errors.telefoonnummer && <small className="error-text">{errors.telefoonnummer}</small>}
                                </label>

                                {formData.rol === "leverancier" && (
                                    <>
                                        <label className="form-field">
                                            <span>Bedrijf</span>
                                            <input name="bedrijf" type="text" value={formData.bedrijf} onChange={handleChange} placeholder="Bedrijfsnaam" />
                                            {errors.bedrijf && <small className="error-text">{errors.bedrijf}</small>}
                                        </label>
                                        <label className="form-field">
                                            <span>KvK Nummer</span>
                                            <input name="KvKNummer" type="text" value={formData.KvKNummer} onChange={handleChange} placeholder="KvK nummer" />
                                            {errors.KvKNummer && <small className="error-text">{errors.KvKNummer}</small>}
                                        </label>
                                        <label className="form-field">
                                            <span>IBAN</span>
                                            <input name="IBANnummer" type="text" value={formData.IBANnummer} onChange={handleChange} placeholder="IBAN nummer" />
                                            {errors.IBANnummer && <small className="error-text">{errors.IBANnummer}</small>}
                                        </label>
                                    </>
                                )}
                            </>
                        )}

                        <label className="form-field">
                            <span>E-mailadres</span>
                            <input name="email" type="email" value={formData.email} onChange={handleChange} placeholder="naam@voorbeeld.nl" />
                            {errors.email && <small className="error-text">{errors.email}</small>}
                        </label>

                        <label className="form-field">
                            <span>Wachtwoord</span>
                            <input name="wachtwoord" type="password" value={formData.wachtwoord} onChange={handleChange} placeholder="Minimaal 8 tekens" />
                            {errors.wachtwoord && <small className="error-text">{errors.wachtwoord}</small>}
                        </label>

                        {isRegister && (
                            <label className="form-field">
                                <span>Bevestig wachtwoord</span>
                                <input name="herhaalWachtwoord" type="password" value={formData.herhaalWachtwoord} onChange={handleChange} placeholder="Nogmaals wachtwoord" />
                                {errors.herhaalWachtwoord && <small className="error-text">{errors.herhaalWachtwoord}</small>}
                            </label>
                        )}

                        <button type="submit" className="primary-action full-width">{isRegister ? "Registreren" : "Aanmelden"}</button>

                        {serverError && <p className="form-message form-message--error">{serverError}</p>}
                        {serverSuccess && <p className="form-message form-message--success">{serverSuccess}</p>}
                    </form>
                </div>

                <div className="auth-card__media">
                    <img
                        src={isRegister
                            ? "https://images.unsplash.com/photo-1545243424-0ce743321e11?auto=format&fit=crop&w=800&q=80"
                            : "https://images.unsplash.com/photo-1517427294546-5aaefec2b2bb?auto=format&fit=crop&w=800&q=80"}
                        alt={isRegister ? "Registreren" : "Inloggen"}
                    />
                </div>
            </section>
            <footer className="auth-footer">© Royal Flora 2025</footer>
        </div>
    );
}

export default AuthPage;