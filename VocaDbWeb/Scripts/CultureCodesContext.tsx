import React, { useEffect, useState } from 'react';

import { userLanguageCultures } from './Components/userLanguageCultures';

type CodeDescription = { englishName: string; nativeName: string };
type Codes = { [k: string]: CodeDescription };

const CultureCodesContext = React.createContext<Codes | undefined>(undefined);

interface CultureCodesProviderProps {
	children?: React.ReactNode;
}

interface CultureCodeTools {
	codes?: Codes;
	getCodeDescription: (code: string) => CodeDescription | undefined;
}

// Lazily loads extendedUserLanguageCultures
export const CultureCodesProvider = ({
	children,
}: CultureCodesProviderProps): React.ReactElement => {
	const [cultureCodes, setCultureCodes] = useState<Codes | undefined>(
		undefined,
	);

	useEffect(() => {
		import('./Components/extendedUserLanguageCultures').then((resp) =>
			setCultureCodes(resp.extendedUserLanguageCultures),
		);
	}, []);

	return (
		<CultureCodesContext.Provider value={cultureCodes}>
			{children}
		</CultureCodesContext.Provider>
	);
};

export const useCultureCodes = (): CultureCodeTools => {
	const codes = React.useContext(CultureCodesContext);

	const getCodeDescription = (code: string): CodeDescription | undefined => {
		if (!(code in userLanguageCultures)) {
			return codes === undefined ? undefined : codes[code];
		}

		return userLanguageCultures[code];
	};

	return { codes, getCodeDescription };
};
