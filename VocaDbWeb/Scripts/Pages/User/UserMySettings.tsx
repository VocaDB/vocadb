import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { Layout } from '@/Components/Shared/Layout';
import {
	CultureDropdownList,
	EmailOptionsDropdownList,
	LanguagePreferenceDropdownList,
	UserLanguageCultureDropdownList,
	UserLanguageProficiencyDropdownList,
} from '@/Components/Shared/Partials/Knockout/DropdownList';
import { WebLinksEditViewKnockout } from '@/Components/Shared/Partials/Knockout/WebLinksEditViewKnockout';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { ImageUploadMessage } from '@/Components/Shared/Partials/Shared/ImageUploadMessage';
import { MarkdownNotice } from '@/Components/Shared/Partials/Shared/MarkdownNotice';
import { SaveBtn } from '@/Components/Shared/Partials/Shared/SaveBtn';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import { UserLanguageProficiency } from '@/DataContracts/User/UserKnownLanguageContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import JQueryUITab from '@/JQueryUI/JQueryUITab';
import JQueryUITabs from '@/JQueryUI/JQueryUITabs';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { ImageSize } from '@/Models/Images/ImageSize';
import { PVService } from '@/Models/PVs/PVService';
import { UserEmailOptions } from '@/Models/Users/UserEmailOptions';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { MySettingsStore } from '@/Stores/User/MySettingsStore';
import { useVdb } from '@/VdbContext';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';

// Code from: https://github.com/browserify/path-browserify/blob/4df8c2ae7efbecf54538aafc34b295c0934f256e/index.js#L27.
function assertPath(path: any): void {
	if (typeof path !== 'string') {
		throw new TypeError(
			'Path must be a string. Received ' + JSON.stringify(path),
		);
	}
}

// Code from: https://github.com/browserify/path-browserify/blob/4df8c2ae7efbecf54538aafc34b295c0934f256e/index.js#L445.
function parsePath(
	path: string,
): {
	root: string;
	dir: string;
	base: string;
	ext: string;
	name: string;
} {
	assertPath(path);

	var ret = { root: '', dir: '', base: '', ext: '', name: '' };
	if (path.length === 0) return ret;
	var code = path.charCodeAt(0);
	var isAbsolute = code === 47; /*/*/
	var start;
	if (isAbsolute) {
		ret.root = '/';
		start = 1;
	} else {
		start = 0;
	}
	var startDot = -1;
	var startPart = 0;
	var end = -1;
	var matchedSlash = true;
	var i = path.length - 1;

	// Track the state of characters (if any) we see before our first dot and
	// after any path separator we find
	var preDotState = 0;

	// Get non-dir info
	for (; i >= start; --i) {
		code = path.charCodeAt(i);
		if (code === 47 /*/*/) {
			// If we reached a path separator that was not part of a set of path
			// separators at the end of the string, stop now
			if (!matchedSlash) {
				startPart = i + 1;
				break;
			}
			continue;
		}
		if (end === -1) {
			// We saw the first non-path separator, mark this as the end of our
			// extension
			matchedSlash = false;
			end = i + 1;
		}
		if (code === 46 /*.*/) {
			// If this is our first dot, mark it as the start of our extension
			if (startDot === -1) startDot = i;
			else if (preDotState !== 1) preDotState = 1;
		} else if (startDot !== -1) {
			// We saw a non-dot and non-path separator before our dot, so we should
			// have a good chance at having a non-empty extension
			preDotState = -1;
		}
	}

	if (
		startDot === -1 ||
		end === -1 ||
		// We saw a non-dot character immediately before the dot
		preDotState === 0 ||
		// The (right-most) trimmed path component is exactly '..'
		(preDotState === 1 && startDot === end - 1 && startDot === startPart + 1)
	) {
		if (end !== -1) {
			if (startPart === 0 && isAbsolute)
				ret.base = ret.name = path.slice(1, end);
			else ret.base = ret.name = path.slice(startPart, end);
		}
	} else {
		if (startPart === 0 && isAbsolute) {
			ret.name = path.slice(1, startDot);
			ret.base = path.slice(1, end);
		} else {
			ret.name = path.slice(startPart, startDot);
			ret.base = path.slice(startPart, end);
		}
		ret.ext = path.slice(startDot, end);
	}

	if (startPart > 0) ret.dir = path.slice(0, startPart - 1);
	else if (isAbsolute) ret.dir = '/';

	return ret;
}

interface AccountSettingsTabContentProps {
	mySettingsStore: MySettingsStore;
}

const AccountSettingsTabContent = observer(
	({ mySettingsStore }: AccountSettingsTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.User']);

		const contract = mySettingsStore.contract;

		return (
			<div>
				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.User:MySettings.Username')}
						dangerouslySetInnerHTML={{
							__html:
								'Username can be changed once per year. Username may contain alphanumeric characters and underscores. After changing your username you need to log in again. Contact staff member if necessary.' /* LOC */,
						}}
					/>
				</div>
				<div className="editor-field">
					{contract.canChangeName ? (
						<input
							type="text"
							value={mySettingsStore.username}
							onChange={(e): void =>
								runInAction(() => {
									mySettingsStore.username = e.target.value;
								})
							}
						/>
					) : (
						mySettingsStore.username
					)}
				</div>

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.User:MySettings.Email')}
						dangerouslySetInnerHTML={{
							__html: t('ViewRes.User:MySettings.EmailNote'),
						}}
					/>
				</div>
				<div className="editor-field">
					<input
						type="text"
						value={mySettingsStore.email}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.email = e.target.value;
							})
						}
						maxLength={50}
					/>
					{mySettingsStore.canVerifyEmail && (
						<>
							{' '}
							<SafeAnchor
								onClick={async (): Promise<void> => {
									await mySettingsStore.verifyEmail();

									showSuccessMessage(
										'Message sent, please check your email' /* LOC */,
									);
								}}
								href="#"
								className="textLink acceptLink"
							>
								Verify email{/* LOC */}
							</SafeAnchor>
						</>
					)}
					{mySettingsStore.emailVerified && (
						<>
							{' '}
							{/* eslint-disable-next-line jsx-a11y/alt-text */}
							<img
								src="/Content/Icons/tick.png"
								title="Verified email" /* LOC */
							/>
						</>
					)}{' '}
					{mySettingsStore.errors && mySettingsStore.errors.email && (
						<span className="field-validation-error">
							{mySettingsStore.errors.email[0]}
						</span>
					)}
				</div>

				<div className="editor-label">
					{t('ViewRes.User:MySettings.EmailOptions')}
				</div>
				<div className="editor-field">
					<EmailOptionsDropdownList
						value={mySettingsStore.emailOptions}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.emailOptions = e.target
									.value as UserEmailOptions;
							})
						}
						className="input-xlarge"
					/>
				</div>

				<div className="editor-label">
					{t('ViewRes.User:MySettings.Privacy')}
				</div>
				<div className="editor-field">
					<p>
						<label className="checkbox">
							<input
								type="checkbox"
								checked={mySettingsStore.showActivity}
								onChange={(e): void =>
									runInAction(() => {
										mySettingsStore.showActivity = e.target.checked;
									})
								}
							/>{' '}
							{t('ViewRes.User:MySettings.AnonymousActivity')}
						</label>
					</p>
					<p>
						<label className="checkbox">
							<input
								type="checkbox"
								checked={mySettingsStore.publicRatings}
								onChange={(e): void =>
									runInAction(() => {
										mySettingsStore.publicRatings = e.target.checked;
									})
								}
							/>{' '}
							{t('ViewRes.User:MySettings.PublicRatings')}
						</label>
					</p>
					<p>
						<label className="checkbox">
							<input
								type="checkbox"
								checked={mySettingsStore.publicAlbumCollection}
								onChange={(e): void =>
									runInAction(() => {
										mySettingsStore.publicAlbumCollection = e.target.checked;
									})
								}
							/>{' '}
							{t('ViewRes.User:MySettings.PublicAlbumCollection')}
						</label>
					</p>
				</div>
			</div>
		);
	},
);

interface PasswordSettingsTabContentProps {
	mySettingsStore: MySettingsStore;
}

const PasswordSettingsTabContent = observer(
	({
		mySettingsStore,
	}: PasswordSettingsTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.User']);

		const contract = mySettingsStore.contract;

		return (
			<>
				<Alert variant="info">
					{t('ViewRes.User:MySettings.ChangePasswordNotice')}
				</Alert>

				{contract.hasPassword && (
					<>
						<div className="editor-label">
							{t('ViewRes.User:MySettings.CurrentPass')}
						</div>
						<div className="editor-field">
							<input
								type="password"
								value={mySettingsStore.oldPass}
								onChange={(e): void =>
									runInAction(() => {
										mySettingsStore.oldPass = e.target.value;
									})
								}
							/>{' '}
							{mySettingsStore.errors && mySettingsStore.errors.oldPass && (
								<span className="field-validation-error">
									{mySettingsStore.errors.oldPass[0]}
								</span>
							)}
						</div>
					</>
				)}

				<div className="editor-label">
					{t('ViewRes.User:MySettings.NewPass')}
				</div>
				<div className="editor-field">
					<input
						type="password"
						value={mySettingsStore.newPass}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.newPass = e.target.value;
							})
						}
					/>{' '}
					{mySettingsStore.errors && mySettingsStore.errors.newPass && (
						<span className="field-validation-error">
							{mySettingsStore.errors.newPass[0]}
						</span>
					)}
				</div>

				<div className="editor-label">
					{t('ViewRes.User:MySettings.NewPassAgain')}
				</div>
				<div className="editor-field">
					<input
						type="password"
						value={mySettingsStore.newPassAgain}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.newPassAgain = e.target.value;
							})
						}
					/>{' '}
					{mySettingsStore.errors && mySettingsStore.errors.newPassAgain && (
						<span className="field-validation-error">
							{mySettingsStore.errors.newPassAgain[0]}
						</span>
					)}
				</div>
			</>
		);
	},
);

interface InterfaceSettingsTabContentProps {
	mySettingsStore: MySettingsStore;
}

const InterfaceSettingsTabContent = observer(
	({
		mySettingsStore,
	}: InterfaceSettingsTabContentProps): React.ReactElement => {
		const vdb = useVdb();

		const { t } = useTranslation(['ViewRes.User']);

		return (
			<>
				<div className="editor-label">
					{t('ViewRes.User:MySettings.InterfaceLanguageSelection')}
				</div>
				<div className="editor-field">
					<CultureDropdownList
						placeholder={t('ViewRes.User:MySettings.Automatic')}
						value={mySettingsStore.interfaceLanguageSelection}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.interfaceLanguageSelection = e.target.value;
							})
						}
						className="input-xlarge"
					/>
				</div>

				<div className="editor-label">
					<label>{t('ViewRes.User:MySettings.CultureName')}</label>
				</div>
				<div className="editor-field">
					<CultureDropdownList
						placeholder={t('ViewRes.User:MySettings.Automatic')}
						value={mySettingsStore.cultureSelection}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.cultureSelection = e.target.value;
							})
						}
						className="input-xlarge"
					/>
				</div>

				<div className="editor-label">
					{t('ViewRes.User:MySettings.DefaultLanguageSelection')}
				</div>
				<div className="editor-field">
					<LanguagePreferenceDropdownList
						value={mySettingsStore.defaultLanguageSelection}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.defaultLanguageSelection = e.target
									.value as ContentLanguagePreference;
							})
						}
					/>
				</div>

				<div className="editor-label">
					{t('ViewRes.User:MySettings.PreferredVideoService')}
				</div>
				<div className="editor-field">
					<select
						value={mySettingsStore.preferredVideoService}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.preferredVideoService = e.target.value;
							})
						}
					>
						{Object.values(PVService)
							.filter((pvService) => isNaN(Number(pvService)))
							.map((pvService) => (
								<option value={pvService} key={pvService}>
									{pvService}
								</option>
							))}
					</select>
				</div>

				<div className="editor-label">
					{t('ViewRes.User:MySettings.UnreadNotificationsToKeep')}
				</div>
				<div className="editor-field">
					<input
						type="number"
						min={1}
						max={390}
						maxLength={3}
						value={mySettingsStore.unreadNotificationsToKeep}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.unreadNotificationsToKeep = e.target.value;
							})
						}
					/>
				</div>

				<div className="editor-label">{t('ViewRes.User:MySettings.Theme')}</div>
				<div className="editor-field">
					<select
						value={mySettingsStore.stylesheet}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.stylesheet = e.target.value;
							})
						}
					>
						<option value="">Default{/* LOC */}</option>
						{vdb.values.stylesheets.map((stylesheet) => (
							<option value={stylesheet} key={stylesheet}>
								{parsePath(stylesheet).name}
							</option>
						))}
					</select>
				</div>
			</>
		);
	},
);

interface ProfileSettingsTabContentProps {
	mySettingsStore: MySettingsStore;
	pictureUploadRef: React.MutableRefObject<HTMLInputElement>;
}

const ProfileSettingsTabContent = observer(
	({
		mySettingsStore,
		pictureUploadRef,
	}: ProfileSettingsTabContentProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes.User',
			'VocaDb.Web.Resources.Domain.Globalization',
		]);

		const contract = mySettingsStore.contract;

		return (
			<>
				<div className="editor-label">
					{t('ViewRes.User:MySettings.AboutMe')} <MarkdownNotice />
				</div>
				<div className="editor-field">
					<textarea
						value={mySettingsStore.aboutMe}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.aboutMe = e.target.value;
							})
						}
						className="input-xxlarge"
						rows={7}
					/>
					<br />

					{t('ViewRes.User:MySettings.LivePreview')}
					<Markdown>{mySettingsStore.aboutMe}</Markdown>
					<br />
				</div>

				<div className="editor-label">
					{t('ViewRes.User:Details.LanguagesIKnow')}
				</div>
				<div className="editor-field">
					<table>
						<tbody>
							{mySettingsStore.knownLanguages.map((knownLanguage, index) => (
								<tr key={index}>
									<td>
										<UserLanguageCultureDropdownList
											placeholder={t(
												'VocaDb.Web.Resources.Domain.Globalization:InterfaceLanguage.Other',
											)}
											value={knownLanguage.cultureCode}
											onChange={(e): void =>
												runInAction(() => {
													knownLanguage.cultureCode = e.target.value;
												})
											}
											className="input-xlarge"
										/>
									</td>
									<td>
										<UserLanguageProficiencyDropdownList
											value={knownLanguage.proficiency}
											onChange={(e): void =>
												runInAction(() => {
													knownLanguage.proficiency = e.target
														.value as UserLanguageProficiency;
												})
											}
										/>
									</td>
									<td>
										<SafeAnchor
											href="#"
											className="textLink deleteLink"
											onClick={(): void =>
												mySettingsStore.removeKnownLanguage(knownLanguage)
											}
										>
											{t('ViewRes:Shared.Delete')}
										</SafeAnchor>
									</td>
								</tr>
							))}
						</tbody>
					</table>
					<SafeAnchor
						href="#"
						className="textLink addLink"
						onClick={mySettingsStore.addKnownLanguage}
					>
						{t('ViewRes.User:MySettings.AddKnownLanguage')}
					</SafeAnchor>
				</div>

				<div className="editor-label">
					{t('ViewRes.User:MySettings.Location')}
				</div>
				<div className="editor-field">
					<input
						type="text"
						value={mySettingsStore.location}
						onChange={(e): void =>
							runInAction(() => {
								mySettingsStore.location = e.target.value;
							})
						}
						className="input-xlarge"
					/>
				</div>

				<div className="editor-label">
					<label>Picture{/* LOC */}</label>
				</div>
				<div className="editor-field">
					<table>
						<tbody>
							<tr>
								<td>
									{/* eslint-disable-next-line jsx-a11y/img-redundant-alt */}
									<img
										src={UrlHelper.imageThumb(
											contract.mainPicture,
											ImageSize.SmallThumb,
										)}
										alt="Picture" /* LOC */
										className="coverPic"
									/>
								</td>
								<td>
									<ImageUploadMessage />
									<input
										type="file"
										id="pictureUpload"
										name="pictureUpload"
										ref={pictureUploadRef}
									/>
								</td>
							</tr>
						</tbody>
					</table>
				</div>

				<div className="editor-label">
					{t('ViewRes.User:MySettings.ExternalLinks')}
				</div>
				<div className="editor-field">
					<WebLinksEditViewKnockout
						webLinksEditStore={mySettingsStore.webLinksStore}
					/>
				</div>
			</>
		);
	},
);

interface ConnectivitySettingsTabContentProps {
	mySettingsStore: MySettingsStore;
}

const ConnectivitySettingsTabContent = observer(
	({
		mySettingsStore,
	}: ConnectivitySettingsTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.User']);

		const contract = mySettingsStore.contract;

		return (
			<>
				<Alert variant="info">
					{t('ViewRes.User:MySettings.AccessKeyDescription')}
				</Alert>

				<div className="editor-label">
					{t('ViewRes.User:MySettings.AccessKey')}
				</div>
				<div className="editor-field">
					{contract.hashedAccessKey}{' '}
					<a
						href="/User/ResetAccesskey" /* TODO: Convert to POST. */
						onClick={(e): void => {
							if (!window.confirm(t('ViewRes.User:MySettings.ReallyReset'))) {
								e.preventDefault();
							}
						}}
						className="textLink refreshLink"
					>
						{t('ViewRes.User:MySettings.Reset')}
					</a>
				</div>

				<div className="editor-label">Twitter</div>
				<div className="editor-field">
					{contract.hasTwitterToken ? (
						<>
							{contract.twitterName} &nbsp;{' '}
							<SafeAnchor
								href="/User/DisconnectTwitter" /* TODO: Convert to POST. */
								className="textLink removeLink"
							>
								Disconnect{/* LOC */}
							</SafeAnchor>
						</>
					) : (
						<SafeAnchor
							href="/User/ConnectTwitter" /* TODO: Convert to POST. */
							className="textLink addLink"
						>
							{t('ViewRes.User:MySettings.Connect')}
						</SafeAnchor>
					)}
				</div>
			</>
		);
	},
);

interface UserMySettingsLayoutProps {
	mySettingsStore: MySettingsStore;
}

const UserMySettingsLayout = observer(
	({ mySettingsStore }: UserMySettingsLayoutProps): React.ReactElement => {
		const vdb = useVdb();

		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.User']);

		const title = t('ViewRes.User:MySettings.MySettingsTitle');

		const contract = mySettingsStore.contract;

		const pictureUploadRef = React.useRef<HTMLInputElement>(undefined!);

		const navigate = useNavigate();

		return (
			<Layout
				pageTitle={title}
				ready={ready}
				title={title}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: '/User',
							}}
							divider
						>
							{t('ViewRes:Shared.Users')}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details_user_byName(contract.name),
							}}
						>
							{contract.name}
						</Breadcrumb.Item>
					</>
				}
			>
				{mySettingsStore.errors && (
					<ValidationSummaryPanel
						message={t('ViewRes.User:MySettings.UnableToSave')}
						errors={mySettingsStore.errors}
					/>
				)}

				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const pictureUpload =
								pictureUploadRef.current.files?.item(0) ?? undefined;

							const name = await mySettingsStore.submit(
								requestToken,
								pictureUpload,
							);

							navigate(EntryUrlMapper.details_user_byName(name));

							await vdb.refresh();
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: t('ViewRes.User:MySettings.UnableToSave'),
							);

							throw error;
						}
					}}
				>
					<SaveBtn submitting={mySettingsStore.submitting} />

					<JQueryUITabs>
						<JQueryUITab
							eventKey="accountSettings"
							title={t('ViewRes.User:MySettings.AccountSettings')}
						>
							<AccountSettingsTabContent mySettingsStore={mySettingsStore} />
						</JQueryUITab>
						<JQueryUITab
							eventKey="password"
							title={t('ViewRes.User:MySettings.ChangePassword')}
						>
							<PasswordSettingsTabContent mySettingsStore={mySettingsStore} />
						</JQueryUITab>
						<JQueryUITab
							eventKey="interface"
							title={t('ViewRes.User:MySettings.Interface')}
						>
							<InterfaceSettingsTabContent mySettingsStore={mySettingsStore} />
						</JQueryUITab>
						<JQueryUITab
							eventKey="profile"
							title={t('ViewRes.User:MySettings.Profile')}
						>
							<ProfileSettingsTabContent
								mySettingsStore={mySettingsStore}
								pictureUploadRef={pictureUploadRef}
							/>
						</JQueryUITab>
						<JQueryUITab
							eventKey="connectivity"
							title={t('ViewRes.User:MySettings.Connectivity')}
						>
							<ConnectivitySettingsTabContent
								mySettingsStore={mySettingsStore}
							/>
						</JQueryUITab>
					</JQueryUITabs>

					<br />
					<SaveBtn submitting={mySettingsStore.submitting} />
				</form>
			</Layout>
		);
	},
);

const UserMySettings = (): React.ReactElement => {
	const [model, setModel] = React.useState<{
		mySettingsStore: MySettingsStore;
	}>();

	React.useEffect(() => {
		userRepo.getForMySettings().then((model) =>
			setModel({
				mySettingsStore: new MySettingsStore(userRepo, model),
			}),
		);
	}, []);

	return model ? (
		<UserMySettingsLayout mySettingsStore={model.mySettingsStore} />
	) : (
		<></>
	);
};

export default UserMySettings;
