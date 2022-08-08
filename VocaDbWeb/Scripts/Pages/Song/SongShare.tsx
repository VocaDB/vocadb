import LargeShareLinks from '@/Components/Shared/Partials/EntryDetails/LargeShareLinks';
import SongDetailsForApi from '@/DataContracts/Song/SongDetailsForApi';
import { MarkdownHelper } from '@/Helpers/MarkdownHelper';
import { SongDetailsTabs } from '@/Pages/Song/SongDetailsRoutes';
import EntryUrlMapper from '@/Shared/EntryUrlMapper';
import functions from '@/Shared/GlobalFunctions';
import SongDetailsStore from '@/Stores/Song/SongDetailsStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongShareProps {
	model: SongDetailsForApi;
	songDetailsStore: SongDetailsStore;
}

const SongShare = ({
	model,
	songDetailsStore,
}: SongShareProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes', 'ViewRes.Song']);

	const url = functions.mergeUrls(
		vdb.values.hostAddress,
		EntryUrlMapper.details_song(model.contract.song),
	);

	return (
		<SongDetailsTabs
			model={model}
			songDetailsStore={songDetailsStore}
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
				{t('ViewRes.Song:Details.Link')}
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
			<div>
				{t('ViewRes.Song:Details.Embed')}
				<br />
				<textarea
					className="input-xxlarge"
					rows={3}
					defaultValue={`<iframe style="overflow: hidden;" src="${functions.mergeUrls(
						vdb.values.hostAddress,
						`/Ext/EmbedSong`,
					)}?songId=${model.id}&lang=${
						vdb.values.languagePreference
					}" height="400" width="570" frameborder="0" scrolling="no"></iframe>`}
				/>
			</div>
		</SongDetailsTabs>
	);
};

export default SongShare;
