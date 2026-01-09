import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { SaveBtn } from '@/Components/Shared/Partials/Shared/SaveBtn';
import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import { adminRepo } from '@/Repositories/AdminRepository';
import { ManageFrontpageStore } from '@/Stores/Admin/ManageFrontpageStore';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const manageFrontpageStore = new ManageFrontpageStore(adminRepo);

const AdminManageFrontpage = observer(
	(): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = 'Manage Frontpage'; /* LOC */

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
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							await manageFrontpageStore.save();

							showSuccessMessage('Saved' /* LOC */);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to save frontpage configuration.' /* LOC */,
							);

							throw error;
						}
					}}
				>
					<SaveBtn submitting={manageFrontpageStore.submitting} />

					<h2>Banners{/* LOC */}</h2>
					<p className="text-muted">
						Configure announcement banners displayed on the homepage. Banners are shown in order when enabled.{/* LOC */}
					</p>

					<Button onClick={manageFrontpageStore.addBanner}>
						Add Banner{/* LOC */}
					</Button>

					{manageFrontpageStore.banners.map((banner, index) => (
						<div key={index} className="well" style={{ marginTop: '15px' }}>
							<h4>
								Banner {index + 1}
								{!banner.enabled && <span className="text-muted"> (Disabled)</span>}
							</h4>

							<div className="form-group">
								<label className="control-label">Title{/* LOC */}</label>
								<input
									type="text"
									className="form-control"
									value={banner.title}
									onChange={(e): void =>
										runInAction(() => {
											banner.title = e.target.value;
										})
									}
									required
								/>
							</div>

							<div className="form-group">
								<label className="control-label">Description{/* LOC */}</label>
								<textarea
									className="form-control"
									rows={3}
									value={banner.description}
									onChange={(e): void =>
										runInAction(() => {
											banner.description = e.target.value;
										})
									}
								/>
							</div>

							<div className="form-group">
								<label className="control-label">Link URL (optional){/* LOC */}</label>
								<input
									type="url"
									className="form-control"
									value={banner.linkUrl}
									onChange={(e): void =>
										runInAction(() => {
											banner.linkUrl = e.target.value;
										})
									}
									placeholder="https://example.com"
								/>
								<small className="text-muted">
									If provided, the entire banner will be clickable{/* LOC */}
								</small>
							</div>

							<div className="form-group">
								<label className="control-label">Image{/* LOC */}</label>
								<input
									type="file"
									className="form-control"
									accept="image/jpeg,image/png,image/gif,image/webp"
									onChange={async (e): Promise<void> => {
										const file = e.target.files?.[0];
										if (file) {
											try {
												const fileName = await manageFrontpageStore.uploadImage(file);
												runInAction(() => {
													banner.imageUrl = fileName;
												});
												showSuccessMessage('Image uploaded' /* LOC */);
											} catch (error: any) {
												showErrorMessage(
													error.response?.data || 'Failed to upload image' /* LOC */,
												);
											}
											e.target.value = '';
										}
									}}
									disabled={manageFrontpageStore.uploadingImage}
								/>
								{banner.imageUrl && (
									<div style={{ marginTop: '10px' }}>
										<img
											src={`/Content/banners/${banner.imageUrl}`}
											alt="Banner preview"
											style={{ maxWidth: '400px', maxHeight: '200px' }}
										/>
										<br />
										<small className="text-muted">{banner.imageUrl}</small>
										<Button
											variant="link"
											onClick={(): void =>
												runInAction(() => {
													banner.imageUrl = '';
												})
											}
										>
											Remove image{/* LOC */}
										</Button>
									</div>
								)}
							</div>

							<div className="checkbox">
								<label>
									<input
										type="checkbox"
										checked={banner.enabled}
										onChange={(e): void =>
											runInAction(() => {
												banner.enabled = e.target.checked;
											})
										}
									/>
									{' Enabled'}{/* LOC */}
								</label>
							</div>

							{banner.title && (
								<>
									<h5 style={{ marginTop: '15px' }}>Preview{/* LOC */}</h5>
									<Alert
										variant="info"
										style={{
											display: 'flex',
											padding: 0,
											overflow: 'hidden',
											maxHeight: '200px',
											cursor: banner.linkUrl ? 'pointer' : 'default'
										}}
										onClick={banner.linkUrl ? (): void => {
											window.open(banner.linkUrl, '_blank');
										} : undefined}
									>
										{banner.imageUrl && (
											<div
												style={{
													flexShrink: 0,
													width: '200px',
													display: 'flex',
													alignItems: 'center',
													justifyContent: 'center',
													backgroundColor: 'rgba(0,0,0,0.05)'
												}}
											>
												<img
													src={`/Content/banners/${banner.imageUrl}`}
													alt={banner.title}
													style={{
														width: '100%',
														height: '100%',
														objectFit: 'cover'
													}}
												/>
											</div>
										)}
										<div style={{
											flex: 1,
											padding: '15px',
											display: 'flex',
											flexDirection: 'column',
											justifyContent: 'center'
										}}>
											<h4 style={{ margin: '0 0 10px 0' }}>{banner.title}</h4>
											{banner.description && <p style={{ margin: 0 }}>{banner.description}</p>}
										</div>
									</Alert>
								</>
							)}

							<div className="btn-group" style={{ marginTop: '10px' }}>
								<Button
									onClick={(): void => manageFrontpageStore.moveBannerUp(index)}
									disabled={index === 0}
								>
									Move Up{/* LOC */}
								</Button>
								<Button
									onClick={(): void => manageFrontpageStore.moveBannerDown(index)}
									disabled={index === manageFrontpageStore.banners.length - 1}
								>
									Move Down{/* LOC */}
								</Button>
								<Button
									variant="danger"
									onClick={(): void => {
										if (confirm('Are you sure you want to remove this banner?' /* LOC */)) {
											manageFrontpageStore.removeBanner(banner);
										}
									}}
								>
									Remove{/* LOC */}
								</Button>
							</div>
						</div>
					))}

					<SaveBtn submitting={manageFrontpageStore.submitting} />
				</form>
			</Layout>
		);
	},
);

export default AdminManageFrontpage;
