import { userLanguageCultures } from '@/Components/userLanguageCultures';
import { SongDetailsForApi } from '@/DataContracts/Song/SongDetailsForApi';
import { JQueryUINavItemComponent } from '@/JQueryUI/JQueryUITabs';
import SongBasicInfo from '@/Pages/Song/SongBasicInfo';
import SongDiscussion from '@/Pages/Song/SongDiscussion';
import SongLyrics from '@/Pages/Song/SongLyrics';
import SongRelated from '@/Pages/Song/SongRelated';
import SongShare from '@/Pages/Song/SongShare';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { SongDetailsStore } from '@/Stores/Song/SongDetailsStore';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, Route, Routes } from 'react-router-dom';

interface SongDetailsTabsProps {
	model: SongDetailsForApi;
	songDetailsStore: SongDetailsStore;
	tab: 'basicInfo' | 'lyrics' | 'discussion' | 'related' | 'share';
	children?: React.ReactNode;
}

export const SongDetailsTabs = React.memo(
	({
		model,
		songDetailsStore,
		tab,
		children,
	}: SongDetailsTabsProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Song',
			'VocaDb.Web.Resources.Domain.Globalization',
		]);

		const lyricsLanguageNames = model.lyrics
			.sortBy((l) => l.translationType)
			.filter((l) => !!l.cultureCode || l!.translationType === 'Romanized')
			.map((l) =>
				l.translationType !== 'Romanized'
					? userLanguageCultures[l.cultureCode!].nativeName
					: t(
							'VocaDb.Web.Resources.Domain.Globalization:TranslationTypeNames.Romanized',
					  ),
			)
			.take(3);

		const additionalLyrics =
			lyricsLanguageNames.length > 0
				? model.lyrics.length - lyricsLanguageNames.length
				: 0;

		const lyricsLanguages =
			lyricsLanguageNames.length > 0
				? `(${lyricsLanguageNames.join(', ')}${
						additionalLyrics > 0
							? ` ${t('ViewRes.Song:Details.LyricsPlusOthers', {
									0: additionalLyrics,
							  })}`
							: ''
				  })`
				: '';

		return (
			<div className="js-cloak ui-tabs ui-widget ui-widget-content ui-corner-all">
				<ul
					className="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all"
					role="tablist"
				>
					<JQueryUINavItemComponent active={tab === 'basicInfo'}>
						<Link
							to={`${EntryUrlMapper.details_song(
								model.contract.song,
							)}?${qs.stringify({ albumId: model.browsedAlbumId })}`}
						>
							{t('ViewRes:EntryDetails.BasicInfoTab')}
						</Link>
					</JQueryUINavItemComponent>
					{model.lyrics.length > 0 && (
						<JQueryUINavItemComponent active={tab === 'lyrics'}>
							<Link
								to={`${EntryUrlMapper.details_song(
									model.contract.song,
								)}/lyrics?${qs.stringify({
									albumId: model.browsedAlbumId,
									lyricsId: songDetailsStore.selectedLyricsId,
								})}`}
							>
								{t('ViewRes.Song:Details.Lyrics')} {lyricsLanguages}
							</Link>
						</JQueryUINavItemComponent>
					)}
					<JQueryUINavItemComponent active={tab === 'discussion'}>
						<Link
							to={`${EntryUrlMapper.details_song(
								model.contract.song,
							)}/discussion?${qs.stringify({ albumId: model.browsedAlbumId })}`}
						>
							{t('ViewRes:EntryDetails.DiscussionTab')} ({model.commentCount})
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'related'}>
						<Link
							to={`${EntryUrlMapper.details_song(
								model.contract.song,
							)}/related?${qs.stringify({ albumId: model.browsedAlbumId })}`}
						>
							{t('ViewRes.Song:Details.RelatedSongs')}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'share'}>
						<Link
							to={`${EntryUrlMapper.details_song(
								model.contract.song,
							)}/share?${qs.stringify({ albumId: model.browsedAlbumId })}`}
						>
							{t('ViewRes.Song:Details.Share')}
						</Link>
					</JQueryUINavItemComponent>
				</ul>

				<div
					className="ui-tabs-panel ui-widget-content ui-corner-bottom"
					role="tabpanel"
				>
					{children}
				</div>
			</div>
		);
	},
);

interface SongDetailsRoutesProps {
	model: SongDetailsForApi;
	songDetailsStore: SongDetailsStore;
}

const SongDetailsRoutes = ({
	model,
	songDetailsStore,
}: SongDetailsRoutesProps): React.ReactElement => {
	return (
		<Routes>
			<Route
				path="lyrics"
				element={
					<SongLyrics model={model} songDetailsStore={songDetailsStore} />
				}
			/>
			<Route
				path="discussion"
				element={
					<SongDiscussion model={model} songDetailsStore={songDetailsStore} />
				}
			/>
			<Route
				path="related"
				element={
					<SongRelated model={model} songDetailsStore={songDetailsStore} />
				}
			/>
			<Route
				path="share"
				element={
					<SongShare model={model} songDetailsStore={songDetailsStore} />
				}
			/>
			<Route
				path="*"
				element={
					<SongBasicInfo model={model} songDetailsStore={songDetailsStore} />
				}
			/>
		</Routes>
	);
};

export default SongDetailsRoutes;
