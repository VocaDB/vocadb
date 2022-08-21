import { Layout } from '@/Components/Shared/Layout';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import React from 'react';
import { useTranslation } from 'react-i18next';

const PlaylistIndex = (): React.ReactElement => {
	const { t, ready } = useTranslation(['ViewRes.Search']);

	const title = t('ViewRes.Search:Index.Playlist');

	useVocaDbTitle(title, ready);

	return <Layout title={title}></Layout>;
};

export default PlaylistIndex;
