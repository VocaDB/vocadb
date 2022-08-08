import LargeShareLinks from '@/Components/Shared/Partials/EntryDetails/LargeShareLinks';
import AlbumDetailsForApi from '@/DataContracts/Album/AlbumDetailsForApi';
import { MarkdownHelper } from '@/Helpers/MarkdownHelper';
import EntryType from '@/Models/EntryType';
import { AlbumDetailsTabs } from '@/Pages/Album/AlbumDetailsRoutes';
import EntryUrlMapper from '@/Shared/EntryUrlMapper';
import functions from '@/Shared/GlobalFunctions';
import AlbumDetailsStore from '@/Stores/Album/AlbumDetailsStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface AlbumShareProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumShare = ({
	model,
	albumDetailsStore,
}: AlbumShareProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	const url = functions.mergeUrls(
		vdb.values.hostAddress,
		EntryUrlMapper.details(EntryType.Album, model.id),
	);

	return (
		<AlbumDetailsTabs
			model={model}
			albumDetailsStore={albumDetailsStore}
			tab="share"
		>
			<div>
				<label>
					{t('ViewRes:EntryDetails.Social')}
					<br />
					<LargeShareLinks title={model.name} url={url} />
					<br />
					<br />
				</label>
			</div>
			<div>
				{t('ViewRes:EntryDetails.Link')}
				<br />
				<input
					type="text"
					defaultValue={url}
					className="input-large"
					onFocus={(e): void => e.target.select()}
				/>
			</div>
			<div>
				<a href="http://daringfireball.net/projects/markdown/">
					Markdown{/* TODO: localize */}
				</a>
				<br />
				<input
					type="text"
					defaultValue={MarkdownHelper.createMarkdownLink(url, model.name)}
					className="input-xxlarge"
					onFocus={(e): void => e.target.select()}
				/>
			</div>
		</AlbumDetailsTabs>
	);
};

export default AlbumShare;
