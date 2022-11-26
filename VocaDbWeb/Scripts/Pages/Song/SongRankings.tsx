import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import { Layout } from '@/Components/Shared/Layout';
import { PVPreviewKnockout } from '@/Components/Shared/Partials/Song/PVPreviewKnockout';
import { SongTypeLabel } from '@/Components/Shared/Partials/Song/SongTypeLabel';
import { SongVoteRating } from '@/Models/SongVoteRating';
import { songRepo } from '@/Repositories/SongRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { SearchType } from '@/Stores/Search/SearchStore';
import { ISongSearchItem } from '@/Stores/Search/SongSearchStore';
import { RankingsStore } from '@/Stores/Song/RankingsStore';
import { useVdb } from '@/VdbContext';
import { useLocationStateStore } from '@vocadb/route-sphere';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const SongRankingsTableHeader = React.memo(
	(): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Song']);

		return (
			<thead>
				<tr>
					<th></th>
					<th colSpan={2}>{t('ViewRes.Song:Rankings.ColName')}</th>
					<th>{t('ViewRes.Song:Rankings.ColPublished')}</th>
					<th>{t('ViewRes.Song:Rankings.ColTags')}</th>
					<th>{t('ViewRes.Song:Rankings.ColRating')}</th>
				</tr>
			</thead>
		);
	},
);

interface SongRankingsTableRowProps {
	rankingsStore: RankingsStore;
	song: ISongSearchItem;
	index: number;
}

const SongRankingsTableRow = observer(
	({
		rankingsStore,
		song,
		index,
	}: SongRankingsTableRowProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes.Song']);

		return (
			<tr>
				<td style={{ width: '30px' }}>
					<h1>{index + 1}</h1>
				</td>
				<td style={{ width: '80px' }}>
					{song.mainPicture && song.mainPicture.urlThumb && (
						<Link
							to={EntryUrlMapper.details_song(song)}
							title={song.additionalNames}
						>
							{/* eslint-disable-next-line jsx-a11y/alt-text */}
							<img
								src={song.mainPicture.urlThumb}
								title="Cover picture" /* LOC */
								className="coverPicThumb img-rounded"
								referrerPolicy="same-origin"
							/>
						</Link>
					)}
				</td>
				<td>
					{song.previewStore && song.previewStore.pvServices && (
						<div className="pull-right">
							<Button
								onClick={(): void => song.previewStore?.togglePreview()}
								className={classNames(
									'previewSong',
									song.previewStore.preview && 'active',
								)}
								href="#"
							>
								<i className="icon-film" /> {t('ViewRes.Song:Rankings.Preview')}
							</Button>
						</div>
					)}
					<Link
						to={EntryUrlMapper.details_song(song)}
						title={song.additionalNames}
					>
						{song.name}
					</Link>{' '}
					<SongTypeLabel songType={song.songType} />{' '}
					{rankingsStore
						.getPVServiceIcons(song.pvServices)
						.map((icon, index) => (
							<React.Fragment key={icon.service}>
								{index > 0 && ' '}
								{/* eslint-disable-next-line jsx-a11y/alt-text */}
								<img src={icon.url} title={icon.service} />
							</React.Fragment>
						))}
					{false /* TODO */ && (
						<>
							{' '}
							<span
								className="icon heartIcon"
								title={t(
									`Resources:SongVoteRatingNames.${SongVoteRating.Favorite}`,
								)}
							/>
						</>
					)}
					{false /* TODO */ && (
						<>
							{' '}
							<span
								className="icon starIcon"
								title={t(
									`Resources:SongVoteRatingNames.${SongVoteRating.Like}`,
								)}
							/>
						</>
					)}
					<br />
					<small className="extraInfo">{song.artistString}</small>
					{song.previewStore && song.previewStore.pvServices && (
						<PVPreviewKnockout
							previewStore={song.previewStore}
							getPvServiceIcons={rankingsStore.getPVServiceIcons}
						/>
					)}
				</td>
				<td>{moment(song.publishDate).format('l')}</td>
				<td className="search-tags-column">
					{song.tags && song.tags.length > 0 && (
						<>
							<i className="icon icon-tags fix-icon-margin" />{' '}
							{song.tags.map((tag, index) => (
								<React.Fragment key={tag.tag.id}>
									{index > 0 && <span>, </span>}
									<Link
										to={rankingsStore.getTagUrl(tag)}
										title={tag.tag.additionalNames}
									>
										{tag.tag.name}
									</Link>
								</React.Fragment>
							))}
						</>
					)}
				</td>
				<td>
					<span>{song.ratingScore}</span>{' '}
					{t('ViewRes.Song:Rankings.TotalScore')}
				</td>
			</tr>
		);
	},
);

interface SongRankingsTableBodyProps {
	rankingsStore: RankingsStore;
}

const SongRankingsTableBody = observer(
	({ rankingsStore }: SongRankingsTableBodyProps): React.ReactElement => {
		return (
			<tbody>
				{rankingsStore.songs.map((song, index) => (
					<SongRankingsTableRow
						rankingsStore={rankingsStore}
						song={song}
						index={index}
						key={song.id}
					/>
				))}
			</tbody>
		);
	},
);

interface SongRankingsTableProps {
	rankingsStore: RankingsStore;
}

const SongRankingsTable = observer(
	({ rankingsStore }: SongRankingsTableProps): React.ReactElement => {
		return (
			<table className="table table-striped">
				<SongRankingsTableHeader />
				<SongRankingsTableBody rankingsStore={rankingsStore} />
			</table>
		);
	},
);

const SongRankings = observer(
	(): React.ReactElement => {
		const vdb = useVdb();

		const [rankingsStore] = React.useState(
			() =>
				new RankingsStore(
					httpClient,
					urlMapper,
					songRepo,
					userRepo,
					vdb.values.languagePreference,
				),
		);

		const { t } = useTranslation(['ViewRes', 'ViewRes.Song']);

		useLocationStateStore(rankingsStore);

		return (
			<Layout
				pageTitle={vdb.resources.song.rankingsTitle}
				ready={true}
				title={vdb.resources.song.rankingsTitle}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Search?${qs.stringify({ searchType: SearchType.Song })}`,
							}}
						>
							{t('ViewRes:Shared.Songs')}
						</Breadcrumb.Item>
					</>
				}
			>
				<ButtonGroup>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.durationHours = 24;
							})
						}
						className={classNames(
							rankingsStore.durationHours === 24 && 'active',
						)}
					>
						{t('ViewRes.Song:Rankings.DurationDaily')}
					</Button>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.durationHours = 168;
							})
						}
						className={classNames(
							rankingsStore.durationHours === 168 && 'active',
						)}
					>
						{t('ViewRes.Song:Rankings.DurationWeekly')}
					</Button>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.durationHours = 720;
							})
						}
						className={classNames(
							rankingsStore.durationHours === 720 && 'active',
						)}
					>
						{t('ViewRes.Song:Rankings.DurationMonthly')}
					</Button>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.durationHours = undefined;
							})
						}
						className={classNames(
							rankingsStore.durationHours === undefined && 'active',
						)}
					>
						{t('ViewRes.Song:Rankings.DurationOverall')}
					</Button>
				</ButtonGroup>{' '}
				<ButtonGroup>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.dateFilterType = 'CreateDate';
							})
						}
						className={classNames(
							rankingsStore.dateFilterType === 'CreateDate' && 'active',
							!rankingsStore.durationHours && 'disabled',
						)}
						title={t('ViewRes.Song:Rankings.FilterCreateDateDescription')}
					>
						{t('ViewRes.Song:Rankings.FilterCreateDate')}
					</Button>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.dateFilterType = 'PublishDate';
							})
						}
						className={classNames(
							rankingsStore.dateFilterType === 'PublishDate' && 'active',
							!rankingsStore.durationHours && 'disabled',
						)}
						title={t('ViewRes.Song:Rankings.FilterPublishDateDescription')}
					>
						{t('ViewRes.Song:Rankings.FilterPublishDate')}
					</Button>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.dateFilterType = 'Popularity';
							})
						}
						className={classNames(
							rankingsStore.dateFilterType === 'Popularity' && 'active',
							!rankingsStore.durationHours && 'disabled',
						)}
						title={t('ViewRes.Song:Rankings.FilterPopularityDescription')}
					>
						{t('ViewRes.Song:Rankings.FilterPopularity')}
					</Button>
				</ButtonGroup>{' '}
				<ButtonGroup>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.vocalistSelection = undefined;
							})
						}
						className={classNames(
							rankingsStore.vocalistSelection === undefined && 'active',
						)}
					>
						{t('ViewRes.Song:Rankings.VocalistAll')}
					</Button>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.vocalistSelection = 'Vocaloid';
							})
						}
						className={classNames(
							rankingsStore.vocalistSelection === 'Vocaloid' && 'active',
						)}
					>
						{t('ViewRes.Song:Rankings.VocalistVocaloid')}
					</Button>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.vocalistSelection = 'UTAU';
							})
						}
						className={classNames(
							rankingsStore.vocalistSelection === 'UTAU' && 'active',
						)}
					>
						{t('ViewRes.Song:Rankings.VocalistUTAU')}
					</Button>
					<Button
						href="#"
						onClick={(): void =>
							runInAction(() => {
								rankingsStore.vocalistSelection = 'Other';
							})
						}
						className={classNames(
							rankingsStore.vocalistSelection === 'Other' && 'active',
						)}
					>
						{t('ViewRes.Song:Rankings.VocalistOther')}
					</Button>
				</ButtonGroup>
				{rankingsStore.songs.length === 0 ? (
					<Alert variant="alert" className="withMargin">
						{t('ViewRes.Song:Rankings.NoSongs')}
					</Alert>
				) : (
					<SongRankingsTable rankingsStore={rankingsStore} />
				)}
			</Layout>
		);
	},
);

export default SongRankings;
