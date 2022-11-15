import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { IPManage } from '@/Components/Shared/KnockoutPartials/IPManage';
import { Layout } from '@/Components/Shared/Layout';
import { SaveBtn } from '@/Components/Shared/Partials/Shared/SaveBtn';
import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import { adminRepo } from '@/Repositories/AdminRepository';
import { ManageIPRulesStore } from '@/Stores/Admin/ManageIPRulesStore';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const manageIPRulesStore = new ManageIPRulesStore(adminRepo);

const AdminManageIPRules = observer(
	(): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = 'Manage blocked IPs'; /* LOC */

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
				<form
					id="manageRules"
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							await manageIPRulesStore.save();

							showSuccessMessage('Saved' /* LOC */);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save IP rules.' /* LOC */,
							);

							throw error;
						}
					}}
				>
					<SaveBtn submitting={manageIPRulesStore.submitting} />

					<Button onClick={manageIPRulesStore.deleteOldRules}>
						Delete rules older than 1 year{/* LOC */}
					</Button>

					<div className="editor-label">
						<label>New address{/* LOC */}</label>
					</div>
					<div className="editor-field">
						<input
							type="text"
							maxLength={40}
							value={manageIPRulesStore.newAddress}
							onChange={(e): void =>
								runInAction(() => {
									manageIPRulesStore.newAddress = e.target.value;
								})
							}
						/>{' '}
						<SafeAnchor
							href="#"
							className="textLink addLink"
							onClick={(): void => {
								const addr = manageIPRulesStore.newAddress.trim();
								if (!addr) return;

								if (manageIPRulesStore.rules.some((r) => r.address === addr)) {
									showErrorMessage('Address already added' /* LOC */);
									return;
								}

								manageIPRulesStore.add(addr);
							}}
						>
							{t('ViewRes:Shared.Add')}
						</SafeAnchor>
					</div>

					<table>
						<thead>
							<tr>
								<th>Address{/* LOC */}</th>
								<th>Notes{/* LOC */}</th>
								<th>Created{/* LOC */}</th>
								<th />
							</tr>
						</thead>
						<tbody>
							{manageIPRulesStore.rules.map((rule, index) => (
								<tr key={index}>
									<td>{rule.address}</td>
									<td>
										<input
											type="text"
											value={rule.notes}
											onChange={(e): void =>
												runInAction(() => {
													rule.notes = e.target.value;
												})
											}
										/>
									</td>
									<td>{moment(rule.created).format('L LT')}</td>
									<td>
										<SafeAnchor
											href="#"
											className="textLink deleteLink"
											onClick={(): void => manageIPRulesStore.remove(rule)}
										>
											{t('ViewRes:Shared.Remove')}
										</SafeAnchor>
									</td>
								</tr>
							))}
						</tbody>
					</table>

					<SaveBtn submitting={manageIPRulesStore.submitting} />
				</form>

				<div>
					{manageIPRulesStore.bannedIPs.length > 0 && (
						<>
							<h2>Automatically banned IPs{/* LOC */}</h2>
							<ul>
								{manageIPRulesStore.bannedIPs.map((bannedIP, index) => (
									<li key={index}>
										<IPManage ip={bannedIP} />
									</li>
								))}
							</ul>
						</>
					)}
				</div>
			</Layout>
		);
	},
);

export default AdminManageIPRules;
