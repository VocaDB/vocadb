import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { userRepo } from '@/Repositories/UserRepository';
import { UserForgotPasswordStore } from '@/Stores/User/UserForgotPasswordStore';
import { useVdb } from '@/VdbContext';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import ReCAPTCHA from 'react-google-recaptcha';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';

interface UserForgotPasswordLayoutProps {
	userForgotPasswordStore: UserForgotPasswordStore;
}

const UserForgotPasswordLayout = observer(
	({
		userForgotPasswordStore,
	}: UserForgotPasswordLayoutProps): React.ReactElement => {
		const vdb = useVdb();

		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.User']);

		const title = t('ViewRes.User:ForgotPassword.RequestPasswordReset');

		const recaptchaRef = React.useRef<ReCAPTCHA>(undefined!);

		const navigate = useNavigate();

		return (
			<Layout pageTitle={title} ready={ready} title={title}>
				{userForgotPasswordStore.errors && (
					<ValidationSummaryPanel
						message={t('ViewRes.User:ForgotPassword.UnableToProcessRequest')}
						errors={userForgotPasswordStore.errors}
					/>
				)}

				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							const recaptchaResponse = recaptchaRef.current.getValue() ?? '';

							await userForgotPasswordStore.submit(recaptchaResponse);

							navigate('/User/Login');
						} catch (error: any) {
							recaptchaRef.current.reset();

							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: t('ViewRes.User:ForgotPassword.UnableToProcessRequest'),
							);

							throw error;
						}
					}}
				>
					<div className="editor-label">
						<label htmlFor="username">{t('ViewRes:Shared.Username')}</label>
					</div>
					<div className="editor-field">
						<input
							type="text"
							id="username"
							value={userForgotPasswordStore.username}
							onChange={(e): void =>
								runInAction(() => {
									userForgotPasswordStore.username = e.target.value;
								})
							}
						/>{' '}
						{userForgotPasswordStore.errors &&
							userForgotPasswordStore.errors.username && (
								<span className="field-validation-error">
									{userForgotPasswordStore.errors.username[0]}
								</span>
							)}
					</div>

					<div className="editor-label">
						<label htmlFor="email">{t('ViewRes:Shared.EmailAddress')}</label>
					</div>
					<div className="editor-field">
						<input
							type="text"
							id="email"
							value={userForgotPasswordStore.email}
							onChange={(e): void =>
								runInAction(() => {
									userForgotPasswordStore.email = e.target.value;
								})
							}
						/>{' '}
						{userForgotPasswordStore.errors &&
							userForgotPasswordStore.errors.email && (
								<span className="field-validation-error">
									{userForgotPasswordStore.errors.email[0]}
								</span>
							)}
					</div>

					<div className="editor-label">CAPTCHA</div>
					<div className="editor-field">
						<ReCAPTCHA
							ref={recaptchaRef}
							sitekey={vdb.values.reCAPTCHAPublicKey}
						/>{' '}
						{userForgotPasswordStore.errors &&
							userForgotPasswordStore.errors.captcha && (
								<span className="field-validation-error">
									{userForgotPasswordStore.errors.captcha[0]}
								</span>
							)}
					</div>

					<br />
					<p>
						<Button
							type="submit"
							variant="primary"
							disabled={userForgotPasswordStore.submitting}
						>
							{t('ViewRes.User:ForgotPassword.SendRequest')}
						</Button>
					</p>
				</form>
			</Layout>
		);
	},
);

const UserForgotPassword = (): React.ReactElement => {
	const [userForgotPasswordStore] = React.useState(
		() => new UserForgotPasswordStore(userRepo),
	);

	return (
		<UserForgotPasswordLayout
			userForgotPasswordStore={userForgotPasswordStore}
		/>
	);
};

export default UserForgotPassword;
