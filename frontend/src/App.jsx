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
import ShopPage from "./pages/ShopPage";
import AllUsers from "./pages/CRUD/AllUsers";
import IdUser from "./pages/CRUD/IdUser";
import DeleteUser from "./pages/CRUD/DeleteUser";
import Logout from "./pages/Logout";

// ========================
// Navigatie per rol
// ========================

// KLANT
const NAVIGATION_ITEMS_KLANT = [
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
    { id: "veiling", label: "Veiling" },
];

// LEVERANCIER
const NAVIGATION_ITEMS_LEVERANCIER = [
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
    { id: "upload", label: "Upload Product" },
    { id: "dashboard", label: "Dashboard" },
];

// VEILINGSMEESTER
const NAVIGATION_ITEMS_VEILINGSMEESTER = [
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
    { id: "veiling", label: "Veiling" },
    { id: "dashboard", label: "Dashboard" },
];

// ANONIEM
const NAVIGATION_ITEMS_ANONYMOUS = [
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
];

// ===== ADMIN (Kan niet aangemaakt worden) =====
const NAVIGATION_ITEMS_ADMIN = [
    { id: 'home', label: 'Home' },
    { id: 'about', label: 'About' },
    { id: 'dashboard', label: 'Dashboard' },
    { id: 'veiling', label: 'Veiling' },
    { id: 'upload', label: 'Upload Veiling' },
    { id: 'reports', label: 'Rapporten' },
    { id: 'allusers', label: 'GetAlleGebruikers' },
];

function App() {
    const navigationRefs = useRef([]);
    const [menuOpen, setMenuOpen] = useState(false);
    const toggleMenu = () => setMenuOpen(prev => !prev);

    // Auth-status uit localStorage
    const token = localStorage.getItem("token");
    const role = (localStorage.getItem("role") || "").toLowerCase();
    const isLoggedIn = !!token;

    // Juiste nav kiezen
    //De 'role' wordt gelezen en kijkt welke rol de ingelogde gebruiker heeft staan in de localstorage.
    let NAVIGATION_ITEMS;
    if (!isLoggedIn) {
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_ANONYMOUS;
    } else if (role === "admin") {
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_ADMIN;
    }  else if (role === "veilingsmeester") {
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_VEILINGSMEESTER;
    } else if (role === "leverancier") {
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_LEVERANCIER;
    } else {
        // alles wat overblijft behandelen we als klant
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_KLANT;
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

    // Alle kavels
    const [lots, setLots] = useState([]);

    const addNewLot = (newLot) => {
        setLots((prev) => [...prev, { ...newLot, status: "pending" }]);
    };

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
                {/* Hamburger links (alleen zichtbaar op mobiel) */}
                <button
                    className={`hamburger ${menuOpen ? "is-active" : ""}`}
                    onClick={toggleMenu}
                    aria-label="Menu openen/sluiten">

                    {/*Voor de hamburger menu*/}
                    <span></span>
                    <span></span>
                    <span></span>
                </button>

                <div className="brand">TREE MARKET</div>

                {/* Navigatie past zich aan op basis van ingelogde / anonieme gebruiker */}
                <nav className={`main-nav ${menuOpen ? "is-open" : ""}`} aria-label="Primaire navigatie">
                    {NAVIGATION_ITEMS.map((item, index) => (
                        <Link
                            key={item.id}
                            to={`/${item.id}`}
                            className="nav-link"
                            ref={el => (navigationRefs.current[index] = el)}
                            onKeyDown={e => handleNavKeyDown(e, index)}
                            onClick={() => setMenuOpen(false)}
                        >
                            {item.label}
                        </Link>
                    ))}
                </nav>

                {/* User chip rechtsboven */}
                {!isLoggedIn ? (
                    <Link className="user-chip" to="/auth">
                        Inloggen
                    </Link>
                ) : (
                    <Link className="user-chip" to="/logout">
                        Uitloggen
                    </Link>
                )}
            </header>

            <main className="page-area" id="main-content">
                <Routes>
                    {/* Root → Home */}
                    <Route path="/" element={<HomePage />} />

                    {/* Dashboard */}
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

                    {/* Veiling */}
                    <Route path="/veiling" element={<AuctionPage lots={lots} />} />
                    <Route
                        path="/veiling/:code"
                        element={
                            <AuctionDetailPage lots={lots} updateLot={updateLot} />
                        }
                    />

                    {/* Upload (zowel leverancier als veilingsmeester gebruiken deze pagina) */}
                    <Route
                        path="/upload"
                        element={<UploadAuctionPage addNewLot={addNewLot} />}
                    />

                    {/* Overige pagina's */}
                    <Route path="/reports" element={<ReportsPage />} />
                    <Route path="/home" element={<HomePage />} />
                    <Route path="/about" element={<AboutUsPage />} />
                    <Route path="/shop" element={<ShopPage />} />
                    <Route path="/auth" element={<AuthPage />} />

                    {/* CRUD demo */}
                    <Route path="/allusers" element={<AllUsers />} />
                    <Route path="/idUser" element={<IdUser />} />
                    <Route path="/deleteUser" element={<DeleteUser />} />

                    {/* Logout */}
                    <Route path="/logout" element={<Logout />} />

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