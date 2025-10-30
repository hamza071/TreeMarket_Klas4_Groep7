//Deze navbar wordt gebruikt voor het volgende:
//1. Bezoeker (niet ingelogt)
//2. Klant
//3. Veilingsmeester
//4. Leverancier

import { useMemo, useState } from 'react'
//Importeerd de styling voor die bestanden
import './assets/css/App.css'
//Importeerd de React pages binnen de map
import DashboardPage from './pages/DashboardPage'
import AuctionPage from './pages/AuctionPage'
import UploadAuctionPage from './pages/UploadAuctionPage'
import ReportsPage from './pages/ReportsPage'
import AuthPage from './pages/AuthPage'
import AboutUsPage from './pages/AboutUsPage'
import HomePage from './pages/HomePage'


//De lijst waar je de navigatie wilt meegeven.
const NAVIGATION_ITEMS = [
    { id: 'dashboard', label: 'Dashboard' },
    { id: 'auction', label: 'Veiling' },
    { id: 'upload', label: 'Upload Veiling' },
    { id: 'reports', label: 'Rapporten' },
    { id: 'home', label: 'Home' },
    { id: 'about', label: 'About' },
]

function App() {
    const [activeView, setActiveView] = useState('dashboard')

    //Deze switch is een navbar waar je kunt navigeren.
    //Probeer het eens uit als je het runned :)
    const ActivePage = useMemo(() => {
        switch (activeView) {
            case 'dashboard':
                return <DashboardPage />
            case 'auction':
                return <AuctionPage />
            case 'upload':
                return <UploadAuctionPage />
            case 'reports':
                return <ReportsPage />
            case 'home':
                return <HomePage />
            case 'shop':
                return <ShopPage />
            case 'auth':
                return <AuthPage />
            case 'about':   
                return <AboutUsPage />
            default:
                return <DashboardPage />

        }
    }, [activeView])

    return (
        <div className="app-shell">
            <header className="app-header">
                <div className="brand">TREE MARKET</div>
                <nav className="main-nav">
                    {NAVIGATION_ITEMS.map((item) => (
                        <button
                            key={item.id}
                            type="button"
                            className={`nav-link ${activeView === item.id ? 'is-active' : ''}`}
                            onClick={() => setActiveView(item.id)}
                        >
                            {item.label}
                        </button>
                    ))}
                </nav>
                <button
                    type="button"
                    className={`user-chip ${activeView === 'auth' ? 'is-active' : ''}`}
                    onClick={() => setActiveView('auth')}
                >
                    User
                </button>
            </header>
            <main className="page-area">{ActivePage}</main>
        </div>
    )
}

export default App
