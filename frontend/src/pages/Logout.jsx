import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

export default function Logout() {
    const navigate = useNavigate();

    useEffect(() => {
        async function logout() {
            // server-side logout (cookie verwijderen)
            await fetch('/api/auth/logout', {
                method: 'POST',
                credentials: 'include',
            });

            // client-side token verwijderen
            localStorage.removeItem('token');
            localStorage.removeItem('gebruikerId');
            localStorage.removeItem('rol');
            localStorage.removeItem('user');

            // redirect
            navigate('/login', { replace: true });
        }

        logout();
    }, [navigate]);

    return <p>Logging out...</p>;
}
