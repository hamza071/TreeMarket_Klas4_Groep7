import { useMemo, useState } from 'react'
import './App.css'
import DashboardPage from './pages/DashboardPage'
import AuctionPage from './pages/AuctionPage'
import UploadAuctionPage from './pages/UploadAuctionPage'
import ReportsPage from './pages/ReportsPage'
import AuthPage from './pages/AuthPage'
import AboutUsPage from './pages/AboutUsPage'

const NAVIGATION_ITEMS = [
    { id: 'dashboard', label: 'Dashboard' },
    { id: 'auction', label: 'Veiling' },
    { id: 'upload', label: 'Upload Veiling' },
    { id: 'reports', label: 'Rapporten' },
    { id: 'home', label: 'Home' },
    { id: 'about', label: 'About Us' },
]

function App() {
    const [activeView, setActiveView] = useState('dashboard')

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
