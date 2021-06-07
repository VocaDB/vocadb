import { interfaces } from 'inversify';
import React from 'react';

import InversifyContext from './InversifyContext';

const useInjection = <T>(identifier: interfaces.ServiceIdentifier<T>): T => {
	const { container } = React.useContext(InversifyContext);
	if (!container) throw new Error();
	return container.get<T>(identifier);
};

export default useInjection;
