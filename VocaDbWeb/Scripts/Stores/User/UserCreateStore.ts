import { userRepo } from '@/Repositories/UserRepository';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class UserCreateStore {
	private readonly entryTime = new Date();
	@observable email = '';
	@observable errors?: Record<string, string[]>;
	@observable extra = '';
	@observable password = '';
	@observable submitting = false;
	@observable userName = '';

	constructor() {
		makeObservable(this);
	}

	@action submit = async (
		requestToken: string,
		recaptchaResponse: string,
	): Promise<void> => {
		this.submitting = true;

		try {
			await userRepo.create(requestToken, {
				email: this.email,
				entryTime: this.entryTime,
				extra: this.extra,
				password: this.password,
				recaptchaResponse: recaptchaResponse,
				userName: this.userName,
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
