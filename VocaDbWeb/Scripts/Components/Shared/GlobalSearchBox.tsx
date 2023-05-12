import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Dropdown from '@/Bootstrap/Dropdown';
import Navbar from '@/Bootstrap/Navbar';
import { MainNavigationItems } from '@/Components/Shared/Partials/MainNavigationItems';
import { ProfileIconKnockout_ImageSize } from '@/Components/Shared/Partials/User/ProfileIconKnockout_ImageSize';
import { ShowRandomPageButton } from '@/Components/Shared/ShowRandomPageButton';
import JQueryUIAutocomplete from '@/JQueryUI/JQueryUIAutocomplete';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { ImageSize } from '@/Models/Images/ImageSize';
import { NameMatchMode } from '@/Models/NameMatchMode';
import { UserGroup } from '@/Models/Users/UserGroup';
import { albumRepo } from '@/Repositories/AlbumRepository';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { entryRepo } from '@/Repositories/EntryRepository';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { songListRepo } from '@/Repositories/SongListRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { tagRepo } from '@/Repositories/TagRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { functions } from '@/Shared/GlobalFunctions';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { AlbumSortRule } from '@/Stores/Search/AlbumSearchStore';
import { ArtistSortRule } from '@/Stores/Search/ArtistSearchStore';
import { SongSortRule } from '@/Stores/Search/SongSearchStore';
import { SongListSortRule } from '@/Stores/SongList/SongListsBaseStore';
import { TopBarStore } from '@/Stores/TopBarStore';
import { useVdb } from '@/VdbContext';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';

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

	const endpoint = urlMapper.mapRelative(
		functions.mergeUrls(apiEndpoint, '/names'),
	);

	return httpClient.get<string[]>(endpoint, { query: query });
};

interface GlobalSearchBoxProps {
	topBarStore: TopBarStore;
}

export const GlobalSearchBox = observer(
	({ topBarStore }: GlobalSearchBoxProps): React.ReactElement => {
		const vdb = useVdb();
		const loginManager = useLoginManager();

		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.User',
			'VocaDb.Web.Resources.Domain',
		]);

		const entryTypeName = t(
			`VocaDb.Web.Resources.Domain:EntryTypeNames.${topBarStore.entryType}`,
		);

		// HACK: jQuery UI's Autocomplete doesn't work properly when controlled.
		const globalSearchTermRef = React.useRef<HTMLInputElement>(undefined!);

		const setLanguagePreferenceCookie = React.useCallback(
			(languagePreference: ContentLanguagePreference): boolean => {
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
			},
			[vdb],
		);

		const navigate = useNavigate();
		const submit = React.useCallback(async (): Promise<void> => {
			const tryRedirectEntry = async (filter: string): Promise<void> => {
				const { items } = await entryRepo.getList({
					paging: { start: 0, maxEntries: 2, getTotalCount: false },
					lang: vdb.values.languagePreference,
					query: filter,
				});

				if (items.length === 1) {
					navigate(EntryUrlMapper.details_entry(items[0]));
				} else {
					navigate(
						`/Search?${qs.stringify({
							filter: filter,
						})}`,
					);
				}
			};

			const tryRedirectAlbum = async (filter: string): Promise<void> => {
				const { items } = await albumRepo.getList({
					paging: { start: 0, maxEntries: 2, getTotalCount: false },
					lang: vdb.values.languagePreference,
					query: filter,
					sort: AlbumSortRule.None,
				});

				if (items.length === 1) {
					navigate(EntryUrlMapper.details(EntryType.Album, items[0].id));
				} else {
					navigate(
						`/Search?${qs.stringify({
							filter: filter,
							searchType: EntryType.Album,
						})}`,
					);
				}
			};

			const tryRedirectArtist = async (filter: string): Promise<void> => {
				const { items } = await artistRepo.getList({
					paging: { start: 0, maxEntries: 2, getTotalCount: false },
					lang: vdb.values.languagePreference,
					query: filter,
					sort: ArtistSortRule.None,
				});

				if (items.length === 1) {
					navigate(EntryUrlMapper.details(EntryType.Artist, items[0].id));
				} else {
					navigate(
						`/Search?${qs.stringify({
							filter: filter,
							searchType: EntryType.Artist,
						})}`,
					);
				}
			};

			const tryRedirectReleaseEvent = async (filter: string): Promise<void> => {
				const { items } = await eventRepo.getList({
					queryParams: {
						getTotalCount: false,
						lang: vdb.values.languagePreference,
						maxResults: 2,
						start: 0,
						query: filter,
					},
				});

				if (items.length === 1) {
					navigate(EntryUrlMapper.details(EntryType.ReleaseEvent, items[0].id));
				} else {
					navigate(
						`/Search?${qs.stringify({
							filter: filter,
							searchType: EntryType.ReleaseEvent,
						})}`,
					);
				}
			};

			const tryRedirectSong = async (filter: string): Promise<void> => {
				const { items } = await songRepo.getList({
					lang: vdb.values.languagePreference,
					paging: { start: 0, maxEntries: 2, getTotalCount: false },
					queryParams: {
						query: filter,
						sort: SongSortRule.None,
					},
				});

				if (items.length === 1) {
					navigate(EntryUrlMapper.details(EntryType.Song, items[0].id));
				} else {
					navigate(
						`/Search?${qs.stringify({
							filter: filter,
							searchType: EntryType.Song,
						})}`,
					);
				}
			};

			const tryRedirectSongList = async (filter: string): Promise<void> => {
				const { items } = await songListRepo.getFeatured({
					query: filter,
					paging: { start: 0, maxEntries: 2, getTotalCount: false },
					sort: SongListSortRule.None,
				});

				if (items.length === 1) {
					navigate(EntryUrlMapper.details(EntryType.SongList, items[0].id));
				} else {
					navigate('/SongList/Featured');
				}
			};

			const tryRedirectTag = async (filter: string): Promise<void> => {
				try {
					const tag = await tagRepo.getByName({
						name: filter,
						lang: vdb.values.languagePreference,
					});
					navigate(EntryUrlMapper.details(EntryType.Tag, tag.id));
				} catch {
					navigate(
						`/Search?${qs.stringify({
							filter: filter,
							searchType: EntryType.Tag,
						})}`,
					);
				}
			};

			const tryRedirectUser = async (filter: string): Promise<void> => {
				const { items } = await userRepo.getList({
					paging: { start: 0, maxEntries: 2, getTotalCount: false },
					query: filter,
					groups: UserGroup.Nothing,
					includeDisabled: false,
					onlyVerified: false,
					nameMatchMode: NameMatchMode.Auto,
				});

				if (items.length === 1) {
					navigate(EntryUrlMapper.details_user_byName(items[0].name));
				} else {
					navigate(
						`/User?${qs.stringify({
							filter: filter,
						})}`,
					);
				}
			};

			const tryRedirectFuncs = {
				[EntryType.Undefined]: tryRedirectEntry,
				[EntryType.Album]: tryRedirectAlbum,
				[EntryType.Artist]: tryRedirectArtist,
				[EntryType.ReleaseEvent]: tryRedirectReleaseEvent,
				[EntryType.Song]: tryRedirectSong,
				[EntryType.SongList]: tryRedirectSongList,
				[EntryType.Tag]: tryRedirectTag,
				[EntryType.User]: tryRedirectUser,
			};

			await tryRedirectFuncs[topBarStore.entryType](
				globalSearchTermRef.current.value,
			);
		}, [vdb, topBarStore, navigate]);

		return (
			<form
				className="navbar-form form-inline pull-left navbar-search"
				id="globalSearchBox"
				onSubmit={async (e): Promise<void> => {
					e.preventDefault();
					await submit();
				}}
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
							submit();
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
								<Dropdown.Item as={Link} to="/User/Create">
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
									<Dropdown.Item as={Link} to="/Admin/ViewEntryReports">
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
								<Dropdown.Item
									onClick={async (): Promise<void> => {
										const requestToken = await antiforgeryRepo.getToken();

										await userRepo.logout(requestToken);

										navigate('/');

										await vdb.refresh();
									}}
								>
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
