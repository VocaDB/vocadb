import SafeAnchor from '@/Bootstrap/SafeAnchor';
import ArtistRoles from '@/Models/Artists/ArtistRoles';
import ArtistForAlbumEditStore from '@/Stores/ArtistForAlbumEditStore';
import SongEditStore from '@/Stores/Song/SongEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import ArtistLink from '../../../Components/Shared/Partials/Artist/ArtistLink';

interface ArtistForSongEditProps {
	songEditStore: SongEditStore;
	artistForAlbumEditStore: ArtistForAlbumEditStore;
}

const ArtistForSongEdit = observer(
	({
		songEditStore,
		artistForAlbumEditStore,
	}: ArtistForSongEditProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Album']);

		return (
			<tr>
				<td>
					{artistForAlbumEditStore.artist ? (
						<div>
							<ArtistLink artist={artistForAlbumEditStore.artist} tooltip />
							<br />
							<span className="extraInfo">
								{artistForAlbumEditStore.artist.additionalNames}
							</span>
						</div>
					) : (
						<div>
							<span className="extraInfo">{artistForAlbumEditStore.name}</span>
						</div>
					)}
					{vdb.values.allowCustomArtistName && (
						<SafeAnchor
							href="#"
							className="textLink editLink"
							onClick={(): void =>
								songEditStore.customizeName(artistForAlbumEditStore)
							}
						>
							{t('ViewRes.Album:ArtistForAlbumEditRow.Customize')}
						</SafeAnchor>
					)}
				</td>

				<td>
					<label className="checkbox inline">
						<input
							type="checkbox"
							checked={artistForAlbumEditStore.isSupport}
							onChange={(e): void =>
								runInAction(() => {
									artistForAlbumEditStore.isSupport = e.target.checked;
								})
							}
						/>
						{t('ViewRes.Album:ArtistForAlbumEditRow.Support')}
					</label>
				</td>
				<td>
					<div>
						{artistForAlbumEditStore.rolesArray
							.filter((role) => role !== ArtistRoles[ArtistRoles.Default])
							.map((role) => (
								<div key={role}>
									<span>{t(`Resources:ArtistRoleNames.${role}`)}</span>
									<br />
								</div>
							))}
					</div>

					{artistForAlbumEditStore.isCustomizable && (
						<SafeAnchor
							href="#"
							className="artistRolesEdit textLink editLink"
							onClick={(): void =>
								songEditStore.editArtistRoles(artistForAlbumEditStore)
							}
						>
							{t('ViewRes.Album:ArtistForAlbumEditRow.Customize')}
						</SafeAnchor>
					)}
				</td>
				<td>
					<SafeAnchor
						onClick={(): void => {
							songEditStore.removeArtist(artistForAlbumEditStore);
						}}
						href="#"
						className="textLink removeLink"
					>
						{t('ViewRes:Shared.Remove')}
					</SafeAnchor>
				</td>
			</tr>
		);
	},
);

export default ArtistForSongEdit;
