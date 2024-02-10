import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { AlbumLink } from '@/Components/Shared/Partials/Album/AlbumLink';
import {
	ArtistList,
	ShowRolesMode,
} from '@/Components/Shared/Partials/Artist/ArtistList';
import { LatestCommentsKnockout } from '@/Components/Shared/Partials/Comment/LatestCommentsKnockout';
import { EnglishTranslatedString } from '@/Components/Shared/Partials/EnglishTranslatedString';
import { ExternalLinksRows } from '@/Components/Shared/Partials/EntryDetails/ExternalLinksRows';
import { PersonalDescriptionMedia } from '@/Components/Shared/Partials/EntryDetails/PersonalDescriptionMedia';
import { EventLink } from '@/Components/Shared/Partials/Event/EventLink';
import { PVServiceIcon } from '@/Components/Shared/Partials/Shared/PVServiceIcon';
import { UniversalTimeLabel } from '@/Components/Shared/Partials/Shared/UniversalTimeLabel';
import { SongGrid } from '@/Components/Shared/Partials/Song/SongGrid';
import { SongLink } from '@/Components/Shared/Partials/Song/SongLink';
import { SongLinkKnockout } from '@/Components/Shared/Partials/Song/SongLinkKnockout';
import { SongTypeLabel } from '@/Components/Shared/Partials/Song/SongTypeLabel';
import { TagLink } from '@/Components/Shared/Partials/Tag/TagLink';
import { TagList } from '@/Components/Shared/Partials/TagList';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { useCultureCodes } from '@/CultureCodesContext';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { SongDetailsForApi } from '@/DataContracts/Song/SongDetailsForApi';
import { BpmHelper } from '@/Helpers/BpmHelper';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import { PVService } from '@/Models/PVs/PVService';
import { SongType } from '@/Models/Songs/SongType';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import SongInListsDialog from '@/Pages/Song/Partials/SongInListsDialog';
import UsersWithSongRating from '@/Pages/Song/Partials/UsersWithSongRating';
import { SongDetailsTabs } from '@/Pages/Song/SongDetailsRoutes';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { functions } from '@/Shared/GlobalFunctions';
import { SongDetailsStore } from '@/Stores/Song/SongDetailsStore';
import dayjs from '@/dayjs';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { Trans, useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface AlbumLinkWithReleaseYearProps {
	album: AlbumForApiContract;
}

const AlbumLinkWithReleaseYear = React.memo(
	({ album }: AlbumLinkWithReleaseYearProps): React.ReactElement => {
		return (
			<>
				<AlbumLink album={album} tooltip={true} />
				{!album.releaseDate.isEmpty && (
					<>
						{' '}
						<small className="muted">&nbsp;({album.releaseDate.year})</small>
					</>
				)}
			</>
		);
	},
);

interface AlternateVersionsProps {
	songs: SongApiContract[];
}

const AlternateVersions = React.memo(
	({ songs }: AlternateVersionsProps): React.ReactElement => {
		return (
			<>
				{songs.map((alternateVersion) => (
					<React.Fragment key={alternateVersion.id}>
						<SongLink song={alternateVersion} tooltip={true} />
						{alternateVersion.lengthSeconds > 0 && (
							<>
								{' '}
								(
								{DateTimeHelper.formatFromSeconds(
									alternateVersion.lengthSeconds,
								)}
								)
							</>
						)}{' '}
						<SongTypeLabel songType={alternateVersion.songType} />
						<br />
						{alternateVersion.artistString}
						<br />
					</React.Fragment>
				))}
			</>
		);
	},
);

interface SongAlbumLinkProps {
	song: SongApiContract;
	icon: string;
	albumId?: number;
}

const SongAlbumLink = React.memo(
	({ song, icon, albumId }: SongAlbumLinkProps): React.ReactElement => {
		return (
			<Link
				to={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
					albumId: albumId,
				})}`}
				className="btn"
				title={song.additionalNames}
			>
				<i className={classNames(icon, 'noMargin')} /> <span>{song.name}</span>
			</Link>
		);
	},
);

interface PVButtonProps {
	songDetailsStore: SongDetailsStore;
	pv: PVContract;
	showPVType: boolean;
}

const PVButton = observer(
	({ songDetailsStore, pv, showPVType }: PVButtonProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes.Song']);

		const { playQueue } = useVdbPlayer();

		React.useLayoutEffect(() => {
			if (pv.id !== playQueue.currentItem?.pv.id) return;

			runInAction(() => {
				songDetailsStore.selectedPvId = pv.id!;
			});
		}, [songDetailsStore, pv, playQueue]);

		return (
			<Button
				className={classNames(
					'pvLink',
					songDetailsStore.selectedPvId === pv.id && 'active',
				)}
				disabled={pv.disabled}
				href="#"
				id={`pv_${pv.id}`}
				onClick={(): void =>
					runInAction(() => {
						songDetailsStore.selectedPvId = pv.id!;
					})
				}
				title={
					pv.publishDate && pv.author
						? `${t('ViewRes.Song:Details.PVDescription', {
								0: dayjs(pv.publishDate).format('ll') /* REVIEW */,
								1: pv.author,
						  })}${
								pv.disabled ? ` ${t('ViewRes.Song:Details.PVUnavailable')}` : ''
						  }`
						: undefined
				}
			>
				<PVServiceIcon service={pv.service} /> {pv.name || pv.service}
				{showPVType && <> ({t(`Resources:PVTypeNames.${pv.pvType}`)})</>}
			</Button>
		);
	},
);

interface PVListProps {
	songDetailsStore: SongDetailsStore;
	pvs: PVContract[];
	showPVType: boolean;
}

const PVList = observer(
	({ songDetailsStore, pvs, showPVType }: PVListProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Song']);

		return (
			<>
				{pvs.map((pv) => (
					<React.Fragment key={pv.id}>
						<PVButton
							songDetailsStore={songDetailsStore}
							pv={pv}
							showPVType={showPVType}
						/>
						{pv.service !== PVService.File &&
							pv.service !== PVService.LocalFile && (
								<>
									{' '}
									&nbsp;
									<a
										href={pv.url}
										onClick={(e): void =>
											functions.trackOutboundLink(e.nativeEvent)
										}
										target="_blank"
										rel="noreferrer"
									>
										<i className="icon-eye-open" />{' '}
										{t('ViewRes.Song:Details.ViewExternal')}
									</a>
								</>
							)}
						{pv.service === PVService.NicoNicoDouga && (
							<>
								{' '}
								<a
									href={`http://nicodata.vocaloid.eu/?NicoUrl=${pv.url}`}
									target="_blank"
									rel="noreferrer"
								>
									<i className="icon-info-sign" />{' '}
									{t('ViewRes.Song:Details.ViewInfo')}
								</a>
							</>
						)}
						<br />
					</React.Fragment>
				))}
			</>
		);
	},
);

const showAlternateVersions = 3;

interface SongBasicInfoProps {
	model: SongDetailsForApi;
	songDetailsStore: SongDetailsStore;
}

const SongBasicInfo = observer(
	({ model, songDetailsStore }: SongBasicInfoProps): React.ReactElement => {
		const loginManager = useLoginManager();
		const { getCodeDescription } = useCultureCodes();

		const { t } = useTranslation(['ViewRes', 'ViewRes.Song']);

		const webLinks = React.useMemo(() => {
			if (model.contract.pvs.every((p) => p.service !== PVService.Youtube)) {
				const nicoPV = VideoServiceHelper.primaryPV(
					model.contract.pvs,
					PVService.NicoNicoDouga,
				);
				const query = encodeURIComponent(
					nicoPV && nicoPV.name
						? nicoPV.name
						: `${model.artistString} ${model.name}`,
				);

				return [
					...model.webLinks,
					{
						id: 0,
						url: `https://www.youtube.com/results?search_query=${query}`,
						description: t('ViewRes.Song:Details.SearchYoutube'),
						category: WebLinkCategory.Other,
						disabled: false,
					},
				];
			}

			return model.webLinks;
		}, [model, t]);

		const cultureCodes =
			model.cultureCodes.length > 0
				? model.cultureCodes
				: model.lyrics.filter((l) => l.translationType === 'Original').first()
						?.cultureCodes;

		return (
			<SongDetailsTabs
				model={model}
				songDetailsStore={songDetailsStore}
				tab="basicInfo"
			>
				<table>
					<tbody>
						<tr>
							<td className="entry-field-label-col">
								{t('ViewRes:Shared.Name')}
							</td>
							<td>
								{model.name}
								<br />
								<span className="extraInfo">{model.additionalNames}</span>
							</td>
						</tr>
						{model.performers.length > 0 && (
							<tr>
								<td>
									{model.songType !== SongType.Illustration
										? t('ViewRes.Song:Details.Vocalists')
										: t('ViewRes:EntryDetails.Subject')}
								</td>
								<td className="artistList">
									<ArtistList
										artists={model.performers}
										showRoles={ShowRolesMode.IfNotVocalist}
										showType={true}
										tooltip={true}
									/>
								</td>
							</tr>
						)}

						{model.subject && model.subject.length > 0 && (
							<tr>
								<td>{t('ViewRes:EntryDetails.Subject')}</td>
								<td className="artistList">
									<ArtistList artists={model.subject} tooltip={true} />
								</td>
							</tr>
						)}

						{model.producers.length > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.Producers')}</td>
								<td className="artistList">
									<ArtistList
										artists={model.producers}
										showRoles={
											model.producers.length > 1
												? ShowRolesMode.IfNotDefault
												: ShowRolesMode.Never
										}
										tooltip={true}
									/>
								</td>
							</tr>
						)}

						{model.illustrators && model.illustrators.length > 0 && (
							<tr>
								<td>Illustrators{/* LOC */}</td>
								<td className="artistList">
									<ArtistList artists={model.illustrators} tooltip={true} />
								</td>
							</tr>
						)}

						{model.bands.length > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.Band')}</td>
								<td className="artistList">
									<ArtistList artists={model.bands} tooltip={true} />
								</td>
							</tr>
						)}

						{model.animators.length > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.Animators')}</td>
								<td className="artistList">
									<ArtistList artists={model.animators} tooltip={true} />
								</td>
							</tr>
						)}

						{model.otherArtists.length > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.OtherArtists')}</td>
								<td className="artistList">
									<ArtistList
										artists={model.otherArtists}
										showRoles={ShowRolesMode.IfNotDefault}
										tooltip={true}
									/>
								</td>
							</tr>
						)}

						<tr>
							<td>{t('ViewRes:Shared.Type')}</td>
							<td>
								<SongTypeLabel songType={model.songType} />{' '}
								{model.songTypeTag ? (
									<TagLink tag={model.songTypeTag} tooltip>
										{t(
											`VocaDb.Model.Resources.Songs:SongTypeNames.${model.songType}`,
										)}
									</TagLink>
								) : (
									<a
										href={`/Tag/DetailsByEntryType?${qs.stringify({
											entryType: EntryType.Song,
											subType: model.songType,
										})}`}
									>
										{t(
											`VocaDb.Model.Resources.Songs:SongTypeNames.${model.songType}`,
										)}
									</a>
								)}
							</td>
						</tr>

						{model.length > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.Duration')}</td>
								<td>{DateTimeHelper.formatFromSeconds(model.length)}</td>
							</tr>
						)}

						{cultureCodes && (
							<tr>
								<td>{t('ViewRes:EntryDetails.Languages')}</td>
								<td>
									{cultureCodes
										.map(
											(c) =>
												getCodeDescription(c)?.englishName ??
												t('ViewRes.Song:Details.LyricsLanguageOther'),
										)
										.join(', ')}
								</td>
							</tr>
						)}

						{model.minMilliBpm && (
							<tr>
								<td>{t('ViewRes.Song:Details.Bpm')}</td>
								<td>
									{BpmHelper.formatFromMilliBpm(
										model.minMilliBpm,
										model.maxMilliBpm,
									)}
								</td>
							</tr>
						)}

						{model.albums.length > 0 && (
							<tr>
								<td>{t('ViewRes:Shared.Albums')}</td>
								<td id="albumList">
									{model.albums.map((album, index) => (
										<React.Fragment key={index}>
											{index > 0 && ', '}
											<AlbumLinkWithReleaseYear album={album} />
										</React.Fragment>
									))}
								</td>
							</tr>
						)}

						<tr>
							<td>{t('ViewRes:Shared.Tags')}</td>
							<td>
								{songDetailsStore.tagUsages.tagUsages.length > 0 && (
									<div className="entry-tag-usages">
										<TagList tagListStore={songDetailsStore.tagUsages} />
									</div>
								)}
								<div>
									<JQueryUIButton
										as={SafeAnchor}
										disabled={
											!loginManager.canEditTagsForEntry({
												...model.contract.song,
												entryType: EntryType.Song,
											})
										}
										icons={{ primary: 'ui-icon-tag' }}
										onClick={songDetailsStore.tagsEditStore.show}
										href="#"
									>
										{t('ViewRes:EntryDetails.EditTags')}
									</JQueryUIButton>
									{model.canRemoveTagUsages /* TODO: Use LoginManager. */ && (
										<>
											{' '}
											<JQueryUIButton
												as={Link}
												to={`/Song/ManageTagUsages/${model.id}`}
												icons={{ primary: 'ui-icon-wrench' }}
											>
												{t('ViewRes:EntryDetails.ManageTags')}
											</JQueryUIButton>
										</>
									)}
								</div>
							</td>
						</tr>

						{model.listCount > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.PoolsAndLists')}</td>
								<td>
									{model.pools.map((pool, index) => (
										<React.Fragment key={pool.id}>
											{index > 0 && ', '}
											<Link
												to={EntryUrlMapper.details(EntryType.SongList, pool.id)}
											>
												{pool.name}
											</Link>
										</React.Fragment>
									))}
									{model.pools.length > 0 && ' - '}
									<SafeAnchor
										id="songInListsLink"
										onClick={songDetailsStore.songInListsDialog.show}
										href="#"
									>
										{t('ViewRes.Song:Details.ViewAllLists', {
											0: model.listCount,
										})}
									</SafeAnchor>
								</td>
							</tr>
						)}

						{model.originalPVs.length > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.OriginalMedia')}</td>
								<td>
									<PVList
										songDetailsStore={songDetailsStore}
										pvs={model.originalPVs}
										showPVType={false}
									/>
								</td>
							</tr>
						)}

						{model.otherPVs.length > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.OtherMedia')}</td>
								<td>
									<PVList
										songDetailsStore={songDetailsStore}
										pvs={model.otherPVs}
										showPVType={true}
									/>
								</td>
							</tr>
						)}

						<ExternalLinksRows webLinks={webLinks} />

						{songDetailsStore.originalVersion.entry && (
							<tr>
								<td>{t('ViewRes.Song:Details.OriginalVersion')}</td>
								<td>
									<SongLinkKnockout
										song={songDetailsStore.originalVersion.entry}
										tooltip={true}
										extUrl={songDetailsStore.originalVersion.url}
										toolTipDomain={songDetailsStore.originalVersion.domain}
									/>{' '}
									<SongTypeLabel
										songType={songDetailsStore.originalVersion.entry.songType}
									/>
									<br />
									<span>
										{songDetailsStore.originalVersion.entry.artistString}
									</span>
								</td>
							</tr>
						)}

						{model.alternateVersions.length > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.AlternateVersions')}</td>
								<td>
									<AlternateVersions
										songs={model.alternateVersions.slice(
											0,
											showAlternateVersions,
										)}
									/>
									{model.alternateVersions.length > showAlternateVersions &&
										(songDetailsStore.allVersionsVisible ? (
											<div>
												<AlternateVersions
													songs={model.alternateVersions.slice(
														showAlternateVersions,
													)}
												/>
											</div>
										) : (
											<SafeAnchor
												href="#"
												onClick={songDetailsStore.showAllVersions}
											>
												{t('ViewRes.Song:Details.ShowAllVersions')} (
												{model.alternateVersions.length})
											</SafeAnchor>
										))}
								</td>
							</tr>
						)}

						{model.notes.original && (
							<tr>
								<td>{t('ViewRes.Song:Details.Notes')}</td>
								<td className="entry-description">
									<EnglishTranslatedString
										englishTranslatedStringStore={songDetailsStore.description}
										string={model.notes}
									/>
								</td>
							</tr>
						)}

						<tr>
							<td>{t('ViewRes:EntryDetails.Stats')}</td>
							<td>
								<SafeAnchor
									href="#"
									id="statsLink"
									onClick={songDetailsStore.getUsers}
								>
									{t('ViewRes.Song:Details.Favorites', {
										0: model.favoritedTimes,
									})}
									, {t('ViewRes.Song:Details.Likes', { 0: model.likedTimes })},
								</SafeAnchor>{' '}
								{t('ViewRes:EntryDetails.Hits', { 0: model.hits })}{' '}
								{t('ViewRes.Song:Details.TotalScore', { 0: model.ratingScore })}
							</td>
						</tr>

						{model.releaseEvents.length > 0 && (
							<tr>
								<td>{t('ViewRes.Song:Details.ReleaseEvent')}</td>
								<td>
									{model.releaseEvents
										.slice()
										.sort(
											(a, b) =>
												(a.date ? new Date(a.date).getTime() : Infinity) -
												(b.date ? new Date(b.date).getTime() : Infinity),
										)
										.map((event, key) => (
											<span key={key}>
												{key !== 0 ? ', ' : ''}
												<EventLink
													entryReleaseDate={model.publishDate}
													bold={key === 0}
													event={event}
													tooltip
												/>
											</span>
										))}
								</td>
							</tr>
						)}

						{model.publishDate && (
							<tr>
								<td>{t('ViewRes:EntryDetails.PublishDate')}</td>
								<td>
									{dayjs.utc(model.publishDate).format('ll') /* REVIEW */}
								</td>
							</tr>
						)}

						<tr>
							<td>{t('ViewRes:EntryDetails.AdditionDate')}</td>
							<td>
								<UniversalTimeLabel dateTime={model.createDate} />
							</td>
						</tr>

						{model.contract.albumSong && (
							<tr>
								<td>{t('ViewRes.Song:Details.Navigation')}</td>
								<td>
									<p>
										{model.contract.albumSong.discNumber > 1 ? (
											<Trans
												i18nKey="ViewRes.Song:Details.TrackOnAlbumDisc"
												components={{
													0: model.contract.albumSong.trackNumber,
													1: model.contract.albumSong.discNumber,
													2: (
														<AlbumLink
															album={model.contract.album!}
															tooltip={false}
														/>
													),
												}}
											/>
										) : (
											<Trans
												i18nKey="ViewRes.Song:Details.TrackOnAlbum"
												components={{
													0: model.contract.albumSong.trackNumber,
													1: (
														<AlbumLink
															album={model.contract.album!}
															tooltip={false}
														/>
													),
												}}
											/>
										)}
									</p>
									{model.contract.previousSong && (
										<>
											{' '}
											<SongAlbumLink
												song={model.contract.previousSong.song}
												icon="icon-fast-backward"
												albumId={model.contract.album?.id}
											/>
										</>
									)}
									{model.contract.nextSong && (
										<>
											{' '}
											<SongAlbumLink
												song={model.contract.nextSong.song}
												icon="icon-fast-forward"
												albumId={model.contract.album?.id}
											/>
										</>
									)}
								</td>
							</tr>
						)}
					</tbody>
				</table>

				{model.suggestions.length > 0 && (
					<>
						<h3 className="withMargin">
							{t('ViewRes.Song:Details.SuggestionsTitle')}
						</h3>
						<SongGrid
							songs={model.suggestions}
							columns={2}
							displayType={true}
							displayPublishDate={true}
						/>
						<p>
							<Link
								to={`${EntryUrlMapper.details_song(
									model.contract.song,
								)}/related`}
							>
								{t('ViewRes.Song:Details.SeeRelatedSongs')}
							</Link>
						</p>
					</>
				)}

				{(model.canEditPersonalDescription /* TODO: Use LoginManager. */ ||
					model.personalDescriptionText) && (
					<>
						<h3
							className="withMargin helpTip"
							title={t('ViewRes:EntryDetails.PersonalDescriptionHelp')}
						>
							{t('ViewRes:EntryDetails.PersonalDescription')}
						</h3>
						<PersonalDescriptionMedia
							personalDescription={songDetailsStore.personalDescription}
							canEditPersonalDescription={model.canEditPersonalDescription}
						/>
					</>
				)}

				<LatestCommentsKnockout
					editableCommentsStore={songDetailsStore.comments}
				/>

				<p>
					<Link
						to={`${EntryUrlMapper.details_song(
							model.contract.song,
						)}/discussion`}
					>
						{t('ViewRes:EntryDetails.ViewAllComments')}
					</Link>
				</p>

				<JQueryUIDialog
					title={t('ViewRes.Song:Details.SongRatings')}
					autoOpen={songDetailsStore.ratingsDialogStore.popupVisible}
					width={400}
					close={(): void =>
						runInAction(() => {
							songDetailsStore.ratingsDialogStore.popupVisible = false;
						})
					}
				>
					<div>
						<UsersWithSongRating
							ratingsStore={songDetailsStore.ratingsDialogStore}
						/>
					</div>
				</JQueryUIDialog>

				<SongInListsDialog
					songInListsStore={songDetailsStore.songInListsDialog}
				/>
			</SongDetailsTabs>
		);
	},
);

export default SongBasicInfo;
