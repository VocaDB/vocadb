import React, { useEffect, useState } from 'react';

import { userLanguageCultureFamilies } from './Components/userLanguageCultureFamilies';
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
	getCodeGroup: (code: string, extended?: boolean) => string[];
}

// Lazily loads extendedUserLanguageCultures
export const CultureCodesProvider = ({
	children,
}: CultureCodesProviderProps): React.ReactElement => {
	const [codeData, setCodeData] = useState<CultureCodeData | undefined>(
		undefined,
	);

	useEffect(() => {
		import('./Components/extendedUserLanguageCultures').then((resp) => {
			setCodeData({
				codes: resp.extendedUserLanguageCultures,
				iso639to1: resp.iso6393To1,
			});
		});
	}, []);

	return (
		<CultureCodesContext.Provider value={codeData}>
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

	const getCodeGroup = (code: string, extended = false): string[] => {
		if (code === '') {
			return [];
		}
		if (code in userLanguageCultureFamilies) {
			//@ts-ignore
			return [code, ...userLanguageCultureFamilies[code]];
		}
		return [code];
	};

	return { ...codes, getCodeDescription, getCodeGroup };
};
