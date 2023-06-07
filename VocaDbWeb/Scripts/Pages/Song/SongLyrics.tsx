import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { useCultureCodes } from '@/CultureCodesContext';
import { SongDetailsForApi } from '@/DataContracts/Song/SongDetailsForApi';
import { SongDetailsTabs } from '@/Pages/Song/SongDetailsRoutes';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { functions } from '@/Shared/GlobalFunctions';
import { SongDetailsStore } from '@/Stores/Song/SongDetailsStore';
import { useLocationStateStore } from '@/route-sphere';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';

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
		const { getCodeDescription } = useCultureCodes();

		useLocationStateStore(songDetailsStore.lyricsStore);

		return (
			<SongDetailsTabs
				model={model}
				songDetailsStore={songDetailsStore}
				tab="lyrics"
			>
				<ul className="nav nav-pills">
					{model.lyrics
						.sortBy((l) => l.translationType)
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
										? 'Romanized' /* LOC */
										: lyrics.cultureCodes
										? lyrics.cultureCodes
												.map((c) =>
													c === ''
														? t('ViewRes.Song:Details.LyricsLanguageOther')
														: getCodeDescription(c)?.nativeName,
												)
												.join(', ')
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
