import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { Layout } from '@/Components/Shared/Layout';
import { UserGroupDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { UserLinkOrName } from '@/Components/Shared/Partials/User/UserLinkOrName';
import { UserGroup } from '@/Models/Users/UserGroup';
import { adminRepo } from '@/Repositories/AdminRepository';
import { functions } from '@/Shared/GlobalFunctions';
import { ViewAuditLogStore } from '@/Stores/Admin/ViewAuditLogStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import classNames from 'classnames';
import $ from 'jquery';
import 'jquery-ui';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const viewAuditLogStore = new ViewAuditLogStore(adminRepo);

interface AuditLogFiltersProps {
	viewAuditLogStore: ViewAuditLogStore;
}

const AuditLogFilters = observer(
	({ viewAuditLogStore }: AuditLogFiltersProps): React.ReactElement => {
		const userNameRef = React.useRef<HTMLInputElement>(undefined!);
		React.useEffect(() => {
			const $userName = $(userNameRef.current);
			$userName.autocomplete({
				source: (
					ui: { term: any },
					callback: (data: any, textStatus: string, jqXHR: JQueryXHR) => any,
				) => {
					const url = functions.mapAbsoluteUrl('/api/users/names');
					$.getJSON(url, { query: ui.term }, callback);
				},
				select: (event, ui) => {
					runInAction(() => {
						viewAuditLogStore.userName = ui.item.value;
					});
					return false;
				},
			});

			return (): void => {
				$userName.autocomplete('destroy');
			};
		}, [viewAuditLogStore]);

		const excludeUsersRef = React.useRef<HTMLInputElement>(undefined!);
		React.useEffect(() => {
			const $excludeUsers = $(excludeUsersRef.current);
			$excludeUsers
				// don't navigate away from the field on tab when selecting an item
				.bind('keydown', function (this: any, event) {
					if (
						event.keyCode === $.ui.keyCode.TAB &&
						$(this).data('ui-autocomplete').menu.active
					) {
						event.preventDefault();
					}
				})
				.autocomplete({
					minLength: 1,
					source: (
						request: { term: string },
						response: {
							(arg0: {}): void;
							(data: any, textStatus: string, jqXHR: JQueryXHR): any;
						},
					) => {
						const name = viewAuditLogStore.extractLast(request.term);
						if (name!.length === 0) response({});
						else
							$.getJSON(
								'/api/users/names',
								{ query: name, startsWith: true } as any,
								response,
							);
					},
					focus: function () {
						// prevent value inserted on focus
						return false;
					},
					select: (event, ui) => {
						const terms = viewAuditLogStore.split($excludeUsers.val());
						// remove the current input
						terms.pop();
						// add the selected item
						terms.push(ui.item.value);
						// add placeholder to get the comma-and-space at the end
						terms.push('');
						runInAction(() => {
							viewAuditLogStore.excludeUsers = terms.join(', ');
						});
						return false;
					},
				});

			return (): void => {
				$excludeUsers.autocomplete('destroy');
			};
		}, [viewAuditLogStore]);

		return (
			<form className="form-horizontal">
				<div className="control-group">
					<div className="control-label">Text query{/* LOC */}</div>
					<div className="controls">
						<DebounceInput
							type="text"
							value={viewAuditLogStore.filter}
							onChange={(e): void =>
								runInAction(() => {
									viewAuditLogStore.filter = e.target.value;
								})
							}
							maxLength={255}
							className="input-xlarge"
							debounceTimeout={300}
						/>
					</div>
				</div>
				<div className="control-group">
					<div className="control-label">Show only user{/* LOC */}</div>
					<div className="controls">
						<DebounceInput
							inputRef={userNameRef}
							type="text"
							value={viewAuditLogStore.userName}
							onChange={(e): void =>
								runInAction(() => {
									viewAuditLogStore.userName = e.target.value;
								})
							}
							maxLength={255}
							className="input-large"
							debounceTimeout={300}
						/>
					</div>
				</div>
				<div className="control-group">
					<div className="control-label">Exclude users{/* LOC */}</div>
					<div className="controls">
						<DebounceInput
							inputRef={excludeUsersRef}
							type="text"
							value={viewAuditLogStore.excludeUsers}
							onChange={(e): void =>
								runInAction(() => {
									viewAuditLogStore.excludeUsers = e.target.value;
								})
							}
							maxLength={255}
							className="input-xlarge"
							debounceTimeout={300}
						/>
					</div>
				</div>
				<div className="control-group">
					<div className="control-label">User group{/* LOC */}</div>
					<div className="controls">
						<UserGroupDropdownList
							value={viewAuditLogStore.group}
							onChange={(e): void =>
								runInAction(() => {
									viewAuditLogStore.group = e.target.value as UserGroup;
								})
							}
						/>
					</div>
				</div>
				<div className="control-group">
					<div className="controls">
						<label className="checkbox">
							<input
								type="checkbox"
								checked={viewAuditLogStore.onlyNewUsers}
								onChange={(e): void =>
									runInAction(() => {
										viewAuditLogStore.onlyNewUsers = e.target.checked;
									})
								}
							/>
							Only show new users{/* LOC */}
						</label>
					</div>
				</div>
			</form>
		);
	},
);

const AdminViewAuditLog = observer(
	(): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = 'View audit log'; /* LOC */

		useLocationStateStore(viewAuditLogStore);

		return (
			<Layout
				pageTitle={title}
				ready={true}
				title={title}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: '/Admin',
							}}
						>
							Manage{/* LOC */}
						</Breadcrumb.Item>
					</>
				}
			>
				<Alert variant="info">
					<Button
						onClick={viewAuditLogStore.toggleFilter}
						className={classNames(
							viewAuditLogStore.filterVisible && 'active',
							'dropdown-toggle',
						)}
					>
						<i className="icon-filter" /> {t('ViewRes:Shared.Filter')}
					</Button>

					{viewAuditLogStore.filterVisible && (
						<AuditLogFilters viewAuditLogStore={viewAuditLogStore} />
					)}
				</Alert>

				<div>
					<table className="table">
						<thead>
							<tr>
								<th>Time{/* LOC */}</th>
								<th>User{/* LOC */}</th>
								<th>Action{/* LOC */}</th>
							</tr>
						</thead>
						<tbody id="logEntries">
							{viewAuditLogStore.items.map((logEntry, index) => (
								<tr key={index}>
									<td>{moment(logEntry.time).format('L LT')}</td>
									<td>
										<UserLinkOrName
											user={logEntry.user}
											name={logEntry.agentName}
										/>
									</td>
									<td>
										<Markdown>{logEntry.action}</Markdown>
									</td>
								</tr>
							))}
						</tbody>
					</table>
				</div>

				<SafeAnchor id="loadMoreLink" onClick={viewAuditLogStore.loadMore}>
					Load more{/* LOC */}
				</SafeAnchor>
			</Layout>
		);
	},
);

export default AdminViewAuditLog;
