import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { JQueryUINavItemComponent } from '@/JQueryUI/JQueryUITabs';
import { EntryType } from '@/Models/EntryType';
import ArtistBasicInfo from '@/Pages/Artist/ArtistBasicInfo';
import ArtistCollaborationAlbums from '@/Pages/Artist/ArtistCollaborationAlbums';
import ArtistDiscussion from '@/Pages/Artist/ArtistDiscussion';
import ArtistMainAlbums from '@/Pages/Artist/ArtistMainAlbums';
import ArtistPictures from '@/Pages/Artist/ArtistPictures';
import ArtistShare from '@/Pages/Artist/ArtistShare';
import ArtistSongs from '@/Pages/Artist/ArtistSongs';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, Route, Routes } from 'react-router-dom';

interface ArtistDetailsTabsProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
	tab:
		| 'basicInfo'
		| 'discussion'
		| 'pictures'
		| 'albums'
		| 'collaborations'
		| 'songs'
		| 'share';
	children?: React.ReactNode;
}

export const ArtistDetailsTabs = React.memo(
	({
		artist,
		artistDetailsStore,
		tab,
		children,
	}: ArtistDetailsTabsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Artist']);

		return (
			<div className="artist-details-page js-cloak ui-tabs ui-widget ui-widget-content ui-corner-all">
				<ul
					className="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all"
					role="tablist"
				>
					<JQueryUINavItemComponent active={tab === 'basicInfo'}>
						<Link to={`${EntryUrlMapper.details(EntryType.Artist, artist.id)}`}>
							{t('ViewRes:EntryDetails.BasicInfoTab')}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'discussion'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Artist,
								artist.id,
							)}/discussion`}
						>
							{t('ViewRes:EntryDetails.DiscussionTab')} ({artist.commentCount})
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'pictures'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Artist,
								artist.id,
							)}/pictures`}
						>
							{t('ViewRes:EntryDetails.PicturesTab')} (
							{artist.pictures.length + 1})
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'albums'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Artist,
								artist.id,
							)}/albums`}
						>
							{t('ViewRes.Artist:Details.MainAlbums')}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'collaborations'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Artist,
								artist.id,
							)}/collaborations`}
						>
							{t('ViewRes.Artist:Details.CollaborationAlbums')}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'songs'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Artist,
								artist.id,
							)}/songs`}
						>
							{t('ViewRes.Artist:Details.AllSongs')}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'share'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Artist,
								artist.id,
							)}/share`}
						>
							{t('ViewRes.Artist:Details.Share')}
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

interface ArtistDetailsRoutesProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistDetailsRoutes = ({
	artist,
	artistDetailsStore,
}: ArtistDetailsRoutesProps): React.ReactElement => {
	return (
		<Routes>
			<Route
				path="discussion"
				element={
					<ArtistDiscussion
						artist={artist}
						artistDetailsStore={artistDetailsStore}
					/>
				}
			/>
			<Route
				path="pictures"
				element={
					<ArtistPictures
						artist={artist}
						artistDetailsStore={artistDetailsStore}
					/>
				}
			/>
			<Route
				path="albums"
				element={
					<ArtistMainAlbums
						artist={artist}
						artistDetailsStore={artistDetailsStore}
					/>
				}
			/>
			<Route
				path="collaborations"
				element={
					<ArtistCollaborationAlbums
						artist={artist}
						artistDetailsStore={artistDetailsStore}
					/>
				}
			/>
			<Route
				path="songs"
				element={
					<ArtistSongs
						artist={artist}
						artistDetailsStore={artistDetailsStore}
					/>
				}
			/>
			<Route
				path="share"
				element={
					<ArtistShare
						artist={artist}
						artistDetailsStore={artistDetailsStore}
					/>
				}
			/>
			<Route
				path="*"
				element={
					<ArtistBasicInfo
						artist={artist}
						artistDetailsStore={artistDetailsStore}
					/>
				}
			/>
		</Routes>
	);
};

export default ArtistDetailsRoutes;
