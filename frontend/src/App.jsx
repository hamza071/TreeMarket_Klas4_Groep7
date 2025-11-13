//Deze navbar wordt gebruikt voor het volgende:
//1. Bezoeker (niet ingelogt)
//2. Klant
//3. Veilingsmeester
//4. Leverancier

import { useCallback, useMemo, useRef, useState } from 'react'
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
import AllUsers from './pages/CRUD/AllUsers'
import IdUser from './pages/CRUD/IdUser'
import DeleteUser from './pages/CRUD/DeleteUser'



//De lijst waar je de navigatie wilt meegeven.
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
    const [activeView, setActiveView] = useState('dashboard')
    const navigationRefs = useRef([])

    const handleNavKeyDown = useCallback((event, currentIndex) => {
        if (!navigationRefs.current.length) {
            return
        }

        const { key } = event
        if (key === 'ArrowRight' || key === 'ArrowLeft') {
            event.preventDefault()
            const direction = key === 'ArrowRight' ? 1 : -1
            const nextIndex = (currentIndex + direction + NAVIGATION_ITEMS.length) % NAVIGATION_ITEMS.length
            navigationRefs.current[nextIndex]?.focus()
        }

        if (key === 'Home') {
            event.preventDefault()
            navigationRefs.current[0]?.focus()
        }

        if (key === 'End') {
            event.preventDefault()
            navigationRefs.current[NAVIGATION_ITEMS.length - 1]?.focus()
        }
    }, [])

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
            case 'auth':
                return <AuthPage />
            case 'about':
                return <AboutUsPage />
            case 'allusers':
                return <AllUsers />
            case 'idUser':
                return < IdUser />
            case 'deleteUser':
                return < DeleteUser />
            default:
                return <DashboardPage />

        }
    }, [activeView])

    return (
        <div className="app-shell">
            <a className="skip-link" href="#main-content">
                Ga direct naar de hoofdinhoud
            </a>
            <header className="app-header">
                <div className="brand">TREE MARKET</div>
                <nav aria-label="Primaire navigatie" className="main-nav">
                    {NAVIGATION_ITEMS.map((item, index) => (
                        <button
                            key={item.id}
                            type="button"
                            className={`nav-link ${activeView === item.id ? 'is-active' : ''}`}
                            onClick={() => setActiveView(item.id)}
                            aria-current={activeView === item.id ? 'page' : undefined}
                            onKeyDown={(event) => handleNavKeyDown(event, index)}
                            ref={(element) => {
                                navigationRefs.current[index] = element
                            }}
                        >
                            {item.label}
                        </button>
                    ))}
                </nav>
                <button
                    type="button"
                    className={`user-chip ${activeView === 'auth' ? 'is-active' : ''}`}
                    onClick={() => setActiveView('auth')}
                    aria-label="Ga naar de gebruikerspagina"
                >
                    User
                </button>
            </header>
            <main className="page-area" id="main-content">
                {ActivePage}
            </main>
        </div>
    )
}

export default App