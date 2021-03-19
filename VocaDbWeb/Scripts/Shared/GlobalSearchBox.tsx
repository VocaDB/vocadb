import { Page } from "@inertiajs/inertia";
import { InertiaLink, usePage } from "@inertiajs/inertia-react";
import React, { Fragment, ReactElement } from "react";
import { useTranslation } from "react-i18next";
import Dropdown from "../Bootstrap/Dropdown";
import MainNavigationItems from "./Partials/MainNavigationItems";
import VocaDbPageProps from "./VocaDbPageProps";
import ContentLanguagePreference from '../../wwwroot/Scripts/Models/Globalization/ContentLanguagePreference';

const GlobalSearchBox = (): ReactElement => {
	const { t } = useTranslation(['ViewRes', 'ViewRes.User', 'Resources']);
	const {
		brandableStrings,

		login,
	} = usePage<Page<VocaDbPageProps>>().props;

	return (
		<form action="/Home/GlobalSearch" method="post" className="navbar-form form-inline pull-left navbar-search" id="globalSearchBox">
			{/* TODO */}

			<a className="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">
				<span className="icon-bar"></span>
				<span className="icon-bar"></span>
				<span className="icon-bar"></span>
			</a>

			<div className="brand hidden-phone">
				<a href={'/'}>{brandableStrings.siteName}</a>
			</div>

			<div className="input-prepend input-append navbar-searchBox">
				<div className="btn-group">
					{/* TODO */}
				</div>
				<input type="text" name="globalSearchTerm" id="globalSearchTerm" className="globalSearchBox search-query" size={50} placeholder={t('ViewRes:Shared.Search')} maxLength={255} />
				<button type="submit" className="btn btn-info" title={t('ViewRes:Shared.Search')}>
					<i className="icon-search"></i>
				</button>
			</div>

			<Dropdown className="navbar-languageBar">
				<Dropdown.Toggle variant="info" href="#" className="navbar-languageBtn">
					<i className="icon-user"></i>
					{' '}
					<span className="visible-desktop">
						{t('ViewRes:Layout.Account')}
					</span>
					{' '}
					<span /* TODO */ className="badge badge-small badge-important" style={{display: 'none'}}>!</span>
					{' '}
					<span className="caret"></span>
				</Dropdown.Toggle>

				<Dropdown.Menu>
					{!login.manager.isLoggedIn
						? (
							<Fragment>
								<li><a href={'/User/Login'} /* TODO */>{t('ViewRes:Layout.LogIn')}</a></li>
								<li><a href={'/User/Create'}>{t('ViewRes:Layout.Register')}</a></li>
							</Fragment>
						)
						: (
							<Fragment>
								<li><a href={`/User/Profile/${login.manager.name}`}>{t('ViewRes.User:MySettings.Profile')}</a></li>
								<li>
									<a href={`/User/Profile/${login.manager.name}#Albums`}>{t('ViewRes:TopBar.MyAlbums')}</a>
								</li>
								<li>
									<a href={`/User/Profile/${login.manager.name}#Songs`}>{t('ViewRes:TopBar.MySongs')}</a>
								</li>
								<li><a href={'/User/MySettings'}>{t('ViewRes.User:Details.MySettings')}</a></li>
								{login.canManageEntryReports && (
									<li>
										<a href={'/Admin/ViewEntryReports'}>
											{t('ViewRes:TopBar.EntryReports')}
											<span /* TODO */ className="badge badge-small badge-important" style={{display: 'none'}}></span>
										</a>
									</li>
								)}
								<li><a href={'/User/Logout'}>{t('ViewRes:Layout.LogOut')}</a></li>
							</Fragment>
						)
					}
					<li className="divider"></li>
					<li><h6>{t('ViewRes.User:MySettings.DefaultLanguageSelection')}</h6></li>
					{Object.keys(ContentLanguagePreference).filter(key => !isNaN(Number(ContentLanguagePreference[key]))).map(lp => (
						<li key={lp}>
							<a href="#" /* TODO */>
								{lp === login.manager.languagePreference
									? <i className="menuIcon icon-ok"></i>
									: <i className="menuIcon icon-"></i>}
								{' '}
								{t(`Resources:ContentLanguageSelectionNames.${lp}`)}
							</a>
						</li>
					))}
				</Dropdown.Menu>
			</Dropdown>

			{login.manager.isLoggedIn && (
				<Dropdown className="navbar-languageBar">
					<Dropdown.Toggle variant="info" href="#" className="navbar-languageBtn" /* TODO */>
						<i className="icon-envelope"></i>
						{' '}
						{/* TODO */}
						{' '}
						<span className="caret"></span>
					</Dropdown.Toggle>

					<Dropdown.Menu>
						{/* TODO */}
						<li className="divider"></li>
						<li>
							<a href={'/User/Messages'}>{t('ViewRes:TopBar.ViewAllMessages')}</a>
						</li>
					</Dropdown.Menu>
				</Dropdown>
			)}

			<div className="nav-collapse collapse">
				<MainNavigationItems />
			</div>
		</form>
	);
};

export default GlobalSearchBox;
