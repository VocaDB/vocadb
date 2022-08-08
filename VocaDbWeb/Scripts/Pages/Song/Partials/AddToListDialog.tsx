import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import { showSuccessMessage } from '@/Components/ui';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { SongListsStore } from '@/Stores/Song/SongDetailsStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface AddToListDialogProps {
	songListsStore: SongListsStore;
}

const AddToListDialog = observer(
	({ songListsStore }: AddToListDialogProps): React.ReactElement => {
		const { t } = useTranslation(['AjaxRes', 'ViewRes.Song']);

		return (
			<JQueryUIDialog
				title={t('ViewRes.Song:Details.AddToList')}
				autoOpen={songListsStore.dialogVisible}
				width={300}
				buttons={[
					{
						text: t('ViewRes:Shared.Save'),
						click: async (): Promise<void> => {
							await songListsStore.addSongToList();

							showSuccessMessage(t('AjaxRes:Song.AddedToList'));
						},
					},
				]}
				close={(): void =>
					runInAction(() => {
						songListsStore.dialogVisible = false;
					})
				}
			>
				<div className="song-add-to-list-dialog">
					<ButtonGroup>
						{songListsStore.personalLists.length > 0 && (
							<Button
								className={classNames(
									'btn-small',
									songListsStore.tabName === SongListsStore.tabName_Personal &&
										'active',
								)}
								onClick={(): void =>
									runInAction(() => {
										songListsStore.tabName = SongListsStore.tabName_Personal;
									})
								}
								href="#"
							>
								{t('ViewRes.Song:Details.ListsTabPersonal')}
							</Button>
						)}
						{songListsStore.featuredLists.length > 0 && (
							<Button
								className={classNames(
									'btn-small',
									songListsStore.tabName === SongListsStore.tabName_Featured &&
										'active',
								)}
								onClick={(): void =>
									runInAction(() => {
										songListsStore.tabName = SongListsStore.tabName_Featured;
									})
								}
								href="#"
							>
								{t('ViewRes.Song:Details.ListsTabFeatured')}
							</Button>
						)}
						<Button
							className={classNames(
								'btn-small',
								songListsStore.tabName === SongListsStore.tabName_New &&
									'active',
							)}
							onClick={(): void =>
								runInAction(() => {
									songListsStore.tabName = SongListsStore.tabName_New;
								})
							}
							href="#"
						>
							{t('ViewRes.Song:Details.ListsTabNew')}
						</Button>
					</ButtonGroup>

					{songListsStore.tabName === SongListsStore.tabName_New ? (
						<div>
							<label>{t('ViewRes.Song:Details.NewListName')}</label>
							<input
								type="text"
								value={songListsStore.newListName}
								onChange={(e): void =>
									runInAction(() => {
										songListsStore.newListName = e.target.value;
									})
								}
								maxLength={200}
								className="add-to-list-name"
							/>
						</div>
					) : (
						<select
							value={
								songListsStore.selectedListId &&
								songListsStore.songLists
									.map((songList) => songList.id)
									.includes(songListsStore.selectedListId)
									? songListsStore.selectedListId
									: undefined
							}
							onChange={(e): void =>
								runInAction(() => {
									songListsStore.selectedListId = Number(e.target.value);
								})
							}
							size={6}
							className="add-to-list-song-lists"
						>
							{songListsStore.songLists.map((songList) => (
								<option value={songList.id} key={songList.id}>
									{songList.name}
								</option>
							))}
						</select>
					)}

					<label>{t('ViewRes.Song:Details.Notes')}</label>
					<input
						type="text"
						value={songListsStore.notes}
						onChange={(e): void =>
							runInAction(() => {
								songListsStore.notes = e.target.value;
							})
						}
						maxLength={200}
						className="add-to-list-notes"
					/>
				</div>
			</JQueryUIDialog>
		);
	},
);

export default AddToListDialog;
