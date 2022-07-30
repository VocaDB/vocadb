import Nav from '@Bootstrap/Nav';
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
			<Nav className="nav-list">
				<Nav.Item>
					<Nav.Link as={Link} to="/">
						{t('ViewRes:Layout.Home')}
					</Nav.Link>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link as={Link} to="/Search?searchType=Artist">
						{t('ViewRes:Shared.Artists')}
					</Nav.Link>
					<ul>
						{loginManager.canManageDatabase && (
							<li>
								<Nav.Link as={Link} to={'/Artist/Create'}>
									{t('ViewRes:Layout.AddArtist')}
								</Nav.Link>
							</li>
						)}
					</ul>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link as={Link} to="/Search?searchType=Album">
						{t('ViewRes:Shared.Albums')}
					</Nav.Link>
					<ul>
						{loginManager.canManageDatabase && (
							<li>
								<Nav.Link href={'/Album/Create'}>
									{t('ViewRes:Layout.AddAlbum')}
								</Nav.Link>
							</li>
						)}
						<li>
							<Nav.Link
								as={Link}
								to="/Search?searchType=Album&sort=RatingAverage"
							>
								{t('ViewRes:Shared.TopRatedAlbums')}
							</Nav.Link>
						</li>
						<li>
							<Nav.Link
								as={Link}
								to="/Search?searchType=Album&sort=ReleaseDate"
							>
								{t('ViewRes:Layout.NewAlbums')}
							</Nav.Link>
						</li>
						<li>
							<Nav.Link
								as={Link}
								to={`/Search?${qs.stringify({
									searchType: SearchType.Album,
									tagId: vdb.values.freeTagId,
									childTags: true,
								})}`}
							>
								{t('ViewRes:Layout.FreeAlbums')}
							</Nav.Link>
						</li>
					</ul>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link as={Link} to="/Search?searchType=Song">
						{t('ViewRes:Shared.Songs')}
					</Nav.Link>
					<ul>
						{loginManager.canManageDatabase && (
							<li>
								<Nav.Link href={'/Song/Create'}>
									{t('ViewRes:Layout.AddSong')}
								</Nav.Link>
							</li>
						)}
						<li>
							<Nav.Link
								as={Link}
								to={`/Song/Rankings?${qs.stringify({ durationHours: 168 })}`}
							>
								{t('ViewRes:Shared.TopFavoritedSongs')}
							</Nav.Link>
						</li>
						<li>
							<Nav.Link
								as={Link}
								to="/Search?searchType=Song&sort=AdditionDate&onlyWithPVs=true"
							>
								{t('ViewRes:Layout.NewPVs')}
							</Nav.Link>
						</li>
					</ul>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link as={Link} to="/Search?searchType=ReleaseEvent">
						{t('ViewRes:Shared.ReleaseEvents')}
					</Nav.Link>
					<ul>
						<li>
							<Nav.Link as={Link} to="/Event">
								{t('ViewRes:Layout.UpcomingEvents')}
							</Nav.Link>
						</li>
					</ul>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link as={Link} to="/SongList/Featured">
						{t('ViewRes:Shared.FeaturedSongLists')}
					</Nav.Link>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link as={Link} to="/Tag">
						{t('ViewRes:Layout.Tags')}
					</Nav.Link>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link as={Link} to="/User">
						{t('ViewRes:Shared.Users')}
					</Nav.Link>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link as={Link} to="/Help">
						{t('ViewRes:Layout.Help')}
					</Nav.Link>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link as={Link} to="/discussion">
						{t('ViewRes:Layout.Discussions')}
					</Nav.Link>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link href="https://wiki.vocadb.net/">
						{t('ViewRes:Layout.Wiki')}
					</Nav.Link>
				</Nav.Item>
				<Nav.Item>
					<Nav.Link href={vdb.values.blogUrl}>
						{t('ViewRes:Layout.Blog')}
					</Nav.Link>
				</Nav.Item>
				{loginManager.canMikuDbImport && (
					<Nav.Item>
						<Nav.Link href={'/MikuDbAlbum'}>
							{t('ViewRes:Layout.MikuDbAlbum')}
						</Nav.Link>
					</Nav.Item>
				)}
				{loginManager.canAccessManageMenu && (
					<Nav.Item>
						<Nav.Link as={Link} to="/Admin">
							{t('ViewRes:Layout.Manage')}
						</Nav.Link>
					</Nav.Item>
				)}
				{!vdb.values.isLoggedIn && (
					<>
						<Nav.Item>
							<Nav.Link
								href={'/User/Login'}
								onClick={(): void => {} /* TODO: showLoginPopup */}
							>
								{t('ViewRes:Layout.LogIn')}
							</Nav.Link>
						</Nav.Item>
						<Nav.Item>
							<Nav.Link href={'/User/Create'}>
								{t('ViewRes:Layout.Register')}
							</Nav.Link>
						</Nav.Item>
					</>
				)}
			</Nav>
		);
	},
);

export default MainNavigationItems;
