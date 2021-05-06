import VocaDbPageContext, { VocaDbPage } from '@Components/VocaDbPageContext';
import React from 'react';

import './i18n';

interface AppProps {
	initialPage: VocaDbPage;
}

const App = ({ initialPage }: AppProps): React.ReactElement => {
	return (
		<VocaDbPageContext.Provider
			value={initialPage}
		></VocaDbPageContext.Provider>
	);
};

export default App;
