import { useEffect, useState } from "react";
import PlaceBid from "./PlaceBid";

export default function AuctionsList() {
    const [auctions, setAuctions] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetch("/api/auctions")
            .then((res) => res.json())
            .then((data) => {
                setAuctions(data);
                setLoading(false);
            })
            .catch((err) => {
                console.error("Error fetching auctions:", err);
                setLoading(false);
            });
    }, []);

    if (loading) return <p>Loading...</p>;

    return (
        <div>
            <h2>Veilingen</h2>
            <ul>
                {auctions.map((auction) => (
                    <li key={auction.id}>
                        <strong>{auction.titel}</strong> - €{auction.startPrijs} <br />
                        Eindigt op: {new Date(auction.eindDatum).toLocaleString()} <br />
                        <PlaceBid auctionId={auction.id} />
                    </li>
                ))}
            </ul>
        </div>
    );
}