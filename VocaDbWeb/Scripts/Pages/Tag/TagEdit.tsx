import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { TagAutoComplete } from '@/Components/KnockoutExtensions/TagAutoComplete';
import { TagCategoryAutoComplete } from '@/Components/KnockoutExtensions/TagCategoryAutoComplete';
import { NamesEditor } from '@/Components/Shared/KnockoutPartials/NamesEditor';
import { Layout } from '@/Components/Shared/Layout';
import { EnglishTranslatedStringEdit } from '@/Components/Shared/Partials/EnglishTranslatedStringEdit';
import { EntryDeletePopup } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopup';
import { EntryTrashPopup } from '@/Components/Shared/Partials/EntryDetails/EntryTrashPopup';
import {
	LanguageSelectionDropdownList,
	EntryStatusDropdownList,
} from '@/Components/Shared/Partials/Knockout/DropdownList';
import { EntryValidationMessage } from '@/Components/Shared/Partials/Knockout/EntryValidationMessage';
import { TagLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/TagLockingAutoComplete';
import { WebLinksEditViewKnockout } from '@/Components/Shared/Partials/Knockout/WebLinksEditViewKnockout';
import { ConcurrentEditWarning } from '@/Components/Shared/Partials/Shared/ConcurrentEditWarning';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { ImageUploadMessage } from '@/Components/Shared/Partials/Shared/ImageUploadMessage';
import { MarkdownNotice } from '@/Components/Shared/Partials/Shared/MarkdownNotice';
import { RequiredField } from '@/Components/Shared/Partials/Shared/RequiredField';
import { SaveAndBackBtn } from '@/Components/Shared/Partials/Shared/SaveAndBackBtn';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { useConflictingEditor } from '@/Components/useConflictingEditor';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { UrlHelper } from '@/Helpers/UrlHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { LoginManager } from '@/Models/LoginManager';
import { TagTargetTypes } from '@/Models/Tags/TagTargetTypes';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { TagEditStore } from '@/Stores/Tag/TagEditStore';
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
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

const allTagTargetTypes: TagTargetTypes[] = [
	TagTargetTypes.Album,
	TagTargetTypes.Artist,
	TagTargetTypes.Song,
	TagTargetTypes.Event,
];

interface TagEditLayoutProps {
	tagEditStore: TagEditStore;
}

const TagEditLayout = observer(
	({ tagEditStore }: TagEditLayoutProps): React.ReactElement => {
		const { t } = useTranslation([
			'Resources',
			'ViewRes',
			'VocaDb.Web.Resources.Views.Tag',
		]);

		const contract = tagEditStore.contract;

		const title = `Edit tag - ${contract.name}`; /* TODO: localize */

		useVocaDbTitle(title, true);

		const thumbUrl = UrlHelper.imageThumb(
			contract.mainPicture,
			ImageSize.SmallThumb,
		);

		const navigate = useNavigate();

		const conflictingEditor = useConflictingEditor(EntryType.Tag);

		const thumbPicUploadRef = React.useRef<HTMLInputElement>(undefined!);
		// HACK
		const categoryNameRef = React.useRef<HTMLInputElement>(undefined!);

		return (
			<Layout
				title={title}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: '/Tag',
							}}
							divider
						>
							Tags{/* TODO: localize */}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details_tag(contract.id),
							}}
						>
							{contract.name}
						</Breadcrumb.Item>
					</>
				}
				toolbar={
					contract.canDelete && (
						<>
							{contract.deleted ? (
								<JQueryUIButton
									as="a"
									href={`/Tag/Restore/${contract.id}`}
									icons={{ primary: 'ui-icon-trash' }}
								>
									{t('ViewRes:EntryEdit.Restore')}
								</JQueryUIButton>
							) : (
								<JQueryUIButton
									as={SafeAnchor}
									href="#"
									onClick={tagEditStore.deleteStore.show}
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
										onClick={tagEditStore.trashStore.show}
										icons={{ primary: 'ui-icon-trash' }}
									>
										{t('ViewRes:EntryEdit.MoveToTrash')}
									</JQueryUIButton>
								</>
							)}
							{loginManager.canMergeEntries && (
								<>
									{' '}
									<JQueryUIButton as="a" href={`/Tag/Merge/${contract.id}`}>
										{t('ViewRes:EntryEdit.Merge')}
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

				{tagEditStore.errors && (
					<ValidationSummaryPanel
						message="Unable to save properties." /* TODO: localize */
						errors={tagEditStore.errors}
					/>
				)}

				<EntryValidationMessage
					draft={contract.status === EntryStatus[EntryStatus.Draft]}
					validationMessages={([] as string[]).concat(
						tagEditStore.validationError_needDescription
							? t(
									'VocaDb.Web.Resources.Views.Tag:Edit.ValidationNeedDescription',
							  )
							: [],
					)}
				/>

				<br />
				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const thumbPicUpload =
								thumbPicUploadRef.current.files?.item(0) ?? undefined;

							const id = await tagEditStore.submit(
								requestToken,
								categoryNameRef.current.value,
								thumbPicUpload,
							);

							navigate(EntryUrlMapper.details_tag(id));
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save properties.' /* TODO: localize */,
							);

							throw e;
						}
					}}
				>
					<SaveAndBackBtn
						backAction={EntryUrlMapper.details_tag(contract.id)}
						submitting={tagEditStore.submitting}
					/>

					<div className="editor-label">
						<HelpLabel
							label="Default language" /* TODO: localize */
							dangerouslySetInnerHTML={{ __html: '' }}
						/>
					</div>
					<div className="editor-field">
						<LanguageSelectionDropdownList
							value={tagEditStore.defaultNameLanguage}
							onChange={(e): void =>
								runInAction(() => {
									tagEditStore.defaultNameLanguage = e.target.value;
								})
							}
						/>
					</div>

					<div className="editor-label">
						<HelpLabel
							label="Names" /* TODO: localize */
							dangerouslySetInnerHTML={{ __html: '' }}
						/>{' '}
						<RequiredField />
					</div>
					<div className="editor-field">
						<NamesEditor namesEditStore={tagEditStore.names} />
					</div>

					<div className="editor-label">
						<label>Category{/* TODO: localize */}</label>
					</div>
					<div className="editor-field">
						<TagCategoryAutoComplete
							type="text"
							maxLength={30}
							onAcceptSelection={(): void => {}}
							clearValue={false}
							ref={categoryNameRef}
							defaultValue={contract.categoryName}
						/>
						{/* TODO */}
					</div>

					<div className="editor-label">
						<HelpLabel
							label="Parent" /* TODO: localize */
							dangerouslySetInnerHTML={{
								__html:
									'Parent tag groups related tags under one parent. Child tags are still considered separate.' /* TODO: localize */,
							}}
						/>
					</div>
					<div className="editor-field">
						<TagLockingAutoComplete
							basicEntryLinkStore={tagEditStore.parent}
							tagFilter={tagEditStore.denySelf}
							clearValue={true}
							allowAliases={false}
						/>
					</div>

					<div className="editor-label">
						<label>Description{/* TODO: localize */}</label>
						<MarkdownNotice />
					</div>
					<div className="editor-field entry-edit-description">
						<EnglishTranslatedStringEdit
							englishTranslatedStringEditStore={tagEditStore.description}
						/>
					</div>

					<div className="editor-label">Thumbnail</div>
					<div className="editor-field">
						<div className="media">
							{thumbUrl && (
								<img
									className="pull-left media-object"
									src={thumbUrl}
									alt="Thumb" /* TODO: localize */
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

					<div
						className="editor-field withMargin"
						title="Hides this tag from suggested tags for albums and artists" /* TODO: localize */
					>
						<input
							type="checkbox"
							id="hideFromSuggestions"
							checked={tagEditStore.hideFromSuggestions}
							onChange={(e): void =>
								runInAction(() => {
									tagEditStore.hideFromSuggestions = e.target.checked;
								})
							}
						/>{' '}
						Hide from suggestions{/* TODO: localize */}
					</div>

					<div className="editor-label withMargin">
						{t('VocaDb.Web.Resources.Views.Tag:Edit.RelatedTags')}
					</div>
					<div className="editor-field">
						<table>
							<tbody>
								{tagEditStore.relatedTags.map((tag, index) => (
									<tr key={index}>
										<td>
											<Link
												to={EntryUrlMapper.details_tag(
													tag.id,
												)} /* TODO: target="_blank" */
											>
												{tag.name}
											</Link>
										</td>
										<td>
											<SafeAnchor
												onClick={(): void => tagEditStore.removeRelatedTag(tag)}
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

						<TagAutoComplete
							type="text"
							onAcceptSelection={(entry): void => {
								tagEditStore.addRelatedTag(entry);
							}}
							tagFilter={tagEditStore.allowRelatedTag}
							maxLength={128}
							placeholder={t('ViewRes:Shared.Search')}
							className="input-xlarge"
						/>
					</div>

					<div className="editor-label">Valid for</div>
					<div className="editor-field">
						{allTagTargetTypes.map((entryType) => (
							<React.Fragment key={entryType}>
								<input
									type="checkbox"
									checked={tagEditStore.hasTargetType(entryType)}
									onChange={(e): void =>
										runInAction(() => {
											tagEditStore.setTargetType(entryType, e.target.checked);
										})
									}
								/>{' '}
								{TagTargetTypes[entryType] /* TODO: localize */}
								<br />
							</React.Fragment>
						))}
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
							webLinksEditStore={tagEditStore.webLinks}
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
							value={tagEditStore.status}
							onChange={(e): void =>
								runInAction(() => {
									tagEditStore.status = e.target.value;
								})
							}
						/>
					</div>

					<br />
					<p>{t('ViewRes:EntryEdit.UpdateNotes')}</p>
					<textarea
						name="updateNotes"
						id="updateNotes"
						className="input-xxlarge"
						rows={4}
						maxLength={200}
						value={tagEditStore.updateNotes}
						onChange={(e): void =>
							runInAction(() => {
								tagEditStore.updateNotes = e.target.value;
							})
						}
					/>

					<br />
					<SaveAndBackBtn
						backAction={EntryUrlMapper.details_tag(contract.id)}
						submitting={tagEditStore.submitting}
					/>
				</form>

				<EntryDeletePopup
					confirmText={t('ViewRes:EntryEdit.ConfirmDelete')}
					deleteEntryStore={tagEditStore.deleteStore}
				/>
				<EntryTrashPopup
					confirmText={t('ViewRes:EntryEdit.ConfirmMoveToTrash')}
					deleteEntryStore={tagEditStore.trashStore}
				/>
			</Layout>
		);
	},
);

const TagEdit = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<{ tagEditStore: TagEditStore }>();

	React.useEffect(() => {
		tagRepo
			.getForEdit({ id: Number(id) })
			.then((model) =>
				setModel({ tagEditStore: new TagEditStore(tagRepo, urlMapper, model) }),
			)
			.catch((error) => {
				if (error.response) {
					if (error.response.status === 404)
						window.location.href = '/Error/NotFound';
				}

				throw error;
			});
	}, [id]);

	return model ? <TagEditLayout tagEditStore={model.tagEditStore} /> : <></>;
};

export default TagEdit;
