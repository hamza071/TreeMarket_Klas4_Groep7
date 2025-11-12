// hierdoor hoef je in andere bestanden niet steeds dezelfde fetch-code te schrijven.

const API_BASE_URL = "https://localhost:7054/api";

export async function getData(endpoint) {
    const response = await fetch(`${API_BASE_URL}/${endpoint}`);
    if (!response.ok) throw new Error("Fout bij ophalen data");
    const json = await response.json();
    // Swagger-controllers geven vaak { value: [...] } terug
    return json.value || json;
}

export async function postData(endpoint, data) {
    const response = await fetch(`${API_BASE_URL}/${endpoint}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
    });
    if (!response.ok) throw new Error("Fout bij opslaan data");
    return await response.json();
}
