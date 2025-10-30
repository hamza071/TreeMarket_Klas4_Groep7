import { useMemo, useState } from 'react'
//Importeerd de styling voor die bestanden
import './assets/css/App.css'
//Importeerd de React pages binnen de map
import DashboardPage from './pages/DashboardPage'
import AuctionPage from './pages/AuctionPage'
import UploadAuctionPage from './pages/UploadAuctionPage'
import ReportsPage from './pages/ReportsPage'
import AuthPage from './pages/AuthPage'
import Navbar from './pages/Navbar'


//De lijst waar je de navigatie wilt meegeven.
const NAVIGATION_ITEMS = [
    { id: 'dashboard', label: 'Dashboard' },
    { id: 'auction', label: 'Veiling' },
    { id: 'upload', label: 'Upload Veiling' },
    { id: 'reports', label: 'Rapporten' },
    { id: 'navbar', label: 'Navbar' },
    { id: 'home', label: 'Home'}
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
