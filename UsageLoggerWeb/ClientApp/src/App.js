import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import About from './components/About';
import Home from './components/Home';

export default () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/about' component={About} />
    </Layout>
);

//<Route path='/fetch-data/:startDateIndex?' component={FetchData} />
