import SafeAnchor from '@Bootstrap/SafeAnchor';
import SongDetailsForApi from '@DataContracts/Song/SongDetailsForApi';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import functions from '@Shared/GlobalFunctions';
import SongDetailsStore from '@Stores/Song/SongDetailsStore';
import { useStoreWithUpdateResults } from '@vocadb/route-sphere';
import classNames from 'classnames';
import _ from 'lodash';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';

import { userLanguageCultures } from '../userLanguageCultures';
import { SongDetailsTabs } from './SongDetailsRoutes';

interface SongLyricsProps {
	model: SongDetailsForApi;
	songDetailsStore: SongDetailsStore;
}

const SongLyrics = observer(
	({ model, songDetailsStore }: SongLyricsProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes.Song',
			'VocaDb.Web.Resources.Domain.Globalization',
		]);

		useStoreWithUpdateResults(songDetailsStore.lyricsStore);

		return (
			<SongDetailsTabs
				model={model}
				songDetailsStore={songDetailsStore}
				tab="lyrics"
			>
				<ul className="nav nav-pills">
					{_.chain(model.lyrics)
						.sortBy((l) => l.translationType)
						.value()
						.map((lyrics) => (
							<li
								className={classNames(
									songDetailsStore.selectedLyricsId === lyrics.id && 'active',
								)}
								key={lyrics.id}
							>
								<SafeAnchor
									href={`${EntryUrlMapper.details_song(
										model.contract.song,
									)}/lyrics?${qs.stringify({
										albumId: model.browsedAlbumId,
										lyricsId: lyrics.id,
									})}`}
									onClick={(e): void => {
										e.preventDefault();

										runInAction(() => {
											songDetailsStore.selectedLyricsId = lyrics.id!;
										});
									}}
								>
									{lyrics.translationType === 'Romanized'
										? 'Romanized' /* TODO: localize */
										: lyrics.cultureCode
										? userLanguageCultures[lyrics.cultureCode].nativeName
										: t('ViewRes.Song:Details.LyricsLanguageOther')}
									{lyrics.translationType === 'Original' &&
										` (${t(
											'VocaDb.Web.Resources.Domain.Globalization:TranslationTypeNames.Original',
										)})`}
								</SafeAnchor>
							</li>
						))}
				</ul>

				{songDetailsStore.selectedLyrics && (
					<div>
						{(songDetailsStore.selectedLyrics.source ||
							songDetailsStore.selectedLyrics.url) && (
							<p>
								({t('ViewRes.Song:Details.LyricsFrom')}{' '}
								{songDetailsStore.selectedLyrics.url ? (
									<span>
										<a
											className="extLink"
											href={songDetailsStore.selectedLyrics.url}
											onClick={(e): void =>
												functions.trackOutboundLink(e.nativeEvent)
											}
										>
											{songDetailsStore.selectedLyrics.source ||
												songDetailsStore.selectedLyrics.url}
										</a>
									</span>
								) : (
									<span>{songDetailsStore.selectedLyrics.source}</span>
								)}
								)
							</p>
						)}

						<p className="song-lyrics">
							{songDetailsStore.selectedLyrics.value}
						</p>
					</div>
				)}
			</SongDetailsTabs>
		);
	},
);

export default SongLyrics;
