import { Link, Routes, Route } from "react-router-dom";
import { useRef, useCallback, useState } from "react";

// Styling
import "./assets/css/App.css";

// Pages
import DashboardPage from "./pages/DashboardPage";
import AuctionPage from "./pages/AuctionPage";
import AuctionDetailPage from "./pages/AuctionDetailPage";
import UploadAuctionPage from "./pages/UploadAuctionPage";
import ReportsPage from "./pages/ReportsPage";
import AuthPage from "./pages/AuthPage";
import AboutUsPage from "./pages/AboutUsPage";
import HomePage from "./pages/HomePage";
import AllUsers from "./pages/CRUD/AllUsers";
import IdUser from "./pages/CRUD/IdUser";
import DeleteUser from "./pages/CRUD/DeleteUser";
import LogOutUser from "./pages/Logout";

// Navigatie voor ingelogde gebruikers (bijv. beheer / andere rollen)
const NAVIGATION_ITEMS_AUTHENTICATED = [
    { id: "dashboard", label: "Dashboard" },
    { id: "veiling", label: "Veiling" },
    { id: "upload", label: "Upload Veiling" },
    { id: "reports", label: "Rapporten" },
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
    { id: "allusers", label: "GetAlleGebruikers" },
    { id: "idUser", label: "GetIdGebruiker" },
    { id: "deleteUser", label: "DeleteIdGebruiker" },
    { id: "logout", label: "LogOutTheUser" },
];

// Navigatie specifiek voor KLANT (userstory: eigen navigatiestructuur)
const NAVIGATION_ITEMS_KLANT = [
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
    { id: "veiling", label: "Veiling" },
];

// Navigatie voor anonieme gebruikers (alleen publieke pagina's)
const NAVIGATION_ITEMS_ANONYMOUS = [
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
];

function App() {
    const navigationRefs = useRef([]);

    // Informatie uit localStorage
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role"); // bv. "klant", "leverancier", ...
    const userName = localStorage.getItem("userName"); // optioneel, uit JWT

    // Check of de gebruiker is ingelogd (op basis van JWT in localStorage)
    const isLoggedIn = !!token;

    // Kies de juiste navigatiestructuur op basis van login status + rol
    let NAVIGATION_ITEMS;
    if (!isLoggedIn) {
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_ANONYMOUS;
    } else if (role === "klant") {
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_KLANT;
    } else {
        // overige rollen (leverancier / veilingsmeester / admin)
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_AUTHENTICATED;
    }

    const handleNavKeyDown = useCallback(
        (event, currentIndex) => {
            const navLength = NAVIGATION_ITEMS.length;

            if (event.key === "ArrowRight") {
                const nextIndex = (currentIndex + 1) % navLength;
                navigationRefs.current[nextIndex]?.focus();
            }
            if (event.key === "ArrowLeft") {
                const prevIndex = (currentIndex - 1 + navLength) % navLength;
                navigationRefs.current[prevIndex]?.focus();
            }
        },
        [NAVIGATION_ITEMS.length]
    );

    // Alle kavels (status: pending / published)
    const [lots, setLots] = useState([]);

    // Leverancier voegt nieuwe kavel toe (status: pending)
    const addNewLot = (newLot) => {
        setLots((prev) => [...prev, { ...newLot, status: "pending" }]);
    };

    // Veilingmeester bewerkt kavel (voegt prijs en sluitingstijd toe en publiceert)
    const updateLot = (updatedLot) => {
        setLots((prev) =>
            prev.map((lot) =>
                lot.code === updatedLot.code
                    ? { ...updatedLot, status: "published" }
                    : lot
            )
        );
    };

    return (
        <div className="app-shell">
            <a className="skip-link" href="#main-content">
                Ga direct naar de hoofdinhoud
            </a>

            <header className="app-header">
                <div className="brand">TREE MARKET</div>

                {/* Navigatie past zich aan op basis van ingelogde / anonieme gebruiker + rol */}
                <nav className="main-nav" aria-label="Primaire navigatie">
                    {NAVIGATION_ITEMS.map((item, index) => (
                        <Link
                            key={item.id}
                            to={`/${item.id}`}
                            className="nav-link"
                            ref={(el) => (navigationRefs.current[index] = el)}
                            onKeyDown={(e) => handleNavKeyDown(e, index)}
                        >
                            {item.label}
                        </Link>
                    ))}
                </nav>

                {/* User chip rechtsboven */}
                {!isLoggedIn ? (
                    // Anonieme gebruiker: naar registreren/inloggen (AuthPage)
                    <Link className="user-chip" to="/auth">
                        Inloggen
                    </Link>
                ) : (
                    // Ingelogde gebruiker: naam + uitloggen
                    <Link className="user-chip" to="/logout">
                        {userName ? `${userName} (uitloggen)` : "Uitloggen"}
                    </Link>
                )}
            </header>

            <main className="page-area" id="main-content">
                <Routes>
                    {/* Extra: root route naar Home */}
                    <Route path="/" element={<HomePage />} />

                    {/* Dashboard krijgt alleen gepubliceerde kavels */}
                    <Route
                        path="/dashboard"
                        element={
                            <DashboardPage
                                lots={lots.filter(
                                    (lot) => lot.status === "published"
                                )}
                            />
                        }
                    />

                    {/* Veiling krijgt alle kavels */}
                    <Route
                        path="/veiling"
                        element={<AuctionPage lots={lots} />}
                    />
                    <Route
                        path="/veiling/:code"
                        element={
                            <AuctionDetailPage
                                lots={lots}
                                updateLot={updateLot}
                            />
                        }
                    />

                    {/* Upload voor leverancier */}
                    <Route
                        path="/upload"
                        element={<UploadAuctionPage addNewLot={addNewLot} />}
                    />

                    {/* Overige pages */}
                    <Route path="/reports" element={<ReportsPage />} />
                    <Route path="/home" element={<HomePage />} />
                    <Route path="/about" element={<AboutUsPage />} />
                    <Route path="/auth" element={<AuthPage />} />
                    <Route path="/allusers" element={<AllUsers />} />
                    <Route path="/idUser" element={<IdUser />} />
                    <Route path="/deleteUser" element={<DeleteUser />} />
                    <Route path="/logout" element={<LogOutUser />} />

                    {/* Fallback */}
                    <Route
                        path="*"
                        element={
                            <DashboardPage
                                lots={lots.filter(
                                    (lot) => lot.status === "published"
                                )}
                            />
                        }
                    />
                </Routes>
            </main>
        </div>
    );
}

export default App;
