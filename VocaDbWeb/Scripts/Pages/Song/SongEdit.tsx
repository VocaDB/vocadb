import Accordion from '@/Bootstrap/Accordion';
import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistAutoComplete } from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { NamesEditor } from '@/Components/Shared/KnockoutPartials/NamesEditor';
import { Layout } from '@/Components/Shared/Layout';
import { PVEdit } from '@/Components/Shared/PVs/PVEdit';
import { ArtistRolesEditViewModel } from '@/Components/Shared/Partials/ArtistRolesEditViewModel';
import { CustomNameEdit } from '@/Components/Shared/Partials/CustomNameEdit';
import { EnglishTranslatedStringEdit } from '@/Components/Shared/Partials/EnglishTranslatedStringEdit';
import { EntryDeletePopup } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopup';
import { UserLanguageCultureDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import {
	LanguageSelectionDropdownList,
	SongTypeDropdownList,
	EntryStatusDropdownList,
	PVTypeDescriptionsDropdownList,
} from '@/Components/Shared/Partials/Knockout/DropdownList';
import { EntryValidationMessage } from '@/Components/Shared/Partials/Knockout/EntryValidationMessage';
import { ReleaseEventLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/ReleaseEventLockingAutoComplete';
import { SongLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/SongLockingAutoComplete';
import { WebLinksEditViewKnockout } from '@/Components/Shared/Partials/Knockout/WebLinksEditViewKnockout';
import { ConcurrentEditWarning } from '@/Components/Shared/Partials/Shared/ConcurrentEditWarning';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { MarkdownNotice } from '@/Components/Shared/Partials/Shared/MarkdownNotice';
import { RequiredField } from '@/Components/Shared/Partials/Shared/RequiredField';
import { SaveAndBackBtn } from '@/Components/Shared/Partials/Shared/SaveAndBackBtn';
import { ValidationErrorIcon } from '@/Components/Shared/Partials/Shared/ValidationErrorIcon';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { SongLink } from '@/Components/Shared/Partials/Song/SongLink';
import { showErrorMessage } from '@/Components/ui';
import { useConflictingEditor } from '@/Components/useConflictingEditor';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import JQueryUIDatepicker from '@/JQueryUI/JQueryUIDatepicker';
import JQueryUITab from '@/JQueryUI/JQueryUITab';
import JQueryUITabs from '@/JQueryUI/JQueryUITabs';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { PVType } from '@/Models/PVs/PVType';
import { SongType } from '@/Models/Songs/SongType';
import SongBpmFilter from '@/Pages/Search/Partials/SongBpmFilter';
import SongLengthFilter from '@/Pages/Search/Partials/SongLengthFilter';
import ArtistForSongEdit from '@/Pages/Song/Partials/ArtistForSongEdit';
import LyricsForSongEdit from '@/Pages/Song/Partials/LyricsForSongEdit';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { pvRepo } from '@/Repositories/PVRepository';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { urlMapper } from '@/Shared/UrlMapper';
import { LyricsForSongListEditStore } from '@/Stores/Song/LyricsForSongListEditStore';
import { SongEditStore } from '@/Stores/Song/SongEditStore';
import { useVdb } from '@/VdbContext';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import {
	Link,
	useNavigate,
	useParams,
	useSearchParams,
} from 'react-router-dom';

const maxMediaSizeMB = 20;

interface BasicInfoTabContentProps {
	songEditStore: SongEditStore;
}

const BasicInfoTabContent = observer(
	({ songEditStore }: BasicInfoTabContentProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.Song',
			'VocaDb.Web.Resources.Domain.Globalization',
			'VocaDb.Model.Resources',
		]);

		return (
			<>
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
						value={songEditStore.defaultNameLanguage}
						onChange={(e): void =>
							runInAction(() => {
								songEditStore.defaultNameLanguage = e.target.value;
							})
						}
					/>
				</div>

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.Song:Edit.BaNames')}
						dangerouslySetInnerHTML={{
							__html: t('ViewRes:EntryEdit.NameHelp'),
						}}
					/>{' '}
					<RequiredField />
					{songEditStore.validationError_unspecifiedNames && (
						<>
							{' '}
							<ValidationErrorIcon
								dangerouslySetInnerHTML={{
									__html: t(
										'VocaDb.Model.Resources:SongValidationErrors.UnspecifiedNames',
									),
								}}
							/>
						</>
					)}
				</div>
				<div className="editor-field">
					<NamesEditor namesEditStore={songEditStore.names} />
				</div>

				<div className="editor-label">
					<label>{t('ViewRes.Song:Edit.BaNotes')}</label> <MarkdownNotice />
				</div>
				<div className="editor-field entry-edit-description">
					<EnglishTranslatedStringEdit
						englishTranslatedStringEditStore={songEditStore.notes}
					/>
				</div>

				<div className="editor-label">
					<label>{t('ViewRes.Song:Edit.BaSongType')}</label>
				</div>
				<div className="editor-field">
					<div className="row-fluid">
						<div className="span4">
							<SongTypeDropdownList
								value={songEditStore.songType}
								onChange={(e): void =>
									runInAction(() => {
										songEditStore.songType = e.target.value as SongType;
									})
								}
							/>
							{songEditStore.validationError_needType && (
								<>
									{' '}
									<ValidationErrorIcon
										dangerouslySetInnerHTML={{
											__html: t(
												'VocaDb.Model.Resources:SongValidationErrors.NeedType',
											),
										}}
									/>
								</>
							)}
						</div>
					</div>
				</div>

				{songEditStore.canHaveOriginalVersion && (
					<>
						<div className="editor-label">
							<HelpLabel
								label={t('ViewRes.Song:Edit.BaOriginalVersion')}
								dangerouslySetInnerHTML={{
									__html: t('ViewRes.Song:Edit.BaOriginalVersionHelp'),
								}}
							/>
						</div>
						<div className="editor-field">
							<div style={{ display: 'inline-block' }} className="input-append">
								<SongLockingAutoComplete
									basicEntryLinkStore={songEditStore.originalVersion}
									songTypes={[
										SongType.Unspecified,
										SongType.Original,
										SongType.Remaster,
										SongType.Remix,
										SongType.Cover,
										SongType.Arrangement,
										SongType.Mashup,
										SongType.DramaPV,
										SongType.Other,
									]}
									ignoreId={songEditStore.contract.id}
									height={250}
								/>
							</div>
							{songEditStore.originalVersion.isEmpty && (
								<div>
									<SafeAnchor
										href="#"
										onClick={songEditStore.findOriginalSongSuggestions}
										className="textLink searchLink"
									>
										Find originals{/* LOC */}
									</SafeAnchor>
									<table>
										<tbody>
											{songEditStore.originalVersionSuggestions.map(
												(originalVersionSuggestion, index) => (
													<tr key={index}>
														<td>
															<SongLink
																song={originalVersionSuggestion}
																tooltip
																target="_blank"
															/>{' '}
															(<span>{originalVersionSuggestion.songType}</span>
															)
															{originalVersionSuggestion.artistString && (
																<div>
																	<span>
																		{originalVersionSuggestion.artistString}
																	</span>
																</div>
															)}
														</td>
														<td style={{ maxWidth: '150px' }}>
															<SafeAnchor
																className="textLink acceptLink"
																href="#"
																onClick={(): void =>
																	songEditStore.selectOriginalVersion(
																		originalVersionSuggestion,
																	)
																}
															>
																Select{/* LOC */}
															</SafeAnchor>
														</td>
													</tr>
												),
											)}
										</tbody>
									</table>
								</div>
							)}
						</div>
					</>
				)}

				<div className="editor-label">
					<label>{t('ViewRes.Song:Edit.BaDuration')}</label>
				</div>
				<div className="editor-field">
					<SongLengthFilter songLengthFilter={songEditStore.lengthFilter} />
				</div>

				<div className="editor-label">
					<label>{t('ViewRes.Song:Edit.BaBpm')}</label>
				</div>
				<div className="editor-field">
					<label>
						<input
							type="radio"
							checked={!songEditStore.hasMaxMilliBpm}
							onChange={(e): void =>
								runInAction(() => {
									songEditStore.hasMaxMilliBpm = false;
								})
							}
						/>{' '}
						{t('ViewRes.Song:Edit.BaSpecificBpm')}
					</label>
					<label>
						<input
							type="radio"
							checked={songEditStore.hasMaxMilliBpm}
							onChange={(e): void =>
								runInAction(() => {
									songEditStore.hasMaxMilliBpm = true;
								})
							}
						/>{' '}
						{t('ViewRes.Song:Edit.BaBpmRange')}
					</label>
					{songEditStore.hasMaxMilliBpm ? (
						<div>
							<SongBpmFilter songBpmFilter={songEditStore.minBpmFilter} /> to{' '}
							<SongBpmFilter songBpmFilter={songEditStore.maxBpmFilter} />
						</div>
					) : (
						<div>
							<SongBpmFilter songBpmFilter={songEditStore.minBpmFilter} />
						</div>
					)}
				</div>

				<div className="editor-label">
					<label>Release event{/* LOC */}</label>
				</div>
				<div className="editor-field">
					<ReleaseEventLockingAutoComplete
						basicEntryLinkStore={songEditStore.releaseEvent}
						// TODO: createNewItem="Create new event '{0}'" /* LOC */
					/>
				</div>

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.Song:Edit.BaPublishDate')}
						dangerouslySetInnerHTML={{
							__html: t('ViewRes.Song:Edit.BaPublishDateHelp'),
						}}
					/>
				</div>
				<div className="editor-field">
					<JQueryUIDatepicker
						type="text"
						value={songEditStore.publishDate}
						onSelect={(date): void =>
							runInAction(() => {
								songEditStore.publishDate = date;
							})
						}
						dateFormat="yy-mm-dd"
						maxLength={10}
					/>

					{/* Suggest publish date based on PV date, album release date or event date. Show only earlier of PV and album dates, if present. */}
					<div>
						{!songEditStore.publishDate &&
							(songEditStore.suggestedPublishDate ||
								songEditStore.eventDate) && (
								<>
									<div>
										{songEditStore.suggestedPublishDate && (
											<Button
												onClick={(): void =>
													runInAction(() => {
														songEditStore.publishDate = songEditStore.suggestedPublishDate?.date.toDate();
													})
												}
											>
												{songEditStore.suggestedPublishDate.source === 'PV' && (
													<span>{t('ViewRes.Song:Edit.BaUsePvDate')}</span>
												)}
												{songEditStore.suggestedPublishDate.source ===
													'Album' && (
													<span>{t('ViewRes.Song:Edit.BaUseAlbumDate')}</span>
												)}{' '}
												<span>
													{songEditStore.suggestedPublishDate.date.format('L')}
												</span>
											</Button>
										)}
									</div>
									{songEditStore.eventDate &&
										(!songEditStore.suggestedPublishDate ||
											songEditStore.eventDate <
												songEditStore.suggestedPublishDate.date) && (
											<div className="inline-block">
												{songEditStore.eventDate && (
													<Button
														onClick={(): void => {
															runInAction(() => {
																songEditStore.publishDate = songEditStore.eventDate?.toDate();
															});
														}}
													>
														{t('ViewRes.Song:Edit.BaUseEventDate')}{' '}
														<span>{songEditStore.eventDate.format('L')}</span>
													</Button>
												)}
											</div>
										)}
								</>
							)}
					</div>
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
						webLinksEditStore={songEditStore.webLinks}
					/>
				</div>

				{songEditStore.songType !== SongType.Instrumental && (
					<>
						<div className="editor-label">
							<label>Language(s){/* LOC */}</label>
						</div>
						<div className="editor-field">
							<tbody>
								{songEditStore.cultureCodes.items.map((c, index) => (
									<tr key={index}>
										<UserLanguageCultureDropdownList
											value={c.toString()}
											placeholder={t(
												'VocaDb.Web.Resources.Domain.Globalization:InterfaceLanguage.Other',
											)}
											extended={songEditStore.cultureCodes.extended}
											onChange={(val): void => {
												songEditStore.cultureCodes.items[index] =
													val.target.value;
											}}
											key={index}
										/>
										<SafeAnchor
											onClick={(): void => songEditStore.cultureCodes.remove(c)}
											href="#"
											className="nameDelete textLink deleteLink"
										>
											{t('ViewRes:Shared.Delete')}
										</SafeAnchor>
										<br />
									</tr>
								))}
							</tbody>
							{songEditStore.cultureCodes.items.length < 3 && (
								<SafeAnchor
									href="#"
									className="textLink addLink"
									onClick={(): void => songEditStore.cultureCodes.add()}
								>
									{t('ViewRes:Shared.Add')}
								</SafeAnchor>
							)}
							{!songEditStore.cultureCodes.extended &&
								songEditStore.cultureCodes.items.length > 0 && (
									<SafeAnchor
										href="#"
										className="textLink addLink"
										onClick={(): void => {
											songEditStore.cultureCodes.extended = true;
										}}
									>
										{t('ViewRes:EntryEdit.LyExtendLanguages')}{' '}
									</SafeAnchor>
								)}
						</div>
					</>
				)}

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
						value={songEditStore.status}
						onChange={(e): void =>
							runInAction(() => {
								songEditStore.status = e.target.value as EntryStatus;
							})
						}
					/>
				</div>
			</>
		);
	},
);

interface ArtistsTabContentProps {
	songEditStore: SongEditStore;
}

const ArtistsTabContent = observer(
	({ songEditStore }: ArtistsTabContentProps): React.ReactElement => {
		const { t } = useTranslation([
			'AjaxRes',
			'ViewRes',
			'ViewRes.Song',
			'VocaDb.Model.Resources',
		]);

		return (
			<div className="row-fluid">
				<div className="span6">
					{songEditStore.artistLinks.length > 0 && (
						<table>
							<thead>
								<tr>
									<th>{t('ViewRes.Song:Edit.ArArtist')}</th>
									<th>{t('ViewRes.Song:Edit.ArSupport')}</th>
									<th>{t('ViewRes.Song:Edit.ArRoles')}</th>
									<th>{t('ViewRes.Song:Edit.ArActions')}</th>
								</tr>
							</thead>
							<tbody>
								{songEditStore.artistLinks.map((artistLink, index) => (
									<ArtistForSongEdit
										songEditStore={songEditStore}
										artistForAlbumEditStore={artistLink}
										key={index}
									/>
								))}
							</tbody>
						</table>
					)}

					<br />
					<h4>{t('ViewRes.Song:Edit.ArAddArtistTitle')}</h4>
					<ArtistAutoComplete
						type="text"
						properties={{
							createNewItem: t('AjaxRes:Shared.AddExtraArtist'),
							acceptSelection: songEditStore.addArtist,
							height: 300,
						}}
						maxLength={128}
						placeholder={t('ViewRes:Shared.Search')}
						className="input-xlarge"
					/>
				</div>
				<div className="span4">
					<Alert variant="info">
						<span>{t('ViewRes.Song:Edit.ArVocaloidsNote')}</span>
					</Alert>
					{songEditStore.hasAlbums && (
						<Alert variant="info">
							<span>{t('ViewRes.Song:Edit.ArArtistsNote')}</span>
						</Alert>
					)}
					{songEditStore.validationError_needArtist && (
						<Alert>
							<span>
								{t('VocaDb.Model.Resources:SongValidationErrors.NeedArtist')}
							</span>
						</Alert>
					)}
					{songEditStore.validationError_needProducer && (
						<Alert>
							<span>
								{t('VocaDb.Model.Resources:SongValidationErrors.NeedProducer')}
							</span>
						</Alert>
					)}
					{songEditStore.validationError_nonInstrumentalSongNeedsVocalists && (
						<Alert>
							<span>
								{t(
									'VocaDb.Model.Resources:SongValidationErrors.NonInstrumentalSongNeedsVocalists',
								)}
							</span>
						</Alert>
					)}
				</div>
			</div>
		);
	},
);

interface PVsTabContentProps {
	songEditStore: SongEditStore;
}

const PVsTabContent = observer(
	({ songEditStore }: PVsTabContentProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation(['ViewRes.Song']);

		const uploadMediaRef = React.useRef<HTMLInputElement>(undefined!);

		return (
			<>
				{songEditStore.showInstrumentalNote && (
					<Alert variant="info">
						{t('ViewRes.Song:Edit.PvInstrumentalNote')}
					</Alert>
				)}

				{songEditStore.pvs.pvs.length > 0 && (
					<table>
						<thead>
							<tr>
								<th>{t('ViewRes.Song:Edit.PvService')}</th>
								<th>{t('ViewRes.Song:Edit.PvType')}</th>
								<th>{t('ViewRes.Song:Edit.PvName')}</th>
								<th>{t('ViewRes.Song:Edit.PvLength')}</th>
								<th>{t('ViewRes.Song:Edit.PvDate')}</th>
								<th>{t('ViewRes.Song:Edit.PvAuthor')}</th>
								<th>
									<HelpLabel
										label={t('ViewRes.Song:Edit.PvStatus')}
										dangerouslySetInnerHTML={{
											__html: t('ViewRes.Song:Edit.PvStatusHelp'),
										}}
									/>
								</th>
								<th />
							</tr>
						</thead>
						<tbody>
							{songEditStore.pvs.pvs.map((pv, index) => (
								<PVEdit
									pvListEditStore={songEditStore.pvs}
									pvEditStore={pv}
									key={index}
								/>
							))}
						</tbody>
					</table>
				)}

				<br />
				<h4>{t('ViewRes.Song:Edit.PvAddMedia')}</h4>

				<p>{t('ViewRes.Song:Edit.PvSupportedServices')}</p>
				<p>
					{t('ViewRes.Song:Edit.PvUrl')}{' '}
					<input
						type="text"
						value={songEditStore.pvs.newPvUrl}
						onChange={(e): void =>
							runInAction(() => {
								songEditStore.pvs.newPvUrl = e.target.value;
							})
						}
						maxLength={255}
						size={40}
						className="input-xlarge"
					/>
				</p>
				<p>
					{t('ViewRes.Song:Edit.PvNewType')}{' '}
					<PVTypeDescriptionsDropdownList
						value={songEditStore.pvs.newPvType}
						onChange={(e): void =>
							runInAction(() => {
								songEditStore.pvs.newPvType = e.target.value as PVType;
							})
						}
						className="input-xlarge"
					/>
				</p>

				<SafeAnchor
					onClick={songEditStore.pvs.add}
					href="#"
					className="textLink addLink"
				>
					{t('ViewRes:Shared.Add')}
				</SafeAnchor>

				{loginManager.canUploadMedia && (
					<>
						<h4 className="withMargin">Upload file{/* LOC */}</h4>
						<p>
							You can use VocaDB for hosting songs you have made (you must be
							the content creator). Choose the file and click "Upload".
							<br />
							Supported formats: .mp3. Maximum file size is {maxMediaSizeMB}MB.
							PV type will automatically be "original".
							{/* LOC */}
						</p>
						<input type="file" id="uploadMedia" ref={uploadMediaRef} />{' '}
						<SafeAnchor
							onClick={async (): Promise<void> => {
								const uploadMedia =
									uploadMediaRef.current.files?.item(0) ?? undefined;

								if (!uploadMedia) return;

								await songEditStore.pvs.uploadMedia(uploadMedia);
							}}
							href="#"
							className="textLink addLink"
						>
							Upload{/* LOC */}
						</SafeAnchor>
					</>
				)}

				{/* TODO: AjaxLoader */}
			</>
		);
	},
);

interface LyricsTabContentProps {
	songEditStore: SongEditStore;
	lyricsForSongListEditStore: LyricsForSongListEditStore;
}

const LyricsTabContent = observer(
	({
		songEditStore,
		lyricsForSongListEditStore,
	}: LyricsTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Song']);

		return (
			<>
				{songEditStore.showLyricsNote && (
					<Alert variant="info">
						{t('ViewRes.Song:Edit.LyInheritanceNote')}
					</Alert>
				)}

				<Accordion alwaysOpen>
					<div>
						<LyricsForSongEdit
							lyricsForSongListEditStore={lyricsForSongListEditStore}
							lyricsForSongEditStore={lyricsForSongListEditStore.original}
							eventKey={(0).toString()}
						/>
					</div>
					<div>
						<LyricsForSongEdit
							lyricsForSongListEditStore={lyricsForSongListEditStore}
							lyricsForSongEditStore={lyricsForSongListEditStore.romanized}
							eventKey={(1).toString()}
						/>
					</div>
					{lyricsForSongListEditStore.items.map((item, index) => (
						<LyricsForSongEdit
							lyricsForSongListEditStore={lyricsForSongListEditStore}
							lyricsForSongEditStore={item}
							eventKey={(index + 2).toString()}
							key={index}
						/>
					))}
				</Accordion>

				<SafeAnchor
					onClick={lyricsForSongListEditStore.add}
					href="#"
					className="textLink addLink"
				>
					{t('ViewRes.Song:Edit.LyAddRow')}
				</SafeAnchor>
			</>
		);
	},
);

interface SongEditLayoutProps {
	songEditStore: SongEditStore;
}

const SongEditLayout = observer(
	({ songEditStore }: SongEditLayoutProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t, ready } = useTranslation([
			'ViewRes',
			'ViewRes.Song',
			'VocaDb.Model.Resources',
		]);

		const contract = songEditStore.contract;

		const title = t('ViewRes.Song:Edit.EditTitle', { 0: contract.name });

		const conflictingEditor = useConflictingEditor(EntryType.Song);

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
								to: '/Song',
							}}
							divider
						>
							{t('ViewRes:Shared.Songs')}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details(EntryType.Song, contract.id),
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
								loginManager.canDeleteEntries && (
									<JQueryUIButton
										as="a"
										href={`/Song/Restore/${contract.id}`}
										icons={{ primary: 'ui-icon-trash' }}
									>
										{t('ViewRes:EntryEdit.Restore')}
									</JQueryUIButton>
								)
							) : (
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={songEditStore.deleteStore.show}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:Shared.Delete')}
								</JQueryUIButton>
							))}
						{loginManager.canMergeEntries && (
							<>
								{' '}
								&nbsp;{' '}
								<JQueryUIButton as={Link} to={`/Song/Merge/${contract.id}`}>
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

				{songEditStore.errors && (
					<ValidationSummaryPanel
						message="Unable to save properties." /* LOC */
						errors={songEditStore.errors}
					/>
				)}

				<EntryValidationMessage
					draft={contract.status === EntryStatus.Draft}
					validationMessages={([] as string[]).concat(
						songEditStore.validationError_duplicateArtist
							? t('VocaDb.Model.Resources:SongValidationErrors.DuplicateArtist')
							: [],
						songEditStore.validationError_unspecifiedNames
							? t(
									'VocaDb.Model.Resources:SongValidationErrors.UnspecifiedNames',
							  )
							: [],
						songEditStore.validationError_needArtist
							? t('VocaDb.Model.Resources:SongValidationErrors.NeedArtist')
							: [],
						songEditStore.validationError_needOriginal
							? t('VocaDb.Model.Resources:SongValidationErrors.NeedOriginal')
							: [],
						songEditStore.validationError_needProducer
							? t('VocaDb.Model.Resources:SongValidationErrors.NeedProducer')
							: [],
						songEditStore.validationError_needReferences
							? t('VocaDb.Model.Resources:SongValidationErrors.NeedReferences')
							: [],
						songEditStore.validationError_needType
							? t('VocaDb.Model.Resources:SongValidationErrors.NeedType')
							: [],
						songEditStore.validationError_nonInstrumentalSongNeedsVocalists
							? t(
									'VocaDb.Model.Resources:SongValidationErrors.NonInstrumentalSongNeedsVocalists',
							  )
							: [],
						songEditStore.validationError_redundantEvent
							? t('VocaDb.Model.Resources:SongValidationErrors.RedundantEvent')
							: [],
					)}
					helpSection="glsongs"
				/>

				<br />
				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						if (
							songEditStore.hasValidationErrors &&
							songEditStore.status !== EntryStatus.Draft &&
							window.confirm(t('ViewRes:EntryEdit.SaveWarning')) === false
						) {
							return;
						}

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const id = await songEditStore.submit(requestToken);

							navigate(
								`${EntryUrlMapper.details(EntryType.Song, id)}?${qs.stringify({
									albumId: songEditStore.albumId,
								})}`,
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
						backAction={EntryUrlMapper.details(EntryType.Song, contract.id)}
						submitting={songEditStore.submitting}
					/>

					<JQueryUITabs>
						<JQueryUITab
							eventKey="basicInfo"
							title={t('ViewRes:EntryEdit.BasicInfo')}
						>
							<BasicInfoTabContent songEditStore={songEditStore} />
						</JQueryUITab>

						<JQueryUITab
							eventKey="artists"
							title={t('ViewRes.Song:Edit.TabArtists')}
						>
							<ArtistsTabContent songEditStore={songEditStore} />
						</JQueryUITab>

						<JQueryUITab eventKey="pvs" title={t('ViewRes.Song:Edit.TabMedia')}>
							<PVsTabContent songEditStore={songEditStore} />
						</JQueryUITab>

						{loginManager.canViewLyrics && (
							<JQueryUITab
								eventKey="lyrics"
								title={t('ViewRes.Song:Edit.TabLyrics')}
							>
								<LyricsTabContent
									songEditStore={songEditStore}
									lyricsForSongListEditStore={songEditStore.lyrics}
								/>
							</JQueryUITab>
						)}
					</JQueryUITabs>
					<br />

					<p>{t('ViewRes:EntryEdit.UpdateNotes')}</p>
					<textarea
						value={songEditStore.updateNotes}
						onChange={(e): void =>
							runInAction(() => {
								songEditStore.updateNotes = e.target.value;
							})
						}
						className="input-xxlarge"
						rows={4}
						maxLength={200}
					/>

					<br />
					<SaveAndBackBtn
						backAction={EntryUrlMapper.details(EntryType.Song, contract.id)}
						submitting={songEditStore.submitting}
					/>
				</form>

				<div>
					<ArtistRolesEditViewModel
						artistRolesEditStore={songEditStore.artistRolesEditStore}
					/>
				</div>

				<div>
					<CustomNameEdit
						customNameEditStore={songEditStore.editedArtistLink}
					/>
				</div>

				<EntryDeletePopup
					confirmText={t('ViewRes:EntryEdit.ConfirmDelete')}
					deleteEntryStore={songEditStore.deleteStore}
					onDelete={(): void =>
						navigate(EntryUrlMapper.details(EntryType.Song, contract.id))
					}
				/>
			</Layout>
		);
	},
);

const SongEdit = (): React.ReactElement => {
	const vdb = useVdb();
	const loginManager = useLoginManager();

	const { t } = useTranslation(['Resources']);

	const artistRoleNames = React.useMemo(
		() =>
			Object.fromEntries(
				vdb.values.artistRoles.map((artistRole): [
					string,
					string | undefined,
				] => [artistRole, t(`Resources:ArtistRoleNames.${artistRole}`)]),
			),
		[vdb, t],
	);

	const { id } = useParams();
	const [searchParams] = useSearchParams();
	const albumId = searchParams.get('albumId');

	const [model, setModel] = React.useState<{ songEditStore: SongEditStore }>();

	React.useEffect(() => {
		songRepo
			.getForEdit({ id: Number(id) })
			.then((model) =>
				setModel({
					songEditStore: new SongEditStore(
						vdb.values,
						antiforgeryRepo,
						songRepo,
						artistRepo,
						pvRepo,
						eventRepo,
						urlMapper,
						artistRoleNames,
						model,
						loginManager.canBulkDeletePVs,
						vdb.values.instrumentalTagId,
						albumId ? Number(albumId) : undefined,
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
	}, [vdb, loginManager, artistRoleNames, id, albumId]);

	return model ? <SongEditLayout songEditStore={model.songEditStore} /> : <></>;
};

export default SongEdit;
