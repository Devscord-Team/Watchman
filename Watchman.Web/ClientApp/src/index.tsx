import "bootstrap/dist/css/bootstrap.css";

import * as React from "react";
import * as ReactDOM from "react-dom";
import { createBrowserHistory } from "history";
import App from "./App";
import { Router } from "react-router";
//import registerServiceWorker from './registerServiceWorker';

// Create browser history to use in the Redux store
const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href") as string;
const history = createBrowserHistory({ basename: baseUrl });

// Get the application-wide store instance, prepopulating with state from the server where available.

console.log("dupa");

ReactDOM.render(
    <Router history={history}>
        <App></App>
    </Router>,
    document.getElementById("root")
);

//registerServiceWorker();
