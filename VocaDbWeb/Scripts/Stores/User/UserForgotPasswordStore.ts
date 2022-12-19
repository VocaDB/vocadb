import { UserRepository } from '@/Repositories/UserRepository';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class UserForgotPasswordStore {
	@observable email = '';
	@observable errors?: Record<string, string[]>;
	@observable submitting = false;
	@observable username = '';

	constructor(private readonly userRepo: UserRepository) {
		makeObservable(this);
	}

	@action submit = async (
		requestToken: string,
		recaptchaResponse: string,
	): Promise<void> => {
		this.submitting = true;

		try {
			await this.userRepo.forgotPassword(requestToken, {
				email: this.email,
				recaptchaResponse: recaptchaResponse,
				username: this.username,
			});
		} catch (error: any) {
			if (error.response) {
				runInAction(() => {
					this.errors = undefined;

					if (error.response.status === 400)
						this.errors = error.response.data.errors;
				});
			}

			throw error;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
