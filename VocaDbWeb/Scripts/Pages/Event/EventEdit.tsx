import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistAutoComplete } from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { NamesEditor } from '@/Components/Shared/KnockoutPartials/NamesEditor';
import { Layout } from '@/Components/Shared/Layout';
import { PVEdit } from '@/Components/Shared/PVs/PVEdit';
import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { ArtistRolesEditViewModel } from '@/Components/Shared/Partials/ArtistRolesEditViewModel';
import { EntryDeletePopup } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopup';
import { EntryTrashPopup } from '@/Components/Shared/Partials/EntryDetails/EntryTrashPopup';
import {
	LanguageSelectionDropdownList,
	ReleaseEventCategoryDropdownList,
	EntryStatusDropdownList,
} from '@/Components/Shared/Partials/Knockout/DropdownList';
import { ReleaseEventSeriesLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/ReleaseEventSeriesLockingAutoComplete';
import { SongListLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/SongListLockingAutoComplete';
import { VenueLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/VenueLockingAutoComplete';
import { WebLinksEditViewKnockout } from '@/Components/Shared/Partials/Knockout/WebLinksEditViewKnockout';
import { ConcurrentEditWarning } from '@/Components/Shared/Partials/Shared/ConcurrentEditWarning';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { ImageUploadMessage } from '@/Components/Shared/Partials/Shared/ImageUploadMessage';
import { SaveAndBackBtn } from '@/Components/Shared/Partials/Shared/SaveAndBackBtn';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { useConflictingEditor } from '@/Components/useConflictingEditor';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { ReleaseEventForEditContract } from '@/DataContracts/ReleaseEvents/ReleaseEventForEditContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import JQueryUIDatepicker from '@/JQueryUI/JQueryUIDatepicker';
import JQueryUITab from '@/JQueryUI/JQueryUITab';
import JQueryUITabs from '@/JQueryUI/JQueryUITabs';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { ArtistEventRoles } from '@/Models/Events/ArtistEventRoles';
import { EventCategory } from '@/Models/Events/EventCategory';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { ImageSize } from '@/Models/Images/ImageSize';
import { LoginManager } from '@/Models/LoginManager';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { PVRepository } from '@/Repositories/PVRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { VenueRepository } from '@/Repositories/VenueRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { ReleaseEventEditStore } from '@/Stores/ReleaseEvent/ReleaseEventEditStore';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import {
	Link,
	useNavigate,
	useParams,
	useSearchParams,
} from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const antiforgeryRepo = new AntiforgeryRepository(httpClient, urlMapper);
const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);
const pvRepo = new PVRepository(httpClient, urlMapper);
const songListRepo = new SongListRepository(httpClient, urlMapper);
const venueRepo = new VenueRepository(httpClient, urlMapper);

interface BasicInfoTabContentProps {
	releaseEventEditStore: ReleaseEventEditStore;
	pictureUploadRef: React.MutableRefObject<HTMLInputElement>;
}

const BasicInfoTabContent = observer(
	({
		releaseEventEditStore,
		pictureUploadRef,
	}: BasicInfoTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes']);

		return (
			<>
				<div>
					<div className="editor-label">Event type{/* TODO: localize */}</div>
					<div className="editor-field">
						<label>
							<input
								type="radio"
								checked={releaseEventEditStore.isSeriesEvent}
								onChange={(e): void =>
									runInAction(() => {
										releaseEventEditStore.isSeriesEvent = e.target.checked;
									})
								}
							/>{' '}
							Series event
							{/* TODO: localize */}
						</label>
						<label>
							<input
								type="radio"
								checked={!releaseEventEditStore.isSeriesEvent}
								onChange={(e): void =>
									runInAction(() => {
										releaseEventEditStore.isSeriesEvent = !e.target.checked;
									})
								}
							/>{' '}
							Standalone event
							{/* TODO: localize */}
						</label>
					</div>
				</div>

				{releaseEventEditStore.isSeriesEvent && (
					<div>
						<div className="editor-label">Series{/* TODO: localize */}</div>
						<div className="editor-field">
							<ReleaseEventSeriesLockingAutoComplete
								basicEntryLinkStore={releaseEventEditStore.series}
							/>
						</div>

						<div className="editor-label">
							<label htmlFor="seriesNumber">
								Series number{/* TODO: localize */}
							</label>
						</div>
						<div className="editor-field">
							<input
								id="seriesNumber"
								type="number"
								value={releaseEventEditStore.seriesNumber}
								onChange={(e): void =>
									runInAction(() => {
										releaseEventEditStore.seriesNumber = e.target.value;
									})
								}
								size={20}
							/>
						</div>

						<div className="editor-label" />
						<div className="editor-field">
							<label>
								<input
									type="checkbox"
									checked={releaseEventEditStore.customName}
									onChange={(e): void =>
										runInAction(() => {
											releaseEventEditStore.customName = e.target.checked;
										})
									}
								/>{' '}
								Customize event name
								{/* TODO: localize */}
							</label>
						</div>

						{!releaseEventEditStore.customName && (
							<div>
								<div className="editor-label">
									<label htmlFor="seriesSuffix">
										Series suffix{/* TODO: localize */}
									</label>
								</div>
								<div className="editor-field">
									<input
										id="seriesSuffix"
										type="text"
										value={releaseEventEditStore.seriesSuffix}
										onChange={(e): void =>
											runInAction(() => {
												releaseEventEditStore.seriesSuffix = e.target.value;
											})
										}
										size={20}
									/>
								</div>
							</div>
						)}
					</div>
				)}

				{(!releaseEventEditStore.isSeriesEvent ||
					releaseEventEditStore.customName) && (
					<div>
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
								value={releaseEventEditStore.defaultNameLanguage}
								onChange={(e): void =>
									runInAction(() => {
										releaseEventEditStore.defaultNameLanguage = e.target
											.value as ContentLanguageSelection;
									})
								}
							/>
						</div>

						<div className="editor-label">
							<label>Names{/* TODO: localize */}</label>
						</div>
						<div className="editor-field">
							<NamesEditor namesEditStore={releaseEventEditStore.names} />
						</div>
					</div>
				)}

				<div className="editor-label">
					<label htmlFor="description">Description{/* TODO: localize */}</label>
				</div>
				<div className="editor-field">
					<textarea
						id="description"
						value={releaseEventEditStore.description}
						onChange={(e): void =>
							runInAction(() => {
								releaseEventEditStore.description = e.target.value;
							})
						}
						cols={60}
						rows={4}
						className="span4"
						maxLength={1000}
					/>
					Live preview{/* TODO: localize */}
					<Markdown>{releaseEventEditStore.description}</Markdown>
				</div>

				{!releaseEventEditStore.isSeriesEvent && (
					<div>
						<div className="editor-label">Category{/* TODO: localize */}</div>
						<div className="editor-field">
							<div className="row-fluid">
								<div className="span4">
									<ReleaseEventCategoryDropdownList
										value={releaseEventEditStore.category}
										onChange={(e): void =>
											runInAction(() => {
												releaseEventEditStore.category = e.target
													.value as EventCategory;
											})
										}
									/>
								</div>
							</div>
						</div>
					</div>
				)}

				<div className="editor-label">
					<HelpLabel
						label="Date" /* TODO: localize */
						dangerouslySetInnerHTML={{
							__html:
								"Enter event begin date. For events lasting multiple days, end date can be entered as well. Both are optional, and end date does not need to be specified if it's the same as begin date." /* TODO: localize */,
						}}
					/>
				</div>
				<div className="editor-field">
					<JQueryUIDatepicker
						type="text"
						value={releaseEventEditStore.date}
						onSelect={(date): void =>
							runInAction(() => {
								releaseEventEditStore.date = date;
							})
						}
						dateFormat="yy-mm-dd"
						className="input-small"
						maxLength={10}
					/>{' '}
					to{' '}
					<JQueryUIDatepicker
						type="text"
						value={releaseEventEditStore.endDate}
						onSelect={(date): void =>
							runInAction(() => {
								releaseEventEditStore.endDate = date;
							})
						}
						dateFormat="yy-mm-dd"
						className="input-small"
						maxLength={10}
					/>
				</div>

				<div className="editor-label">
					<HelpLabel
						label="Setlist" /* TODO: localize */
						dangerouslySetInnerHTML={{
							__html:
								'If this event is a live performance such as concert or club event, a setlist of the songs performed can be specified here.' /* TODO: localize */,
						}}
					/>
				</div>
				<div className="editor-field">
					<SongListLockingAutoComplete
						basicEntryLinkStore={releaseEventEditStore.songList}
						songListCategory={SongListFeaturedCategory.Concerts}
					/>
				</div>

				<div className="editor-label">Venue{/* TODO: localize */}</div>
				<div className="editor-field">
					<VenueLockingAutoComplete
						basicEntryLinkStore={releaseEventEditStore.venue}
					/>
				</div>

				{!releaseEventEditStore.venue.entry && (
					<div>
						<div className="editor-label">
							<HelpLabel
								label="Venue name" /* TODO: localize */
								dangerouslySetInnerHTML={{
									__html:
										'Can be either a physical location such as concert hall, or a virtual location (website).' /* TODO: localize */,
								}}
							/>
						</div>
						<div className="editor-field">
							<input
								type="text"
								value={releaseEventEditStore.venueName}
								onChange={(e): void =>
									runInAction(() => {
										releaseEventEditStore.venueName = e.target.value;
									})
								}
								size={40}
								className="span3"
								maxLength={1000}
							/>
						</div>
					</div>
				)}

				<br />

				<div className="editor-label">
					<label>Picture{/* TODO: localize */}</label>
				</div>
				<div className="editor-field">
					<table>
						<tbody>
							<tr>
								<td>
									{/* eslint-disable-next-line jsx-a11y/img-redundant-alt */}
									<img
										src={UrlHelper.imageThumb(
											releaseEventEditStore.contract.mainPicture,
											ImageSize.SmallThumb,
										)}
										alt="Picture" /* TODO: localize */
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
						webLinksEditStore={releaseEventEditStore.webLinks}
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
						allowedEntryStatuses={loginManager.allowedEntryStatuses({
							id: releaseEventEditStore.contract.id,
							entryType: EntryType[EntryType.ReleaseEvent],
						})}
						value={releaseEventEditStore.status}
						onChange={(e): void =>
							runInAction(() => {
								releaseEventEditStore.status = e.target.value as EntryStatus;
							})
						}
					/>
				</div>
			</>
		);
	},
);

interface ArtistsTabContentProps {
	releaseEventEditStore: ReleaseEventEditStore;
}

const ArtistsTabContent = observer(
	({ releaseEventEditStore }: ArtistsTabContentProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Album',
			'VocaDb.Web.Resources.Domain.ReleaseEvents',
		]);

		return (
			<div className="row-fluid">
				<div className="span6">
					{releaseEventEditStore.artistLinks.length > 0 && (
						<table>
							<thead>
								<tr>
									<th>Artist{/* TODO: localize */}</th>
									<th>Roles{/* TODO: localize */}</th>
									<th>Actions{/* TODO: localize */}</th>
								</tr>
							</thead>
							<tbody>
								{releaseEventEditStore.artistLinks.map((artistLink, index) => (
									<tr key={index}>
										<td>
											{artistLink.artist ? (
												<div>
													<ArtistLink
														artist={artistLink.artist}
														tooltip={true}
														/* TODO: target="_blank" */
													/>
													<br />
													<span className="extraInfo">
														{artistLink.artist.additionalNames}
													</span>
												</div>
											) : (
												<div>
													<span>{artistLink.name}</span>
												</div>
											)}
										</td>
										<td>
											<div>
												{artistLink.rolesArray
													.filter(
														(role) =>
															role !==
															ArtistEventRoles[ArtistEventRoles.Default],
													)
													.map((role) => (
														<div key={role}>
															<span>
																{t(
																	`VocaDb.Web.Resources.Domain.ReleaseEvents:ArtistEventRoleNames.${role}`,
																)}
															</span>
															<br />
														</div>
													))}
											</div>
											<SafeAnchor
												href="#"
												className="artistRolesEdit textLink editLink"
												onClick={(): void =>
													releaseEventEditStore.editArtistRoles(artistLink)
												}
											>
												{t('ViewRes.Album:ArtistForAlbumEditRow.Customize')}
											</SafeAnchor>
										</td>
										<td>
											<SafeAnchor
												onClick={(): void =>
													releaseEventEditStore.removeArtist(artistLink)
												}
												href="#"
												className="textLink removeLink"
											>
												{t('ViewRes:Shared.Remove')}
											</SafeAnchor>
										</td>
									</tr>
								))}
							</tbody>
						</table>
					)}

					<br />
					<h4>Add artist{/* TODO: localize */}</h4>
					<ArtistAutoComplete
						type="text"
						properties={{
							createNewItem:
								"Add custom artist named '{0}'" /* TODO: localize */,
							acceptSelection: releaseEventEditStore.addArtist,
						}}
						maxLength={128}
						placeholder={t('ViewRes:Shared.Search')}
						className="input-xlarge"
					/>
				</div>
			</div>
		);
	},
);

interface PVsTabContentProps {
	releaseEventEditStore: ReleaseEventEditStore;
}

const PVsTabContent = observer(
	({ releaseEventEditStore }: PVsTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<>
				{releaseEventEditStore.pvs.pvs.length > 0 && (
					<table>
						<thead>
							<tr>
								<th>Service{/* TODO: localize */}</th>
								<th>Type{/* TODO: localize */}</th>
								<th>Name{/* TODO: localize */}</th>
								<th>Length{/* TODO: localize */}</th>
								<th>Date{/* TODO: localize */}</th>
								<th>Author{/* TODO: localize */}</th>
								<th />
							</tr>
						</thead>
						<tbody>
							{releaseEventEditStore.pvs.pvs.map((pv, index) => (
								<PVEdit
									pvListEditStore={releaseEventEditStore.pvs}
									pvEditStore={pv}
									key={index}
								/>
							))}
						</tbody>
					</table>
				)}

				<br />
				<h4>Add media{/* TODO: localize */}</h4>

				<p>
					Supported services: YouTube, NicoNicoDouga, Vimeo, SoundCloud, Piapro
					and Bilibili.{/* TODO: localize */}
				</p>
				<p>
					URL:{' '}
					<input
						type="text"
						value={releaseEventEditStore.pvs.newPvUrl}
						onChange={(e): void =>
							runInAction(() => {
								releaseEventEditStore.pvs.newPvUrl = e.target.value;
							})
						}
						maxLength={255}
						size={40}
						className="input-xlarge"
					/>
				</p>

				<SafeAnchor
					onClick={releaseEventEditStore.pvs.add}
					href="#"
					className="textLink addLink"
				>
					{t('ViewRes:Shared.Add')}
				</SafeAnchor>

				{/* TODO: AjaxLoader */}
			</>
		);
	},
);

interface EventEditLayoutProps {
	releaseEventEditStore: ReleaseEventEditStore;
}

const EventEditLayout = observer(
	({ releaseEventEditStore }: EventEditLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const contract = releaseEventEditStore.contract;
		const isNew = contract.id === 0;

		const title = isNew
			? 'Create a new event' /* TODO: localize */
			: `Edit event - ${contract.name}`; /* TODO: localize */

		useVocaDbTitle(title, true);

		const backAction = isNew
			? '/Event'
			: EntryUrlMapper.details(EntryType.ReleaseEvent, contract.id);

		const navigate = useNavigate();

		const conflictingEditor = useConflictingEditor(EntryType.ReleaseEvent);

		const pictureUploadRef = React.useRef<HTMLInputElement>(undefined!);

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
								Events{/* TODO: localize */}
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
								Events{/* TODO: localize */}
							</Breadcrumb.Item>
							<Breadcrumb.Item
								linkAs={Link}
								linkProps={{
									to: EntryUrlMapper.details(
										EntryType.ReleaseEvent,
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
									href={`/Event/Restore/${contract.id}`}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:EntryEdit.Restore')}
								</JQueryUIButton>
							) : (
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={releaseEventEditStore.deleteStore.show}
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
										onClick={releaseEventEditStore.trashStore.show}
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

				{releaseEventEditStore.errors && (
					<ValidationSummaryPanel
						message="Unable to save properties." /* TODO: localize */
						errors={releaseEventEditStore.errors}
					/>
				)}

				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const pictureUpload =
								pictureUploadRef.current.files?.item(0) ?? undefined;

							const id = await releaseEventEditStore.submit(
								requestToken,
								pictureUpload,
							);

							navigate(EntryUrlMapper.details(EntryType.ReleaseEvent, id));
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save properties.' /* TODO: localize */,
							);

							throw error;
						}
					}}
				>
					<SaveAndBackBtn
						backAction={backAction}
						submitting={releaseEventEditStore.submitting}
					/>

					<JQueryUITabs>
						<JQueryUITab
							eventKey="basicInfo"
							title={t('ViewRes:EntryEdit.BasicInfo')}
						>
							<BasicInfoTabContent
								releaseEventEditStore={releaseEventEditStore}
								pictureUploadRef={pictureUploadRef}
							/>
						</JQueryUITab>

						<JQueryUITab
							eventKey="artists"
							title="Artists" /* TODO: localize */
						>
							<ArtistsTabContent
								releaseEventEditStore={releaseEventEditStore}
							/>
						</JQueryUITab>

						<JQueryUITab eventKey="pvs" title="Media" /* TODO: localize */>
							<PVsTabContent releaseEventEditStore={releaseEventEditStore} />
						</JQueryUITab>
					</JQueryUITabs>

					<br />
					<SaveAndBackBtn
						backAction={backAction}
						submitting={releaseEventEditStore.submitting}
					/>
				</form>

				<div>
					<ArtistRolesEditViewModel
						artistRolesEditStore={releaseEventEditStore.artistRolesEditStore}
					/>
				</div>

				<EntryDeletePopup
					confirmText={t('ViewRes:EntryEdit.ConfirmDelete')}
					deleteEntryStore={releaseEventEditStore.deleteStore}
					onDelete={(): void =>
						navigate(
							EntryUrlMapper.details(EntryType.ReleaseEvent, contract.id),
						)
					}
				/>
				<EntryTrashPopup
					confirmText={t('ViewRes:EntryEdit.ConfirmMoveToTrash')}
					deleteEntryStore={releaseEventEditStore.trashStore}
					onDelete={(): void => navigate('/Event')}
				/>
			</Layout>
		);
	},
);

const defaultModel: ReleaseEventForEditContract = {
	artists: [],
	category: EventCategory.Unspecified,
	customName: false,
	defaultNameLanguage: ContentLanguageSelection.Unspecified,
	deleted: false,
	description: '',
	id: 0,
	name: '',
	names: [],
	pvs: [],
	seriesNumber: 0,
	seriesSuffix: '',
	status: EntryStatus.Draft,
	venueName: '',
	webLinks: [],
};

const EventEdit = (): React.ReactElement => {
	const { t } = useTranslation(['VocaDb.Web.Resources.Domain.ReleaseEvents']);

	const artistRoleNames = React.useMemo(
		() =>
			Object.fromEntries(
				Object.values(ArtistEventRoles)
					.filter((artistRole) => isNaN(Number(artistRole)))
					.map((artistRole): [string, string | undefined] => [
						artistRole as string,
						t(`VocaDb.Web.Resources.Domain.ReleaseEvents:${artistRole}`),
					]),
			),
		[t],
	);

	const { id } = useParams();
	const [searchParams] = useSearchParams();
	const seriesId = searchParams.get('seriesId');
	const venueId = searchParams.get('venueId');

	const [model, setModel] = React.useState<{
		releaseEventEditStore: ReleaseEventEditStore;
	}>();

	React.useEffect(() => {
		if (id) {
			eventRepo
				.getForEdit({ id: Number(id) })
				.then((model) =>
					setModel({
						releaseEventEditStore: new ReleaseEventEditStore(
							vdb.values,
							eventRepo,
							artistRepo,
							pvRepo,
							songListRepo,
							venueRepo,
							urlMapper,
							artistRoleNames,
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
				releaseEventEditStore: new ReleaseEventEditStore(
					vdb.values,
					eventRepo,
					artistRepo,
					pvRepo,
					songListRepo,
					venueRepo,
					urlMapper,
					artistRoleNames,
					{
						...defaultModel,
						series: seriesId ? { id: Number(seriesId) } : undefined,
						venue: venueId ? { id: Number(venueId) } : undefined,
					},
				),
			});
		}
	}, [artistRoleNames, id, seriesId, venueId]);

	return model ? (
		<EventEditLayout releaseEventEditStore={model.releaseEventEditStore} />
	) : (
		<></>
	);
};

export default EventEdit;
