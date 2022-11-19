import Alert from '@/Bootstrap/Alert';
import Button from '@/Bootstrap/Button';
import { ArtistAutoComplete } from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { Layout } from '@/Components/Shared/Layout';
import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import { EntryType } from '@/Models/EntryType';
import { loginManager } from '@/Models/LoginManager';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import RequestVerificationStore from '@/Stores/User/RequestVerificationStore';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import { Trans, useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const requestVerificationStore = new RequestVerificationStore(
	vdb.values,
	artistRepo,
);

const UserRequestVerification = observer(
	(): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.User']);

		const title = t('ViewRes.User:RequestVerification.PageTitle');

		return (
			<Layout pageTitle={title} ready={ready} title={title}>
				<div className="row-fluid">
					{loginManager.isLoggedIn ? (
						<form
							onSubmit={async (e): Promise<void> => {
								e.preventDefault();

								try {
									const requestToken = await antiforgeryRepo.getToken();

									await requestVerificationStore.submit(requestToken);

									showSuccessMessage('Request sent' /* LOC */);
								} catch (error: any) {
									showErrorMessage(
										error.response && error.response.status
											? getReasonPhrase(error.response.status)
											: 'Unable to send request.' /* LOC */,
									);

									throw error;
								}
							}}
							className="span6 form"
						>
							<label>{t('ViewRes.User:RequestVerification.ArtistTitle')}</label>
							{requestVerificationStore.selectedArtist ? (
								<div>
									<Link
										to={EntryUrlMapper.details(
											EntryType.Artist,
											requestVerificationStore.selectedArtist.id,
										)}
									>
										{requestVerificationStore.selectedArtist.name}
									</Link>{' '}
									<Button
										onClick={requestVerificationStore.clearArtist}
										variant="danger"
										className="btn-mini"
									>
										{t('ViewRes:Shared.Clear')}
									</Button>
								</div>
							) : (
								<div>
									<ArtistAutoComplete
										type="text"
										maxLength={128}
										placeholder={t('ViewRes:Shared.Search')}
										className="input-xlarge"
										required
										properties={{
											acceptSelection: requestVerificationStore.setArtist,
										}}
									/>
								</div>
							)}

							<br />
							<label>
								<input
									type="radio"
									checked={requestVerificationStore.privateMessage === false}
									onChange={(): void =>
										runInAction(() => {
											requestVerificationStore.privateMessage = false;
										})
									}
								/>{' '}
								{t('ViewRes.User:RequestVerification.NoPrivateMessage')}
							</label>
							<label>
								<input
									type="radio"
									checked={requestVerificationStore.privateMessage === true}
									onChange={(): void =>
										runInAction(() => {
											requestVerificationStore.privateMessage = true;
										})
									}
								/>{' '}
								{t('ViewRes.User:RequestVerification.PrivateMessage')}
							</label>

							{!requestVerificationStore.privateMessage && (
								<div>
									<label>{t('ViewRes.User:RequestVerification.URL')}</label>
									<div className="inline input-prepend">
										<span className="add-on" title="URL" /* LOC */>
											<i className="icon-globe" />
										</span>
										<input
											type="text"
											className="input-xlarge"
											maxLength={255}
											required
											value={requestVerificationStore.linkToProof}
											onChange={(e): void =>
												runInAction(() => {
													requestVerificationStore.linkToProof = e.target.value;
												})
											}
										/>
									</div>
								</div>
							)}

							<br />
							<label>
								{t('ViewRes.User:RequestVerification.MessageTitle')}
							</label>
							<textarea
								className="span11"
								cols={50}
								rows={10}
								value={requestVerificationStore.message}
								onChange={(e): void =>
									runInAction(() => {
										requestVerificationStore.message = e.target.value;
									})
								}
							/>

							<br />
							<Button
								type="submit"
								variant="primary"
								disabled={
									!requestVerificationStore.selectedArtist ||
									requestVerificationStore.submitting
								}
							>
								{t('ViewRes.User:RequestVerification.Send')}
							</Button>
						</form>
					) : (
						<Alert className="span6">
							<Trans
								i18nKey="ViewRes.User:RequestVerification.NotLoggedInMessage"
								components={{
									1: (
										<Link to="/User/Login">
											{t('ViewRes.User:RequestVerification.Login')}
										</Link>
									),
									2: (
										<a href="/User/Create">
											{t('ViewRes.User:RequestVerification.Create')}
										</a>
									),
								}}
							/>
						</Alert>
					)}

					<Alert variant="info" className="span4">
						<span
							dangerouslySetInnerHTML={{
								__html: vdb.resources.user.requestVerificationInfo ?? '',
							}}
						/>
					</Alert>
				</div>
			</Layout>
		);
	},
);

export default UserRequestVerification;
