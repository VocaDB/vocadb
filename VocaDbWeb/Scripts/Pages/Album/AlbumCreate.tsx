import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistAutoComplete } from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { DuplicateEntriesMessage } from '@/Components/Shared/KnockoutPartials/DuplicateEntriesMessage';
import { Layout } from '@/Components/Shared/Layout';
import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { AlbumTypeDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { RequiredField } from '@/Components/Shared/Partials/Shared/RequiredField';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { AlbumCreateStore } from '@/Stores/Album/AlbumCreateStore';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const antiforgeryRepo = new AntiforgeryRepository(httpClient, urlMapper);
const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);

interface AlbumCreateLayoutProps {
	albumCreateStore: AlbumCreateStore;
}

const AlbumCreateLayout = observer(
	({ albumCreateStore }: AlbumCreateLayoutProps): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.Album']);

		const title = t('ViewRes.Album:Create.SubmitAlbum');

		useVdbTitle(title, ready);

		const navigate = useNavigate();

		return (
			<Layout
				title={title}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Album`,
							}}
						>
							{t('ViewRes:Shared.Albums')}
						</Breadcrumb.Item>
					</>
				}
			>
				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const id = await albumCreateStore.submit(requestToken);

							navigate(`/Album/Edit/${id}`);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: t('ViewRes.Album:Create.UnableToCreateAlbum'),
							);

							throw error;
						}
					}}
				>
					{albumCreateStore.errors && (
						<ValidationSummaryPanel
							message={t('ViewRes.Album:Create.UnableToCreateAlbum')}
							errors={albumCreateStore.errors}
						/>
					)}

					<div className="row-fluid">
						<div className="span5 well well-transparent">
							<div className="editor-label">
								{t('ViewRes:EntryCreate.Name')} <RequiredField />
							</div>
							<div className="editor-field">
								{albumCreateStore.errors && albumCreateStore.errors.names && (
									<span className="field-validation-error">
										{albumCreateStore.errors.names}
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
													value={albumCreateStore.nameOriginal}
													onChange={(e): void =>
														runInAction(() => {
															albumCreateStore.nameOriginal = e.target.value;
														})
													}
													onBlur={albumCreateStore.checkDuplicates}
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
													value={albumCreateStore.nameRomaji}
													onChange={(e): void =>
														runInAction(() => {
															albumCreateStore.nameRomaji = e.target.value;
														})
													}
													onBlur={albumCreateStore.checkDuplicates}
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
													value={albumCreateStore.nameEnglish}
													onChange={(e): void =>
														runInAction(() => {
															albumCreateStore.nameEnglish = e.target.value;
														})
													}
													onBlur={albumCreateStore.checkDuplicates}
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
								<label htmlFor="discType">
									{t('ViewRes.Album:Create.DiscType')}
								</label>
							</div>
							<div className="editor-field">
								<AlbumTypeDropdownList
									id="discType"
									value={albumCreateStore.discType}
									onChange={(e): void =>
										runInAction(() => {
											albumCreateStore.discType = e.target.value as AlbumType;
										})
									}
								/>
							</div>

							<div className="editor-label">
								<span>{t('ViewRes.Album:Create.ArtistsInfo')}</span>{' '}
								<RequiredField />
								<br />
								<span className="extraInfo">
									{vdb.resources.album.newAlbumArtistDesc}
								</span>
							</div>
							<div className="editor-field">
								{albumCreateStore.errors && albumCreateStore.errors.artists && (
									<span className="field-validation-error">
										{albumCreateStore.errors.artists}
									</span>
								)}
								<table>
									<tbody>
										{albumCreateStore.artists.map((artist, index) => (
											<tr key={index}>
												<td>
													<ArtistLink artist={artist} tooltip />
												</td>
												<td>
													<SafeAnchor
														onClick={(): void =>
															albumCreateStore.removeArtist(artist)
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
								<br />
								<ArtistAutoComplete
									type="text"
									properties={{
										acceptSelection: albumCreateStore.addArtist,
									}}
									maxLength={128}
									placeholder={t('ViewRes:Shared.Search')}
									className="span8"
								/>
							</div>

							<br />
							<Button
								type="submit"
								disabled={albumCreateStore.submitting}
								variant="primary"
							>
								{t('ViewRes:Shared.Save')}
							</Button>
						</div>

						<div className="span4">
							<Alert>
								<span
									dangerouslySetInnerHTML={{
										__html: vdb.resources.album.newAlbumInfo ?? '',
									}}
								/>
							</Alert>
							<Alert variant="info">
								<p>{t('ViewRes.Album:Create.AlbumInfo2')}</p>
								<p>{t('ViewRes:EntryCreate.NoArtistsToName')}</p>
								<p>{t('ViewRes:EntryCreate.NameHelp')}</p>
							</Alert>

							<DuplicateEntriesMessage
								dupeEntries={albumCreateStore.dupeEntries}
							/>
						</div>
					</div>
				</form>
			</Layout>
		);
	},
);

const AlbumCreate = (): React.ReactElement => {
	const [albumCreateStore] = React.useState(
		() => new AlbumCreateStore(vdb.values, albumRepo, artistRepo),
	);

	return <AlbumCreateLayout albumCreateStore={albumCreateStore} />;
};

export default AlbumCreate;
