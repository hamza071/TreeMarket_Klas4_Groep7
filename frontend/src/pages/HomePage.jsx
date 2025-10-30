import toadImage from '../assets/MLG Toad.jpg'


function HomePage() {
    return (
         <main>
            <article>
                <section>
                    <h1>Veilingen</h1>
                    <p>VInd hier veilingen voor de sierteeltsector</p>

                    <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                </section>

                <section>
                    <h2>Section heading</h2>
                    <div class="container_column">
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Test</h3>
                            <p>Body text for whatever you’d like to add more to the subheading. </p>
                        </div>
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Test</h3>
                            <p>Body text for whatever you’d like to add more to the subheading. </p>
                        </div>
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Test</h3>
                            <p>Body text for whatever you’d like to add more to the subheading. </p>
                        </div>
                    </div>
                </section>

                <section>
                    <h2>Section heading</h2>
                    <div class="container_row">
                        <aside>
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                        </aside>
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Test</h3>
                            <p>Body text for whatever you’d like to add more to the subheading. </p>
                        </div>
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Test</h3>
                            <p>Body text for whatever you’d like to add more to the subheading. </p>
                        </div>
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Test</h3>
                            <p>Body text for whatever you’d like to add more to the subheading. </p>
                        </div>
                        <div class="button_positioning">
                            <a href="login.html">
                                <button type="button" class="btn btn-success">
                                    Primary
                                </button>
                            </a>
                            <button type="button" class="btn btn-secondary">Secondary</button>
                        </div>
                    </div>
                </section>

                <section>
                    <h2>Section Heading</h2>
                    <div class="container_column_2_tiles">
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Subheading</h3>
                            <p>Body text for whatever you'd like to add mroe to the subheading'</p>
                        </div>
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Subheading</h3>
                            <p>Body text for whatever you’d like to expand on the main point.</p>
                        </div>
                    </div>
                </section>

                <section>
                    <h2>Section Heading</h2>
                    <div class="container_column">
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Subheading</h3>
                            <p>Body text for whatever you'd like to add mroe to the subheading'</p>
                        </div>
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Subheading</h3>
                            <p>Body text for whatever you’d like to expand on the main point.</p>
                        </div>
                        <div class="grid">
                            <img src={toadImage} alt="MLG Toad" height="150px" max-width="auto" />
                            <h3>Subheading</h3>
                            <p>Body text for whatever you’d like to expand on the main point.</p>
                        </div>
                    </div>
                </section>

            </article>
        </main>



    );
}

export default HomePage

