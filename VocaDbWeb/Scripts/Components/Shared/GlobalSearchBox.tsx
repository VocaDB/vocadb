import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Dropdown from '@/Bootstrap/Dropdown';
import Navbar from '@/Bootstrap/Navbar';
import { MainNavigationItems } from '@/Components/Shared/Partials/MainNavigationItems';
import { ProfileIconKnockout_ImageSize } from '@/Components/Shared/Partials/User/ProfileIconKnockout_ImageSize';
import { ShowRandomPageButton } from '@/Components/Shared/ShowRandomPageButton';
import JQueryUIAutocomplete from '@/JQueryUI/JQueryUIAutocomplete';
import { EntryType } from '@/Models/EntryType';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { ImageSize } from '@/Models/Images/ImageSize';
import { loginManager } from '@/Models/LoginManager';
import { userRepo } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { functions } from '@/Shared/GlobalFunctions';
import { httpClient } from '@/Shared/HttpClient';
import { TopBarStore } from '@/Stores/TopBarStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const allObjectTypes = [
	EntryType.Undefined,
	EntryType.Artist,
	EntryType.Album,
	EntryType.Song,
	EntryType.Tag,
	EntryType.User,
	EntryType.ReleaseEvent,
	EntryType.SongList,
] as const; /* TODO */

export const apiEndpointsForEntryType = {
	[EntryType.Undefined]: '/api/entries',
	[EntryType.Album]: '/api/albums',
	[EntryType.Artist]: '/api/artists',
	[EntryType.ReleaseEvent]: '/api/releaseEvents',
	[EntryType.Song]: '/api/songs',
	[EntryType.SongList]: '/api/songLists/featured',
	[EntryType.Tag]: '/api/tags',
	[EntryType.User]: '/api/users',
};

const globalSearchBoxSource = (
	entryType: typeof TopBarStore.entryTypes[number],
	query: string,
): Promise<string[]> => {
	const apiEndpoint = apiEndpointsForEntryType[entryType];

	if (!apiEndpoint) return Promise.reject();

	const endpoint = functions.mergeUrls(apiEndpoint, '/names');

	return httpClient.get<string[]>(endpoint, { query: query });
};

const setLanguagePreferenceCookie = (
	languagePreference: ContentLanguagePreference,
): boolean => {
	userRepo
		.updateUserSetting({
			userId: vdb.values.loggedUserId,
			settingName: 'languagePreference',
			value: languagePreference,
		})
		.then(() => {
			window.location.reload();
		});

	return false;
};

interface GlobalSearchBoxProps {
	topBarStore: TopBarStore;
}

export const GlobalSearchBox = observer(
	({ topBarStore }: GlobalSearchBoxProps): React.ReactElement => {
		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.User',
			'VocaDb.Web.Resources.Domain',
		]);

		const entryTypeName = t(
			`VocaDb.Web.Resources.Domain:EntryTypeNames.${topBarStore.entryType}`,
		);

		const formRef = React.useRef<HTMLFormElement>(undefined!);
		// HACK: jQuery UI's Autocomplete doesn't work properly when controlled.
		const globalSearchTermRef = React.useRef<HTMLInputElement>(undefined!);

		return (
			<form
				action="/Home/GlobalSearch"
				method="post"
				className="navbar-form form-inline pull-left navbar-search"
				id="globalSearchBox"
				ref={formRef}
			>
				<input
					type="hidden"
					name="objectType"
					value={topBarStore.entryType}
					onChange={(event): void => {
						runInAction(() => {
							topBarStore.entryType = event.target
								.value as typeof TopBarStore.entryTypes[number];
						});
					}}
				/>
				<Navbar.Toggle />
				<Navbar.Brand as={Link} to={'/'}>
					{vdb.values.siteName}
				</Navbar.Brand>
				<div className="input-prepend input-append navbar-searchBox">
					<Dropdown as={ButtonGroup}>
						<Dropdown.Toggle variant="info" href="#">
							<span>{entryTypeName}</span> <span className="caret"></span>
						</Dropdown.Toggle>
						<Dropdown.Menu>
							{allObjectTypes.map((entryType) => (
								<Dropdown.Item
									onClick={(): void =>
										runInAction(() => {
											topBarStore.entryType = entryType;
										})
									}
									key={entryType}
								>
									{t(`VocaDb.Web.Resources.Domain:EntryTypeNames.${entryType}`)}
								</Dropdown.Item>
							))}
						</Dropdown.Menu>
					</Dropdown>
					<JQueryUIAutocomplete
						type="text"
						name="globalSearchTerm"
						className="globalSearchBox search-query"
						size={50}
						placeholder={t('ViewRes:Shared.Search')}
						maxLength={255}
						source={(
							request: { term: string },
							response: (items: string[]) => void,
						): void => {
							globalSearchBoxSource(topBarStore.entryType, request.term).then(
								response,
							);
						}}
						select={(event: Event, ui): void => {
							globalSearchTermRef.current.value = ui.item.value;
							formRef.current.submit();
						}}
						ref={globalSearchTermRef}
					/>
					<Button
						type="submit"
						variant="info"
						title={t('ViewRes:Shared.Search')}
					>
						<i className="icon-search"></i>
					</Button>
				</div>{' '}
				<ButtonGroup className="navbar-languageBar">
					<ShowRandomPageButton
						entryType={topBarStore.entryType}
						globalSearchTermRef={globalSearchTermRef}
					/>
				</ButtonGroup>{' '}
				<Dropdown className="navbar-languageBar" as={ButtonGroup}>
					<Dropdown.Toggle
						variant="info"
						href="#"
						className="navbar-languageBtn"
					>
						<i className="icon-user"></i>{' '}
						<span className="visible-desktop">
							{t('ViewRes:Layout.Account')}
						</span>
						{topBarStore.hasNotifications && (
							<>
								{' '}
								<span className="badge badge-small badge-important">!</span>
							</>
						)}{' '}
						<span className="caret"></span>
					</Dropdown.Toggle>

					<Dropdown.Menu>
						{!vdb.values.loggedUser ? (
							<>
								<Dropdown.Item
									as={Link}
									to="/User/Login" /* TODO: showLoginPopup */
								>
									{t('ViewRes:Layout.LogIn')}
								</Dropdown.Item>
								<Dropdown.Item href={'/User/Create'}>
									{t('ViewRes:Layout.Register')}
								</Dropdown.Item>
							</>
						) : (
							<>
								<Dropdown.Item
									as={Link}
									to={EntryUrlMapper.details_user_byName(
										vdb.values.loggedUser.name,
									)}
								>
									{t('ViewRes.User:MySettings.Profile')}
								</Dropdown.Item>
								<Dropdown.Item
									as={Link}
									to={`${EntryUrlMapper.details_user_byName(
										vdb.values.loggedUser.name,
									)}/albums`}
								>
									{t('ViewRes:TopBar.MyAlbums')}
								</Dropdown.Item>
								<Dropdown.Item
									as={Link}
									to={`${EntryUrlMapper.details_user_byName(
										vdb.values.loggedUser.name,
									)}/songs`}
								>
									{t('ViewRes:TopBar.MySongs')}
								</Dropdown.Item>
								<Dropdown.Item as={Link} to="/User/MySettings">
									{t('ViewRes.User:Details.MySettings')}
								</Dropdown.Item>
								{loginManager.canManageEntryReports && (
									<Dropdown.Item href={'/Admin/ViewEntryReports'}>
										{t('ViewRes:TopBar.EntryReports')}
										{topBarStore.reportCount > 0 && (
											<>
												{' '}
												<span className="badge badge-small badge-important">
													{topBarStore.reportCount}
												</span>
											</>
										)}
									</Dropdown.Item>
								)}
								<Dropdown.Item href={'/User/Logout'}>
									{t('ViewRes:Layout.LogOut')}
								</Dropdown.Item>
							</>
						)}
						<Dropdown.Divider />
						<Dropdown.Header>
							{t('ViewRes.User:MySettings.DefaultLanguageSelection')}
						</Dropdown.Header>
						{Object.values(ContentLanguagePreference).map((lp) => (
							<Dropdown.Item
								onClick={(): void => {
									setLanguagePreferenceCookie(lp);
								}}
								key={lp}
							>
								{lp === vdb.values.languagePreference ? (
									<i className="menuIcon icon-ok"></i>
								) : (
									<i className="menuIcon icon-"></i>
								)}{' '}
								{t(`Resources:ContentLanguageSelectionNames.${lp}`)}
							</Dropdown.Item>
						))}
					</Dropdown.Menu>
				</Dropdown>
				{vdb.values.loggedUser && (
					<>
						{' '}
						<Dropdown
							className="navbar-languageBar"
							onToggle={topBarStore.ensureMessagesLoaded}
							as={ButtonGroup}
						>
							<Dropdown.Toggle
								variant="info"
								href="#"
								className="navbar-languageBtn"
							>
								<i className="icon-envelope"></i>
								{vdb.values.loggedUser.unreadMessagesCount > 0 && (
									<>
										{' '}
										<span className="badge badge-small badge-important">
											{vdb.values.loggedUser.unreadMessagesCount}
										</span>
									</>
								)}{' '}
								<span className="caret"></span>
							</Dropdown.Toggle>

							<Dropdown.Menu>
								{vdb.values.loggedUser.unreadMessagesCount > 0 &&
									!topBarStore.isLoaded && (
										<Dropdown.ItemText>
											{t('ViewRes:Shared.Loading')}
										</Dropdown.ItemText>
									)}
								{topBarStore.unreadMessages.map((unreadMessage) => (
									<Dropdown.Item
										as={Link}
										to={`/User/Messages?${qs.stringify({
											messageId: unreadMessage.id,
											inbox: unreadMessage.inbox,
										})}`}
										key={unreadMessage.id}
									>
										<div className="media">
											<div className="pull-left media-image-usermessage">
												{unreadMessage.sender ? (
													// eslint-disable-next-line react/jsx-pascal-case
													<ProfileIconKnockout_ImageSize
														imageSize={ImageSize.SmallThumb}
														user={unreadMessage.sender}
														size={40}
													/>
												) : (
													<img
														src="/Content/vocadb_40.png"
														alt="Notification"
													/>
												)}
											</div>
											<div className="media-body media-body-usermessage">
												<span>
													{unreadMessage.sender
														? `${unreadMessage.sender?.name} -`
														: ''}
												</span>{' '}
												<small>{unreadMessage.createdFormatted}</small>
												<br />
												<span>{unreadMessage.subject}</span>
											</div>
										</div>
									</Dropdown.Item>
								))}
								{topBarStore.isLoaded &&
									topBarStore.unreadMessages.length === 0 && (
										<Dropdown.ItemText>
											{t('ViewRes:TopBar.NoUnreadMessages')}
										</Dropdown.ItemText>
									)}
								<Dropdown.Divider />
								<Dropdown.Item as={Link} to="/User/Messages">
									{t('ViewRes:TopBar.ViewAllMessages')}
								</Dropdown.Item>
							</Dropdown.Menu>
						</Dropdown>
					</>
				)}
				<Navbar.Collapse>
					<MainNavigationItems />
				</Navbar.Collapse>
			</form>
		);
	},
);
