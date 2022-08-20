import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { NamesEditor } from '@/Components/Shared/KnockoutPartials/NamesEditor';
import { Layout } from '@/Components/Shared/Layout';
import { EntryDeletePopup } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopup';
import { EntryTrashPopup } from '@/Components/Shared/Partials/EntryDetails/EntryTrashPopup';
import {
	LanguageSelectionDropdownList,
	RegionDropdownList,
	EntryStatusDropdownList,
} from '@/Components/Shared/Partials/Knockout/DropdownList';
import { WebLinksEditViewKnockout } from '@/Components/Shared/Partials/Knockout/WebLinksEditViewKnockout';
import { ConcurrentEditWarning } from '@/Components/Shared/Partials/Shared/ConcurrentEditWarning';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { SaveAndBackBtn } from '@/Components/Shared/Partials/Shared/SaveAndBackBtn';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { useConflictingEditor } from '@/Components/useConflictingEditor';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { VenueForEditContract } from '@/DataContracts/Venue/VenueForEditContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { LoginManager } from '@/Models/LoginManager';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { VenueRepository } from '@/Repositories/VenueRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { VenueEditStore } from '@/Stores/Venue/VenueEditStore';
import { getReasonPhrase } from 'http-status-codes';
import _ from 'lodash';
import { reaction, runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const antiforgeryRepo = new AntiforgeryRepository(httpClient, urlMapper);
const venueRepo = new VenueRepository(httpClient, urlMapper);

interface VenueEditLayoutProps {
	venueEditStore: VenueEditStore;
}

const VenueEditLayout = observer(
	({ venueEditStore }: VenueEditLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Venue']);

		const contract = venueEditStore.contract;
		const isNew = contract.id === 0;

		const title = isNew
			? `Create a new venue` /* TODO: localize */
			: `Edit venue - ${venueEditStore.name}`; /* TODO: localize */

		useVocaDbTitle(title, true);

		const backAction = isNew
			? '/Event/EventsByVenue'
			: EntryUrlMapper.details(EntryType.Venue, contract.id);

		const navigate = useNavigate();

		const conflictingEditor = useConflictingEditor(EntryType.Venue);

		React.useEffect(() => {
			if (venueEditStore.contract.id) return;

			const disposers = [
				venueEditStore.names.originalName,
				venueEditStore.names.romajiName,
				venueEditStore.names.englishName,
			].map((name) =>
				reaction(() => name.value, _.debounce(venueEditStore.checkName, 500)),
			);

			return (): void => {
				for (const disposer of disposers) disposer();
			};
		}, [venueEditStore]);

		return (
			<Layout
				title={title}
				parents={
					isNew ? (
						<>
							<Breadcrumb.Item href="/Event/EventsByVenue">
								{t('ViewRes:Shared.Venues')}
							</Breadcrumb.Item>
						</>
					) : (
						<>
							<Breadcrumb.Item href="/Event/EventsByVenue" divider>
								{t('ViewRes:Shared.Venues')}
							</Breadcrumb.Item>
							<Breadcrumb.Item
								linkAs={Link}
								linkProps={{
									to: EntryUrlMapper.details(EntryType.Venue, contract.id),
								}}
							>
								{venueEditStore.name}
							</Breadcrumb.Item>
						</>
					)
				}
				toolbar={
					!isNew &&
					loginManager.canDeleteEntries && (
						<>
							{venueEditStore.deleted ? (
								<JQueryUIButton
									as="a"
									href={`/Venue/Restore/${contract.id}`} /* TODO: Convert to POST. */
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:EntryEdit.Restore')}
								</JQueryUIButton>
							) : (
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={venueEditStore.deleteStore.show}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:Shared.Delete')}
								</JQueryUIButton>
							)}{' '}
							<JQueryUIButton
								as={SafeAnchor}
								href="#"
								onClick={venueEditStore.trashStore.show}
								icons={{ primary: 'ui-icon-trash' }}
							>
								{t('ViewRes:EntryEdit.MoveToTrash')}
							</JQueryUIButton>
						</>
					)
				}
			>
				{conflictingEditor && conflictingEditor.userId !== 0 && (
					<ConcurrentEditWarning conflictingEditor={conflictingEditor} />
				)}

				{venueEditStore.errors && (
					<ValidationSummaryPanel
						message="Unable to save properties." /* TODO: localize */
						errors={venueEditStore.errors}
					/>
				)}

				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const id = await venueEditStore.submit(requestToken);

							navigate(EntryUrlMapper.details(EntryType.Venue, id));
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save properties.' /* TODO: localize */,
							);
						}
					}}
				>
					<SaveAndBackBtn
						backAction={backAction}
						submitting={venueEditStore.submitting}
					/>

					<div className="editor-label">
						<HelpLabel
							label={t('ViewRes:EntryEdit.DefaultLanguageSelection')}
							title={t('ViewRes:EntryEdit.DefaultLanguageHelp')}
						/>
					</div>
					<div className="editor-field">
						<LanguageSelectionDropdownList
							value={venueEditStore.defaultNameLanguage}
							onChange={(e): void =>
								runInAction(() => {
									venueEditStore.defaultNameLanguage = e.target.value;
								})
							}
						/>
					</div>

					<div className="editor-label">
						<label>Names{/* TODO: localize */}</label>
					</div>
					<div className="editor-field">
						<table>
							<tbody>
								<tr>
									<td>
										<NamesEditor namesEditStore={venueEditStore.names} />
									</td>
									<td style={{ verticalAlign: 'top' }}>
										{venueEditStore.duplicateName && (
											<Alert>
												Venue already exists with name{/* TODO: localize */}{' '}
												<span>{venueEditStore.duplicateName}</span>
											</Alert>
										)}
									</td>
								</tr>
							</tbody>
						</table>
					</div>

					<div className="editor-label">Description{/* TODO: localize */}</div>
					<div className="editor-field">
						<textarea
							value={venueEditStore.description}
							onChange={(e): void =>
								runInAction(() => {
									venueEditStore.description = e.target.value;
								})
							}
							cols={60}
							rows={4}
							maxLength={1000}
							className="span4"
						/>
						Live preview{/* TODO: localize */}
						<Markdown>{venueEditStore.description}</Markdown>
					</div>

					<table>
						<tbody>
							<tr>
								<td>{t('ViewRes.Venue:Edit.Coordinates')}</td>
								<td>
									<div className="editor-label">
										<label>{t('ViewRes.Venue:Edit.Latitude')}</label>
									</div>
									<div className="editor-field">
										<input
											type="number"
											value={venueEditStore.latitude}
											onChange={(e): void =>
												runInAction(() => {
													venueEditStore.latitude = Number(e.target.value);
												})
											}
											className="input-medium"
											min={-90}
											max={90}
											step={0.0000001}
										/>
									</div>
								</td>
								<td>
									<div className="editor-label">
										<label>{t('ViewRes.Venue:Edit.Longitude')}</label>
									</div>
									<div className="editor-field">
										<input
											type="number"
											value={venueEditStore.longitude}
											onChange={(e): void =>
												runInAction(() => {
													venueEditStore.longitude = Number(e.target.value);
												})
											}
											className="input-medium"
											min={-180}
											max={180}
											step={0.0000001}
										/>
									</div>
								</td>
							</tr>
						</tbody>
					</table>

					<div className="editor-label">
						<label>{t('ViewRes.Venue:Edit.Country')}</label>
					</div>
					<div className="editor-field">
						<RegionDropdownList
							value={venueEditStore.addressCountryCode}
							onChange={(e): void =>
								runInAction(() => {
									venueEditStore.addressCountryCode = e.target.value;
								})
							}
						/>
					</div>

					<div className="editor-label">
						<label>{t('ViewRes.Venue:Edit.Address')}</label>
					</div>
					<div className="editor-field">
						<input
							type="text"
							value={venueEditStore.address}
							onChange={(e): void =>
								runInAction(() => {
									venueEditStore.address = e.target.value;
								})
							}
							className="input-xlarge"
							maxLength={255}
						/>
					</div>

					<div className="editor-label">
						<HelpLabel
							label={t('ViewRes:EntryEdit.WebLinks')}
							title={t('ViewRes:EntryEdit.ExternalLinksQuickHelp')}
						/>
					</div>
					<div className="editor-field">
						<WebLinksEditViewKnockout
							webLinksEditStore={venueEditStore.webLinks}
						/>
					</div>

					<div className="editor-label">
						<HelpLabel
							label={t('ViewRes:EntryEdit.Status')}
							title={t('Resources:CommonMessages.EntryStatusExplanation')}
						/>
					</div>
					<div className="editor-field">
						<EntryStatusDropdownList
							allowedEntryStatuses={loginManager.allowedEntryStatuses()}
							value={venueEditStore.status}
							onChange={(e): void =>
								runInAction(() => {
									venueEditStore.status = e.target.value;
								})
							}
						/>
					</div>

					<br />
					<SaveAndBackBtn
						backAction={backAction}
						submitting={venueEditStore.submitting}
					/>
				</form>

				<EntryDeletePopup
					confirmText={t('ViewRes:EntryEdit.ConfirmDelete')}
					deleteEntryStore={venueEditStore.deleteStore}
					onDelete={(): void =>
						navigate(EntryUrlMapper.details(EntryType.Venue, contract.id))
					}
				/>
				<EntryTrashPopup
					confirmText={t('ViewRes:EntryEdit.ConfirmMoveToTrash')}
					deleteEntryStore={venueEditStore.trashStore}
					onDelete={(): void => {
						navigate('/Event');
					}}
				/>
			</Layout>
		);
	},
);

const defaultModel: VenueForEditContract = {
	address: '',
	addressCountryCode: '',
	deleted: false,
	defaultNameLanguage:
		ContentLanguageSelection[ContentLanguageSelection.Unspecified],
	description: '',
	id: 0,
	name: '',
	names: [],
	status: EntryStatus[EntryStatus.Draft],
	webLinks: [],
};

const VenueEdit = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<{
		venueEditStore: VenueEditStore;
	}>();

	React.useEffect(() => {
		if (id) {
			venueRepo
				.getForEdit({ id: Number(id) })
				.then((model) =>
					setModel({ venueEditStore: new VenueEditStore(venueRepo, model) }),
				)
				.catch((error) => {
					if (error.response) {
						if (error.response.status === 404)
							window.location.href = '/Error/NotFound';
					}

					throw error;
				});
		} else {
			setModel({ venueEditStore: new VenueEditStore(venueRepo, defaultModel) });
		}
	}, [id]);

	return model ? (
		<VenueEditLayout venueEditStore={model.venueEditStore} />
	) : (
		<></>
	);
};

export default VenueEdit;
