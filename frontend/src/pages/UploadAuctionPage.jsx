import { useState } from 'react';
import '../assets/css/UploadAuctionPage.css';

const defaultForm = {
    title: '',
    variety: '',
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
        } else {
            setForm(prev => ({ ...prev, [name]: value }));
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!form.minPrice || Number(form.minPrice) <= 0) {
            return alert("Vul een geldige minimumprijs in.");
        }

        if (!form.title.trim() || !form.quantity) {
            return alert("Productnaam en aantal stuks zijn verplicht.");
        }

        const formData = new FormData();
        formData.append("Title", form.title);
        if (form.variety) formData.append("Variety", form.variety);
        formData.append("Quantity", form.quantity);
        if (form.description) formData.append("Description", form.description);
        formData.append("MinPrice", form.minPrice);
        if (form.image) formData.append("Image", form.image);
        formData.append("LeverancierID", 2); // tijdelijk, vervang met dynamisch ID indien nodig

        try {
            const response = await fetch("https://localhost:7054/api/Product/upload", {
                method: "POST",
                body: formData
            });

            if (!response.ok) {
                const err = await response.text();
                return alert("Fout vanuit server: " + err);
            }

            alert("Kavel is succesvol geüpload!");
            setForm(defaultForm);

        } catch (error) {
            alert("Er ging iets mis: " + error.message);
        }
    };

    return (
        <div className="upload-page">
            <header className="section-header">
                <h1>Upload nieuwe veiling (leverancier)</h1>
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
                        <span className="form-label">Variëteit</span>
                        <input
                            name="variety"
                            value={form.variety}
                            onChange={handleChange}
                            placeholder="Kleur of soort"
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
                            type="number"
                            name="minPrice"
                            value={form.minPrice}
                            onChange={handleChange}
                            min="0.01"
                            step="0.01"
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