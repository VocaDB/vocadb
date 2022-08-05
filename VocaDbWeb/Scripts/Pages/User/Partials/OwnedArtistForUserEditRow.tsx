import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { ArtistForUserForApiContract } from '@/DataContracts/User/ArtistForUserForApiContract';
import { UserEditStore } from '@/Stores/User/UserEditStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface OwnedArtistForUserEditRowProps {
	userEditStore: UserEditStore;
	ownedArtist: ArtistForUserForApiContract;
}

export const OwnedArtistForUserEditRow = observer(
	({
		userEditStore,
		ownedArtist,
	}: OwnedArtistForUserEditRowProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<tr>
				<td>
					<ArtistLink artist={ownedArtist.artist} />
				</td>
				<td>
					<SafeAnchor
						href="#"
						className="artistRemove textLink deleteLink"
						onClick={(): void => userEditStore.removeArtist(ownedArtist)}
					>
						{t('ViewRes:Shared.Delete')}
					</SafeAnchor>
				</td>
			</tr>
		);
	},
);
