import { useEffect, useState } from "react";
// We importeren de authFetch zodat we de token mee kunnen geven 
import { authFetch } from "../../apiClient";
function AllUsers() {
    const [users, setUsers] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        // We maken een async functie voor de aanroep
        const fetchUsers = async () => {
            try {
             
                const data = await authFetch("/Gebruiker/GetAllUsers", {
                    method: "GET"
                });

                setUsers(data);
            } catch (err) {
                console.error("Fout:", err);
                setError(err.message); // Bijv: "Niet geautoriseerd"
            }
        };

        fetchUsers();
    }, []);

    if (error) return <div style={{color: "red"}}>Error: 404. U heeft geen rechten tot deze pagina</div>;

    return (
        <div>
            <h2>Alle Gebruikers</h2>
            <table border="5" cellPadding="5" cellSpacing="5">
                <thead>
                <tr>
                    <th>ID</th>
                    <th>Naam</th>
                    <th>Email</th>
                    <th>Rol</th>
                </tr>
                </thead>
                <tbody>
                {users.map(user => (
                    <tr key={user.gebruikerId}>
                        <td>{user.gebruikerId}</td>
                        <td>{user.naam}</td>
                        <td>{user.email}</td>
                        <td>{user.rol}</td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
}

export default AllUsers;