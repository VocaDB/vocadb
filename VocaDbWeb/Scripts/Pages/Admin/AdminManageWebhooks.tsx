import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { SaveBtn } from '@/Components/Shared/Partials/Shared/SaveBtn';
import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { AdminRepository } from '@/Repositories/AdminRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { ManageWebhooksStore } from '@/Stores/Admin/ManageWebhooksStore';
import classNames from 'classnames';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const adminRepo = new AdminRepository(httpClient, urlMapper);

const manageWebhooksStore = new ManageWebhooksStore(adminRepo);

const AdminManageWebhooks = observer(
	(): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Domain']);

		const title = 'Manage webhooks'; /* LOCALIZE */

		useVdbTitle(title, true);

		return (
			<Layout
				title={title}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: '/Admin',
							}}
						>
							Manage{/* LOCALIZE */}
						</Breadcrumb.Item>
					</>
				}
			>
				<form className="form-horizontal">
					<h3>New webhook{/* LOCALIZE */}</h3>
					<div className="control-group">
						<label className="control-label" htmlFor="newUrl">
							Payload URL{/* LOCALIZE */}
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
							Events{/* LOCALIZE */}
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
										showErrorMessage('Hook already exists' /* LOCALIZE */);
										return;
									}

									manageWebhooksStore.addWebhook();
								}}
							>
								Add{/* LOCALIZE */}
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

							showSuccessMessage('Saved' /* LOCALIZE */);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save webhooks.' /* LOCALIZE */,
							);

							throw error;
						}

						await manageWebhooksStore.loadWebhooks();
					}}
				>
					<h3>Webhooks{/* LOCALIZE */}</h3>

					<SaveBtn submitting={manageWebhooksStore.submitting} />

					<table>
						<thead>
							<tr>
								<th>Payload URL{/* LOCALIZE */}</th>
								<th>Events{/* LOCALIZE */}</th>
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
											Delete{/* LOCALIZE */}
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
