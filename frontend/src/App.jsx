// Deze navbar wordt gebruikt voor het volgende:
// 1. Bezoeker (niet ingelogd)
// 2. Klant
// 3. Veilingsmeester
// 4. Leverancier

import { Link, Routes, Route, useLocation } from "react-router-dom";
import { useRef, useCallback } from "react";

// Import styling
import './assets/css/App.css'

// Import React pages
import DashboardPage from './pages/DashboardPage'
import AuctionPage from './pages/AuctionPage'
import UploadAuctionPage from './pages/UploadAuctionPage'
import ReportsPage from './pages/ReportsPage'
import AuthPage from './pages/AuthPage'
import AboutUsPage from './pages/AboutUsPage'
import HomePage from './pages/HomePage'
import AllUsers from './pages/CRUD/AllUsers'
import IdUser from './pages/CRUD/IdUser'
import DeleteUser from './pages/CRUD/DeleteUser'

// NAVIGATION_ITEMS bevat alle items van de navbar
const NAVIGATION_ITEMS = [
    { id: 'dashboard', label: 'Dashboard' },
    { id: 'auction', label: 'Veiling' },
    { id: 'upload', label: 'Upload Veiling' },
    { id: 'reports', label: 'Rapporten' },
    { id: 'home', label: 'Home' },
    { id: 'about', label: 'About' },
    { id: 'allusers', label: 'GetAlleGebruikers' },
    { id: 'idUser', label: 'GetIdGebruiker' },
    { id: 'deleteUser', label: 'DeleteIdGebruiker' },
]

function App() {
    // useLocation voor debug console
    const location = useLocation();
    console.log("Current URL:", location.pathname);

    // useRef om focus op de navigatie links te beheren
    const navigationRefs = useRef([])

    // Keyboard navigatie handler
    const handleNavKeyDown = useCallback((event, currentIndex) => {
        if (event.key === "ArrowRight") {
            const nextIndex = (currentIndex + 1) % NAVIGATION_ITEMS.length
            navigationRefs.current[nextIndex]?.focus()
        }

        if (event.key === "ArrowLeft") {
            const prevIndex =
                (currentIndex - 1 + NAVIGATION_ITEMS.length) % NAVIGATION_ITEMS.length
            navigationRefs.current[prevIndex]?.focus()
        }
    }, [])

    return (
        <div className="app-shell">
            {/* Skip link voor toegankelijkheid */}
            <a className="skip-link" href="#main-content">
                Ga direct naar de hoofdinhoud
            </a>

            <header className="app-header">
                <div className="brand">TREE MARKET</div>

                {/* Navbar */}
                <nav aria-label="Primaire navigatie" className="main-nav">
                    {NAVIGATION_ITEMS.map((item, index) => (
                        <Link
                            key={item.id}
                            to={item.id === 'dashboard' ? '/dashboard' : '/' + item.id}
                            className="nav-link"
                            ref={(el) => (navigationRefs.current[index] = el)}
                            onKeyDown={(e) => handleNavKeyDown(e, index)}
                        >
                            {item.label}
                        </Link>
                    ))}
                </nav>

                {/* User knop */}
                <Link className="user-chip" to="/auth">
                    User
                </Link>
            </header>

            {/* Routes renderen de juiste pagina op basis van URL */}
            <main className="page-area" id="main-content">
                <Routes>
                    <Route path="/dashboard" element={<DashboardPage />} />
                    <Route path="/auction" element={<AuctionPage />} />
                    <Route path="/upload" element={<UploadAuctionPage />} />
                    <Route path="/reports" element={<ReportsPage />} />
                    <Route path="/home" element={<HomePage />} />
                    <Route path="/about" element={<AboutUsPage />} />
                    <Route path="/auth" element={<AuthPage />} />
                    <Route path="/allusers" element={<AllUsers />} />
                    <Route path="/idUser" element={<IdUser />} />
                    <Route path="/deleteUser" element={<DeleteUser />} />
                    <Route path="*" element={<DashboardPage />} />
                </Routes>
            </main>
        </div>
    )
}

export default App