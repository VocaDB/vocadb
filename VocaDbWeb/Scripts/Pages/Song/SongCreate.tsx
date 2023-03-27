import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistAutoComplete } from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { CoverArtistMessage } from '@/Components/Shared/KnockoutPartials/CoverArtistMessage';
import { DuplicateEntriesMessage } from '@/Components/Shared/KnockoutPartials/DuplicateEntriesMessage';
import { Layout } from '@/Components/Shared/Layout';
import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { SongTypeDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { SongLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/SongLockingAutoComplete';
import { EntryLink } from '@/Components/Shared/Partials/Shared/EntryLink';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { RequiredField } from '@/Components/Shared/Partials/Shared/RequiredField';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { SongHelper } from '@/Helpers/SongHelper';
import { useLoginManager } from '@/LoginManagerContext';
import { SongType } from '@/Models/Songs/SongType';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { tagRepo } from '@/Repositories/TagRepository';
import { SongCreateStore } from '@/Stores/Song/SongCreateStore';
import { useVdb } from '@/VdbContext';
import { getReasonPhrase } from 'http-status-codes';
import { truncate } from 'lodash-es';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';

interface SongCreateLayoutProps {
	songCreateStore: SongCreateStore;
}

const SongCreateLayout = observer(
	({ songCreateStore }: SongCreateLayoutProps): React.ReactElement => {
		const vdb = useVdb();
		const loginManager = useLoginManager();

		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.Song']);

		const title = t('ViewRes.Song:Create.SubmitSong');

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
								to: `/Song`,
							}}
						>
							{t('ViewRes:Shared.Songs')}
						</Breadcrumb.Item>
					</>
				}
			>
				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const requestToken = await antiforgeryRepo.getToken();

							const id = await songCreateStore.submit(requestToken);

							navigate(`/Song/Edit/${id}`);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: t('ViewRes.Song:Create.UnableToCreateSong'),
							);

							throw error;
						}
					}}
				>
					{songCreateStore.errors && (
						<ValidationSummaryPanel
							message={t('ViewRes.Song:Create.UnableToCreateSong')}
							errors={songCreateStore.errors}
						/>
					)}

					<div className="row-fluid">
						<div className="span5 well well-transparent">
							{/* TODO: AjaxLoader */}

							{songCreateStore.isDuplicatePV && (
								<Alert variant="danger">
									{t('ViewRes.Song:Create.DuplicatePV')}
								</Alert>
							)}

							<div className="editor-label">
								<label htmlFor="pvUrl">
									{t('ViewRes.Song:Create.OriginalPV')}
								</label>
							</div>
							<div className="editor-field">
								<input
									type="text"
									id="pvUrl"
									value={songCreateStore.pv1}
									onChange={(e): void =>
										runInAction(() => {
											songCreateStore.pv1 = e.target.value;
										})
									}
									onBlur={songCreateStore.checkDuplicatesAndPV}
									className="span8"
									maxLength={255}
									size={30}
								/>
								{/* TODO: ValidationMessageFor */}
							</div>

							{loginManager.canViewOtherPVs && (
								<>
									<div className="editor-label">
										<label htmlFor="reprintPVUrl">
											{t('ViewRes.Song:Create.ReprintPV')}
										</label>
									</div>
									<div className="editor-field">
										<input
											type="text"
											id="reprintPVUrl"
											value={songCreateStore.pv2}
											onChange={(e): void =>
												runInAction(() => {
													songCreateStore.pv2 = e.target.value;
												})
											}
											onBlur={songCreateStore.checkDuplicates}
											className="span8"
											maxLength={255}
											size={30}
										/>
										{/* TODO: ValidationMessageFor */}
									</div>
								</>
							)}

							<div className="editor-label">
								{t('ViewRes:EntryCreate.Name')} <RequiredField />
							</div>
							<div className="editor-field">
								{songCreateStore.errors && songCreateStore.errors.names && (
									<span className="field-validation-error">
										{songCreateStore.errors.names[0]}
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
													value={songCreateStore.nameOriginal}
													onChange={(e): void =>
														runInAction(() => {
															songCreateStore.nameOriginal = e.target.value;
														})
													}
													onBlur={songCreateStore.checkDuplicates}
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
													value={songCreateStore.nameRomaji}
													onChange={(e): void =>
														runInAction(() => {
															songCreateStore.nameRomaji = e.target.value;
														})
													}
													onBlur={songCreateStore.checkDuplicates}
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
													value={songCreateStore.nameEnglish}
													onChange={(e): void =>
														runInAction(() => {
															songCreateStore.nameEnglish = e.target.value;
														})
													}
													onBlur={songCreateStore.checkDuplicates}
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
								<label htmlFor="songType">
									{t('ViewRes.Song:Create.SongType')}
								</label>
							</div>
							<div className="editor-field">
								<SongTypeDropdownList
									id="songType"
									value={songCreateStore.songType}
									onChange={(e): void =>
										runInAction(() => {
											songCreateStore.songType = e.target.value as SongType;
										})
									}
								/>
							</div>

							{songCreateStore.canHaveOriginalVersion && (
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
										<div
											style={{ display: 'inline-block' }}
											className="input-append"
										>
											<SongLockingAutoComplete
												basicEntryLinkStore={songCreateStore.originalVersion}
												songTypes={SongHelper.originalVersionTypes}
											/>
										</div>
										{songCreateStore.originalVersion.isEmpty &&
											songCreateStore.originalSongSuggestions.length > 0 && (
												<div>
													<h4>
														{t('ViewRes.Song:Create.OriginalSuggestionsTitle')}
													</h4>
													<table>
														<tbody>
															{songCreateStore.originalSongSuggestions.map(
																(originalSongSuggestion, index) => (
																	<tr key={index}>
																		<td>
																			<EntryLink
																				entry={originalSongSuggestion.entry}
																				tooltip
																				/* TODO: target="_blank" */
																			>
																				{
																					originalSongSuggestion.entry.name
																						.displayName
																				}
																			</EntryLink>{' '}
																			(
																			<span>
																				{
																					originalSongSuggestion.entry
																						.entryTypeName
																				}
																			</span>
																			)
																			{originalSongSuggestion.entry
																				.artistString && (
																				<div>
																					<span>
																						{
																							originalSongSuggestion.entry
																								.artistString
																						}
																					</span>
																				</div>
																			)}
																		</td>
																		<td style={{ maxWidth: '150px' }}>
																			<SafeAnchor
																				className="textLink acceptLink"
																				href="#"
																				onClick={(): Promise<void> =>
																					songCreateStore.selectOriginal(
																						originalSongSuggestion,
																					)
																				}
																			>
																				{t('ViewRes.Song:Create.Select')}
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
								<span>{t('ViewRes.Song:Create.ArtistsInfo')}</span>{' '}
								<RequiredField />
								<br />
								<span className="extraInfo">
									{t('ViewRes.Song:Create.ArtistDesc')}
								</span>
							</div>
							<div className="editor-field">
								{songCreateStore.errors && songCreateStore.errors.artists && (
									<span className="field-validation-error">
										{songCreateStore.errors.artists[0]}
									</span>
								)}
								<table>
									<tbody>
										{songCreateStore.artists.map((artist, index) => (
											<tr key={index}>
												<td>
													<ArtistLink
														artist={artist}
														tooltip
														typeLabel
														/* TODO: target="_blank" */
													/>
												</td>
												<td>
													<SafeAnchor
														onClick={(): void =>
															songCreateStore.removeArtist(artist)
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
										acceptSelection: songCreateStore.addArtist,
										height: 300,
									}}
									maxLength={128}
									placeholder={t('ViewRes:Shared.Search')}
									className="span8"
								/>
							</div>

							<br />
							<p>
								<label className="checkbox">
									<input
										type="checkbox"
										checked={songCreateStore.draft}
										onChange={(e): void =>
											runInAction(() => {
												songCreateStore.draft = e.target.checked;
											})
										}
									/>
									{t('ViewRes.Song:Create.Draft')}
								</label>
							</p>

							<br />
							<Button
								type="submit"
								disabled={songCreateStore.submitting}
								variant="primary"
							>
								{t('ViewRes:Shared.Save')}
							</Button>
						</div>

						<div className="span4">
							<Alert variant="info" className="pre-line">
								<span
									dangerouslySetInnerHTML={{
										__html: vdb.resources.song.newSongInfo ?? '',
									}}
								/>
							</Alert>
							<Alert variant="info">
								<p>{t('ViewRes.Song:Create.NoArtistsToName')}</p>
								<p>{t('ViewRes.Song:Create.NameHelp')}</p>
								<p>{t('ViewRes.Song:Create.ArtistHelp')}</p>
							</Alert>

							{songCreateStore.coverArtists.length > 0 &&
								!songCreateStore.canHaveOriginalVersion && (
									<CoverArtistMessage
										coverArtists={songCreateStore.coverArtists}
									/>
								)}

							<DuplicateEntriesMessage
								dupeEntries={songCreateStore.dupeEntries}
							/>

							{songCreateStore.songTypeInfo && (
								<Alert variant="info">
									<h3>
										<Link to={songCreateStore.songTypeTagUrl!}>
											{songCreateStore.songTypeName}
										</Link>
									</h3>
									<Markdown>
										{truncate(songCreateStore.songTypeInfo, {
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

const SongCreate = (): React.ReactElement => {
	const vdb = useVdb();

	const [searchParams] = useSearchParams();
	const pvUrl = searchParams.get('pvUrl');

	const [songCreateStore] = React.useState(
		() =>
			new SongCreateStore(vdb.values, songRepo, artistRepo, tagRepo, {
				pvUrl: pvUrl ?? '',
			}),
	);

	return <SongCreateLayout songCreateStore={songCreateStore} />;
};

export default SongCreate;
