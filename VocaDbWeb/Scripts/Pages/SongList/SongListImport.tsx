import Alert from '@/Bootstrap/Alert';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { ImportSongListStore } from '@/Stores/SongList/ImportSongListStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';

const importSongListStore = new ImportSongListStore(httpClient, urlMapper);

const SongListImport = observer(
	(): React.ReactElement => {
		const { t, ready } = useTranslation([
			'ViewRes',
			'VocaDb.Web.Resources.Views.SongList',
		]);

		const title = t('VocaDb.Web.Resources.Views.SongList:Import.Title');

		useVdbTitle(title, ready);

		const navigate = useNavigate();

		return (
			<Layout title={title}>
				<form className="form-horizontal">
					<div className="control-group">
						<div className="control-label">
							<HelpLabel
								label={t('VocaDb.Web.Resources.Views.SongList:Import.UrlLabel')}
								dangerouslySetInnerHTML={{
									__html: t(
										'VocaDb.Web.Resources.Views.SongList:Import.UrlDescription',
									),
								}}
							/>
						</div>
						<div className="controls">
							<input
								type="text"
								value={importSongListStore.url}
								onChange={(e): void =>
									runInAction(() => {
										importSongListStore.url = e.target.value;
									})
								}
								size={60}
								className="input-xlarge"
								required
							/>
						</div>
					</div>
					<div className="control-group">
						<div className="controls">
							<label className="checkbox inline">
								<label>
									<input
										type="checkbox"
										checked={importSongListStore.onlyRanked}
										onChange={(e): void =>
											runInAction(() => {
												importSongListStore.onlyRanked = e.target.checked;
											})
										}
									/>
									{t('VocaDb.Web.Resources.Views.SongList:Import.OnlyRanked')}
								</label>
							</label>
						</div>
					</div>

					<div className="control-group">
						<div className="controls">
							<Button
								variant="primary"
								onClick={async (): Promise<void> => {
									try {
										await importSongListStore.parse();
									} catch (error: any) {
										if (error.response && error.response.data)
											alert(error.response.data);

										throw error;
									}
								}}
								disabled={!importSongListStore.url}
							>
								{t('VocaDb.Web.Resources.Views.SongList:Import.Process')}
							</Button>
						</div>
					</div>
				</form>

				{importSongListStore.parsed && (
					<div>
						<form className="form-horizontal">
							<h4>
								{t(
									'VocaDb.Web.Resources.Views.SongList:Import.MylistDescription',
								)}
							</h4>
							<div className="control-group">
								<div className="control-label">
									{t('VocaDb.Web.Resources.Views.SongList:Import.ListName')}
								</div>
								<div className="controls">
									<input
										type="text"
										value={importSongListStore.name}
										onChange={(e): void =>
											runInAction(() => {
												importSongListStore.name = e.target.value;
											})
										}
										className="input-xlarge"
										required
									/>
								</div>
							</div>
							<div className="control-group">
								<div className="control-label">
									{t(
										'VocaDb.Web.Resources.Views.SongList:Import.ListDescription',
									)}
								</div>
								<div className="controls">
									<textarea
										value={importSongListStore.description}
										onChange={(e): void =>
											runInAction(() => {
												importSongListStore.description = e.target.value;
											})
										}
										cols={40}
										rows={3}
										className="input-xlarge"
									/>
								</div>
							</div>
							<div className="control-group">
								<div className="control-label">
									{t('VocaDb.Web.Resources.Views.SongList:Import.TotalSongs')}
								</div>
								<div className="controls">
									{importSongListStore.totalSongs}{' '}
									{importSongListStore.hasMore && (
										<small>
											{t(
												'VocaDb.Web.Resources.Views.SongList:Import.LoadMoreHelp',
											)}
										</small>
									)}
								</div>
							</div>
						</form>

						{importSongListStore.missingSongs && (
							<Alert>
								{t(
									'VocaDb.Web.Resources.Views.SongList:Import.SongsMissingError',
								)}
							</Alert>
						)}

						<h4 className="withMargin">
							{t('VocaDb.Web.Resources.Views.SongList:Import.SongsInList')}
						</h4>
						<table className="table table-condensed">
							<thead>
								<tr>
									<th>
										{t('VocaDb.Web.Resources.Views.SongList:Import.Order')}
									</th>
									<th>
										{t('VocaDb.Web.Resources.Views.SongList:Import.NicoPV')}
									</th>
									<th>
										{t(
											'VocaDb.Web.Resources.Views.SongList:Import.VocaDbEntry',
										)}
									</th>
								</tr>
							</thead>
							<tbody>
								{importSongListStore.items.map((item, index) => (
									<tr key={index}>
										<td>{item.sortIndex}</td>
										<td>
											<a href={item.url}>{item.name}</a>
										</td>
										<td>
											{item.matchedSong ? (
												<Link
													to={EntryUrlMapper.details(
														EntryType.Song,
														item.matchedSong.id,
													)}
												>
													{item.matchedSong.name}
												</Link>
											) : (
												<>
													<span>
														{t(
															'VocaDb.Web.Resources.Views.SongList:Import.SongMissing',
														)}
													</span>{' '}
													(
													<Link
														to={`/Song/Create?${qs.stringify({
															pvUrl: item.url,
														})}`}
													>
														{t(
															'VocaDb.Web.Resources.Views.SongList:Import.Submit',
														)}
													</Link>
													)
												</>
											)}
										</td>
									</tr>
								))}
							</tbody>
						</table>

						{importSongListStore.hasMore && (
							<h4>
								<SafeAnchor
									href="#"
									onClick={async (): Promise<void> => {
										try {
											await importSongListStore.loadMore();
										} catch (error: any) {
											if (error.response && error.response.data)
												alert(error.response.data);

											throw error;
										}
									}}
								>
									{t('ViewRes:Shared.ShowMore')}
								</SafeAnchor>{' '}
								&nbsp;{' '}
								<small>
									({importSongListStore.items.length} /{' '}
									{importSongListStore.totalSongs}{' '}
									{t('VocaDb.Web.Resources.Views.SongList:Import.ItemsLoaded')})
								</small>
							</h4>
						)}

						<br />
						<Button
							variant="primary"
							onClick={async (): Promise<void> => {
								const listId = await importSongListStore.submit();

								navigate(EntryUrlMapper.details(EntryType.SongList, listId));
							}}
							disabled={
								!importSongListStore.name || importSongListStore.submitting
							}
						>
							{t('VocaDb.Web.Resources.Views.SongList:Import.Accept')}
						</Button>
					</div>
				)}
			</Layout>
		);
	},
);

export default SongListImport;
