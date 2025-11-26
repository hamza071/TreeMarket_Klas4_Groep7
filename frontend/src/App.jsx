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
import HomePage from './pages/HomePage'
import AllUsers from './pages/CRUD/AllUsers'
import IdUser from './pages/CRUD/IdUser'
import DeleteUser from './pages/CRUD/DeleteUser'

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

    // Alle kavels (pending + published)
    const [lots, setLots] = useState([]);

    // Leverancier voegt nieuwe kavel toe
    const addNewLot = (newLot) => {
        setLots(prev => [...prev, { ...newLot, status: 'pending' }]);
    }

    // Veilingmeester publiceert kavel
    const updateLot = (updatedLot) => {
        setLots(prev =>
            prev.map(lot =>
                lot.id === updatedLot.id
                    ? { ...updatedLot, status: 'published' }
                    : lot
            )
        );
    }

    // Optioneel: haal kavels op bij load (voor demo/initial state)
    useEffect(() => {
        fetch('/api/auctions')
            .then(res => res.json())
            .then(data => setLots(data))
            .catch(err => console.error('Error fetching auctions:', err));
    }, []);

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
                    <Route
                        path="/dashboard"
                        element={
                            <DashboardPage
                                lots={lots.filter(lot => lot.status === 'published')}
                            />
                        }
                    />
                    <Route
                        path="/veiling"
                        element={
                            <AuctionPage
                                lots={lots.filter(lot => lot.status === 'pending')}
                            />
                        }
                    />
                    <Route
                        path="/veiling/:code"
                        element={<AuctionDetailPage updateLot={updateLot} />}
                    />
                    <Route path="/upload" element={<UploadAuctionPage addNewLot={addNewLot} />} />
                    <Route path="/reports" element={<ReportsPage />} />
                    <Route path="/home" element={<HomePage />} />
                    <Route path="/about" element={<AboutUsPage />} />
                    <Route path="/auth" element={<AuthPage />} />
                    <Route path="/allusers" element={<AllUsers />} />
                    <Route path="/idUser" element={<IdUser />} />
                    <Route path="/deleteUser" element={<DeleteUser />} />
                    <Route
                        path="*"
                        element={
                            <DashboardPage
                                lots={lots.filter(lot => lot.status === 'published')}
                            />
                        }
                    />
                </Routes>
            </main>
        </div>
    )
}

export default App;