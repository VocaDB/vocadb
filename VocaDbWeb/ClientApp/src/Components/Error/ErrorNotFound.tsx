import React from 'react';
import Layout from '../Shared/Layout';

const ErrorNotFound = (): React.ReactElement => (
  <Layout
    title="404 - Not found"
    subtitle="Sorry, the page/resource you were looking for was not found"
  >
    <iframe
      width="560"
      height="315"
      src="https://www.youtube.com/embed/z4D6bwsU6CA"
      frameBorder="0"
      /*wmode="Opaque"*/
      allowFullScreen
    ></iframe>
    <br />
    <br />

    <p>
      <a href={'/Details/Song/18486'}>Video entry</a>
    </p>
  </Layout>
);

export default ErrorNotFound;
