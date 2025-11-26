const API = "https://jouw-api-url.com/api/auctions";

export async function fetchAuctions() {
    const res = await fetch(API);
    return res.json();
}

export async function createAuction(data) {
    const res = await fetch(API, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
    });
    return res.json();
}

export async function placeBid(id, amount) {
    const res = await fetch(`${API}/${id}/bid`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ amount }),
    });
    return res.json();
}