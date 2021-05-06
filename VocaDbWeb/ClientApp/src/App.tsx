import './i18n';
import React from 'react';
import VocaDbPageProvider, { VocaDbPage } from './VocaDbPageProvider';

interface AppProps {
  initialPage: VocaDbPage;
}

const App = ({ initialPage }: AppProps): React.ReactElement => {
  return <VocaDbPageProvider value={initialPage}></VocaDbPageProvider>;
};

export default App;
