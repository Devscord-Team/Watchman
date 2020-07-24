import * as React from "react";
import { Route } from "react-router";
import Layout from "./components/Layout";
import Home from "./components/Home/Home";
import Responses from "./components/Responses/Responses";

import "./custom.css";

export default () => (
    <Layout>
        <Route exact path="/" component={Home} />
        <Route exact path="/responses" component={Responses} />
    </Layout>
);
