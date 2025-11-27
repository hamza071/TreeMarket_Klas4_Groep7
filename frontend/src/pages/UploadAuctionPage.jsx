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

function UploadAuctionPage({ addNewLot }) {
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
            return alert('Vul een geldige minimumprijs in.');
        }

        const formData = new FormData();
        formData.append('name', form.title);
        formData.append('variety', form.variety);
        formData.append('quantity', form.quantity);
        formData.append('description', form.description);
        formData.append('minPrice', Number(form.minPrice));
        if (form.image) formData.append('image', form.image);

        try {
            const response = await fetch('/api/Product', {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                const savedLot = await response.json();
                // Frontend state bijwerken
                addNewLot({
                    ...savedLot,
                    status: 'pending', // zorg dat het zichtbaar wordt in VeilingPage
                    image: form.image ? URL.createObjectURL(form.image) : null
                });
                setForm(defaultForm);
                alert('Kavel toegevoegd! Deze verschijnt bij Veiling.');
            } else {
                console.error('Fout bij toevoegen kavel');
            }
        } catch (err) {
            console.error(err);
        }
    };

    return (
        <div className="upload-page">
            <header className="section-header">
                <h1>Upload nieuwe veiling (leverancier)</h1>
                <p id="upload-intro">
                    Voer je kavelgegevens in. Minimumprijs wordt door jou ingesteld, beginprijs en sluitingstijd later door de veilingmeester.
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
                            placeholder="Beschrijf kwaliteit, verpakking en keurmerken"
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
                    <button type="submit" className="primary-action">Kavel toevoegen</button>
                </div>
            </form>
        </div>
    );
}

export default UploadAuctionPage;
