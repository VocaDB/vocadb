import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { PVContent } from '@/Components/Shared/PVs/PVContent';
import { ActivityEntryKnockout } from '@/Components/Shared/Partials/Activityfeed/ActivityEntryKnockout';
import { AlbumThumbs } from '@/Components/Shared/Partials/Album/AlbumThumbs';
import { CommentWithEntryVertical } from '@/Components/Shared/Partials/Comment/CommentWithEntryVertical';
import { EventThumbs } from '@/Components/Shared/Partials/Shared/EventThumbs';
import { FrontPageContract } from '@/DataContracts/FrontPageContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { userRepo } from '@/Repositories/UserRepository';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import {
	FrontPagePVPlayerStore,
	FrontPageStore,
} from '@/Stores/FrontPageStore';
import { AlbumSortRule } from '@/Stores/Search/AlbumSearchStore';
import { SearchType } from '@/Stores/Search/SearchStore';
import { SongSortRule } from '@/Stores/Search/SongSearchStore';
import { useVdb } from '@/VdbContext';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface PVPlayerProps {
	model: FrontPageContract;
	pvPlayerStore: FrontPagePVPlayerStore;
}

const PVPlayer = observer(
	({ model, pvPlayerStore }: PVPlayerProps): React.ReactElement => {
		return (
			<div className="pvPlayer">
				<div className="scrollable-navi">
					<SafeAnchor
						href="#"
						className={classNames(
							'prev',
							'scrollable-browse-up',
							pvPlayerStore.paging.isFirstPage && 'disabled',
						)}
						onClick={pvPlayerStore.paging.previousPage}
					/>
					<div className="scrollable scrollable-vertical" id="songs-navi">
						<div
							className="scrollable-items"
							style={{ top: -384 * (pvPlayerStore.paging.page - 1) }}
						>
							{model.newSongs.chunk(4).map((chunk, index) => (
								<div key={index}>
									{chunk.map((song) => (
										<div
											className={classNames(
												'scrollable-item',
												'alignTop',
												song.id === pvPlayerStore.selectedSong?.id && 'active',
											)}
											key={song.id}
											onClick={(): void =>
												runInAction(() => {
													pvPlayerStore.selectedSong = song;
												})
											}
										>
											<input
												type="hidden"
												className="js-songId"
												value={song.id}
											/>
											{song.mainPicture && song.mainPicture.urlThumb && (
												<img
													src={UrlHelper.upgradeToHttps(
														song.mainPicture.urlThumb,
													)}
													alt="Cover" /* LOC */
													className="coverPicThumb"
													referrerPolicy="same-origin"
												/>
											)}
											<strong className="songName">{song.name}</strong>
											<span className="songArtists">{song.artistString}</span>
										</div>
									))}
								</div>
							))}
						</div>
					</div>
					<SafeAnchor
						href="#"
						className={classNames(
							'next',
							'scrollable-browse-down',
							pvPlayerStore.paging.isLastPage && 'disabled',
						)}
						onClick={pvPlayerStore.paging.nextPage}
					/>
				</div>

				<div id="songPreview" className="pvViewerContent">
					{pvPlayerStore.selectedSong && (
						<PVContent
							pvPlayerStore={pvPlayerStore}
							selectedSong={pvPlayerStore.selectedSong}
						/>
					)}
				</div>
			</div>
		);
	},
);

interface HomeIndexLayoutProps {
	model: FrontPageContract;
	frontPageStore: FrontPageStore;
}

const HomeIndexLayout = ({
	model,
	frontPageStore,
}: HomeIndexLayoutProps): React.ReactElement => {
	const vdb = useVdb();

	const { t } = useTranslation(['ViewRes.Comment', 'ViewRes.Home']);

	return (
		<Layout pageTitle={undefined} ready={true}>
			{/* TODO: <link rel="alternate" type="application/rss+xml" title="RSS" href='@Url.Action("LatestVideos", "Song")'> */}
			<h1 className="page-title home-title">
				{vdb.resources.home.welcome}
				<small>{vdb.resources.home.welcomeSubtitle}</small>
			</h1>

			{model.newSongs && (
				<div id="recentSongs">
					<h3 className="withMargin">
						{t('ViewRes.Home:Index.RecentSongs')} (
						<Link
							to={`/Search?${qs.stringify({
								searchType: SearchType.Song,
								sort: SongSortRule.AdditionDate,
								onlyWithPVs: true,
							})}`}
						>
							{t('ViewRes.Home:Index.ViewMore')}
						</Link>
						)
					</h3>

					<PVPlayer
						model={model}
						pvPlayerStore={frontPageStore.pvPlayerStore}
					/>
				</div>
			)}

			<br />

			{model.newAlbums.length > 0 && (
				<div id="newAlbums">
					<h3 className="withMargin">
						{t('ViewRes.Home:Index.NewAlbums')} (
						<Link
							to={`/Search?${qs.stringify({
								searchType: SearchType.Album,
								sort: AlbumSortRule.ReleaseDate,
							})}`}
						>
							{t('ViewRes.Home:Index.ViewMore')}
						</Link>
						)
					</h3>
					<div id="newAlbums">
						<AlbumThumbs albums={model.newAlbums} tooltip={true} />
					</div>
				</div>
			)}

			{model.topAlbums.length > 0 && (
				<div id="topAlbums">
					<h3 className="withMargin">
						{t('ViewRes.Home:Index.TopAlbums')} (
						<Link
							to={`/Search?${qs.stringify({
								searchType: SearchType.Album,
								sort: AlbumSortRule.RatingTotal,
							})}`}
						>
							{t('ViewRes.Home:Index.ViewMore')}
						</Link>
						)
					</h3>
					<div id="topAlbums">
						<AlbumThumbs albums={model.topAlbums} tooltip={true} />
					</div>
				</div>
			)}

			{model.newEvents.length > 0 && (
				<div id="recentEvents">
					<h3 className="withMargin">{t('ViewRes.Home:Index.RecentEvents')}</h3>
					<EventThumbs events={model.newEvents} />
				</div>
			)}

			<div className="row-fluid">
				<div id="recentActivity" className="span7">
					<h3 className="withMargin">
						{t('ViewRes.Home:Index.RecentActivity')} (
						<Link to="/ActivityEntry">{t('ViewRes.Home:Index.ViewMore')}</Link>)
					</h3>

					{model.activityEntries.map((entry, index) => (
						<ActivityEntryKnockout entry={entry} key={index} />
					))}

					<Link to="/ActivityEntry">{t('ViewRes.Home:Index.ViewMore')}</Link>
				</div>
				<div id="recentComments" className="span5">
					<h3 className="withMargin">
						{t('ViewRes.Comment:Index.RecentComments')} (
						<Link to="/Comment">{t('ViewRes.Home:Index.ViewMore')}</Link>)
					</h3>

					{model.recentComments.map((comment, index) => (
						<CommentWithEntryVertical
							entry={comment}
							maxLength={400}
							key={index}
						/>
					))}

					<Link to="/Comment">{t('ViewRes.Home:Index.ViewMore')}</Link>
				</div>
			</div>
		</Layout>
	);
};

const HomeIndex = (): React.ReactElement => {
	const vdb = useVdb();

	const [model, setModel] = React.useState<
		{ contract: FrontPageContract; frontPageStore: FrontPageStore } | undefined
	>();

	React.useEffect(() => {
		httpClient
			.get<FrontPageContract>(urlMapper.mapRelative('/api/frontpage'))
			.then((contract) =>
				setModel({
					contract: contract,
					frontPageStore: new FrontPageStore(vdb.values, userRepo, contract),
				}),
			);
	}, [vdb]);

	return model ? (
		<HomeIndexLayout
			model={model.contract}
			frontPageStore={model.frontPageStore}
		/>
	) : (
		<></>
	);
};

export default HomeIndex;
