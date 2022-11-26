import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { TagLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/TagLockingAutoComplete';
import { SaveBtn } from '@/Components/Shared/Partials/Shared/SaveBtn';
import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import { tagRepo } from '@/Repositories/TagRepository';
import { ManageEntryTagMappingsStore } from '@/Stores/Admin/ManageEntryTagMappingsStore';
import classNames from 'classnames';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { Link } from 'react-router-dom';

const manageEntryTagMappingsStore = new ManageEntryTagMappingsStore(tagRepo);

const AdminManageEntryTagMappings = observer(
	(): React.ReactElement => {
		return (
			<Layout
				pageTitle={undefined}
				ready={true}
				title="Manage entry type to tag mappings" /* LOC */
				parents={
					<>
						<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/Admin' }}>
							Manage{/* LOC */}
						</Breadcrumb.Item>
					</>
				}
			>
				<Alert variant="info">
					Only one tag can be mapped to entry type / sub type combination.
					{/* LOC */}
				</Alert>

				<form className="form-horizontal">
					<h3>New mapping{/* LOC */}</h3>
					<div className="control-group">
						<label className="control-label" htmlFor="newSourceName">
							Entry type{/* LOC */}
						</label>
						<div className="controls">
							<select
								value={manageEntryTagMappingsStore.newEntryType}
								onChange={(e): void =>
									runInAction(() => {
										manageEntryTagMappingsStore.newEntryType = e.target
											.value as typeof ManageEntryTagMappingsStore.entryTypes[number];
									})
								}
							>
								<option value="" />
								{ManageEntryTagMappingsStore.entryTypes.map((entryType) => (
									<option value={entryType} key={entryType}>
										{entryType}
									</option>
								))}
							</select>
						</div>
					</div>
					<div className="control-group">
						<label className="control-label" htmlFor="newSourceName">
							Sub type{/* LOC */}
						</label>
						<div className="controls">
							<select
								value={manageEntryTagMappingsStore.newEntrySubType}
								onChange={(e): void =>
									runInAction(() => {
										manageEntryTagMappingsStore.newEntrySubType =
											e.target.value;
									})
								}
							>
								<option value="" />
								{manageEntryTagMappingsStore.entrySubTypes.map(
									(entrySubType) => (
										<option value={entrySubType} key={entrySubType}>
											{entrySubType}
										</option>
									),
								)}
							</select>
						</div>
					</div>
					<div className="control-group">
						<label className="control-label">Target tag{/* LOC */}</label>
						<div className="controls">
							<TagLockingAutoComplete
								basicEntryLinkStore={manageEntryTagMappingsStore.newTargetTag}
							/>
						</div>
					</div>
					<div className="control-group">
						<div className="controls">
							<Button
								variant="primary"
								onClick={(): void => {
									if (
										manageEntryTagMappingsStore.mappings.some(
											(m) =>
												m.tag.id ===
													manageEntryTagMappingsStore.newTargetTag.id &&
												m.entryType.entryType ===
													manageEntryTagMappingsStore.newEntryType &&
												m.entryType.subType ===
													manageEntryTagMappingsStore.newEntrySubType,
										)
									) {
										showErrorMessage(
											`Mapping already exists for entry type ${manageEntryTagMappingsStore.newEntryType}, ${manageEntryTagMappingsStore.newEntrySubType}` /* LOC */,
										);
										return;
									}

									manageEntryTagMappingsStore.addMapping();
								}}
							>
								Add{/* LOC */}
							</Button>
						</div>
					</div>
				</form>

				<hr />

				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							await manageEntryTagMappingsStore.save();

							showSuccessMessage('Saved' /* LOC */);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save entry type to tag mappings.' /* LOC */,
							);

							throw error;
						}

						await manageEntryTagMappingsStore.loadMappings();
					}}
				>
					<h3>Mappings{/* LOC */}</h3>

					<br />
					<br />

					<SaveBtn submitting={manageEntryTagMappingsStore.submitting} />

					<table>
						<thead>
							<tr>
								<th>Entry type{/* LOC */}</th>
								<th>Tag{/* LOC */}</th>
								<th></th>
							</tr>
						</thead>
						<tbody>
							{manageEntryTagMappingsStore.mappings.map((mapping, index) => (
								<tr
									className={classNames(
										mapping.isNew && 'row-new',
										mapping.isDeleted && 'row-deleted',
									)}
									key={index}
								>
									<td>
										{mapping.entryType.entryType} - {mapping.entryType.subType}
									</td>
									<td>
										<a
											className="extLink"
											href={manageEntryTagMappingsStore.getTagUrl(mapping)}
											target="_blank"
											rel="noreferrer"
										>
											{mapping.tag.name}
										</a>
									</td>
									<td>
										<Button
											variant="danger"
											className="btn-small"
											onClick={mapping.deleteMapping}
											disabled={mapping.isDeleted}
										>
											Delete{/* LOC */}
										</Button>
									</td>
								</tr>
							))}
						</tbody>
					</table>
					<br />

					<SaveBtn submitting={manageEntryTagMappingsStore.submitting} />
				</form>
			</Layout>
		);
	},
);

export default AdminManageEntryTagMappings;
