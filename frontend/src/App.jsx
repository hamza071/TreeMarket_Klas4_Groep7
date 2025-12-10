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
import Logout from "./pages/Logout";

// Navigatie voor KLANT
const NAVIGATION_ITEMS_KLANT = [
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
    { id: "veiling", label: "Veiling" },
];

// Navigatie voor VEILINGSMEESTER
const NAVIGATION_ITEMS_VEILINGSMEESTER = [
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
    { id: "upload", label: "Upload Veiling" },
    { id: "dashboard", label: "Dashboard" },
];

// Navigatie voor anonieme gebruikers
const NAVIGATION_ITEMS_ANONYMOUS = [
    { id: "home", label: "Home" },
    { id: "about", label: "About" },
];

function App() {
    const navigationRefs = useRef([]);

    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role"); // "klant" of "veilingsmeester"
    const isLoggedIn = !!token;

    let NAVIGATION_ITEMS;
    if (!isLoggedIn) {
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_ANONYMOUS;
    } else if (role === "veilingsmeester") {
        NAVIGATION_ITEMS = NAVIGATION_ITEMS_VEILINGSMEESTER;
    } else {
        // default ingelogd = klant
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
                <div className="brand">TREE MARKET</div>

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
                    <Route path="/" element={<HomePage />} />

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

                    <Route path="/veiling" element={<AuctionPage lots={lots} />} />
                    <Route
                        path="/veiling/:code"
                        element={
                            <AuctionDetailPage lots={lots} updateLot={updateLot} />
                        }
                    />

                    <Route
                        path="/upload"
                        element={<UploadAuctionPage addNewLot={addNewLot} />}
                    />

                    <Route path="/reports" element={<ReportsPage />} />
                    <Route path="/home" element={<HomePage />} />
                    <Route path="/about" element={<AboutUsPage />} />
                    <Route path="/auth" element={<AuthPage />} />
                    <Route path="/allusers" element={<AllUsers />} />
                    <Route path="/idUser" element={<IdUser />} />
                    <Route path="/deleteUser" element={<DeleteUser />} />
                    <Route path="/logout" element={<Logout />} />

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
