//import { StrictMode } from 'react'
//import { createRoot } from 'react-dom/client'
//import './assets/css/index.css'
//import App from './App.jsx'

//createRoot(document.getElementById('root')).render(
//  <StrictMode>
//    <App />
//  </StrictMode>,
//)

// HIERONDER IS TIJDELIJK OM DE ENDPOINTS TE TESTEN, HIERBOVEN WAS DE OUDE CODE!!

import React from "react";
import ReactDOM from "react-dom/client";
import ApiTestsWrapper from "./ApiTestsWrapper"; // <-- nieuw bestand

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
    <React.StrictMode>
        <ApiTestsWrapper />
    </React.StrictMode>
);