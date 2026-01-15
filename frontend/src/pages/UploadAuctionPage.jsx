import { useState } from 'react';
import '../assets/css/UploadAuctionPage.css';

const defaultForm = {
    title: '',
    quantity: '',
    description: '',
    image: null,
    minPrice: '',
};

function UploadAuctionPage() {
    const [form, setForm] = useState(defaultForm);

    const handleChange = (e) => {
        const { name, value, files } = e.target;

        if (name === 'image') {
            setForm(prev => ({ ...prev, image: files[0] }));
        } else if (name === 'minPrice') {
            // Alleen geldige decimalen toelaten
            if (/^\d*\.?\d*$/.test(value)) {
                setForm(prev => ({ ...prev, minPrice: value }));
            }
        } else {
            setForm(prev => ({ ...prev, [name]: value }));
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        // Frontend-validatie
        if (!form.title.trim()) return alert("Productnaam is verplicht.");
        if (!form.quantity || Number(form.quantity) < 1) return alert("Aantal moet minimaal 1 zijn.");
        if (!form.minPrice || parseFloat(form.minPrice) <= 0) return alert("Minimumprijs moet groter dan 0 zijn.");

        const formData = new FormData();
        formData.append("ProductNaam", form.title.trim());
        formData.append("Omschrijving", form.description?.trim() ?? "");
        formData.append("Hoeveelheid", Number(form.quantity));
        formData.append("MinimumPrijs", parseFloat(form.minPrice)); // parseFloat houdt decimalen
        if (form.image) formData.append("Foto", form.image);

        try {
            const token = localStorage.getItem("token");
            if (!token) return alert("Je bent niet ingelogd.");

            const response = await fetch("https://localhost:7054/api/Product/CreateProduct", {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${token}`
                    // Let op: Content-Type niet instellen bij FormData
                },
                body: formData
            });

            const text = await response.text();
            console.log("Server response:", text);

            if (!response.ok) return alert("Fout vanuit server: " + text);

            alert("Kavel is succesvol geüpload!");
            setForm(defaultForm);

        } catch (error) {
            console.error("Fout bij upload:", error);
            alert("Er ging iets mis: " + error.message);
        }
    };

    return (
        <div className="upload-page">
            <header className="section-header">
                <h1>Upload nieuwe kavel (leverancier)</h1>
                <p id="upload-intro">
                    Voer je kavelgegevens in. Minimumprijs wordt door jou ingesteld.
                </p>
            </header>

            <form className="upload-form" aria-describedby="upload-intro" onSubmit={handleSubmit}>
                <fieldset className="form-grid">
                    <legend className="sr-only">Kavelgegevens</legend>

                    <label className="form-field">
                        <span className="form-label">Productnaam</span>
                        <input
                            name="title"
                            value={form.title}
                            onChange={handleChange}
                            required
                            placeholder="Bijv. Dahlia Summer"
                        />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Aantal stuks</span>
                        <input
                            type="number"
                            name="quantity"
                            value={form.quantity}
                            onChange={handleChange}
                            min="1"
                            required
                        />
                    </label>

                    <label className="form-field full-width">
                        <span className="form-label">Omschrijving</span>
                        <textarea
                            name="description"
                            value={form.description}
                            onChange={handleChange}
                            rows="4"
                        />
                    </label>

                    <label className="form-field full-width">
                        <span className="form-label">Minimumprijs (€)</span>
                        <input
                            type="text" // let op: text, niet number
                            name="minPrice"
                            value={form.minPrice}
                            onChange={handleChange}
                            placeholder="Bijv. 1.50"
                            required
                        />
                    </label>

                    <label className="form-field full-width">
                        <span className="form-label">Upload afbeelding</span>
                        <input type="file" name="image" accept="image/*" onChange={handleChange} />
                    </label>
                </fieldset>

                <div className="form-actions">
                    <button type="button" className="link-button">Opslaan als concept</button>
                    <button type="submit" className="primary-action">Kavel toevoegen</button>
                </div>
            </form>
        </div>
    );
}

export default UploadAuctionPage;