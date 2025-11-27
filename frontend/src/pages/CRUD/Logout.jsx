import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

/*
  Logout page/component
  - Attempts a server-side logout (best-effort)
  - Clears local/session storage
  - Redirects the user to the login page (or home) after logout
*/

export default function Logout() {
  const navigate = useNavigate();
  const [status, setStatus] = useState('Logging out...');

  useEffect(() => {
    let mounted = true;

    async function performLogout() {
      try {
        // Try to call server logout endpoint if available
        await fetch('/api/auth/logout', {
          method: 'POST',
          credentials: 'include',
          headers: { 'Content-Type': 'application/json' },
        });
      } catch (err) {
        // Ignore network errors; proceed to clear client state
        // console.warn('Logout request failed', err);
      } finally {
        // Clear persisted client-side auth info
        try {
          localStorage.removeItem('token');
          localStorage.removeItem('user');
          sessionStorage.clear();
        } catch (e) {
          // ignore storage errors
        }

        if (!mounted) return;

        setStatus('You have been logged out. Redirecting...');
        // Give a short delay so user can see the message
        setTimeout(() => {
          // Adjust redirect target as appropriate for the app
          navigate('/login', { replace: true });
        }, 700);
      }
    }

    performLogout();

    return () => {
      mounted = false;
    };
  }, [navigate]);

  return (
    <div style={{ padding: 24, maxWidth: 640, margin: '40px auto', textAlign: 'center' }}>
      <h2>Logout</h2>
      <p>{status}</p>
      <button
        onClick={() => navigate('/', { replace: true })}
        style={{
          marginTop: 12,
          padding: '8px 16px',
          borderRadius: 4,
          border: '1px solid #ccc',
          background: '#fff',
          cursor: 'pointer',
        }}
      >
        Go to Home
      </button>
    </div>
  );
}