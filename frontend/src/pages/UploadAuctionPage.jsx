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

        // Product payload voor backend
        const productPayload = {
            Foto: form.image
                ? URL.createObjectURL(form.image)
                : "https://dummyimage.com/600x400/000/fff.png&text=Placeholder",
            Artikelkenmerken: form.variety || form.title,
            Hoeveelheid: Number(form.quantity) || 1,
            MinimumPrijs: Number(form.minPrice),
            Dagdatum: new Date().toISOString(),
            LeverancierID: 2 // PAS AAN: echte leverancier id
        };

        try {
            // POST naar backend via proxy
            const productResp = await fetch('/api/product', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(productPayload)
            });

            if (!productResp.ok) {
                const err = await productResp.json().catch(() => null);
                return alert('Kon product niet aanmaken: ' + (err?.message || productResp.statusText));
            }

            const createdProduct = await productResp.json();
            const productId = createdProduct?.productId ?? createdProduct?.ProductId;

            if (!productId) {
                return alert('Geen productId ontvangen van backend.');
            }

            // Maak nieuwe veiling
            const newLot = {
                Code: 'X' + Math.floor(Math.random() * 100000),
                Naam: form.title,
                Beschrijving: form.description,
                Lots: Number(form.quantity) || 1,
                Image: productPayload.Foto,
                Status: 'pending',
                ProductID: productId
            };

            const veilingResp = await fetch('/api/veiling', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newLot)
            });

            if (!veilingResp.ok) {
                const err = await veilingResp.json().catch(() => null);
                return alert('Kon veiling niet aanmaken: ' + (err?.message || veilingResp.statusText));
            }

            const createdVeiling = await veilingResp.json();

            // Update parent state
            addNewLot({
                code: createdVeiling.code ?? newLot.Code,
                name: createdVeiling.naam ?? newLot.Naam,
                specs: productPayload.Artikelkenmerken,
                lots: createdVeiling.lots ?? newLot.Lots,
                description: createdVeiling.beschrijving ?? newLot.Beschrijving,
                image: productPayload.Foto,
                minPrice: productPayload.MinimumPrijs,
                status: createdVeiling.status ?? newLot.Status,
                productId: productId
            });

            setForm(defaultForm);
            alert('Kavel toegevoegd en opgeslagen in database!');
        } catch (err) {
            console.error(err);
            alert('Er ging iets mis bij het aanmaken van de kavel.');
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
                    <button type="button" className="link-button">Opslaan als concept</button>
                    <button type="submit" className="primary-action">Kavel toevoegen</button>
                </div>
            </form>
        </div>
    );
}

export default UploadAuctionPage;