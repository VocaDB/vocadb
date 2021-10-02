import LoginManager from '@Models/LoginManager';
import { SearchType } from '@Stores/Search/SearchStore';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const MainNavigationItems = React.memo(
	(): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<ul className="nav nav-list">
				<li>
					<a href={'/'}>{t('ViewRes:Layout.Home')}</a>
				</li>
				<li>
					<Link to="/Search?searchType=Artist">
						{t('ViewRes:Shared.Artists')}
					</Link>
					<ul>
						{loginManager.canManageDatabase && (
							<li>
								<a href={'/Artist/Create'}>{t('ViewRes:Layout.AddArtist')}</a>
							</li>
						)}
					</ul>
				</li>
				<li>
					<Link to="/Search?searchType=Album">
						{t('ViewRes:Shared.Albums')}
					</Link>
					<ul>
						{loginManager.canManageDatabase && (
							<li>
								<a href={'/Album/Create'}>{t('ViewRes:Layout.AddAlbum')}</a>
							</li>
						)}
						<li>
							<Link to="/Search?searchType=Album&sort=RatingAverage">
								{t('ViewRes:Shared.TopRatedAlbums')}
							</Link>
						</li>
						<li>
							<Link to="/Search?searchType=Album&sort=ReleaseDate">
								{t('ViewRes:Layout.NewAlbums')}
							</Link>
						</li>
						<li>
							<Link
								to={`/Search?${qs.stringify({
									searchType: SearchType.Album,
									tagId: vdb.values.freeTagId,
									childTags: true,
								})}`}
							>
								{t('ViewRes:Layout.FreeAlbums')}
							</Link>
						</li>
					</ul>
				</li>
				<li>
					<Link to="/Search?searchType=Song">{t('ViewRes:Shared.Songs')}</Link>
					<ul>
						{loginManager.canManageDatabase && (
							<li>
								<a href={'/Song/Create'}>{t('ViewRes:Layout.AddSong')}</a>
							</li>
						)}
						<li>
							<Link
								to={`/Song/Rankings?${qs.stringify({ durationHours: 168 })}`}
							>
								{t('ViewRes:Shared.TopFavoritedSongs')}
							</Link>
						</li>
						<li>
							<Link to="/Search?searchType=Song&sort=AdditionDate&onlyWithPVs=true">
								{t('ViewRes:Layout.NewPVs')}
							</Link>
						</li>
					</ul>
				</li>
				<li>
					<Link to="/Search?searchType=ReleaseEvent">
						{t('ViewRes:Shared.ReleaseEvents')}
					</Link>
					<ul>
						<li>
							<a href={'/Event'}>{t('ViewRes:Layout.UpcomingEvents')}</a>
						</li>
					</ul>
				</li>
				<li>
					<Link to="/SongList/Featured">
						{t('ViewRes:Shared.FeaturedSongLists')}
					</Link>
				</li>
				<li>
					<a href={'/Tag'}>{t('ViewRes:Layout.Tags')}</a>
				</li>
				<li>
					<Link to="/User">{t('ViewRes:Shared.Users')}</Link>
				</li>
				<li>
					<a href={'/Help'}>{t('ViewRes:Layout.Help')}</a>
				</li>
				<li>
					<Link to="/discussion">{t('ViewRes:Layout.Discussions')}</Link>
				</li>
				<li>
					<a href="https://wiki.vocadb.net/">{t('ViewRes:Layout.Wiki')}</a>
				</li>
				<li>
					<a href={vdb.values.blogUrl}>{t('ViewRes:Layout.Blog')}</a>
				</li>
				{loginManager.canMikuDbImport && (
					<li>
						<a href={'/MikuDbAlbum'}>{t('ViewRes:Layout.MikuDbAlbum')}</a>
					</li>
				)}
				{loginManager.canAccessManageMenu && (
					<li>
						<Link to="/Admin">{t('ViewRes:Layout.Manage')}</Link>
					</li>
				)}
				{!vdb.values.isLoggedIn && (
					<>
						<li>
							<a
								href={'/User/Login'}
								onClick={(): void => {} /* TODO: showLoginPopup */}
							>
								{t('ViewRes:Layout.LogIn')}
							</a>
						</li>
						<li>
							<a href={'/User/Create'}>{t('ViewRes:Layout.Register')}</a>
						</li>
					</>
				)}
			</ul>
		);
	},
);

export default MainNavigationItems;
