import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { ArtistAutoComplete } from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { Layout } from '@/Components/Shared/Layout';
import { SaveBtn } from '@/Components/Shared/Partials/Shared/SaveBtn';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { useVdbTitle } from '@/Components/useVdbTitle';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { EntryType } from '@/Models/EntryType';
import { LoginManager } from '@/Models/LoginManager';
import { UserGroup } from '@/Models/Users/UserGroup';
import { OwnedArtistForUserEditRow } from '@/Pages/User/Partials/OwnedArtistForUserEditRow';
import { PermissionEditRow } from '@/Pages/User/Partials/PermissionEditRow';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { UserEditStore } from '@/Stores/User/UserEditStore';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const antiforgeryRepo = new AntiforgeryRepository(httpClient, urlMapper);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);

interface UserEditLayoutProps {
	userEditStore: UserEditStore;
}

const UserEditLayout = observer(
	({ userEditStore }: UserEditLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		const contract = userEditStore.contract;

		const title = `Edit user - ${contract.name}`; /* LOC */

		useVdbTitle(title, true);

		const navigate = useNavigate();

		return (
			<Layout
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
							Users{/* LOC */}
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
				toolbar={
					<>
						{!contract.active && (
							<JQueryUIButton
								as="a"
								href={`/User/Clear/${contract.id}`} /* TODO: Convert to POST */
								onClick={(e): void => {
									if (
										!window.confirm(
											'Are you sure you want to clear ratings for this user?' /* LOC */,
										)
									) {
										e.preventDefault();
									}
								}}
							>
								Clear user ratings{/* LOC */}
							</JQueryUIButton>
						)}
					</>
				}
			>
				{userEditStore.errors && (
					<ValidationSummaryPanel
						message="Unable to edit user." /* LOC */
						errors={userEditStore.errors}
					/>
				)}

				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const id = await userEditStore.submit(requestToken);

							navigate(EntryUrlMapper.details(EntryType.User, id));
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to edit user.' /* LOC */,
							);

							throw error;
						}
					}}
				>
					<fieldset>
						<legend>Account settings{/* LOC */}</legend>
						<p>
							<input
								type="checkbox"
								checked={userEditStore.active}
								onChange={(e): void =>
									runInAction(() => {
										userEditStore.active = e.target.checked;
									})
								}
							/>{' '}
							Active {/* LOC */}
						</p>
						<p>
							<input
								type="checkbox"
								checked={userEditStore.poisoned}
								onChange={(e): void =>
									runInAction(() => {
										userEditStore.poisoned = e.target.checked;
									})
								}
							/>{' '}
							Poisoned (autoban for logging in) {/* LOC */}
						</p>
						<p>
							<input
								type="checkbox"
								checked={userEditStore.supporter}
								onChange={(e): void =>
									runInAction(() => {
										userEditStore.supporter = e.target.checked;
									})
								}
							/>{' '}
							Patreon supporter {/* LOC */}
						</p>

						<label htmlFor="name">Username{/* LOC */}</label>
						<input
							type="text"
							id="name"
							value={userEditStore.name}
							onChange={(e): void =>
								runInAction(() => {
									userEditStore.name = e.target.value;
								})
							}
							maxLength={100}
						/>

						<label htmlFor="email">Email{/* LOC */}</label>
						<input
							type="text"
							id="email"
							value={userEditStore.email}
							onChange={(e): void =>
								runInAction(() => {
									userEditStore.email = e.target.value;
								})
							}
							maxLength={50}
						/>

						<label htmlFor="groupId">User group{/* LOC */}</label>
						<select
							id="groupId"
							value={userEditStore.groupId}
							onChange={(e): void =>
								runInAction(() => {
									userEditStore.groupId = e.target.value as UserGroup;
								})
							}
						>
							{Object.values(UserGroup)
								.filter((userGroup) => loginManager.canEditGroupTo(userGroup))
								.map((userGroup) => (
									<option value={userGroup} key={userGroup}>
										{t(`Resources:UserGroupNames.${userGroup}`)}
									</option>
								))}
						</select>
					</fieldset>

					{loginManager.loggedUser?.groupId === UserGroup.Admin && (
						<fieldset>
							<legend>Additional permissions</legend>
							{userEditStore.permissions.map((permission) => (
								<PermissionEditRow
									permissionEditStore={permission}
									key={permission.permissionToken}
								/>
							))}
						</fieldset>
					)}

					<fieldset>
						<legend>Owned artists{/* LOC */}</legend>
						<table>
							<tbody id="ownedArtistsTableBody">
								{userEditStore.ownedArtists.map((ownedArtist, index) => (
									<OwnedArtistForUserEditRow
										userEditStore={userEditStore}
										ownedArtist={ownedArtist}
										key={index}
									/>
								))}
							</tbody>
						</table>
						<ArtistAutoComplete
							type="text"
							className="input-large"
							properties={{
								acceptSelection: userEditStore.addArtist,
							}}
							placeholder={t('ViewRes:Shared.Search')}
						/>
					</fieldset>

					<SaveBtn submitting={userEditStore.submitting} />
				</form>
			</Layout>
		);
	},
);

const UserEdit = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<{ userEditStore: UserEditStore }>();

	React.useEffect(() => {
		userRepo.getForEdit({ id: Number(id) }).then((model) =>
			setModel({
				userEditStore: new UserEditStore(
					model,
					vdb.values,
					artistRepo,
					userRepo,
				),
			}),
		);
	}, [id]);

	return model ? <UserEditLayout userEditStore={model.userEditStore} /> : <></>;
};

export default UserEdit;
