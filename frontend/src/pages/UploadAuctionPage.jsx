import { useState } from 'react';
import '../assets/css/UploadAuctionPage.css';

const defaultForm = {
    title: '',
    variety: '',
    quantity: '',
    price: '',
    closing: 10,
    description: '',
    image: null, // nieuw veld voor plaatje
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

    const handleSubmit = (e) => {
        e.preventDefault();

        const closingTimestamp = Date.now() + Number(form.closing) * 1000;

        const newLot = {
            code: 'X' + Math.floor(Math.random() * 100000),
            name: form.title,
            specs: form.variety,
            lots: form.quantity,
            price: `€${form.price}`,
            closing: Number(form.closing),
            closingTimestamp,
            description: form.description,
            image: form.image ? URL.createObjectURL(form.image) : null, // previewable image
        };

        addNewLot(newLot);
        setForm(defaultForm);
        alert('Kavel toegevoegd!');
    };

    return (
        <div className="upload-page">
            <header className="section-header">
                <h1>Upload nieuwe veiling</h1>
                <p id="upload-intro">Voer je kavelgegevens in en maak deze direct beschikbaar voor kopers.</p>
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
                            min="0"
                            required
                        />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Startprijs per bos (€)</span>
                        <input
                            name="price"
                            value={form.price}
                            onChange={handleChange}
                            placeholder="0"
                        />
                    </label>

                    <label className="form-field">
                        <span className="form-label">Sluitingstijd (seconden)</span>
                        <input
                            type="number"
                            name="closing"
                            value={form.closing}
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
                        <span className="form-label">Upload afbeelding</span>
                        <input type="file" name="image" accept="image/*" onChange={handleChange} />
                    </label>
                </fieldset>

                <div className="form-actions">
                    <button type="button" className="link-button">Opslaan als concept</button>
                    <button type="submit" className="primary-action">Kavel publiceren</button>
                </div>
            </form>
        </div>
    );
}

export default UploadAuctionPage;