import { Link, Routes, Route } from "react-router-dom";
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

 ///*   // ---------- currentUser ophalen ----------
 //   const [currentUser, setCurrentUser] = useState(null);
 //   const [loadingUser, setLoadingUser] = useState(true);
 //   const [userError, setUserError] = useState(null);

 //   useEffect(() => {
 //       const fetchCurrentUser = async () => {
 //           const url = 'https://localhost:7054/api/User/me'; // pas aan indien endpoint anders
 //           console.log("Fetching current user vanaf:", url);

 //           try {
 //               const res = await fetch(url, { credentials: 'include' });
 //               if (!res.ok) {
 //                   const text = await res.text();
 //                   throw new Error(`Kon gebruiker niet ophalen: ${text}`);
 //               }
 //               const data = await res.json();
 //               console.log("Current user:", data);
 //               setCurrentUser(data);
 //           } catch (err) {
 //               console.error("Fout bij ophalen gebruiker:", err);
 //               setUserError(err.message);
 //           } finally {
 //               setLoadingUser(false);
 //           }
 //       };
 //       fetchCurrentUser();
 //   }, []);

 //   if (loadingUser) return <p>Even wachten, gebruiker wordt geladen...</p>;
 //   if (userError) return <p>{userError}</p>; */

    // ---------- NIEUW: currentUser ophalen ----------
    const [currentUser, setCurrentUser] = useState(null);
    const [loadingUser, setLoadingUser] = useState(true);

    useEffect(() => {
        const fetchCurrentUser = async () => {
            try {
                // Optie 1: JWT token in localStorage (na login)
                const token = localStorage.getItem("jwtToken");

                const res = await fetch('https://localhost:7054/api/Gebruiker/me', {
                    method: "GET",
                    headers: token ? {
                        "Authorization": `Bearer ${token}`,
                        "Content-Type": "application/json"
                    } : undefined,
                    credentials: 'include' // Als je HttpOnly cookie gebruikt
                });

                if (!res.ok) {
                    throw new Error(`Kon gebruiker niet ophalen: ${res.status} ${res.statusText}`);
                }

                const data = await res.json();
                setCurrentUser(data);

            } catch (err) {
                console.error("Fout bij ophalen gebruiker:", err);
            } finally {
                setLoadingUser(false);
            }
        };

        fetchCurrentUser();
    }, []);

    if (loadingUser) return <p>Even wachten, gebruiker wordt geladen...</p>;

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
                <Link className="user-chip" to="/auth">User</Link>
            </header>

            <main className="page-area" id="main-content">
                <Routes>
                    <Route path="/dashboard" element={<DashboardPage lots={lots.filter(lot => lot.status === 'published')} />} />
                    <Route path="/veiling" element={<AuctionPage lots={lots} currentUser={currentUser} />} />
                    <Route path="/veiling/:code" element={<AuctionDetailPage lots={lots} updateLot={updateLot} currentUser={currentUser} />} />
                    <Route path="/upload" element={<UploadAuctionPage addNewLot={addNewLot} />} />
                    <Route path="/reports" element={<ReportsPage />} />
                    <Route path="/home" element={<HomePage />} />
                    <Route path="/about" element={<AboutUsPage />} />
                    <Route path="/auth" element={<AuthPage />} />
                    <Route path="/allusers" element={<AllUsers />} />
                    <Route path="/idUser" element={<IdUser />} />
                    <Route path="/deleteUser" element={<DeleteUser />} />
                    <Route path="/logout" element={<LogOutUser />} />
                    <Route path="*" element={<DashboardPage lots={lots.filter(lot => lot.status === 'published')} />} />
                </Routes>
            </main>
        </div>
    )
}

export default App;