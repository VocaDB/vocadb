import {
	FrontpageBannerContract,
	FrontpageConfigContract,
} from '@/DataContracts/FrontpageConfigContracts';
import { AdminRepository } from '@/Repositories/AdminRepository';
import { pull } from 'lodash-es';
import { action, makeObservable, observable, runInAction } from 'mobx';

class FrontpageBanner {
	@observable id: number;
	@observable title: string;
	@observable description: string;
	@observable imageUrl: string;
	@observable linkUrl: string;
	@observable enabled: boolean;
	@observable sortIndex: number;

	constructor(data: FrontpageBannerContract) {
		makeObservable(this);

		this.id = data.id;
		this.title = data.title;
		this.description = data.description;
		this.imageUrl = data.imageUrl;
		this.linkUrl = data.linkUrl;
		this.enabled = data.enabled;
		this.sortIndex = data.sortIndex;
	}

	toContract(): FrontpageBannerContract {
		return {
			id: this.id,
			title: this.title,
			description: this.description,
			imageUrl: this.imageUrl,
			linkUrl: this.linkUrl,
			enabled: this.enabled,
			sortIndex: this.sortIndex,
		};
	}
}

export class ManageFrontpageStore {
	@observable banners: FrontpageBanner[] = [];
	@observable submitting = false;
	@observable uploadingImage = false;

	constructor(private readonly adminRepo: AdminRepository) {
		makeObservable(this);

		this.loadConfig();
	}

	@action loadConfig = async (): Promise<void> => {
		const config = await this.adminRepo.getFrontpageConfig();
		runInAction(() => {
			this.banners = config.banners.map((b) => new FrontpageBanner(b));
		});
	};

	@action addBanner = (): void => {
		const maxIndex = Math.max(0, ...this.banners.map((b) => b.sortIndex));
		this.banners.push(
			new FrontpageBanner({
				id: 0,
				title: '',
				description: '',
				imageUrl: '',
				linkUrl: '',
				enabled: true,
				sortIndex: maxIndex + 1,
			}),
		);
	};

	@action removeBanner = (banner: FrontpageBanner): void => {
		pull(this.banners, banner);
		this.reindexBanners();
	};

	@action moveBannerUp = (index: number): void => {
		if (index === 0) return;
		const temp = this.banners[index];
		this.banners[index] = this.banners[index - 1];
		this.banners[index - 1] = temp;
		this.reindexBanners();
	};

	@action moveBannerDown = (index: number): void => {
		if (index === this.banners.length - 1) return;
		const temp = this.banners[index];
		this.banners[index] = this.banners[index + 1];
		this.banners[index + 1] = temp;
		this.reindexBanners();
	};

	@action private reindexBanners = (): void => {
		this.banners.forEach((b, i) => (b.sortIndex = i));
	};

	@action uploadImage = async (file: File): Promise<string> => {
		this.uploadingImage = true;
		try {
			const fileName = await this.adminRepo.uploadBannerImage(file);
			return fileName;
		} finally {
			runInAction(() => {
				this.uploadingImage = false;
			});
		}
	};

	@action save = async (): Promise<void> => {
		try {
			this.submitting = true;

			const config: FrontpageConfigContract = {
				banners: this.banners.map((b) => b.toContract()),
			};

			await this.adminRepo.saveFrontpageConfig(config);
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
