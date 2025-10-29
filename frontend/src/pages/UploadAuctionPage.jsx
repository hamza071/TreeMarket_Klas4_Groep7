const defaultForm = {
    title: '',
    variety: '',
    quantity: '',
    price: '',
    closing: '',
    description: '',
}

function UploadAuctionPage() {
    return (
        <div className="upload-page">
            <header className="section-header">
                <div>
                    <h1>Upload nieuwe veiling</h1>
                    <p>Voer je kavelgegevens in en maak deze direct beschikbaar voor kopers.</p>
                </div>
                <button type="button" className="secondary-action">
                    Voorbeeld bekijken
                </button>
            </header>

            <form className="upload-form" onSubmit={(event) => event.preventDefault()}>
                <div className="form-grid">
                    <label className="form-field">
                        <span>Productnaam</span>
                        <input type="text" placeholder="Bijv. Dahlia Summer" defaultValue={defaultForm.title} />
                    </label>
                    <label className="form-field">
                        <span>Variëteit</span>
                        <input type="text" placeholder="Kleur of soort" defaultValue={defaultForm.variety} />
                    </label>
                    <label className="form-field">
                        <span>Aantal stuks</span>
                        <input type="number" placeholder="0" min="0" defaultValue={defaultForm.quantity} />
                    </label>
                    <label className="form-field">
                        <span>Startprijs per bos</span>
                        <input type="text" placeholder="€ 0,00" defaultValue={defaultForm.price} />
                    </label>
                    <label className="form-field">
                        <span>Sluitingstijd</span>
                        <input type="time" defaultValue={defaultForm.closing} />
                    </label>
                </div>

                <label className="form-field">
                    <span>Omschrijving</span>
                    <textarea
                        rows="4"
                        placeholder="Beschrijf de kwaliteit, verpakking en eventuele keurmerken van het product."
                        defaultValue={defaultForm.description}
                    />
                </label>

                <div className="form-actions">
                    <button type="button" className="link-button">
                        Opslaan als concept
                    </button>
                    <button type="submit" className="primary-action">
                        Kavel publiceren
                    </button>
                </div>
            </form>
        </div>
    )
}

export default UploadAuctionPage