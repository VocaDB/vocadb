import JQueryUICheckbox from '@/JQueryUI/JQueryUICheckbox';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { ArtistRolesEditStore } from '@/Stores/Artist/AlbumArtistRolesEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistRolesEditViewModelProps {
	artistRolesEditStore: ArtistRolesEditStore;
}

const ArtistRolesEditViewModel = observer(
	({
		artistRolesEditStore,
	}: ArtistRolesEditViewModelProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Album']);

		return (
			<JQueryUIDialog
				title={t('ViewRes.Album:Edit.ArtistRolesTitle')}
				autoOpen={artistRolesEditStore.dialogVisible}
				width={550}
				close={(): void =>
					runInAction(() => {
						artistRolesEditStore.dialogVisible = false;
					})
				}
				buttons={[
					{
						text: t('ViewRes:Shared.Save'),
						click: artistRolesEditStore.save,
					},
				]}
			>
				<div>
					{artistRolesEditStore.roleSelections.map((selection, index) => (
						<React.Fragment key={index}>
							{index > 0 && ' '}
							<span className="tag">
								<JQueryUICheckbox
									id={`artistRole_${selection.id}`}
									checked={selection.selected}
									onChange={(e): void =>
										runInAction(() => {
											selection.selected = e.target.checked;
										})
									}
								>
									{selection.name}
								</JQueryUICheckbox>
							</span>
						</React.Fragment>
					))}
				</div>
			</JQueryUIDialog>
		);
	},
);

export default ArtistRolesEditViewModel;
