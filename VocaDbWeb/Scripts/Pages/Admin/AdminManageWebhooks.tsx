import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { SaveBtn } from '@/Components/Shared/Partials/Shared/SaveBtn';
import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import { adminRepo } from '@/Repositories/AdminRepository';
import { ManageWebhooksStore } from '@/Stores/Admin/ManageWebhooksStore';
import classNames from 'classnames';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const manageWebhooksStore = new ManageWebhooksStore(adminRepo);

const AdminManageWebhooks = observer(
	(): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Domain']);

		const title = 'Manage webhooks'; /* LOC */

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
				<form className="form-horizontal">
					<h3>New webhook{/* LOC */}</h3>
					<div className="control-group">
						<label className="control-label" htmlFor="newUrl">
							Payload URL{/* LOC */}
						</label>
						<div className="controls">
							<input
								type="text"
								id="newUrl"
								placeholder="https://example.com/postreceive"
								value={manageWebhooksStore.newUrl}
								onChange={(e): void =>
									runInAction(() => {
										manageWebhooksStore.newUrl = e.target.value;
									})
								}
							/>
						</div>
					</div>
					<div className="control-group">
						<label className="control-label" htmlFor="newWebhookEvents">
							Events{/* LOC */}
						</label>
						<div className="controls">
							<div>
								<div>
									{manageWebhooksStore.webhookEventsEditStore.webhookEventSelections.map(
										(webhookEventSelection) => (
											<label
												className="checkbox"
												key={webhookEventSelection.id}
											>
												<input
													type="checkbox"
													checked={webhookEventSelection.selected}
													onChange={(): void =>
														runInAction(() => {
															webhookEventSelection.selected = !webhookEventSelection.selected;
														})
													}
												/>
												<span>
													{t(
														`VocaDb.Web.Resources.Domain:WebhookEventNames.${webhookEventSelection.id}`,
													)}
												</span>
											</label>
										),
									)}
								</div>
							</div>
						</div>
					</div>
					<div className="control-group">
						<div className="controls">
							<Button
								variant="primary"
								onClick={(): void => {
									if (
										manageWebhooksStore.webhooks.some(
											(w) => w.url === manageWebhooksStore.newUrl,
										)
									) {
										showErrorMessage('Hook already exists' /* LOC */);
										return;
									}

									manageWebhooksStore.addWebhook();
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
							await manageWebhooksStore.save();

							showSuccessMessage('Saved' /* LOC */);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save webhooks.' /* LOC */,
							);

							throw error;
						}

						await manageWebhooksStore.loadWebhooks();
					}}
				>
					<h3>Webhooks{/* LOC */}</h3>

					<SaveBtn submitting={manageWebhooksStore.submitting} />

					<table>
						<thead>
							<tr>
								<th>Payload URL{/* LOC */}</th>
								<th>Events{/* LOC */}</th>
								<th />
							</tr>
						</thead>
						<tbody>
							{manageWebhooksStore.webhooks.map((webhook, index) => (
								<tr
									className={classNames(
										webhook.isNew && 'row-new',
										webhook.isDeleted && 'row-deleted',
									)}
									key={index}
								>
									<td>{webhook.url}</td>
									<td>
										<div>
											{webhook.webhookEventsArray.map((webhookEvent, index) => (
												<>
													{index > 0 && ', '}
													<span key={webhookEvent}>
														{t(
															`VocaDb.Web.Resources.Domain:WebhookEventNames.${webhookEvent}`,
														)}
													</span>
												</>
											))}
										</div>
									</td>
									<td>
										<Button
											className="btn-small"
											variant="danger"
											onClick={webhook.deleteWebhook}
											disabled={webhook.isDeleted}
										>
											Delete{/* LOC */}
										</Button>
									</td>
								</tr>
							))}
						</tbody>
					</table>
					<br />

					<SaveBtn submitting={manageWebhooksStore.submitting} />
				</form>
			</Layout>
		);
	},
);

export default AdminManageWebhooks;
