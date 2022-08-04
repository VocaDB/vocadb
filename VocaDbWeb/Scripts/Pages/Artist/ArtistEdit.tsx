import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistAutoComplete } from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { EntryPictureFileEdit } from '@/Components/Shared/KnockoutPartials/EntryPictureFileEdit';
import { NamesEditor } from '@/Components/Shared/KnockoutPartials/NamesEditor';
import { Layout } from '@/Components/Shared/Layout';
import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { ArtistTypesDropdownKnockout } from '@/Components/Shared/Partials/Artist/ArtistTypesDropdownKnockout';
import { EnglishTranslatedStringEdit } from '@/Components/Shared/Partials/EnglishTranslatedStringEdit';
import { EntryDeletePopup } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopup';
import { ArtistLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/ArtistLockingAutoComplete';
import {
	LanguageSelectionDropdownList,
	AssociatedArtistTypeDropdownList,
	EntryStatusDropdownList,
} from '@/Components/Shared/Partials/Knockout/DropdownList';
import { EntryValidationMessage } from '@/Components/Shared/Partials/Knockout/EntryValidationMessage';
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
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { ImageHelper } from '@/Helpers/ImageHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import JQueryUIDatepicker from '@/JQueryUI/JQueryUIDatepicker';
import JQueryUITab from '@/JQueryUI/JQueryUITab';
import JQueryUITabs from '@/JQueryUI/JQueryUITabs';
import { ArtistLinkType } from '@/Models/Artists/ArtistLinkType';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { LoginManager } from '@/Models/LoginManager';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { ArtistEditStore } from '@/Stores/Artist/ArtistEditStore';
import _ from 'lodash';
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

interface BasicInfoTabContentProps {
	artistEditStore: ArtistEditStore;
	coverPicUploadRef: React.MutableRefObject<HTMLInputElement>;
}

const BasicInfoTabContent = observer(
	({
		artistEditStore,
		coverPicUploadRef,
	}: BasicInfoTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.Artist']);

		const contract = artistEditStore.contract;

		return (
			<div>
				<div className="editor-label">
					<label>{t('ViewRes:EntryEdit.DefaultLanguageSelection')}</label>
				</div>
				<div className="editor-field">
					<LanguageSelectionDropdownList
						value={artistEditStore.defaultNameLanguage}
						onChange={(e): void =>
							runInAction(() => {
								artistEditStore.defaultNameLanguage = e.target.value;
							})
						}
					/>
				</div>

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes.Artist:Edit.BaNames')}
						title={t('ViewRes:EntryEdit.NameHelp')}
					/>{' '}
					<RequiredField />
					{artistEditStore.validationError_unspecifiedNames && (
						<>
							{' '}
							<ValidationErrorIcon
								title={t(
									'VocaDb.Model.Resources:ArtistValidationErrors.UnspecifiedNames',
								)}
							/>
						</>
					)}
				</div>
				<div className="editor-field">
					<NamesEditor namesEditStore={artistEditStore.names} />
				</div>

				<div className="editor-label">
					<label>{t('ViewRes.Artist:Edit.BaMainPicture')}</label>
				</div>
				<div className="editor-field">
					<table>
						<tbody>
							<tr>
								<td>
									{/* eslint-disable-next-line jsx-a11y/img-redundant-alt */}
									<img
										src={`/Artist/PictureThumb/${contract.id}`}
										alt="Artist picture" /* TODO: localize */
										className="coverPic"
									/>
								</td>
								<td>
									<p>
										{t('ViewRes.Artist:Edit.BaPictureInfo', {
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
					<label className="inline-block">
						{t('ViewRes.Artist:Edit.BaDescription')}
					</label>{' '}
					<MarkdownNotice />
					{artistEditStore.validationError_needReferences && (
						<>
							{' '}
							<ValidationErrorIcon
								title={t(
									'VocaDb.Model.Resources:ArtistValidationErrors.NeedReferences',
								)}
							/>
						</>
					)}
				</div>
				<div className="editor-field entry-edit-description">
					<EnglishTranslatedStringEdit
						englishTranslatedStringEditStore={artistEditStore.description}
					/>
				</div>

				<div className="editor-label">
					<label>{t('ViewRes.Artist:Edit.BaArtistType')}</label>
				</div>
				<div className="editor-field">
					<ArtistTypesDropdownKnockout
						value={artistEditStore.artistType}
						onChange={(e): void =>
							runInAction(() => {
								artistEditStore.artistType = e.target.value as ArtistType;
							})
						}
					/>
					{artistEditStore.validationError_needType && (
						<>
							{' '}
							<ValidationErrorIcon
								title={t(
									'VocaDb.Model.Resources:ArtistValidationErrors.NeedType',
								)}
							/>
						</>
					)}
				</div>

				{artistEditStore.canHaveRelatedArtists && (
					<>
						<div className="editor-label">
							<HelpLabel
								label="Associated artists" /* TODO: localize */
								title="Artists related to this voicebank. These are inherited to derived voicebanks. Character designer only needs to be specified if it's different from the illustrator and manager only needs to be specified if it's different from the voice provider." /* TODO: localize */
							/>
						</div>
						<div className="editor-field">
							<table>
								<thead>
									<tr>
										<th>Role{/* TODO: localize */}</th>
										<th>Artist{/* TODO: localize */}</th>
										<th />
									</tr>
								</thead>
								<tbody>
									<tr>
										<td>
											<HelpLabel
												label="Illustrator" /* TODO: localize */
												title="Person who illustrated the character design. This is inherited to derived voicebanks." /* TODO: localize */
											/>
										</td>
										<td>
											<ArtistLockingAutoComplete
												basicEntryLinkStore={artistEditStore.illustrator}
												properties={{
													extraQueryParams: {
														artistTypes: [
															ArtistType.Unknown,
															ArtistType.Circle,
															ArtistType.Producer,
															ArtistType.Illustrator,
															ArtistType.Animator,
															ArtistType.Lyricist,
															ArtistType.OtherVocalist,
															ArtistType.OtherGroup,
															ArtistType.OtherIndividual,
															ArtistType.CoverArtist,
														].join(','),
													},
												}}
											/>
										</td>
										<td />
									</tr>
									<tr>
										<td>
											<HelpLabel
												label="Voice provider" /* TODO: localize */
												title="Person who provided their voice for this voicebank. This is inherited to derived voicebanks." /* TODO: localize */
											/>
										</td>
										<td>
											<ArtistLockingAutoComplete
												basicEntryLinkStore={artistEditStore.voiceProvider}
												properties={{
													extraQueryParams: {
														artistTypes: [
															ArtistType.Unknown,
															ArtistType.Producer,
															ArtistType.Illustrator,
															ArtistType.Animator,
															ArtistType.Lyricist,
															ArtistType.OtherVocalist,
															ArtistType.OtherIndividual,
															ArtistType.CoverArtist,
														].join(','),
													},
												}}
											/>
										</td>
									</tr>
									{artistEditStore.associatedArtists.map(
										(associatedArtist, index) => (
											<tr key={index}>
												<td>
													<AssociatedArtistTypeDropdownList
														value={associatedArtist.linkType}
														onChange={(e): void =>
															runInAction(() => {
																associatedArtist.linkType = e.target.value;
															})
														}
													/>
												</td>
												<td>
													<div className="input-append">
														<input
															type="text"
															readOnly
															value={associatedArtist.parent.name}
														/>
														<Button
															variant="danger"
															onClick={(): void =>
																runInAction(() => {
																	_.pull(
																		artistEditStore.associatedArtists,
																		associatedArtist,
																	);
																})
															}
														>
															Remove{/* TODO: localize */}
														</Button>
													</div>
												</td>
											</tr>
										),
									)}
									<tr>
										<td>
											<AssociatedArtistTypeDropdownList
												value={artistEditStore.newAssociatedArtistType}
												onChange={(e): void =>
													runInAction(() => {
														artistEditStore.newAssociatedArtistType = e.target
															.value as ArtistLinkType;
													})
												}
											/>
										</td>
										<td>
											<ArtistLockingAutoComplete
												basicEntryLinkStore={
													artistEditStore.newAssociatedArtist
												}
												properties={{
													extraQueryParams: {
														artistTypes: [
															ArtistType.Unknown,
															ArtistType.Circle,
															ArtistType.Producer,
															ArtistType.Illustrator,
															ArtistType.Animator,
															ArtistType.Lyricist,
															ArtistType.OtherVocalist,
															ArtistType.OtherGroup,
															ArtistType.OtherIndividual,
															ArtistType.CoverArtist,
														].join(','),
													},
												}}
											/>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
					</>
				)}

				{artistEditStore.canHaveCircles && (
					<>
						<div className="editor-label">
							<label>{t('ViewRes.Artist:Edit.BaGroups')}</label>
						</div>
						<div className="editor-field">
							<table>
								<tbody>
									{artistEditStore.groups.map((group, index) => (
										<tr key={index}>
											<td>
												<ArtistLink artist={group.parent} tooltip />
											</td>
											<td>
												<SafeAnchor
													onClick={(): void =>
														artistEditStore.removeGroup(group)
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

							<ArtistAutoComplete
								type="text"
								properties={{
									acceptSelection: artistEditStore.addGroup,
									extraQueryParams: {
										artistTypes: [
											ArtistType.Label,
											ArtistType.Circle,
											ArtistType.OtherGroup,
											ArtistType.Band,
										].join(','),
									},
									height: 300,
								}}
								maxLength={128}
								placeholder={t('ViewRes:Shared.Search')}
								className="input-xlarge"
							/>
						</div>
					</>
				)}

				{artistEditStore.allowBaseVoicebank && (
					<>
						<div className="editor-label">
							<label>{t('ViewRes.Artist:Edit.BaBaseVoicebank')}</label>
						</div>
						<div className="editor-field">
							<ArtistLockingAutoComplete
								basicEntryLinkStore={artistEditStore.baseVoicebank}
								properties={{
									extraQueryParams: {
										artistTypes: [
											ArtistType.Vocaloid,
											ArtistType.UTAU,
											ArtistType.CeVIO,
											ArtistType.SynthesizerV,
											ArtistType.OtherVocalist,
											ArtistType.OtherVoiceSynthesizer,
											ArtistType.Unknown,
										].join(','),
									},
									ignoreId: artistEditStore.contract.id,
								}}
							/>
						</div>
					</>
				)}

				{artistEditStore.canHaveReleaseDate && (
					<>
						<div className="editor-label">
							<label>{t('ViewRes.Artist:Edit.BaReleaseDate')}</label>
						</div>
						<div className="editor-field">
							<JQueryUIDatepicker
								type="text"
								value={artistEditStore.releaseDate}
								onSelect={(date): void =>
									runInAction(() => {
										artistEditStore.releaseDate = date;
									})
								}
								dateFormat="yy-mm-dd"
								maxLength={10}
							/>
						</div>
					</>
				)}

				<div className="editor-label">
					<HelpLabel
						label={t('ViewRes:EntryEdit.WebLinks')}
						title={t('ViewRes:EntryEdit.ExternalLinksQuickHelp')}
					/>
				</div>
				<div className="editor-field">
					<WebLinksEditViewKnockout
						webLinksEditStore={artistEditStore.webLinks}
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
						value={artistEditStore.status}
						onChange={(e): void =>
							runInAction(() => {
								artistEditStore.status = e.target.value;
							})
						}
					/>
				</div>
			</div>
		);
	},
);

interface AdditionalPicturesTabContentProps {
	artistEditStore: ArtistEditStore;
}

const AdditionalPicturesTabContent = observer(
	({
		artistEditStore,
	}: AdditionalPicturesTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Artist']);

		return (
			<>
				<p>{t('ViewRes.Artist:Edit.PiPicturesNote')}</p>
				<p>
					{t('ViewRes.Artist:Edit.BaPictureInfo', {
						0: ImageHelper.allowedExtensions.join(', '),
						1: ImageHelper.maxImageSizeMB,
					})}
				</p>

				<table>
					<tbody>
						{artistEditStore.pictures.pictures.map((picture, index) => (
							<EntryPictureFileEdit
								entryPictureFileListEditStore={artistEditStore.pictures}
								entryPictureFileEditStore={picture}
								key={index}
							/>
						))}
					</tbody>
				</table>

				<SafeAnchor
					onClick={artistEditStore.pictures.add}
					href="#"
					className="addLink textLink"
				>
					{t('ViewRes.Artist:Edit.PiAdd')}
				</SafeAnchor>
			</>
		);
	},
);

interface ArtistEditLayoutProps {
	artistEditStore: ArtistEditStore;
}

const ArtistEditLayout = observer(
	({ artistEditStore }: ArtistEditLayoutProps): React.ReactElement => {
		const { t, ready } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.Artist',
			'VocaDb.Model.Resources',
		]);

		const contract = artistEditStore.contract;

		const title = t('ViewRes.Artist:Edit.EditTitle', { 0: contract.name });

		useVocaDbTitle(title, ready);

		const conflictingEditor = useConflictingEditor(EntryType.Artist);

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
								to: '/Artist',
							}}
							divider
						>
							{t('ViewRes:Shared.Artists')}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details(EntryType.Artist, contract.id),
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
								<JQueryUIButton
									as="a"
									href={`/Artist/Restore/${contract.id}`}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:EntryEdit.Restore')}
								</JQueryUIButton>
							) : (
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={artistEditStore.deleteStore.show}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:Shared.Delete')}
								</JQueryUIButton>
							))}
						{loginManager.canMergeEntries && (
							<>
								{' '}
								&nbsp;{' '}
								<JQueryUIButton as="a" href={`/Artist/Merge/${contract.id}`}>
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

				{artistEditStore.errors && (
					<ValidationSummaryPanel
						message="Unable to save properties." /* TODO: localize */
						errors={artistEditStore.errors}
					/>
				)}

				<EntryValidationMessage
					draft={contract.status === EntryStatus[EntryStatus.Draft]}
					validationMessages={([] as string[])
						.concat(
							artistEditStore.validationError_unnecessaryPName
								? t(
										'VocaDb.Model.Resources:ArtistValidationErrors.UnnecessaryPName',
								  )
								: [],
						)
						.concat(
							artistEditStore.validationError_unspecifiedNames
								? t(
										'VocaDb.Model.Resources:ArtistValidationErrors.UnspecifiedNames',
								  )
								: [],
						)
						.concat(
							artistEditStore.validationError_needReferences
								? t(
										'VocaDb.Model.Resources:ArtistValidationErrors.NeedReferences',
								  )
								: [],
						)
						.concat(
							artistEditStore.validationError_needType
								? t('VocaDb.Model.Resources:ArtistValidationErrors.NeedType')
								: [],
						)}
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
							const pictureUpload = _.chain(
								document.getElementsByName('pictureUpload'),
							)
								.map((element) => (element as HTMLInputElement).files?.[0])
								.filter((file) => file !== undefined)
								.map((file) => file as File)
								.value();

							const id = await artistEditStore.submit(
								requestToken,
								coverPicUpload,
								pictureUpload,
							);

							navigate(EntryUrlMapper.details(EntryType.Artist, id));
						} catch (e) {
							showErrorMessage(
								'Unable to save properties.' /* TODO: localize */,
							);

							throw e;
						}
					}}
				>
					<SaveAndBackBtn
						backAction={EntryUrlMapper.details(EntryType.Artist, contract.id)}
						submitting={artistEditStore.submitting}
					/>

					<JQueryUITabs>
						<JQueryUITab
							eventKey="basicInfo"
							title={t('ViewRes:EntryEdit.BasicInfo')}
						>
							<BasicInfoTabContent
								artistEditStore={artistEditStore}
								coverPicUploadRef={coverPicUploadRef}
							/>
						</JQueryUITab>

						<JQueryUITab
							eventKey="pics"
							title={t('ViewRes.Artist:Edit.TabAdditionalPictures')}
						>
							<AdditionalPicturesTabContent artistEditStore={artistEditStore} />
						</JQueryUITab>
					</JQueryUITabs>
					<br />

					<p>{t('ViewRes:EntryEdit.UpdateNotes')}</p>
					<textarea
						value={artistEditStore.updateNotes}
						onChange={(e): void =>
							runInAction(() => {
								artistEditStore.updateNotes = e.target.value;
							})
						}
						className="input-xxlarge"
						rows={4}
						maxLength={200}
					/>

					<br />
					<SaveAndBackBtn
						backAction={EntryUrlMapper.details(EntryType.Artist, contract.id)}
						submitting={artistEditStore.submitting}
					/>
				</form>

				<EntryDeletePopup
					confirmText={t('ViewRes:EntryEdit.ConfirmDelete')}
					deleteEntryStore={artistEditStore.deleteStore}
				/>
			</Layout>
		);
	},
);

const ArtistEdit = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<{
		artistEditStore: ArtistEditStore;
	}>();

	React.useEffect(() => {
		artistRepo
			.getForEdit({ id: Number(id) })
			.then((model) =>
				setModel({
					artistEditStore: new ArtistEditStore(
						vdb.values,
						artistRepo,
						urlMapper,
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
	}, [id]);

	return model ? (
		<ArtistEditLayout artistEditStore={model.artistEditStore} />
	) : (
		<></>
	);
};

export default ArtistEdit;
