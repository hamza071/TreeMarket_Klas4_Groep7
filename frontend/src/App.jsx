import { Link, Routes, Route, Navigate } from "react-router-dom";
import { useRef, useCallback, useState, useEffect } from "react";

// Styling
import './assets/css/App.css'

// Pages
import DashboardPage from './pages/DashboardPage'
import AuctionPage from './pages/AuctionPage'
import AuctionDetailPage from './pages/AuctionDetailPage'
import UploadAuctionPage from './pages/UploadAuctionPage'
import ReportsPage from './pages/ReportsPage'
import AuthPage from './pages/AuthPage'
import AboutUsPage from './pages/AboutUsPage'
import AllUsers from './pages/CRUD/AllUsers'
import IdUser from './pages/CRUD/IdUser'
import DeleteUser from './pages/CRUD/DeleteUser'
import LogOutUser from './pages/Logout'

const NAVIGATION_ITEMS = [
    { id: 'dashboard', label: 'Dashboard' },
    { id: 'veiling', label: 'Veiling' },
    { id: 'upload', label: 'Upload Veiling' },
    { id: 'reports', label: 'Rapporten' },
    { id: 'home', label: 'Home' },
    { id: 'about', label: 'About' },
    { id: 'allusers', label: 'GetAlleGebruikers' },
    { id: 'idUser', label: 'GetIdGebruiker' },
    { id: 'deleteUser', label: 'DeleteIdGebruiker' },
    { id: 'logout', label: 'LogOutTheUser' },
]

function App() {
    const navigationRefs = useRef([]);

    const handleNavKeyDown = useCallback((event, currentIndex) => {
        if (event.key === "ArrowRight") {
            const nextIndex = (currentIndex + 1) % NAVIGATION_ITEMS.length
            navigationRefs.current[nextIndex]?.focus()
        }
        if (event.key === "ArrowLeft") {
            const prevIndex = (currentIndex - 1 + NAVIGATION_ITEMS.length) % NAVIGATION_ITEMS.length
            navigationRefs.current[prevIndex]?.focus()
        }
    }, [])

    // Alle kavels (status: pending / published)
    const [lots, setLots] = useState([])

    const addNewLot = (newLot) => {
        setLots(prev => [...prev, { ...newLot, status: 'pending' }])
    }

    const updateLot = (updatedLot) => {
        setLots(prev => prev.map(lot =>
            lot.code === updatedLot.code
                ? { ...updatedLot, status: 'published' }
                : lot
        ))
    }

    // ---------- currentUser ophalen ----------
    const [currentUser, setCurrentUser] = useState(null);
    const [loadingUser, setLoadingUser] = useState(true);

    useEffect(() => {
        const fetchCurrentUser = async () => {
            const token = localStorage.getItem("jwtToken");
            if (!token) {
                console.error("Geen JWT token gevonden. Log in a.u.b.");
                setLoadingUser(false);
                return;
            }

            try {
                const res = await fetch('https://localhost:7054/api/Gebruiker/me', {
                    method: "GET",
                    headers: {
                        "Authorization": `Bearer ${token}`,
                        "Content-Type": "application/json"
                    }
                });

                if (!res.ok) {
                    if (res.status === 401) {
                        throw new Error("Niet ingelogd of token ongeldig. Log in a.u.b.");
                    } else {
                        throw new Error(`Kon gebruiker niet ophalen: ${res.status} ${res.statusText}`);
                    }
                }

                const data = await res.json();
                setCurrentUser(data);
                console.log("Current user:", data);

            } catch (err) {
                console.error("Fout bij ophalen gebruiker:", err);
                setCurrentUser(null);
            } finally {
                setLoadingUser(false);
            }
        };

        fetchCurrentUser();
    }, []);

    const handleLogout = () => {
        localStorage.removeItem("jwtToken");
        setCurrentUser(null);
    };

    if (loadingUser) return <p>Even wachten, gebruiker wordt geladen...</p>;

    // ---------- ProtectedRoute component ----------
    const ProtectedRoute = ({ children }) => {
        if (!currentUser) {
            return <Navigate to="/auth" replace />
        }
        return children;
    };

    return (
        <div className="app-shell">
            <a className="skip-link" href="#main-content">Ga direct naar de hoofdinhoud</a>

            <header className="app-header">
                <div className="brand">TREE MARKET</div>
                <nav className="main-nav" aria-label="Primaire navigatie">
                    {NAVIGATION_ITEMS.map((item, index) => (
                        <Link
                            key={item.id}
                            to={`/${item.id}`}
                            className="nav-link"
                            ref={el => (navigationRefs.current[index] = el)}
                            onKeyDown={e => handleNavKeyDown(e, index)}
                        >
                            {item.label}
                        </Link>
                    ))}
                </nav>
                {currentUser
                    ? <button className="user-chip" onClick={handleLogout}>Uitloggen</button>
                    : <Link className="user-chip" to="/auth">Login</Link>
                }
            </header>

            <main className="page-area" id="main-content">
                <Routes>
                    <Route path="/dashboard" element={<DashboardPage lots={lots.filter(lot => lot.status === 'published')} />} />
                    <Route path="/veiling" element={<AuctionPage lots={lots} currentUser={currentUser} />} />
                    <Route path="/veiling/:code" element={<AuctionDetailPage lots={lots} updateLot={updateLot} currentUser={currentUser} />} />
                    <Route path="/upload" element={
                        <ProtectedRoute>
                            <UploadAuctionPage addNewLot={addNewLot} />
                        </ProtectedRoute>
                    } />
                    <Route path="/reports" element={<ReportsPage />} />
                    <Route path="/home" element={<DashboardPage lots={lots.filter(lot => lot.status === 'published')} />} />
                    <Route path="/about" element={<AboutUsPage />} />
                    <Route path="/auth" element={<AuthPage setCurrentUser={setCurrentUser} />} />
                    <Route path="/allusers" element={
                        <ProtectedRoute>
                            <AllUsers />
                        </ProtectedRoute>
                    } />
                    <Route path="/idUser" element={
                        <ProtectedRoute>
                            <IdUser />
                        </ProtectedRoute>
                    } />
                    <Route path="/deleteUser" element={
                        <ProtectedRoute>
                            <DeleteUser />
                        </ProtectedRoute>
                    } />
                    <Route path="/logout" element={<LogOutUser handleLogout={handleLogout} />} />
                    <Route path="*" element={<DashboardPage lots={lots.filter(lot => lot.status === 'published')} />} />
                </Routes>
            </main>
        </div>
    )
}

export default App;