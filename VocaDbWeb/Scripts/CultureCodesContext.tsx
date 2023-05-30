import React, { useEffect, useState } from 'react';

import { userLanguageCultures } from './Components/userLanguageCultures';

type CodeDescription = { englishName: string; nativeName: string };
type Codes = Record<string, CodeDescription>;

const CultureCodesContext = React.createContext<CultureCodeData | undefined>(
	undefined,
);

interface CultureCodesProviderProps {
	children?: React.ReactNode;
}

interface CultureCodeData {
	codes?: Codes;
	iso639to1?: Record<string, string>;
}

interface CultureCodeTools {
	codes?: Codes;
	getCodeDescription: (code: string) => CodeDescription | undefined;
	iso639to1?: Record<string, string>;
}

// Lazily loads extendedUserLanguageCultures
export const CultureCodesProvider = ({
	children,
}: CultureCodesProviderProps): React.ReactElement => {
	const [cultureCodes, setCultureCodes] = useState<Codes | undefined>(
		undefined,
	);
	const [iso639to1, setIso639to1] = useState<Record<string, string>>({});

	useEffect(() => {
		import('./Components/extendedUserLanguageCultures').then((resp) => {
			setCultureCodes(resp.extendedUserLanguageCultures);
			setIso639to1(resp.iso6393To1);
		});
	}, []);

	return (
		<CultureCodesContext.Provider value={{ codes: cultureCodes, iso639to1 }}>
			{children}
		</CultureCodesContext.Provider>
	);
};

export const useCultureCodes = (): CultureCodeTools => {
	const codes = React.useContext(CultureCodesContext);
	const getCodeDescription = (code: string): CodeDescription | undefined => {
		if (!(code in userLanguageCultures)) {
			return codes?.codes === undefined ? undefined : codes.codes[code];
		}

		return userLanguageCultures[code];
	};
	return { ...codes, getCodeDescription };
};
