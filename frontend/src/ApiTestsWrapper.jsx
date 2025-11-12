import React from "react";
import ProductenTest from "./api-tests/ProductenTest";
import NieuwProductTest from "./api-tests/NieuwProductTest";
import LeverancierTest from "./api-tests/LeverancierTest";

export default function ApiTestsWrapper() {
    return (
        <div>
            <h1>API Tests</h1>

            <section style={{ border: "1px solid gray", margin: "20px", padding: "10px" }}>
                <h2>GET Producten</h2>
                <ProductenTest />
            </section>

            <section style={{ border: "1px solid gray", margin: "20px", padding: "10px" }}>
                <h2>POST Nieuw Product</h2>
                <NieuwProductTest />
            </section>

            <section style={{ border: "1px solid gray", margin: "20px", padding: "10px" }}>
                <h2>POST Leverancier</h2>
                <LeverancierTest />
            </section>
        </div>
    );
}