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
	ReleaseEventCategoryDropdownList,
	EntryStatusDropdownList,
} from '@/Components/Shared/Partials/Knockout/DropdownList';
import { WebLinksEditViewKnockout } from '@/Components/Shared/Partials/Knockout/WebLinksEditViewKnockout';
import { ConcurrentEditWarning } from '@/Components/Shared/Partials/Shared/ConcurrentEditWarning';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { ImageUploadMessage } from '@/Components/Shared/Partials/Shared/ImageUploadMessage';
import { SaveAndBackBtn } from '@/Components/Shared/Partials/Shared/SaveAndBackBtn';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { useConflictingEditor } from '@/Components/useConflictingEditor';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { ReleaseEventSeriesForEditContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesForEditContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { EventCategory } from '@/Models/Events/EventCategory';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { ImageSize } from '@/Models/Images/ImageSize';
import { loginManager } from '@/Models/LoginManager';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { ReleaseEventSeriesEditStore } from '@/Stores/ReleaseEvent/ReleaseEventSeriesEditStore';
import { getReasonPhrase } from 'http-status-codes';
import { debounce } from 'lodash-es';
import { reaction, runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

interface EventEditSeriesLayoutProps {
	releaseEventSeriesEditStore: ReleaseEventSeriesEditStore;
}

const EventEditSeriesLayout = observer(
	({
		releaseEventSeriesEditStore,
	}: EventEditSeriesLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes']);

		const contract = releaseEventSeriesEditStore.contract;
		const isNew = contract.id === 0;
		const backAction = isNew
			? '/Event'
			: EntryUrlMapper.details(EntryType.ReleaseEventSeries, contract.id);

		const title = isNew
			? 'Create a new series' /* LOC */
			: `Edit series - ${contract.name}`; /* LOC */

		useVdbTitle(title, true);

		const navigate = useNavigate();

		const conflictingEditor = useConflictingEditor(
			EntryType.ReleaseEventSeries,
		);

		const pictureUploadRef = React.useRef<HTMLInputElement>(undefined!);

		React.useEffect(() => {
			if (releaseEventSeriesEditStore.contract.id) return;

			const disposers = [
				releaseEventSeriesEditStore.names.originalName,
				releaseEventSeriesEditStore.names.romajiName,
				releaseEventSeriesEditStore.names.englishName,
			].map((name) =>
				reaction(
					() => name.value,
					debounce(releaseEventSeriesEditStore.checkName, 500),
				),
			);

			return (): void => {
				for (const disposer of disposers) disposer();
			};
		}, [releaseEventSeriesEditStore]);

		return (
			<Layout
				title={title}
				parents={
					isNew ? (
						<>
							<Breadcrumb.Item
								linkAs={Link}
								linkProps={{
									to: '/Event',
								}}
							>
								Events{/* LOC */}
							</Breadcrumb.Item>
						</>
					) : (
						<>
							<Breadcrumb.Item
								linkAs={Link}
								linkProps={{
									to: '/Event',
								}}
								divider
							>
								Events{/* LOC */}
							</Breadcrumb.Item>
							<Breadcrumb.Item
								linkAs={Link}
								linkProps={{
									to: EntryUrlMapper.details(
										EntryType.ReleaseEventSeries,
										contract.id,
									),
								}}
							>
								{contract.name}
							</Breadcrumb.Item>
						</>
					)
				}
				toolbar={
					!isNew &&
					loginManager.canDeleteEntries && (
						<>
							{contract.deleted ? (
								<JQueryUIButton
									as="a"
									href={`/Event/RestoreSeries/${contract.id}`}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:EntryEdit.Restore')}
								</JQueryUIButton>
							) : (
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={releaseEventSeriesEditStore.deleteStore.show}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:Shared.Delete')}
								</JQueryUIButton>
							)}
							{loginManager.canMoveToTrash && (
								<>
									{' '}
									<JQueryUIButton
										as={SafeAnchor}
										href="#"
										onClick={releaseEventSeriesEditStore.trashStore.show}
										icons={{ primary: 'ui-icon-trash' }}
									>
										{t('ViewRes:EntryEdit.MoveToTrash')}
									</JQueryUIButton>
								</>
							)}
						</>
					)
				}
			>
				{conflictingEditor && conflictingEditor.userId !== 0 && (
					<ConcurrentEditWarning conflictingEditor={conflictingEditor} />
				)}

				{releaseEventSeriesEditStore.errors && (
					<ValidationSummaryPanel
						message="Unable to save properties." /* LOC */
						errors={releaseEventSeriesEditStore.errors}
					/>
				)}

				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const pictureUpload =
								pictureUploadRef.current.files?.item(0) ?? undefined;

							const id = await releaseEventSeriesEditStore.submit(
								requestToken,
								pictureUpload,
							);

							navigate(
								EntryUrlMapper.details(EntryType.ReleaseEventSeries, id),
							);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save properties.' /* LOC */,
							);

							throw error;
						}
					}}
				>
					<SaveAndBackBtn
						backAction={backAction}
						submitting={releaseEventSeriesEditStore.submitting}
					/>

					<div className="editor-label">
						<HelpLabel
							label={t('ViewRes:EntryEdit.DefaultLanguageSelection')}
							dangerouslySetInnerHTML={{
								__html: t('ViewRes:EntryEdit.DefaultLanguageHelp'),
							}}
						/>
					</div>
					<div className="editor-field">
						<LanguageSelectionDropdownList
							value={releaseEventSeriesEditStore.defaultNameLanguage}
							onChange={(e): void =>
								runInAction(() => {
									releaseEventSeriesEditStore.defaultNameLanguage = e.target
										.value as ContentLanguageSelection;
								})
							}
						/>
					</div>

					<div className="editor-label">
						<label>Names{/* LOC */}</label>
					</div>
					<div className="editor-field">
						<table>
							<tbody>
								<tr>
									<td>
										<NamesEditor
											namesEditStore={releaseEventSeriesEditStore.names}
										/>
									</td>
									<td style={{ verticalAlign: 'top' }}>
										{releaseEventSeriesEditStore.duplicateName && (
											<Alert>
												Series already exists with name{/* LOC */}{' '}
												<span>{releaseEventSeriesEditStore.duplicateName}</span>
											</Alert>
										)}
									</td>
								</tr>
							</tbody>
						</table>
					</div>

					<div className="editor-label">
						<label htmlFor="description">Description{/* LOC */}</label>
					</div>
					<div className="editor-field">
						<textarea
							id="description"
							value={releaseEventSeriesEditStore.description}
							onChange={(e): void =>
								runInAction(() => {
									releaseEventSeriesEditStore.description = e.target.value;
								})
							}
							cols={60}
							rows={4}
							maxLength={4000}
							className="span4"
						/>
						Live preview{/* LOC */}
						<Markdown>{releaseEventSeriesEditStore.description}</Markdown>
					</div>

					<div className="editor-label">
						<label>Category{/* LOC */}</label>
					</div>
					<div className="editor-field">
						<div className="row-fluid">
							<div className="span4">
								<ReleaseEventCategoryDropdownList
									value={releaseEventSeriesEditStore.category}
									onChange={(e): void =>
										runInAction(() => {
											releaseEventSeriesEditStore.category = e.target
												.value as EventCategory;
										})
									}
								/>
							</div>
						</div>
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
						<HelpLabel
							label={t('ViewRes:EntryEdit.WebLinks')}
							dangerouslySetInnerHTML={{
								__html: t('ViewRes:EntryEdit.ExternalLinksQuickHelp'),
							}}
						/>
					</div>
					<div className="editor-field">
						<WebLinksEditViewKnockout
							webLinksEditStore={releaseEventSeriesEditStore.webLinks}
						/>
					</div>

					<div className="editor-label">
						<HelpLabel
							label={t('ViewRes:EntryEdit.Status')}
							dangerouslySetInnerHTML={{
								__html: t('Resources:CommonMessages.EntryStatusExplanation'),
							}}
						/>
					</div>
					<div className="editor-field">
						<EntryStatusDropdownList
							allowedEntryStatuses={loginManager.allowedEntryStatuses()}
							value={releaseEventSeriesEditStore.status}
							onChange={(e): void =>
								runInAction(() => {
									releaseEventSeriesEditStore.status = e.target
										.value as EntryStatus;
								})
							}
						/>
					</div>

					<br />
					<SaveAndBackBtn
						backAction={backAction}
						submitting={releaseEventSeriesEditStore.submitting}
					/>
				</form>

				<EntryDeletePopup
					confirmText={t('ViewRes:EntryEdit.ConfirmDelete')}
					deleteEntryStore={releaseEventSeriesEditStore.deleteStore}
					onDelete={(): void =>
						navigate(
							EntryUrlMapper.details(EntryType.ReleaseEventSeries, contract.id),
						)
					}
				/>
				<EntryTrashPopup
					confirmText={t('ViewRes:EntryEdit.ConfirmMoveToTrash')}
					deleteEntryStore={releaseEventSeriesEditStore.trashStore}
					onDelete={(): void => navigate('/Event')}
				/>
			</Layout>
		);
	},
);

const defaultModel: ReleaseEventSeriesForEditContract = {
	category: EventCategory.Unspecified,
	defaultNameLanguage: ContentLanguageSelection.Unspecified,
	deleted: false,
	description: '',
	id: 0,
	name: '',
	names: [],
	status: EntryStatus.Draft,
	webLinks: [],
};

const EventEditSeries = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<{
		releaseEventSeriesEditStore: ReleaseEventSeriesEditStore;
	}>();

	React.useEffect(() => {
		if (id) {
			eventRepo
				.getSeriesForEdit({ id: Number(id) })
				.then((model) =>
					setModel({
						releaseEventSeriesEditStore: new ReleaseEventSeriesEditStore(
							eventRepo,
							model,
						),
					}),
				)
				.catch((error) => {
					if (error.response) {
						if (error.response.status === 404)
							window.location.href = '/Error/NotFound';
					}

					throw error;
				});
		} else {
			setModel({
				releaseEventSeriesEditStore: new ReleaseEventSeriesEditStore(
					eventRepo,
					defaultModel,
				),
			});
		}
	}, [id]);

	return model ? (
		<EventEditSeriesLayout
			releaseEventSeriesEditStore={model.releaseEventSeriesEditStore}
		/>
	) : (
		<></>
	);
};

export default EventEditSeries;
