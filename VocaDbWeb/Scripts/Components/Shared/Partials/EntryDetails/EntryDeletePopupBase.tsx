import JQueryUIDialog from '@JQueryUI/JQueryUIDialog';
import DeleteEntryStore from '@Stores/DeleteEntryStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EntryDeletePopupBaseProps {
	confirmText: string;
	deleteEntryStore: DeleteEntryStore;
	title: string;
	deleteButtonProps: {
		text: string;
		icons?: any;
	};
	onDelete?: () => void;
}

const EntryDeletePopupBase = observer(
	({
		confirmText,
		deleteEntryStore,
		title,
		deleteButtonProps,
		onDelete,
	}: EntryDeletePopupBaseProps): React.ReactElement => {
		const { t } = useTranslation(['AjaxRes', 'ViewRes']);

		return (
			<JQueryUIDialog
				title={title}
				autoOpen={deleteEntryStore.dialogVisible}
				width={310}
				close={(): void =>
					runInAction(() => {
						deleteEntryStore.dialogVisible = false;
					})
				}
				buttons={[
					{
						...deleteButtonProps,
						click: async (): Promise<void> => {
							await deleteEntryStore.deleteEntry();

							// TODO: showSuccessMessage();

							onDelete?.();
						},
						disabled: !deleteEntryStore.isValid,
					},
					{
						text: t('ViewRes:Shared.Cancel'),
						click: (): void =>
							runInAction(() => {
								deleteEntryStore.dialogVisible = false;
							}),
					},
				]}
			>
				<p>{confirmText}</p>

				<textarea
					value={deleteEntryStore.notes}
					onChange={(e): void =>
						runInAction(() => {
							deleteEntryStore.notes = e.target.value;
						})
					}
					required={deleteEntryStore.notesRequired}
					className="input-xlarge"
					maxLength={200}
				/>
			</JQueryUIDialog>
		);
	},
);

export default EntryDeletePopupBase;
