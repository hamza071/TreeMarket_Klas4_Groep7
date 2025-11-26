import { useState } from "react";

export default function PlaceBid({ auctionId }) {
    const [amount, setAmount] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        const response = await fetch(`/api/auctions/bid`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ auctionId, bidAmount: parseFloat(amount) }),
        });

        if (response.ok) {
            alert("Bod geplaatst!");
            setAmount("");
        } else {
            const data = await response.json();
            alert("Fout: " + (data.message || "Onbekende fout"));
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <input
                type="number"
                step="0.01"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                placeholder="Bod bedrag"
                required
            />
            <button type="submit">Plaats Bod</button>
        </form>
    );
}