import SafeAnchor from '@/Bootstrap/SafeAnchor';
import EntryType from '@/Models/EntryType';
import EntryUrlMapper from '@/Shared/EntryUrlMapper';
import AlbumEditStore from '@/Stores/Album/AlbumEditStore';
import SongInAlbumEditStore from '@/Stores/SongInAlbumEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongInAlbumEditProps {
	albumEditStore: AlbumEditStore;
	songInAlbumEditStore: SongInAlbumEditStore;
}

const SongInAlbumEdit = observer(
	({
		albumEditStore,
		songInAlbumEditStore,
	}: SongInAlbumEditProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Album']);

		return (
			<tr className="ui-state-default">
				<td style={{ cursor: 'move' }} className="handle">
					<span className="ui-icon ui-icon-arrowthick-2-n-s" />
				</td>
				<td>
					<input
						type="checkbox"
						checked={songInAlbumEditStore.selected}
						onChange={(e): void =>
							runInAction(() => {
								songInAlbumEditStore.selected = e.target.checked;
							})
						}
					/>
				</td>
				<td>
					<span>{songInAlbumEditStore.discNumber}</span>
				</td>
				<td>
					<span>{songInAlbumEditStore.trackNumber}</span>
				</td>
				<td>
					{songInAlbumEditStore.isCustomTrack ? (
						<span>{songInAlbumEditStore.songName}</span>
					) : (
						<SafeAnchor
							title={songInAlbumEditStore.songAdditionalNames}
							onClick={(): void =>
								albumEditStore.editTrackProperties(songInAlbumEditStore)
							}
							href="#"
						>
							{songInAlbumEditStore.songName}
						</SafeAnchor>
					)}
					<br />
					<span className="extraInfo">{songInAlbumEditStore.artistString}</span>
				</td>
				<td>
					<input
						type="checkbox"
						checked={songInAlbumEditStore.isNextDisc}
						onChange={(e): void =>
							runInAction(() => {
								songInAlbumEditStore.isNextDisc = e.target.checked;
							})
						}
					/>{' '}
					{t('ViewRes.Album:Edit.TrNextDisc')}
				</td>
				<td>
					<span>
						{!!songInAlbumEditStore.songId && (
							<SafeAnchor
								href={EntryUrlMapper.details(
									EntryType.Song,
									songInAlbumEditStore.songId,
								)}
								target="_blank"
								title="Open for edit" /* TODO: localize */
								className="iconLink editLink"
							>
								{t('ViewRes:Shared.Edit')}
							</SafeAnchor>
						)}
					</span>
					<SafeAnchor
						onClick={(): void =>
							albumEditStore.removeTrack(songInAlbumEditStore)
						}
						href="#"
						className="iconLink removeLink"
						title="Remove from album" /* TODO: localize */
					>
						{t('ViewRes:Shared.Remove')}
					</SafeAnchor>
				</td>
			</tr>
		);
	},
);

export default SongInAlbumEdit;
