import JQueryUIDialog from '@JQueryUI/JQueryUIDialog';
import { CustomizeArtistSubscriptionStore } from '@Stores/Artist/ArtistDetailsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface CustomizeArtistSubscriptionDialogProps {
	customizeArtistSubscriptionStore: CustomizeArtistSubscriptionStore;
}

const CustomizeArtistSubscriptionDialog = observer(
	({
		customizeArtistSubscriptionStore,
	}: CustomizeArtistSubscriptionDialogProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Artist']);

		return (
			<JQueryUIDialog
				autoOpen={customizeArtistSubscriptionStore.dialogVisible}
				width={400}
				height={160}
				close={(): void =>
					runInAction(() => {
						customizeArtistSubscriptionStore.dialogVisible = false;
					})
				}
				title={t('ViewRes.Artist:Details.CustomizeSubscriptionTitle')}
			>
				<div>
					<p>{t('ViewRes.Artist:Details.SubscriptionOptions')}</p>

					<label>
						<input
							type="radio"
							value="Nothing"
							checked={
								customizeArtistSubscriptionStore.notificationsMethod ===
								'Nothing'
							}
							onChange={(e): void =>
								runInAction(() => {
									customizeArtistSubscriptionStore.notificationsMethod =
										e.target.value;
								})
							}
						/>{' '}
						{t('ViewRes.Artist:Details.SubscriptionNoNotification')}
					</label>
					<label>
						<input
							type="radio"
							value="Site"
							checked={
								customizeArtistSubscriptionStore.notificationsMethod === 'Site'
							}
							onChange={(e): void =>
								runInAction(() => {
									customizeArtistSubscriptionStore.notificationsMethod =
										e.target.value;
								})
							}
						/>{' '}
						{t('ViewRes.Artist:Details.SubscriptionOnSite')}
					</label>
					<label>
						<input
							type="radio"
							value="Email"
							checked={
								customizeArtistSubscriptionStore.notificationsMethod === 'Email'
							}
							onChange={(e): void =>
								runInAction(() => {
									customizeArtistSubscriptionStore.notificationsMethod =
										e.target.value;
								})
							}
						/>{' '}
						{t('ViewRes.Artist:Details.SubscriptionEmail')}
					</label>
				</div>
			</JQueryUIDialog>
		);
	},
);

export default CustomizeArtistSubscriptionDialog;
