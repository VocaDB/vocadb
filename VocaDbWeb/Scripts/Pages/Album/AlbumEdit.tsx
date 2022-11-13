import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistAutoComplete } from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { SongAutoComplete } from '@/Components/KnockoutExtensions/SongAutoComplete';
import { EntryPictureFileEdit } from '@/Components/Shared/KnockoutPartials/EntryPictureFileEdit';
import { NamesEditor } from '@/Components/Shared/KnockoutPartials/NamesEditor';
import { Layout } from '@/Components/Shared/Layout';
import { PVEdit } from '@/Components/Shared/PVs/PVEdit';
import { ArtistRolesEditViewModel } from '@/Components/Shared/Partials/ArtistRolesEditViewModel';
import { CustomNameEdit } from '@/Components/Shared/Partials/CustomNameEdit';
import { EnglishTranslatedStringEdit } from '@/Components/Shared/Partials/EnglishTranslatedStringEdit';
import { EntryDeletePopup } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopup';
import {
	LanguageSelectionDropdownList,
	AlbumTypeDropdownList,
	EntryStatusDropdownList,
} from '@/Components/Shared/Partials/Knockout/DropdownList';
import { EntryValidationMessage } from '@/Components/Shared/Partials/Knockout/EntryValidationMessage';
import { ReleaseEventLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/ReleaseEventLockingAutoComplete';
import { WebLinksEditViewKnockout } from '@/Components/Shared/Partials/Knockout/WebLinksEditViewKnockout';
import { ConcurrentEditWarning } from '@/Components/Shared/Partials/Shared/ConcurrentEditWarning';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { MarkdownNotice } from '@/Components/Shared/Partials/Shared/MarkdownNotice';
import { RequiredField } from '@/Components/Shared/Partials/Shared/RequiredField';
import { SaveAndBackBtn } from '@/Components/Shared/Partials/Shared/SaveAndBackBtn';
import { ValidationErrorIcon } from '@/Components/Shared/Partials/Shared/ValidationErrorIcon';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { useConflictingEditor } from '@/Components/useConflictingEditor';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { DiscMediaType } from '@/DataContracts/Album/AlbumDetailsForApi';
import { ImageHelper } from '@/Helpers/ImageHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import JQueryUITab from '@/JQueryUI/JQueryUITab';
import JQueryUITabs from '@/JQueryUI/JQueryUITabs';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { loginManager } from '@/Models/LoginManager';
import { SongType } from '@/Models/Songs/SongType';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import ArtistForAlbumEdit from '@/Pages/Album/Partials/ArtistForAlbumEdit';
import SongInAlbumEdit from '@/Pages/Album/Partials/SongInAlbumEdit';
import TrackProperties from '@/Pages/Album/Partials/TrackProperties';
import { albumRepo } from '@/Repositories/AlbumRepository';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { pvRepo } from '@/Repositories/PVRepository';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { urlMapper } from '@/Shared/UrlMapper';
import { AlbumEditStore } from '@/Stores/Album/AlbumEditStore';
import { getReasonPhrase } from 'http-status-codes';
import { map } from 'lodash-es';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { ReactSortable } from 'react-sortablejs';

interface BasicInfoTabContentProps {
	albumEditStore: AlbumEditStore;
	coverPicUploadRef: React.MutableRefObject<HTMLInputElement>;
}

const BasicInfoTabContent = observer(
	({
		albumEditStore,
		coverPicUploadRef,
	}: BasicInfoTabContentProps): React.ReactElement => {
		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.Album',
			'VocaDb.Model.Resources',
			'VocaDb.Model.Resources.Albums',
		]);

		const discTypeDescriptions = React.useMemo(
			() =>
				`${t(
					'ViewRes.Album:Edit.BaDiscTypeExplanation',
				)}<br /><br /><ul>${Object.values(AlbumType)
					.filter((value) => value !== AlbumType.Unknown)
					.map(
						(value) =>
							`<li><strong>${t(
								`VocaDb.Model.Resources.Albums:DiscTypeNames.${value}`,
							)}</strong>: ${t(
								`Resources:DiscTypeDescriptions.${value}`,
							)}</li>`,
					)
					.join('')}</ul>`,
			[t],
		);

		return (
			<>
				<div className="editor-label">
					<label>{t('ViewRes:EntryEdit.DefaultLanguageSelection')}</label>
				</div>
				<div className="editor-field">
					<LanguageSelectionDropdownList
						value={albumEditStore.defaultNameLanguage}
						onChange={(e): void =>
							runInAction(() => {
								albumEditStore.defaultNameLanguage = e.target.value;
							})
						}
					/>
				</div>

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.Album:Edit.BaNames')}
						dangerouslySetInnerHTML={{
							__html: t('ViewRes:EntryEdit.NameHelp'),
						}}
					/>{' '}
					<RequiredField />
					{albumEditStore.validationError_unspecifiedNames && (
						<>
							{' '}
							<ValidationErrorIcon
								dangerouslySetInnerHTML={{
									__html: t('ViewRes:EntryEdit.NameHelp'),
								}}
							/>
						</>
					)}
				</div>
				<div className="editor-field">
					<NamesEditor namesEditStore={albumEditStore.names} />
				</div>

				<div className="editor-label">
					<label>{t('ViewRes.Album:Edit.BaMainCoverPicture')}</label>
				</div>
				<div className="editor-field">
					<table>
						<tbody>
							<tr>
								<td>
									<img
										src={`/Album/CoverPictureThumb/${albumEditStore.contract.id}`}
										alt={t('ViewRes.Album:Edit.ImagePreview')}
										className="coverPic"
									/>
								</td>
								<td>
									<p>
										{t('ViewRes.Album:Edit.BaPictureInfo', {
											0: ImageHelper.allowedExtensions.join(', '),
											1: ImageHelper.maxImageSizeMB,
										})}
									</p>
									<input
										type="file"
										id="coverPicUpload"
										name="coverPicUpload"
										ref={coverPicUploadRef}
									/>
								</td>
							</tr>
						</tbody>
					</table>
				</div>

				<div className="editor-label">
					<label>{t('ViewRes.Album:Edit.BaDescription')}</label>{' '}
					<MarkdownNotice />
				</div>
				<div className="editor-field entry-edit-description">
					<EnglishTranslatedStringEdit
						englishTranslatedStringEditStore={albumEditStore.description}
					/>
				</div>

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.Album:Edit.BaDiscType')}
						dangerouslySetInnerHTML={{ __html: discTypeDescriptions }}
					/>
				</div>
				<div className="editor-field">
					<div className="row-fluid">
						<AlbumTypeDropdownList
							value={albumEditStore.discType}
							onChange={(e): void =>
								runInAction(() => {
									albumEditStore.discType = e.target.value as AlbumType;
								})
							}
						/>
						{albumEditStore.validationError_needType && (
							<>
								{' '}
								<ValidationErrorIcon
									dangerouslySetInnerHTML={{
										__html: t(
											'VocaDb.Model.Resources:AlbumValidationErrors.NeedType',
										),
									}}
								/>
							</>
						)}
					</div>
				</div>

				<div className="editor-label">
					<label>{t('ViewRes.Album:Edit.BaReleaseEvent')}</label>
				</div>
				<div className="editor-field">
					<ReleaseEventLockingAutoComplete
						basicEntryLinkStore={albumEditStore.releaseEvent}
						// TODO: createNewItem="Create new event '{0}'" /* LOC */
					/>
				</div>

				<table>
					<tbody>
						<tr>
							<td>{t('ViewRes.Album:Edit.BaReleaseDate')}</td>
							<td>
								<div className="editor-label">
									<label>{t('ViewRes.Album:Edit.BaReleaseYear')}</label>
								</div>
								<div className="editor-field">
									<input
										type="number"
										value={albumEditStore.releaseYear ?? ''}
										onChange={(e): void =>
											runInAction(() => {
												albumEditStore.releaseYear = e.target.value
													? Number(e.target.value)
													: undefined;
											})
										}
										className="input-small"
										size={10}
										maxLength={4}
										min={39}
										max={3939}
									/>
									{albumEditStore.validationError_needReleaseYear && (
										<>
											{' '}
											<ValidationErrorIcon
												dangerouslySetInnerHTML={{
													__html: t(
														'VocaDb.Model.Resources:AlbumValidationErrors.NeedReleaseYear',
													),
												}}
											/>
										</>
									)}
								</div>
							</td>
							<td>
								<div className="editor-label">
									<label>{t('ViewRes.Album:Edit.BaReleaseMonth')}</label>
								</div>
								<div className="editor-field">
									<input
										type="number"
										value={albumEditStore.releaseMonth ?? ''}
										onChange={(e): void =>
											runInAction(() => {
												albumEditStore.releaseMonth = e.target.value
													? Number(e.target.value)
													: undefined;
											})
										}
										className="input-mini"
										size={4}
										maxLength={2}
										min={1}
										max={12}
									/>
								</div>
							</td>
							<td>
								<div className="editor-label">
									<label>{t('ViewRes.Album:Edit.BaReleaseDay')}</label>
								</div>
								<div className="editor-field">
									<input
										type="number"
										value={albumEditStore.releaseDay ?? ''}
										onChange={(e): void =>
											runInAction(() => {
												albumEditStore.releaseDay = e.target.value
													? Number(e.target.value)
													: undefined;
											})
										}
										className="input-mini"
										size={4}
										maxLength={2}
										min={1}
										max={31}
									/>
								</div>
							</td>
						</tr>
					</tbody>
				</table>

				{!albumEditStore.releaseDate && albumEditStore.eventDate && (
					<Button
						onClick={(): void =>
							runInAction(() => {
								albumEditStore.releaseDate = albumEditStore.eventDate;
							})
						}
					>
						Use event date{/* LOC */}{' '}
						<span>
							{albumEditStore.eventDate && albumEditStore.eventDate.format('L')}
						</span>
					</Button>
				)}

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.Album:Edit.BaCatNum')}
						dangerouslySetInnerHTML={{
							__html:
								'Usually catalog numbers are in the format ABC-1234, please do not add extra whitespace.' /* LOC */,
						}}
					/>
				</div>
				<div className="editor-field">
					<input
						type="text"
						value={albumEditStore.catalogNumber}
						onChange={(e): void =>
							runInAction(() => {
								albumEditStore.catalogNumber = e.target.value;
							})
						}
					/>
				</div>

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.Album:Edit.BaBarcode')}
						dangerouslySetInnerHTML={{
							__html: `Barcodes are usually plain numbers, for example 01234567. They can be scanned from the product package.<br /><br /><img src='/Content/barcode.png' />` /* LOC */,
						}}
					/>
				</div>
				<div className="editor-field">
					<div>
						{albumEditStore.identifiers.map((identifier, index) => (
							<div key={index}>
								<span style={{ width: '150px', display: 'inline-block' }}>
									{identifier}
								</span>
								<SafeAnchor
									href="#"
									className="textLink deleteLink"
									onClick={(): void =>
										albumEditStore.removeIdentifier(identifier)
									}
								>
									{t('ViewRes:Shared.Delete')}
								</SafeAnchor>
							</div>
						))}
					</div>
					<input
						type="text"
						placeholder="New barcode" /* LOC */
						maxLength={30}
						value={albumEditStore.newIdentifier}
						onChange={(e): void =>
							runInAction(() => {
								albumEditStore.newIdentifier = e.target.value;
							})
						}
						onBlur={albumEditStore.createNewIdentifier}
					></input>
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
						webLinksEditStore={albumEditStore.webLinks}
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
						value={albumEditStore.status}
						onChange={(e): void =>
							runInAction(() => {
								albumEditStore.status = e.target.value as EntryStatus;
							})
						}
					/>
				</div>
			</>
		);
	},
);

interface ArtistsTabContentProps {
	albumEditStore: AlbumEditStore;
}

const ArtistsTabContent = observer(
	({ albumEditStore }: ArtistsTabContentProps): React.ReactElement => {
		const { t } = useTranslation([
			'AjaxRes',
			'ViewRes',
			'VocaDb.Model.Resources',
		]);

		return (
			<>
				{albumEditStore.validationError_needArtist && (
					<Alert>
						<span>
							{t('VocaDb.Model.Resources:AlbumValidationErrors.NeedArtist')}
						</span>
					</Alert>
				)}

				{albumEditStore.artistLinks.length > 0 && (
					<table>
						<thead>
							<tr>
								<th>Artist{/* LOC */}</th>
								<th>Support{/* LOC */}</th>
								<th>Roles{/* LOC */}</th>
								<th>Actions{/* LOC */}</th>
							</tr>
						</thead>
						<tbody>
							{albumEditStore.artistLinks.map((artistLink, index) => (
								<ArtistForAlbumEdit
									albumEditStore={albumEditStore}
									artistForAlbumEditStore={artistLink}
									key={index}
								/>
							))}
						</tbody>
					</table>
				)}

				<br />
				<h4>{t('ViewRes.Album:Edit.ArAddArtist')}</h4>
				<ArtistAutoComplete
					type="text"
					properties={{
						createNewItem: t('AjaxRes:Shared.AddExtraArtist'),
						acceptSelection: albumEditStore.addArtist,
						height: 300,
					}}
					maxLength={128}
					placeholder={t('ViewRes:Shared.Search')}
					className="input-xlarge"
				/>
			</>
		);
	},
);

interface DiscsTabContentProps {
	albumEditStore: AlbumEditStore;
}

const DiscsTabContent = observer(
	({ albumEditStore }: DiscsTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Album']);

		return (
			<>
				<Alert variant="info">{t('ViewRes.Album:Edit.DiNote')}</Alert>

				<table>
					<thead>
						<tr>
							<th />
							<th>{t('ViewRes.Album:Edit.DiName')}</th>
							<th>{t('ViewRes.Album:Edit.DiType')}</th>
							<th />
						</tr>
					</thead>
					<tbody>
						{albumEditStore.discs.items.map((item, index) => (
							<tr key={index}>
								<td>
									<span>{index + 1}</span>
								</td>
								<td>
									<input
										type="text"
										value={item.name}
										onChange={(e): void =>
											runInAction(() => {
												item.name = e.target.value;
											})
										}
										placeholder="Name"
										maxLength={50}
									/>
								</td>
								<td>
									<select
										value={item.mediaType}
										onChange={(e): void =>
											runInAction(() => {
												item.mediaType = e.target.value as DiscMediaType;
											})
										}
										className="input-small"
									>
										<option value="Audio">
											{t('ViewRes.Album:Edit.DiAudio')}
										</option>
										<option value="Video">
											{t('ViewRes.Album:Edit.DiVideo')}
										</option>
									</select>
								</td>
								<td>
									<SafeAnchor
										onClick={(): void => albumEditStore.discs.remove(item)}
										href="#"
										className="iconLink removeLink"
									>
										{t('ViewRes:Shared.Remove')}
									</SafeAnchor>
								</td>
							</tr>
						))}
					</tbody>
				</table>

				<SafeAnchor
					href="#"
					className="textLink addLink"
					onClick={albumEditStore.discs.add}
				>
					{t('ViewRes.Album:Edit.DiAddRow')}
				</SafeAnchor>
			</>
		);
	},
);

interface TracksTabContentProps {
	albumEditStore: AlbumEditStore;
}

const TracksTabContent = observer(
	({ albumEditStore }: TracksTabContentProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Album',
			'VocaDb.Model.Resources',
		]);

		return (
			<>
				{albumEditStore.validationError_needTracks && (
					<Alert>
						<span>
							{t('VocaDb.Model.Resources:AlbumValidationErrors.NeedTracks')}
						</span>
					</Alert>
				)}

				{albumEditStore.tracks.length > 0 && (
					<>
						<p>{t('ViewRes.Album:Edit.TrTrackNameHelp')}</p>

						<table>
							<thead>
								<tr>
									<th />
									<th>
										<input
											type="checkbox"
											checked={albumEditStore.allTracksSelected}
											onChange={(e): void =>
												runInAction(() => {
													albumEditStore.allTracksSelected = e.target.checked;
												})
											}
										/>
									</th>
									<th>{t('ViewRes.Album:Edit.TrDiscHead')}</th>
									<th>{t('ViewRes.Album:Edit.TrTrackHead')}</th>
									<th>{t('ViewRes.Album:Edit.TrNameHead')}</th>
									<th colSpan={3} />
								</tr>
							</thead>
							<ReactSortable
								tag="tbody"
								list={albumEditStore.tracks}
								setList={(tracks): void =>
									runInAction(() => {
										albumEditStore.tracks = tracks;
									})
								}
								handle=".handle"
							>
								{albumEditStore.tracks.map((track, index) => (
									<SongInAlbumEdit
										albumEditStore={albumEditStore}
										songInAlbumEditStore={track}
										key={index}
									/>
								))}
							</ReactSortable>
						</table>

						<br />
						<SafeAnchor
							onClick={albumEditStore.editMultipleTrackProperties}
							href="#"
							className="textLink editLink"
						>
							{t('ViewRes.Album:Edit.TrSetArtists')}
						</SafeAnchor>
						<br />
						<br />
					</>
				)}

				<h4>{t('ViewRes.Album:Edit.AddNew')}</h4>
				<SongAutoComplete
					type="text"
					properties={{
						acceptSelection: albumEditStore.acceptTrackSelection,
						createNewItem: "Create new song named '{0}'.", // LOC
						createCustomItem: vdb.values.allowCustomTracks
							? "Create custom track named '{0}'" /* LOC */
							: null!,
						extraQueryParams: {
							songTypes: [
								SongType.Unspecified,
								SongType.Original,
								SongType.Remaster,
								SongType.Remix,
								SongType.Cover,
								SongType.Arrangement,
								SongType.Mashup,
								SongType.Other,
								SongType.Instrumental,
								SongType.Live,
								SongType.Illustration,
								...(albumEditStore.contract.discType === AlbumType.Video
									? [SongType.MusicPV, SongType.DramaPV]
									: []),
							],
						},
					}}
					maxLength={128}
					placeholder={t('ViewRes:Shared.Search')}
					className="input-xlarge"
				/>
				<p>{t('ViewRes.Album:Edit.TrAddTrackHelp')}</p>
			</>
		);
	},
);

interface PicturesTabContentProps {
	albumEditStore: AlbumEditStore;
}

const PicturesTabContent = observer(
	({ albumEditStore }: PicturesTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Album']);

		return (
			<>
				<p>{t('ViewRes.Album:Edit.PiPicturesGuide')}</p>
				<p>
					{t('ViewRes.Album:Edit.BaPictureInfo', {
						0: ImageHelper.allowedExtensions.join(', '),
						1: ImageHelper.maxImageSizeMB,
					})}
				</p>

				<table>
					<tbody>
						{albumEditStore.pictures.pictures.map((picture, index) => (
							<EntryPictureFileEdit
								entryPictureFileListEditStore={albumEditStore.pictures}
								entryPictureFileEditStore={picture}
								key={index}
							/>
						))}
					</tbody>
				</table>

				<SafeAnchor
					onClick={albumEditStore.pictures.add}
					href="#"
					className="addLink textLink"
				>
					{t('ViewRes.Album:Edit.PiCreateNew')}
				</SafeAnchor>
			</>
		);
	},
);

interface MediaTabContentProps {
	albumEditStore: AlbumEditStore;
}

const MediaTabContent = observer(
	({ albumEditStore }: MediaTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Album']);

		return (
			<>
				<p>{t('ViewRes.Album:Edit.PvIntro')}</p>

				{albumEditStore.pvs.pvs.length > 0 && (
					<table>
						<thead>
							<tr>
								<th>{t('ViewRes.Album:Edit.PvService')}</th>
								<th>{t('ViewRes.Album:Edit.PvType')}</th>
								<th colSpan={2}>{t('ViewRes.Album:Edit.PvName')}</th>
								<th>Date{/* LOC */}</th>
								<th>{t('ViewRes.Album:Edit.PvAuthor')}</th>
								<th />
							</tr>
						</thead>
						<tbody>
							{albumEditStore.pvs.pvs.map((pv, index) => (
								<PVEdit
									pvListEditStore={albumEditStore.pvs}
									pvEditStore={pv}
									key={index}
								/>
							))}
						</tbody>
					</table>
				)}

				<br />
				<h4>{t('ViewRes.Album:Edit.PvAdd')}</h4>
				<p>{t('ViewRes.Album:Edit.PvSupportedServices')}</p>
				<p>
					{t('ViewRes.Album:Edit.PvURL')}{' '}
					<input
						type="text"
						value={albumEditStore.pvs.newPvUrl}
						onChange={(e): void =>
							runInAction(() => {
								albumEditStore.pvs.newPvUrl = e.target.value;
							})
						}
						maxLength={255}
						size={40}
						className="input-xlarge"
					/>
				</p>

				<SafeAnchor
					onClick={albumEditStore.pvs.add}
					href="#"
					className="textLink addLink"
				>
					{t('ViewRes:Shared.Add')}
				</SafeAnchor>
				{/* TODO */}
			</>
		);
	},
);

interface AlbumEditLayoutProps {
	albumEditStore: AlbumEditStore;
}

const AlbumEditLayout = observer(
	({ albumEditStore }: AlbumEditLayoutProps): React.ReactElement => {
		const { t, ready } = useTranslation([
			'ViewRes',
			'ViewRes.Album',
			'VocaDb.Model.Resources',
		]);

		const contract = albumEditStore.contract;

		const title = t('ViewRes.Album:Edit.EditTitle', { 0: contract.name });

		useVdbTitle(title, ready);

		const conflictingEditor = useConflictingEditor(EntryType.Album);

		const navigate = useNavigate();

		const coverPicUploadRef = React.useRef<HTMLInputElement>(undefined!);

		return (
			<Layout
				title={title}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: '/Album',
							}}
							divider
						>
							{t('ViewRes:Shared.Albums')}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details(EntryType.Album, contract.id),
							}}
						>
							{contract.name}
						</Breadcrumb.Item>
					</>
				}
				toolbar={
					<>
						{contract.canDelete &&
							(contract.deleted ? (
								<>
									<JQueryUIButton
										as="a"
										href={`/Album/Restore/${contract.id}`}
										icons={{ primary: 'ui-icon-trash' }}
									>
										{t('ViewRes:EntryEdit.Restore')}
									</JQueryUIButton>
									{loginManager.canMoveToTrash && (
										<>
											{' '}
											&nbsp;{' '}
											<JQueryUIButton
												as="a"
												href={`/Album/MoveToTrash/${contract.id}`} /* TODO: Convert to POST */
												onClick={(e): void => {
													if (
														!window.confirm(
															t('ViewRes.Album:Edit.ConfirmMoveToTrash'),
														)
													) {
														e.preventDefault();
													}
												}}
												icons={{ primary: 'ui-icon-trash' }}
											>
												{t('ViewRes.Album:Edit.MoveToTrash')}
											</JQueryUIButton>
										</>
									)}
								</>
							) : (
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={albumEditStore.deleteStore.show}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:Shared.Delete')}
								</JQueryUIButton>
							))}
						{loginManager.canMergeEntries && (
							<>
								{' '}
								&nbsp;{' '}
								<JQueryUIButton as={Link} to={`/Album/Merge/${contract.id}`}>
									{t('ViewRes:EntryEdit.Merge')}
								</JQueryUIButton>
							</>
						)}
					</>
				}
			>
				{conflictingEditor && conflictingEditor.userId !== 0 && (
					<ConcurrentEditWarning conflictingEditor={conflictingEditor} />
				)}

				{albumEditStore.errors && (
					<ValidationSummaryPanel
						message="Unable to save properties." /* LOC */
						errors={albumEditStore.errors}
					/>
				)}

				<EntryValidationMessage
					draft={contract.status === EntryStatus.Draft}
					validationMessages={([] as string[]).concat(
						albumEditStore.validationError_duplicateArtist
							? t(
									'VocaDb.Model.Resources:AlbumValidationErrors.DuplicateArtist',
							  )
							: [],
						albumEditStore.validationError_unspecifiedNames
							? t(
									'VocaDb.Model.Resources:AlbumValidationErrors.UnspecifiedNames',
							  )
							: [],
						albumEditStore.validationError_needArtist
							? t('VocaDb.Model.Resources:AlbumValidationErrors.NeedArtist')
							: [],
						albumEditStore.validationError_needCover
							? t('VocaDb.Model.Resources:AlbumValidationErrors.NeedCover')
							: [],
						albumEditStore.validationError_needReferences
							? t('VocaDb.Model.Resources:AlbumValidationErrors.NeedReferences')
							: [],
						albumEditStore.validationError_needReleaseYear
							? t(
									'VocaDb.Model.Resources:AlbumValidationErrors.NeedReleaseYear',
							  )
							: [],
						albumEditStore.validationError_needTracks
							? t('VocaDb.Model.Resources:AlbumValidationErrors.NeedTracks')
							: [],
						albumEditStore.validationError_needType
							? t('VocaDb.Model.Resources:AlbumValidationErrors.NeedType')
							: [],
					)}
					helpSection="glalbums"
				/>

				<br />
				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const coverPicUpload =
								coverPicUploadRef.current.files?.item(0) ?? undefined;

							// TODO: Use useRef.
							const pictureUpload = map(
								document.getElementsByName('pictureUpload'),
								(element) => (element as HTMLInputElement).files?.[0],
							)
								.filter((file) => file !== undefined)
								.map((file) => file as File);

							const id = await albumEditStore.submit(
								requestToken,
								coverPicUpload,
								pictureUpload,
							);

							navigate(EntryUrlMapper.details(EntryType.Album, id));
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
						backAction={EntryUrlMapper.details(EntryType.Album, contract.id)}
						submitting={albumEditStore.submitting}
					/>

					<JQueryUITabs>
						<JQueryUITab
							eventKey="basicInfo"
							title={t('ViewRes:EntryEdit.BasicInfo')}
						>
							<BasicInfoTabContent
								albumEditStore={albumEditStore}
								coverPicUploadRef={coverPicUploadRef}
							/>
						</JQueryUITab>

						<JQueryUITab
							eventKey="artists"
							title={t('ViewRes.Album:Edit.ArtistsTab')}
						>
							<ArtistsTabContent albumEditStore={albumEditStore} />
						</JQueryUITab>

						<JQueryUITab
							eventKey="discs"
							title={t('ViewRes.Album:Edit.DiscsTab')}
						>
							<DiscsTabContent albumEditStore={albumEditStore} />
						</JQueryUITab>

						<JQueryUITab
							eventKey="tracks"
							title={t('ViewRes.Album:Edit.TracksTab')}
						>
							<TracksTabContent albumEditStore={albumEditStore} />
						</JQueryUITab>

						<JQueryUITab
							eventKey="pics"
							title={t('ViewRes.Album:Edit.PicturesTab')}
						>
							<PicturesTabContent albumEditStore={albumEditStore} />
						</JQueryUITab>

						<JQueryUITab
							eventKey="pvs"
							title={t('ViewRes.Album:Edit.MediaTab')}
						>
							<MediaTabContent albumEditStore={albumEditStore} />
						</JQueryUITab>
					</JQueryUITabs>
					<br />

					<p>{t('ViewRes:EntryEdit.UpdateNotes')}</p>
					<textarea
						value={albumEditStore.updateNotes}
						onChange={(e): void =>
							runInAction(() => {
								albumEditStore.updateNotes = e.target.value;
							})
						}
						className="input-xxlarge"
						rows={4}
						maxLength={200}
					/>

					<br />
					<SaveAndBackBtn
						backAction={EntryUrlMapper.details(EntryType.Album, contract.id)}
						submitting={albumEditStore.submitting}
					/>
				</form>

				<div>
					<ArtistRolesEditViewModel
						artistRolesEditStore={albumEditStore.artistRolesEditStore}
					/>
				</div>

				<div>
					<CustomNameEdit
						customNameEditStore={albumEditStore.editedArtistLink}
					/>
				</div>

				<JQueryUIDialog
					title={t('ViewRes.Album:Edit.TrackPropertiesTitle')}
					autoOpen={albumEditStore.trackPropertiesDialogVisible}
					width={550}
					close={(): void =>
						runInAction(() => {
							albumEditStore.trackPropertiesDialogVisible = false;
						})
					}
					buttons={
						albumEditStore.editedSong &&
						(albumEditStore.editedSong.song
							? [
									{
										text: 'Save' /* LOC */,
										click: albumEditStore.saveTrackProperties,
									},
							  ]
							: [
									{
										text: 'Add to tracks' /* LOC */,
										click: albumEditStore.addArtistsToSelectedTracks,
									},
									{
										text: 'Remove from tracks' /* LOC */,
										click: albumEditStore.removeArtistsFromSelectedTracks,
									},
							  ])
					}
				>
					<div>
						{albumEditStore.editedSong && (
							<TrackProperties
								trackPropertiesStore={albumEditStore.editedSong}
							/>
						)}
					</div>
				</JQueryUIDialog>

				<EntryDeletePopup
					confirmText={t('ViewRes:EntryEdit.ConfirmDelete')}
					deleteEntryStore={albumEditStore.deleteStore}
				/>
			</Layout>
		);
	},
);

const AlbumEdit = (): React.ReactElement => {
	const { t } = useTranslation(['Resources']);

	const artistRoleNames = React.useMemo(
		() =>
			Object.fromEntries(
				vdb.values.artistRoles.map((artistRole): [
					string,
					string | undefined,
				] => [artistRole, t(`Resources:ArtistRoleNames.${artistRole}`)]),
			),
		[t],
	);

	const { id } = useParams();

	const [model, setModel] = React.useState<{
		albumEditStore: AlbumEditStore;
	}>();

	React.useEffect(() => {
		albumRepo
			.getForEdit({ id: Number(id) })
			.then((model) =>
				setModel({
					albumEditStore: new AlbumEditStore(
						vdb.values,
						albumRepo,
						songRepo,
						artistRepo,
						pvRepo,
						eventRepo,
						urlMapper,
						artistRoleNames,
						Object.values(WebLinkCategory),
						model,
						loginManager.canBulkDeletePVs,
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
	}, [artistRoleNames, id]);

	return model ? (
		<AlbumEditLayout albumEditStore={model.albumEditStore} />
	) : (
		<></>
	);
};

export default AlbumEdit;
