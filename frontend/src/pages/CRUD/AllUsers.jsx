import { useEffect, useState } from "react";

function AllUsers() {
    const [users, setUsers] = useState([]);

    useEffect(() => {
        try {
            //Fetch van alle gebruikers die er bestaat
            fetch("https://localhost:7054/api/Gebruiker/GetAllUsers")
                .then(res => res.json())
                .then(data => setUsers(data.value || data)) // data.value als je JsonResult gebruikt
                .catch(err => console.error(err));
        } catch (error) {
            console.error("Fout bij ophalen gebruikers:", error);
        }
    }, []);

    return (
        <div>
            <h2>Alle Gebruikers</h2>
            <table border="5" cellPadding="5" cellSpacing="5">
                <thead>
                    <tr>
                        <th>Gebruikers ID</th>
                        <th>Gebruikersnaam</th>
                        <th>Emailadres</th>
                        <th>Rol</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user => (
                        <tr key={user.gebruikerId}>
                            <td>{user.gebruikerId}</td>
                            <td> {user.naam} </td>
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
