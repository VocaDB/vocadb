import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { DuplicateEntriesMessage } from '@/Components/Shared/KnockoutPartials/DuplicateEntriesMessage';
import { Layout } from '@/Components/Shared/Layout';
import { ArtistTypesDropdownKnockout } from '@/Components/Shared/Partials/Artist/ArtistTypesDropdownKnockout';
import { RequiredField } from '@/Components/Shared/Partials/Shared/RequiredField';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { ImageHelper } from '@/Helpers/ImageHelper';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { ArtistCreateStore } from '@/Stores/Artist/ArtistCreateStore';
import classNames from 'classnames';
import { getReasonPhrase } from 'http-status-codes';
import _ from 'lodash';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const antiforgeryRepo = new AntiforgeryRepository(httpClient, urlMapper);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

interface ArtistCreateLayoutProps {
	artistCreateStore: ArtistCreateStore;
}

const ArtistCreateLayout = observer(
	({ artistCreateStore }: ArtistCreateLayoutProps): React.ReactElement => {
		const { t, ready } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.Artist',
		]);

		const title = t('ViewRes.Artist:Create.AddArtist');

		useVocaDbTitle(title, ready);

		const navigate = useNavigate();

		const pictureUploadRef = React.useRef<HTMLInputElement>(undefined!);

		return (
			<Layout
				title={title}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Artist`,
							}}
						>
							{t('ViewRes:Shared.Artists')}
						</Breadcrumb.Item>
					</>
				}
			>
				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const pictureUpload =
								pictureUploadRef.current.files?.item(0) ?? undefined;

							const id = await artistCreateStore.submit(
								requestToken,
								pictureUpload,
							);

							navigate(`/Artist/Edit/${id}`);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: t('ViewRes.Artist:Create.UnableToCreateArtist'),
							);

							throw e;
						}
					}}
				>
					{artistCreateStore.errors && (
						<ValidationSummaryPanel
							message={t('ViewRes.Artist:Create.UnableToCreateArtist')}
							errors={artistCreateStore.errors}
						/>
					)}

					<div className="row-fluid">
						<div className="span5 well well-transparent">
							<div className="editor-label">
								{t('ViewRes.Artist:Create.Name')} <RequiredField />
							</div>

							<div className="editor-field">
								{artistCreateStore.errors && artistCreateStore.errors.names && (
									<span className="field-validation-error">
										{artistCreateStore.errors.names}
									</span>
								)}

								<table>
									<tbody>
										<tr>
											<td className="formLabel">
												<label htmlFor="nameOriginal">
													{t('ViewRes:EntryCreate.NonEnglishName')}
												</label>
											</td>
											<td>
												<input
													type="text"
													id="nameOriginal"
													value={artistCreateStore.nameOriginal}
													onChange={(e): void =>
														runInAction(() => {
															artistCreateStore.nameOriginal = e.target.value;
														})
													}
													onBlur={artistCreateStore.checkDuplicates}
													className="span12"
													maxLength={255}
													size={40}
												/>
											</td>
										</tr>

										<tr>
											<td className="formLabel">
												<label htmlFor="nameRomaji">
													{t('ViewRes:EntryCreate.RomajiName')}
												</label>
											</td>
											<td>
												<input
													type="text"
													id="nameRomaji"
													value={artistCreateStore.nameRomaji}
													onChange={(e): void =>
														runInAction(() => {
															artistCreateStore.nameRomaji = e.target.value;
														})
													}
													onBlur={artistCreateStore.checkDuplicates}
													className="span12"
													maxLength={255}
													size={40}
												/>
											</td>
										</tr>

										<tr>
											<td className="formLabel">
												<label htmlFor="nameEnglish">
													{t('ViewRes:EntryCreate.EnglishName')}
												</label>
											</td>
											<td>
												<input
													type="text"
													id="nameEnglish"
													value={artistCreateStore.nameEnglish}
													onChange={(e): void =>
														runInAction(() => {
															artistCreateStore.nameEnglish = e.target.value;
														})
													}
													onBlur={artistCreateStore.checkDuplicates}
													className="span12"
													maxLength={255}
													size={40}
												/>
											</td>
										</tr>
									</tbody>
								</table>
							</div>

							<div className="editor-label">
								<label htmlFor="ArtistType">
									{t('ViewRes.Artist:Create.ArtistType')}
								</label>
							</div>
							<div className="editor-field">
								<ArtistTypesDropdownKnockout
									id="ArtistType"
									value={artistCreateStore.artistType}
									onChange={(e): void =>
										runInAction(() => {
											artistCreateStore.artistType = e.target
												.value as ArtistType;
										})
									}
								/>
							</div>

							<div className="editor-label">
								<label htmlFor="Description">
									{t('ViewRes.Artist:Create.Description')}
								</label>
							</div>
							<div className="editor-field">
								<textarea
									id="Description"
									value={artistCreateStore.description}
									onChange={(e): void =>
										runInAction(() => {
											artistCreateStore.description = e.target.value;
										})
									}
									rows={7}
									cols={70}
									className={classNames(
										'span12',
										artistCreateStore.errors &&
											artistCreateStore.errors.description &&
											'input-validation-error',
									)}
								/>
								<br />
							</div>

							<div className="editor-label">
								{vdb.resources.artist.newArtistExternalLink}
							</div>
							<div className="editor-field">
								<table>
									<tbody>
										<tr>
											<td className="formLabel">
												{t('ViewRes.Artist:Create.WebLinkURL')}{' '}
												<RequiredField />
											</td>
											<td>
												<input
													type="text"
													id="WebLinkUrl"
													value={artistCreateStore.webLink.url}
													onChange={(e): void =>
														runInAction(() => {
															artistCreateStore.webLink.url = e.target.value;
														})
													}
													onBlur={artistCreateStore.checkDuplicates}
													className="input-xlarge"
													maxLength={512}
												/>
											</td>
										</tr>

										<tr>
											<td className="formLabel">
												{t('ViewRes.Artist:Create.WebLinkDescription')}{' '}
												{t('ViewRes.Artist:Create.Optional')}
											</td>
											<td>
												<input
													type="text"
													id="WebLinkDescription"
													value={artistCreateStore.webLink.description}
													onChange={(e): void =>
														runInAction(() => {
															artistCreateStore.webLink.description =
																e.target.value;
														})
													}
													className="input-xlarge"
													maxLength={512}
												/>
											</td>
										</tr>

										<tr>
											<td className="formLabel">
												{t('ViewRes.Artist:Create.WebLinkCategory')}
											</td>
											<td>
												<select
													id="WebLinkCategory"
													value={artistCreateStore.webLink.category}
													onChange={(e): void =>
														runInAction(() => {
															artistCreateStore.webLink.category = e.target
																.value as WebLinkCategory;
														})
													}
												>
													{Object.values(WebLinkCategory).map((value) => (
														<option value={value} key={value}>
															{t(`Resources:WebLinkCategoryNames.${value}`)}
														</option>
													))}
												</select>
											</td>
										</tr>
									</tbody>
								</table>
							</div>

							<div className="editor-label">
								{t('ViewRes.Artist:Create.Picture')}
							</div>
							<div className="editor-field">
								<p>
									{t('ViewRes:EntryCreate.PictureInfo', {
										0: ImageHelper.allowedExtensions.join(', '),
										1: ImageHelper.maxImageSizeMB,
									})}
								</p>
								<input
									type="file"
									id="pictureUpload"
									name="pictureUpload"
									ref={pictureUploadRef}
								/>
							</div>

							<br />
							<p>
								<label className="checkbox">
									<input
										id="Draft"
										checked={artistCreateStore.draft}
										onChange={(e): void =>
											runInAction(() => {
												artistCreateStore.draft = e.target.checked;
											})
										}
										type="checkbox"
									/>
									{t('ViewRes.Artist:Create.Draft')}
								</label>
							</p>

							<br />
							<Button
								type="submit"
								variant="primary"
								className={classNames(
									artistCreateStore.submitting && 'disabled',
								)}
							>
								{t('ViewRes:Shared.Save')}
							</Button>
						</div>
						<div className="span4">
							<Alert variant="info">
								{t('ViewRes.Artist:Create.ArtistInfo')}
							</Alert>
							<Alert variant="info">
								<p>{t('ViewRes.Artist:Create.NameHelp')}</p>
							</Alert>

							<DuplicateEntriesMessage
								dupeEntries={artistCreateStore.dupeEntries}
							/>

							{artistCreateStore.artistTypeInfo && (
								<Alert variant="info">
									<h3>
										<Link
											to={artistCreateStore.artistTypeTagUrl!}
											/* TODO: target="_blank" */
										>
											{artistCreateStore.artistTypeName}
										</Link>
									</h3>
									<Markdown>
										{_.truncate(artistCreateStore.artistTypeInfo, {
											length: 500,
										})}
									</Markdown>
								</Alert>
							)}
						</div>
					</div>
				</form>
			</Layout>
		);
	},
);

const ArtistCreate = (): React.ReactElement => {
	const [artistCreateStore] = React.useState(
		() => new ArtistCreateStore(vdb.values, artistRepo, tagRepo),
	);

	return <ArtistCreateLayout artistCreateStore={artistCreateStore} />;
};

export default ArtistCreate;
