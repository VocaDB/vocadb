import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { SongInListsStore } from '@/Stores/Song/SongDetailsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongInListsDialogProps {
	songInListsStore: SongInListsStore;
}

const SongInListsDialog = observer(
	({ songInListsStore }: SongInListsDialogProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Song']);

		return (
			<JQueryUIDialog
				title={t('ViewRes.Song:Details.SongInLists')}
				autoOpen={songInListsStore.dialogVisible}
				width={400}
				close={(): void =>
					runInAction(() => {
						songInListsStore.dialogVisible = false;
					})
				}
			>
				{songInListsStore.contentHtml && (
					// TODO: Replace this with React
					<div
						dangerouslySetInnerHTML={{
							__html: songInListsStore.contentHtml,
						}}
					/>
				)}
			</JQueryUIDialog>
		);
	},
);

export default SongInListsDialog;
