import React from 'react';

import VocaDbPageContext, { VocaDbPage } from './VocaDbPageContext';

const useVocaDbPage = (): VocaDbPage => React.useContext(VocaDbPageContext);

export default useVocaDbPage;
