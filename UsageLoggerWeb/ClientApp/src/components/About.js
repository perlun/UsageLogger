import React, { Component } from 'react';
import { connect } from 'react-redux';

class About extends Component {
    render() {
        return (
            <div>
                <p>
                    This page displays information collected by UsageLogger,
                    running on one or more Windows machines.
                </p>
                <p>
                    The UsageLogger source code can be found here: <a href="https://github.com/perlun/UsageLogger">https://github.com/perlun/UsageLogger</a>
                </p>
            </div>
        );
    }
}

export default connect()(About);
