// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/8a7e095e8032fdeac4fd1fdb41e6dfb452ae4494/src/ThemeProvider.tsx
import PropTypes from 'prop-types';
import * as React from 'react';
import { useContext, useMemo } from 'react';

export interface ThemeProviderProps {
	prefixes: Record<string, unknown>;
}

const ThemeContext = React.createContext<any>({});
const { Consumer, Provider } = ThemeContext;

function ThemeProvider({ prefixes, children }: any): React.ReactElement {
	const copiedPrefixes = useMemo(() => ({ ...prefixes }), [prefixes]);

	return <Provider value={copiedPrefixes}>{children}</Provider>;
}

ThemeProvider.propTypes = {
	prefixes: PropTypes.object.isRequired,
};

export function useBootstrapPrefix(
	prefix: string | undefined,
	defaultPrefix: string,
): string {
	const prefixes = useContext(ThemeContext);
	return prefix || prefixes[defaultPrefix] || defaultPrefix;
}

export { Consumer as ThemeConsumer };
export default ThemeProvider;
