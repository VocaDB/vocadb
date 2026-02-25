import { useVdb } from '@/VdbContext';
import { useTranslation } from 'react-i18next';

const BRANDABLE_NAMESPACE: Record<string, string> = {
	UtaiteDB: 'VocaDb.UtaiteDb.Resources.ViewRes',
	TouhouDB: 'VocaDb.TouhouDb.Resources.ViewRes',
};

export const useBrandableTranslation = () => {
	const { values } = useVdb();
	const ns =
		BRANDABLE_NAMESPACE[values.siteName] ?? 'VocaDb.Web.Resources.ViewRes';
	return useTranslation([ns]);
};
