import React from 'react';
import ReactDOM from 'react-dom';

import App from './App';

const app = document.getElementById('app');

ReactDOM.render(
  <React.StrictMode>
    <App initialPage={app ? JSON.parse(app.dataset.page!) : '{}'} />
  </React.StrictMode>,
  app,
);
