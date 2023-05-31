import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { SongAutoComplete } from '@/Components/KnockoutExtensions/SongAutoComplete';
import { Layout } from '@/Components/Shared/Layout';
import { EntryDeletePopup } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopup';
import { EntryTrashPopup } from '@/Components/Shared/Partials/EntryDetails/EntryTrashPopup';
import {
	SongListFeaturedCategoryDropdownList,
	EntryStatusDropdownList,
} from '@/Components/Shared/Partials/Knockout/DropdownList';
import { ConcurrentEditWarning } from '@/Components/Shared/Partials/Shared/ConcurrentEditWarning';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { ImageUploadMessage } from '@/Components/Shared/Partials/Shared/ImageUploadMessage';
import { MarkdownNotice } from '@/Components/Shared/Partials/Shared/MarkdownNotice';
import { SaveAndBackBtn } from '@/Components/Shared/Partials/Shared/SaveAndBackBtn';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { useConflictingEditor } from '@/Components/useConflictingEditor';
import { SongListForEditContract } from '@/DataContracts/Song/SongListForEditContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import JQueryUIDatepicker from '@/JQueryUI/JQueryUIDatepicker';
import JQueryUITab from '@/JQueryUI/JQueryUITab';
import JQueryUITabs from '@/JQueryUI/JQueryUITabs';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { songListRepo } from '@/Repositories/SongListRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import {
	CsvData,
	SongInListEditStore,
	SongListEditStore,
} from '@/Stores/SongList/SongListEditStore';
import { useVdb } from '@/VdbContext';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React, { useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { ReactSortable } from 'react-sortablejs';

interface PropertiesTabContentProps {
	songListEditStore: SongListEditStore;
	thumbPicUploadRef: React.MutableRefObject<HTMLInputElement>;
}

const PropertiesTabContent = observer(
	({
		songListEditStore,
		thumbPicUploadRef,
	}: PropertiesTabContentProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.SongList']);

		const thumbUrl = UrlHelper.imageThumb(
			songListEditStore.contract.mainPicture,
			ImageSize.SmallThumb,
			false,
		);

		return (
			<>
				<div className="editor-label">{t('ViewRes.SongList:Edit.Name')}</div>
				<div className="editor-field">
					<input
						type="text"
						value={songListEditStore.name}
						onChange={(e): void =>
							runInAction(() => {
								songListEditStore.name = e.target.value;
							})
						}
						className="required input-xxlarge"
						size={200}
						required
					/>
				</div>

				<div className="editor-label">
					{t('ViewRes.SongList:Edit.Description')} <MarkdownNotice />
				</div>
				<div className="editor-field">
					<textarea
						value={songListEditStore.description}
						onChange={(e): void =>
							runInAction(() => {
								songListEditStore.description = e.target.value;
							})
						}
						rows={6}
						cols={60}
						maxLength={3000}
						className="input-xxlarge"
					/>

					<div>
						{t('ViewRes:EntryEdit.LivePreview')}
						<Markdown>{songListEditStore.description}</Markdown>
					</div>
				</div>

				{loginManager.canEditFeaturedLists && (
					<>
						<div className="editor-label">
							{t('ViewRes.SongList:Edit.FeaturedCategory')}
						</div>
						<div className="editor-field">
							<SongListFeaturedCategoryDropdownList
								value={songListEditStore.featuredCategory}
								onChange={(e): void =>
									runInAction(() => {
										songListEditStore.featuredCategory = e.target
											.value as SongListFeaturedCategory;
									})
								}
							/>
						</div>
					</>
				)}

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.SongList:Edit.EventDate')}
						dangerouslySetInnerHTML={{
							__html: t('ViewRes.SongList:Edit.EventDateHelp'),
						}}
					/>
				</div>
				<div className="editor-field">
					<JQueryUIDatepicker
						type="text"
						value={songListEditStore.eventDateDate}
						onSelect={(date): void =>
							runInAction(() => {
								songListEditStore.eventDateDate = date;
							})
						}
						dateFormat="yy-mm-dd"
						className="span2"
						maxLength={10}
					/>
				</div>

				{loginManager.canViewCoverArtImages && (
					<>
						<div className="editor-label">
							{t('ViewRes.SongList:Edit.Thumbnail')}
						</div>
						<div className="editor-field">
							<div className="media">
								{thumbUrl && (
									<img
										className="pull-left media-object"
										src={thumbUrl}
										alt="Thumb" /* LOC */
									/>
								)}
								<div className="media-body">
									<ImageUploadMessage />
									<input
										type="file"
										id="thumbPicUpload"
										name="thumbPicUpload"
										ref={thumbPicUploadRef}
									/>
								</div>
							</div>
						</div>
					</>
				)}

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.SongList:Edit.Status')}
						dangerouslySetInnerHTML={{
							__html: t('Resources:CommonMessages.EntryStatusExplanation'),
						}}
					/>
				</div>
				<div className="editor-field">
					<EntryStatusDropdownList
						allowedEntryStatuses={[EntryStatus.Draft, EntryStatus.Finished]}
						value={songListEditStore.status}
						onChange={(e): void =>
							runInAction(() => {
								songListEditStore.status = e.target.value as EntryStatus;
							})
						}
					/>
				</div>
			</>
		);
	},
);

interface SongListDifference {
	songsAdded: number;
	songsRemoved: number;
	songsUpdated: number;
}

const calcCsvDifference = (
	store: SongListEditStore,
	csvData: CsvData[] | null,
): SongListDifference => {
	let difference: SongListDifference = {
		songsAdded: 0,
		songsRemoved: 0,
		songsUpdated: 0,
	};

	if (!csvData) {
		return difference;
	}

	const previousSongs = store.songLinks.reduce(
		(map: { [id: number]: SongInListEditStore }, obj: SongInListEditStore) => {
			map[obj.song.id] = obj;
			return map;
		},
		{},
	);

	const newSongs = csvData.reduce(
		(map: { [id: number]: CsvData }, obj: CsvData) => {
			map[obj.id] = obj;
			return map;
		},
		{},
	);

	difference.songsAdded = csvData.filter(
		(s) => !(s.id in previousSongs),
	).length;
	difference.songsRemoved = Object.keys(previousSongs).filter(
		(s) => !(s in newSongs),
	).length;
	difference.songsUpdated = Object.keys(previousSongs).filter((s) => {
		const id = Number(s);
		return (
			s in newSongs &&
			(newSongs[id].notes !== previousSongs[id].notes ||
				newSongs[id].order !== previousSongs[id].order)
		);
	}).length;

	return difference;
};

interface CsvDifferenceAlertProps {
	store: SongListEditStore;
	data: CsvData[] | null;
}

const CsvDifferenceAlert = ({
	store,
	data,
}: CsvDifferenceAlertProps): React.ReactElement => {
	const diff = calcCsvDifference(store, data);
	const pluralize = (count: number, suffix = 's'): string =>
		`${count} Song${count !== 1 ? suffix : ''}`;

	if (data === null) {
		return <></>;
	}

	if (diff.songsAdded + diff.songsRemoved + diff.songsUpdated === 0) {
		return <Alert variant="success">No changes</Alert>;
	}

	return (
		<Alert variant="warning">
			<b>CSV update statistics:</b>
			<ul>
				<li>{pluralize(diff.songsAdded)} added</li>
				<li>{pluralize(diff.songsRemoved)} removed</li>
				<li>{pluralize(diff.songsUpdated)} updated</li>
			</ul>
		</Alert>
	);
};

interface SongsTabContentProps {
	songListEditStore: SongListEditStore;
}

const SongsTabContent = observer(
	({ songListEditStore }: SongsTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.SongList']);
		const fileInputRef = useRef<HTMLInputElement | null>(null);
		const [csvData, setCsvData] = useState<string | null>(null);

		const parsedCsvData = !csvData
			? null
			: csvData
					.split('\n')
					.map((r) => r.split(','))
					.slice(1)
					.map((r) => ({
						id: Number(r[0]),
						order: Number(r[1]),
						notes: r[2],
					}))
					.filter((r) => !Number.isNaN(r.id) && !Number.isNaN(r.order));

		const applyCsv = (): void => {
			if (!csvData || !parsedCsvData) {
				return;
			}

			songListEditStore.importCsvData(parsedCsvData);
		};

		return (
			<>
				<div>
					<input
						onChange={(e): void => {
							if (e.target.files === null) {
								return;
							}
							let reader = new FileReader();
							reader.readAsText(e.target.files[0]);
							reader.onloadend = (): void => {
								setCsvData(reader.result as string);
							};
						}}
						type="file"
						ref={fileInputRef}
						accept=".csv"
						hidden
					/>
					<Button onClick={(): void => fileInputRef.current?.click()}>
						{t('ViewRes:Shared.ImportCsv')}
					</Button>
					{csvData && (
						<>
							{' '}
							<Button variant="primary" onClick={(): void => applyCsv()}>
								{t('ViewRes:Shared.ApplyCsv')}
							</Button>
						</>
					)}
				</div>
				<br />
				<CsvDifferenceAlert store={songListEditStore} data={parsedCsvData} />
				<br />
				<table>
					{songListEditStore.csvData && (
						<ReactSortable
							tag="tbody"
							list={songListEditStore.csvData}
							setList={(csvData): void =>
								runInAction(() => {
									console.log(csvData);
									songListEditStore.csvData = csvData;
								})
							}
							handle=".handle"
						>
							{songListEditStore.csvData.map((song, index) => (
								<tr className="ui-state-default" key={index}>
									<td style={{ cursor: 'move' }} className="handle">
										<span className="ui-icon ui-icon-arrowthick-2-n-s" />
									</td>
									<td>
										<span>{song.order}</span>
									</td>
									<td>
										<span title="SongId">{'ID: ' + song.id}</span>
									</td>
									<td>
										<input
											type="text"
											value={song.notes}
											onChange={(e): void =>
												runInAction(() => {
													song.notes = e.target.value;
												})
											}
											maxLength={200}
										/>
									</td>
									{/* <td>
										<SafeAnchor
											href="#"
											className="iconLink removeLink"
											title={t('ViewRes.SongList:Edit.RemoveFromList')}
											onClick={(): void =>
												songListEditStore.removeSong(songLink)
											}
										>
											{t('ViewRes:Shared.Remove')}
										</SafeAnchor>
									</td> */}
								</tr>
							))}
						</ReactSortable>
					)}
					{!songListEditStore.csvData && (
						<ReactSortable
							tag="tbody"
							list={songListEditStore.songLinks}
							setList={(songLinks): void =>
								runInAction(() => {
									songListEditStore.songLinks = songLinks;
								})
							}
							handle=".handle"
						>
							{songListEditStore.songLinks.map((songLink, index) => (
								<tr className="ui-state-default" key={index}>
									<td style={{ cursor: 'move' }} className="handle">
										<span className="ui-icon ui-icon-arrowthick-2-n-s" />
									</td>
									<td>
										<span>{songLink.order}</span>
									</td>
									<td>
										<span title={songLink.song.additionalNames}>
											{songLink.song.name}
										</span>
										<br />
										<span className="extraInfo">
											{songLink.song.artistString}
										</span>
									</td>
									<td>
										<input
											type="text"
											value={songLink.notes}
											onChange={(e): void =>
												runInAction(() => {
													songLink.notes = e.target.value;
												})
											}
											maxLength={200}
										/>
									</td>
									<td>
										<SafeAnchor
											href="#"
											className="iconLink removeLink"
											title={t('ViewRes.SongList:Edit.RemoveFromList')}
											onClick={(): void =>
												songListEditStore.removeSong(songLink)
											}
										>
											{t('ViewRes:Shared.Remove')}
										</SafeAnchor>
									</td>
								</tr>
							))}
						</ReactSortable>
					)}
				</table>

				<br />

				<h4>{t('ViewRes.SongList:Edit.AddSong')}</h4>
				<SongAutoComplete
					type="text"
					properties={{
						acceptSelection: songListEditStore.acceptSongSelection,
					}}
					maxLength={128}
					placeholder={t('ViewRes:Shared.Search')}
					className="input-xlarge"
				/>
			</>
		);
	},
);

interface SongListEditLayoutProps {
	songListEditStore: SongListEditStore;
}

const SongListEditLayout = observer(
	({ songListEditStore }: SongListEditLayoutProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.SongList']);

		const contract = songListEditStore.contract;
		const isNew = contract.id === 0;
		const parentUrl = isNew
			? '/SongList/Featured'
			: EntryUrlMapper.details(EntryType.SongList, contract.id);

		const title = isNew
			? t('ViewRes.SongList:Edit.CreateTitle')
			: t(`ViewRes.SongList:Edit.EditTitle`, {
					0: contract.name,
			  });

		const conflictingEditor = useConflictingEditor(EntryType.SongList);

		const navigate = useNavigate();

		const thumbPicUploadRef = React.useRef<HTMLInputElement>(undefined!);

		return (
			<Layout
				pageTitle={title}
				ready={ready}
				title={title}
				parents={
					!isNew && (
						<>
							<Breadcrumb.Item
								linkAs={Link}
								linkProps={{
									to: EntryUrlMapper.details(
										EntryType.SongList,
										songListEditStore.contract.id,
									),
								}}
							>
								{songListEditStore.contract.name}
							</Breadcrumb.Item>
						</>
					)
				}
				toolbar={
					!isNew && (
						<>
							{!songListEditStore.contract.deleted && (
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									id="deleteLink"
									onClick={(): void =>
										runInAction(() => {
											songListEditStore.deleteStore.dialogVisible = true;
										})
									}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:Shared.Delete')}
								</JQueryUIButton>
							)}{' '}
							<JQueryUIButton
								as={SafeAnchor}
								href="#"
								id="trashLink"
								onClick={(): void =>
									runInAction(() => {
										songListEditStore.trashStore.dialogVisible = true;
									})
								}
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

				{songListEditStore.errors && (
					<ValidationSummaryPanel
						message="Unable to save properties." /* LOC */
						errors={songListEditStore.errors}
					/>
				)}

				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const thumbPicUpload = loginManager.canViewCoverArtImages
								? thumbPicUploadRef.current.files?.item(0) ?? undefined
								: undefined;

							const id = await songListEditStore.submit(
								requestToken,
								thumbPicUpload,
							);

							navigate(EntryUrlMapper.details(EntryType.SongList, id));
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
						backAction={parentUrl}
						submitting={songListEditStore.submitting}
					/>

					<JQueryUITabs>
						<JQueryUITab
							eventKey="properties"
							title={t('ViewRes.SongList:Edit.TabProperties')}
						>
							<PropertiesTabContent
								songListEditStore={songListEditStore}
								thumbPicUploadRef={thumbPicUploadRef}
							/>
						</JQueryUITab>

						<JQueryUITab
							eventKey="songs"
							title={t('ViewRes.SongList:Edit.TabSongs')}
						>
							<SongsTabContent songListEditStore={songListEditStore} />
						</JQueryUITab>
					</JQueryUITabs>

					<br />
					<p>{t('ViewRes:EntryEdit.UpdateNotes')}</p>
					<textarea
						value={songListEditStore.updateNotes}
						onChange={(e): void =>
							runInAction(() => {
								songListEditStore.updateNotes = e.target.value;
							})
						}
						className="input-xxlarge"
						rows={4}
						maxLength={200}
					/>

					<br />
					<SaveAndBackBtn
						backAction={parentUrl}
						submitting={songListEditStore.submitting}
					/>
				</form>

				<EntryDeletePopup
					confirmText={t('ViewRes:EntryEdit.ConfirmDelete')}
					deleteEntryStore={songListEditStore.deleteStore}
					onDelete={(): void =>
						navigate(EntryUrlMapper.details(EntryType.SongList, contract.id))
					}
				/>
				<EntryTrashPopup
					confirmText={t('ViewRes:EntryEdit.ConfirmMoveToTrash')}
					deleteEntryStore={songListEditStore.trashStore}
					onDelete={(): void => navigate('/SongList/Featured')}
				/>
			</Layout>
		);
	},
);

const defaultModel: SongListForEditContract = {
	author: { id: 0 },
	name: '',
	description: '',
	featuredCategory: SongListFeaturedCategory.Nothing /* TODO: enum */,
	id: 0,
	songLinks: [],
	status: EntryStatus.Draft,
};

const SongListEdit = (): React.ReactElement => {
	const vdb = useVdb();

	const { id } = useParams();

	const [model, setModel] = React.useState<{
		songListEditStore: SongListEditStore;
	}>();

	React.useEffect(() => {
		if (id) {
			songListRepo
				.getForEdit({ id: Number(id) })
				.then((model) =>
					setModel({
						songListEditStore: new SongListEditStore(
							vdb.values,
							antiforgeryRepo,
							songListRepo,
							songRepo,
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
				songListEditStore: new SongListEditStore(
					vdb.values,
					antiforgeryRepo,
					songListRepo,
					songRepo,
					defaultModel,
				),
			});
		}
	}, [vdb, id]);

	return model ? (
		<SongListEditLayout songListEditStore={model.songListEditStore} />
	) : (
		<></>
	);
};

export default SongListEdit;
