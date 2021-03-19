import { Page } from "@inertiajs/inertia";
import { InertiaLink, usePage } from "@inertiajs/inertia-react";
import React, { Fragment, ReactElement } from "react";
import { useTranslation } from "react-i18next";
import VocaDbPageProps from "../VocaDbPageProps";

const MainNavigationItems = (): ReactElement => {
	const { t } = useTranslation(['ViewRes']);
	const {
		blogUrl,

		login,
	} = usePage<Page<VocaDbPageProps>>().props;

	return (
		<ul className="nav nav-list">
			<li><a href={'/'}>{t('ViewRes:Layout.Home')}</a></li>
			<li>
				<InertiaLink href={'/Search?searchType=Artist'}>{t('ViewRes:Shared.Artists')}</InertiaLink>
				<ul>
					{login.canManageDb && (
						<li><a href={'/Artist/Create'}>{t('ViewRes:Layout.AddArtist')}</a></li>
					)}
				</ul>
			</li>
			<li>
				<InertiaLink href={'/Search?searchType=Album'}>{t('ViewRes:Shared.Albums')}</InertiaLink>
				<ul>
					{login.canManageDb && (
						<li><a href={'/Album/Create'}>{t('ViewRes:Layout.AddAlbum')}</a></li>
					)}
					<li><InertiaLink href={'/Search?searchType=Album&sort=RatingAverage'}>{t('ViewRes:Shared.TopRatedAlbums')}</InertiaLink></li>
					<li><InertiaLink href={'/Search?searchType=Album&sort=ReleaseDate'}>{t('ViewRes:Layout.NewAlbums')}</InertiaLink></li>
					<li><a href={'#'/* TODO */}>{t('ViewRes:Layout.FreeAlbums')}</a></li>
				</ul>
			</li>
			<li>
				<InertiaLink href={'/Search?searchType=Song'}>{t('ViewRes:Shared.Songs')}</InertiaLink>
				<ul>
					{login.canManageDb && (
						<li><a href={'/Song/Create'}>{t('ViewRes:Layout.AddSong')}</a></li>
					)}
					<li><a href={'/Song/Rankings'}>{t('ViewRes:Shared.TopFavoritedSongs')}</a></li>
					<li><InertiaLink href={'/Search?searchType=Song&sort=AdditionDate&onlyWithPVs=true'}>{t('ViewRes:Layout.NewPVs')}</InertiaLink></li>
				</ul>
			</li>
			<li>
				<InertiaLink href={'/Search?searchType=ReleaseEvent'}>{t('ViewRes:Shared.ReleaseEvents')}</InertiaLink>
				<ul>
					<li><a href={'/Event'}>{t('ViewRes:Layout.UpcomingEvents')}</a></li>
				</ul>
			</li>
			<li><a href={'/SongList/Featured'}>{t('ViewRes:Shared.FeaturedSongLists')}</a></li>
			<li><a href={'/Tag'}>{t('ViewRes:Layout.Tags')}</a></li>
			<li><a href={'/User'}>{t('ViewRes:Shared.Users')}</a></li>
			<li><a href={'/Help'}>{t('ViewRes:Layout.Help')}</a></li>
			<li><a href={'/Discussion'}>{t('ViewRes:Layout.Discussions')}</a></li>
			<li><a href="https://wiki.vocadb.net/">{t('ViewRes:Layout.Wiki')}</a></li>
			<li><a href={blogUrl}>{t('ViewRes:Layout.Blog')}</a></li>
			{/* TODO */}
			{login.canAccessManageMenu && (
				<li><a href={'/Admin'}>{t('ViewRes:Layout.Manage')}</a></li>
			)}
			{!login.manager.isLoggedIn && (
				<Fragment>
					<li><a href={'/User/Login'} onClick={null/* TODO */}>{t('ViewRes:Layout.LogIn')}</a></li>
					<li><a href={'/User/Create'}>{t('ViewRes:Layout.Register')}</a></li>
				</Fragment>
			)}
		</ul>
	);
};

export default MainNavigationItems;
