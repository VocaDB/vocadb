import { userRepo } from '@/Repositories/UserRepository';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class UserLoginStore {
	@observable errors?: Record<string, string[]>;
	@observable keepLoggedIn = false;
	@observable password = '';
	@observable submitting = false;
	@observable userName = '';

	constructor() {
		makeObservable(this);
	}

	@action submit = async (requestToken: string): Promise<void> => {
		this.submitting = true;

		try {
			await userRepo.login(requestToken, {
				keepLoggedIn: this.keepLoggedIn,
				password: this.password,
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
