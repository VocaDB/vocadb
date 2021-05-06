import './i18n';
import React from 'react';
import VocaDbPageProvider, { VocaDbPage } from './VocaDbPageProvider';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import ErrorNotFound from './Components/Error/ErrorNotFound';

interface AppProps {
  initialPage: VocaDbPage;
}

const App = ({ initialPage }: AppProps): React.ReactElement => {
  return (
    <VocaDbPageProvider value={initialPage}>
      <BrowserRouter>
        <Routes>
          <Route path="/*" element={<ErrorNotFound />} />
        </Routes>
      </BrowserRouter>
    </VocaDbPageProvider>
  );
};

export default App;
