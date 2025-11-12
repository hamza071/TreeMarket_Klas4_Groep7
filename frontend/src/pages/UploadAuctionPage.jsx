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
                    <p id="upload-intro">Voer je kavelgegevens in en maak deze direct beschikbaar voor kopers.</p>
                </div>
                <button type="button" className="secondary-action">
                    Voorbeeld bekijken
                </button>
            </header>

            <form className="upload-form" aria-describedby="upload-intro" onSubmit={(event) => event.preventDefault()}>
                <fieldset className="form-grid">
                    <legend className="sr-only">Kavelgegevens</legend>
                    <label className="form-field" htmlFor="auction-title">
                        <span className="form-label">Productnaam</span>
                        <input
                            id="auction-title"
                            name="title"
                            type="text"
                            placeholder="Bijvoorbeeld: Dahlia Summer"
                            defaultValue={defaultForm.title}
                            autoComplete="off"
                            required
                        />
                    </label>
                    <label className="form-field" htmlFor="auction-variety">
                        <span className="form-label">Variëteit</span>
                        <input
                            id="auction-variety"
                            name="variety"
                            type="text"
                            placeholder="Kleur of soort"
                            defaultValue={defaultForm.variety}
                            autoComplete="off"
                        />
                    </label>
                    <label className="form-field" htmlFor="auction-quantity">
                        <span className="form-label">Aantal stuks</span>
                        <input
                            id="auction-quantity"
                            name="quantity"
                            type="number"
                            placeholder="0"
                            min="0"
                            defaultValue={defaultForm.quantity}
                            aria-describedby="quantity-help"
                            required
                        />
                        <small id="quantity-help" className="field-help">
                            Vul het aantal bossen of planten in zoals getoond op de veilingklok.
                        </small>
                    </label>
                    <label className="form-field" htmlFor="auction-price">
                        <span className="form-label">Startprijs per bos</span>
                        <input
                            id="auction-price"
                            name="price"
                            type="text"
                            placeholder="€ 0,00"
                            defaultValue={defaultForm.price}
                            inputMode="decimal"
                        />
                    </label>
                    <label className="form-field" htmlFor="auction-closing">
                        <span className="form-label">Sluitingstijd</span>
                        <input
                            id="auction-closing"
                            name="closing"
                            type="time"
                            defaultValue={defaultForm.closing}
                            required
                        />
                    </label>
                </fieldset>

                <label className="form-field" htmlFor="auction-description">
                    <span className="form-label">Omschrijving</span>
                    <textarea
                        id="auction-description"
                        name="description"
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