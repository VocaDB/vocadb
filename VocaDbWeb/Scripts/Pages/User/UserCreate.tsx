import Alert from '@/Bootstrap/Alert';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { showErrorMessage } from '@/Components/ui';
import { UserCreateStore } from '@/Stores/User/UserCreateStore';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import ReCAPTCHA from 'react-google-recaptcha';
import { useTranslation } from 'react-i18next';

interface UserCreateLayoutProps {
	userCreateStore: UserCreateStore;
}

const UserCreateLayout = observer(
	({ userCreateStore }: UserCreateLayoutProps): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes.User']);

		const title = t('ViewRes.User:Create.Register');

		const recaptchaRef = React.useRef<ReCAPTCHA>(undefined!);

		return (
			<Layout pageTitle={title} ready={ready} title={title} /* TODO */>
				{vdb.values.signupsDisabled ? (
					<Alert>
						Sorry - signups are disabled at the moment on this site.
						{/* LOC */}
					</Alert>
				) : (
					<form
						onSubmit={async (e): Promise<void> => {
							e.preventDefault();

							try {
								const recaptchaResponse = recaptchaRef.current.getValue() ?? '';

								await userCreateStore.submit(recaptchaResponse);

								// TODO: Replace window.location.href with navigate.
								window.location.href = '/';
							} catch (error: any) {
								recaptchaRef.current.reset();

								showErrorMessage(
									error.response && error.response.status
										? getReasonPhrase(error.response.status)
										: 'Unable to create user' /* LOC */,
								);

								throw error;
							}
						}}
					>
						<div className="row-fluid">
							<div className="span5 well well-transparent">
								<div className="editor-label">
									<label htmlFor="userName">
										{t('ViewRes.User:Create.Username')}
									</label>
								</div>
								<div className="editor-field">
									<input
										type="text"
										id="userName"
										size={40}
										maxLength={100}
										value={userCreateStore.userName}
										onChange={(e): void =>
											runInAction(() => {
												userCreateStore.userName = e.target.value;
											})
										}
									/>{' '}
									{userCreateStore.errors &&
										userCreateStore.errors.userName && (
											<span className="field-validation-error">
												{userCreateStore.errors.userName[0]}
											</span>
										)}
								</div>

								<div className="editor-label">
									<label htmlFor="password">
										{t('ViewRes.User:Create.Password')}
									</label>
								</div>
								<div className="editor-field">
									<input
										type="password"
										id="password"
										size={40}
										maxLength={100}
										value={userCreateStore.password}
										onChange={(e): void =>
											runInAction(() => {
												userCreateStore.password = e.target.value;
											})
										}
									/>{' '}
									{userCreateStore.errors &&
										userCreateStore.errors.password && (
											<span className="field-validation-error">
												{userCreateStore.errors.password[0]}
											</span>
										)}
								</div>

								<div className="editor-label">
									<label htmlFor="email">
										{t('ViewRes.User:Create.Email')}
									</label>
								</div>
								<div className="editor-field">
									<input
										type="text"
										id="email"
										size={40}
										maxLength={50}
										value={userCreateStore.email}
										onChange={(e): void =>
											runInAction(() => {
												userCreateStore.email = e.target.value;
											})
										}
									/>{' '}
									{userCreateStore.errors && userCreateStore.errors.email && (
										<span className="field-validation-error">
											{userCreateStore.errors.email[0]}
										</span>
									)}
								</div>

								<div className="editor-label">
									{t('ViewRes.User:Create.Captcha')}
								</div>
								<div className="editor-field">
									<ReCAPTCHA
										ref={recaptchaRef}
										sitekey={vdb.values.reCAPTCHAPublicKey}
									/>{' '}
									{userCreateStore.errors && userCreateStore.errors.captcha && (
										<span className="field-validation-error">
											{userCreateStore.errors.captcha[0]}
										</span>
									)}
								</div>

								<div className="editor-field" style={{ display: 'none' }}>
									<input
										type="text"
										id="extra"
										name="extra"
										value={userCreateStore.extra}
										onChange={(e): void =>
											runInAction(() => {
												userCreateStore.extra = e.target.value;
											})
										}
									/>
								</div>

								<br />
								<br />
								<p>
									<Button
										type="submit"
										variant="primary"
										disabled={userCreateStore.submitting}
									>
										{t('ViewRes.User:Create.DoRegister')}
									</Button>
								</p>

								<hr />
								<a href="/User/LoginTwitter">
									<img
										src="/Content/Sign-in-with-Twitter-darker.png"
										alt={t('ViewRes.User:Create.LoginWithTwitter')}
									/>
								</a>

								<br />
								<br />
								<small>
									<a
										href="https://wiki.vocadb.net/wiki/50/privacy-and-cookie-policy"
										target="_blank"
										rel="noreferrer"
									>
										{t('ViewRes:Layout.PrivacyPolicy')}
									</a>
								</small>
							</div>

							<div className="span3">
								<Alert variant="info">
									{t('ViewRes.User:Create.UsernameNote', { 0: 8 })}
								</Alert>
								<Alert variant="info">
									{t('ViewRes.User:Create.EmailNote')}
								</Alert>
								<Alert variant="info">
									{t('ViewRes.User:Create.TwitterNote')}
								</Alert>
								<Alert variant="info">
									{t('ViewRes.User:Create.DuplicateUserWarning')}
								</Alert>
							</div>
						</div>
					</form>
				)}
			</Layout>
		);
	},
);

const UserCreate = (): React.ReactElement => {
	const [userCreateStore] = React.useState(() => new UserCreateStore());

	return <UserCreateLayout userCreateStore={userCreateStore} />;
};

export default UserCreate;
