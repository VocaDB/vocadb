import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { ServerSidePaging } from '@/Components/Shared/Partials/Knockout/ServerSidePaging';
import { TagLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/TagLockingAutoComplete';
import { SaveBtn } from '@/Components/Shared/Partials/Shared/SaveBtn';
import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import { TagRepository } from '@/Repositories/TagRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { ManageTagMappingsStore } from '@/Stores/Admin/ManageTagMappingsStore';
import classNames from 'classnames';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { Link } from 'react-router-dom';

const httpClient = new HttpClient();

const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

const manageTagMappingsStore = new ManageTagMappingsStore(tagRepo);

const AdminManageTagMappings = observer(
	(): React.ReactElement => {
		const siteName = vdb.values.siteName;

		return (
			<Layout
				title="Manage NicoNicoDouga tag mappings" /* TODO: localize */
				parents={
					<>
						<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/Admin' }}>
							Manage{/* TODO: localize */}
						</Breadcrumb.Item>
					</>
				}
			>
				<Alert variant="info">
					These mappings will be used to automatically add tags from external
					video streaming service. Currently only NicoNicoDouga is supported.
					Tags will be mapped from the source system (NND) to target tag on{' '}
					{siteName}. Multiple tags from the source system may be mapped to a
					single tag on {siteName}, but one source tag can be mapped to only one
					target tag (additional mappings are ignored).
				</Alert>

				<form className="form-horizontal">
					<h3>New mapping{/* TODO: localize */}</h3>
					<div className="control-group">
						<label className="control-label" htmlFor="newSourceName">
							Source tag name{/* TODO: localize */}
						</label>
						<div className="controls">
							<input
								type="text"
								id="newSourceName"
								maxLength={200}
								value={manageTagMappingsStore.newSourceName}
								onChange={(e): void =>
									runInAction(() => {
										manageTagMappingsStore.newSourceName = e.target.value;
									})
								}
								placeholder="Tag name" /* TODO: localize */
							/>
						</div>
					</div>
					<div className="control-group">
						<label className="control-label">
							Target tag{/* TODO: localize */}
						</label>
						<div className="controls">
							<TagLockingAutoComplete
								basicEntryLinkStore={manageTagMappingsStore.newTargetTag}
							/>
						</div>
					</div>
					<div className="control-group">
						<div className="controls">
							<Button
								variant="primary"
								onClick={(): void => {
									if (
										manageTagMappingsStore.mappings.some(
											(m) =>
												m.tag.id === manageTagMappingsStore.newTargetTag.id &&
												m.sourceTag.toLowerCase() ===
													manageTagMappingsStore.newSourceName.toLowerCase(),
										)
									) {
										showErrorMessage(
											`Mapping already exists for source tag ${manageTagMappingsStore.newSourceName}` /* TODO: localize */,
										);
										return;
									}

									manageTagMappingsStore.addMapping();
								}}
							>
								Add{/* TODO: localize */}
							</Button>
						</div>
					</div>
				</form>

				<hr />

				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							await manageTagMappingsStore.save();

							showSuccessMessage('Saved' /* TODO: localize */);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save tag mappings.' /* TODO: localize */,
							);

							throw error;
						}

						await manageTagMappingsStore.loadMappings();
					}}
				>
					<h3>Mappings{/* TODO: localize */}</h3>

					<div className="input-append input-prepend">
						<span className="add-on">
							<i className="icon-search" />
						</span>
						<input
							type="text"
							value={manageTagMappingsStore.filter}
							onChange={(e): void =>
								runInAction(() => {
									manageTagMappingsStore.filter = e.target.value;
								})
							}
							placeholder="Search" /* TODO: localize */
						/>
						<Button
							variant="danger"
							onClick={(): void =>
								runInAction(() => {
									manageTagMappingsStore.filter = '';
								})
							}
							disabled={!manageTagMappingsStore.filter}
						>
							Clear{/* TODO: localize */}
						</Button>
					</div>

					<br />
					<br />

					<SaveBtn submitting={manageTagMappingsStore.submitting} />

					<ServerSidePaging pagingStore={manageTagMappingsStore.paging} />

					<table>
						<thead>
							<tr>
								<th>Source (NND){/* TODO: localize */}</th>
								<th>
									Target ({siteName}){/* TODO: localize */}
								</th>
								<th></th>
							</tr>
						</thead>
						<tbody>
							{manageTagMappingsStore.sortedMappingsPage.map((item, index) => (
								<tr
									className={classNames(
										item.isNew && 'row-new',
										item.isDeleted && 'row-deleted',
									)}
									key={index}
								>
									<td>
										<a
											className="extLink"
											href={manageTagMappingsStore.getSourceTagUrl(item)}
											target="_blank"
											rel="noreferrer"
										>
											{item.sourceTag}
										</a>
									</td>
									<td>
										<a
											className="extLink"
											href={manageTagMappingsStore.getTagUrl(item)}
											target="_blank"
											rel="noreferrer"
										>
											{item.tag.name}
										</a>
									</td>
									<td>
										<Button
											variant="danger"
											className="btn-small"
											onClick={item.deleteMapping}
											disabled={item.isDeleted}
										>
											Delete{/* TODO: localize */}
										</Button>
									</td>
								</tr>
							))}
						</tbody>
					</table>
					<br />

					<ServerSidePaging pagingStore={manageTagMappingsStore.paging} />

					<SaveBtn submitting={manageTagMappingsStore.submitting} />
				</form>
			</Layout>
		);
	},
);

export default AdminManageTagMappings;
