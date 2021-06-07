import { Container } from 'inversify';
import React from 'react';

const InversifyContext = React.createContext<{ container: Container }>({
	container: undefined!,
});

export default InversifyContext;
