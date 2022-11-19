import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { ValidationSummaryPanel } from '@/Components/Shared/Partials/Shared/ValidationSummaryPanel';
import { showErrorMessage } from '@/Components/ui';
import { UserLoginStore } from '@/Stores/User/UserLoginStore';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSearchParams } from 'react-router-dom';

interface UserLoginLayoutProps {
	userLoginStore: UserLoginStore;
}

const UserLoginLayout = observer(
	({ userLoginStore }: UserLoginLayoutProps): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.User']);

		const title = t('ViewRes.User:Login.Login');

		const [searchParams] = useSearchParams();
		const returnUrl = searchParams.get('returnUrl');

		return (
			<Layout pageTitle={title} ready={ready} title={title}>
				{userLoginStore.errors && (
					<ValidationSummaryPanel
						message={t('ViewRes.User:Login.UnableToLogin')}
						errors={userLoginStore.errors}
					/>
				)}
				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							await userLoginStore.submit();

							// TODO: TempData.SetSuccessMessage(string.Format(ViewRes.User.LoginStrings.Welcome, user.Name));

							// TODO: should not allow redirection to URLs outside the site
							// TODO: Replace window.location.href with navigate.
							window.location.href = returnUrl ?? '/';
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: t('ViewRes.User:Login.UnableToLogin'),
							);

							throw error;
						}
					}}
				>
					<div className="editor-label">
						<label htmlFor="userName">{t('ViewRes.User:Login.Username')}</label>
					</div>
					<div className="editor-field">
						<input
							type="text"
							id="userName"
							value={userLoginStore.userName}
							onChange={(e): void =>
								runInAction(() => {
									userLoginStore.userName = e.target.value;
								})
							}
						/>{' '}
						{userLoginStore.errors && userLoginStore.errors.userName && (
							<span className="field-validation-error">
								{userLoginStore.errors.userName[0]}
							</span>
						)}
					</div>

					<div className="editor-label">
						<label htmlFor="password">{t('ViewRes.User:Login.Password')}</label>
					</div>
					<div className="editor-field">
						<input
							type="password"
							id="password"
							value={userLoginStore.password}
							onChange={(e): void =>
								runInAction(() => {
									userLoginStore.password = e.target.value;
								})
							}
						/>{' '}
						{userLoginStore.errors && userLoginStore.errors.password && (
							<span className="field-validation-error">
								{userLoginStore.errors.password[0]}
							</span>
						)}
					</div>

					<br />
					<label className="checkbox">
						<input
							type="checkbox"
							checked={userLoginStore.keepLoggedIn}
							onChange={(e): void =>
								runInAction(() => {
									userLoginStore.keepLoggedIn = e.target.checked;
								})
							}
						/>
						{t('ViewRes.User:Login.KeepMeLoggedIn')}
					</label>
					<br />

					<p>
						<Button
							type="submit"
							variant="primary"
							disabled={userLoginStore.submitting}
						>
							{t('ViewRes.User:Login.DoLogin')}
						</Button>
					</p>
				</form>
				{t('ViewRes.User:Login.NoAccount')}{' '}
				<a href="/User/Create">{t('ViewRes.User:Login.RegisterHere')}</a>
				{t('ViewRes:Shared.Period')}
				<br />
				{t('ViewRes.User:Login.ForgotPassword')}{' '}
				<a href="/User/ForgotPassword">{t('ViewRes.User:Login.ResetPass')}</a>
				{t('ViewRes:Shared.Period')}
				<br />
				<hr />
				<a
					href={`/User/LoginTwitter?${qs.stringify({
						returnUrl: returnUrl,
					})}`}
				>
					<img
						src="/Content/Sign-in-with-Twitter-darker.png"
						alt={t('ViewRes.User:Login.LoginWithTwitter')}
					/>
				</a>
			</Layout>
		);
	},
);

const UserLogin = (): React.ReactElement => {
	const [userLoginStore] = React.useState(() => new UserLoginStore());

	return <UserLoginLayout userLoginStore={userLoginStore} />;
};

export default UserLogin;
