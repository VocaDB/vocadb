import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { ArtistRoles } from '@/Models/Artists/ArtistRoles';
import { AlbumEditStore } from '@/Stores/Album/AlbumEditStore';
import { ArtistForAlbumEditStore } from '@/Stores/ArtistForAlbumEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistForAlbumEditProps {
	albumEditStore: AlbumEditStore;
	artistForAlbumEditStore: ArtistForAlbumEditStore;
}

const ArtistForAlbumEdit = observer(
	({
		albumEditStore,
		artistForAlbumEditStore,
	}: ArtistForAlbumEditProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.Album']);

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
							<span>{artistForAlbumEditStore.name}</span>
						</div>
					)}
					{vdb.values.allowCustomArtistName && (
						<SafeAnchor
							href="#"
							className="textLink editLink"
							onClick={(): void =>
								albumEditStore.customizeName(artistForAlbumEditStore)
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
						/>{' '}
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
								albumEditStore.editArtistRoles(artistForAlbumEditStore)
							}
						>
							{t('ViewRes.Album:ArtistForAlbumEditRow.Customize')}
						</SafeAnchor>
					)}
				</td>
				<td>
					<SafeAnchor
						onClick={(): void =>
							albumEditStore.removeArtist(artistForAlbumEditStore)
						}
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

export default ArtistForAlbumEdit;
