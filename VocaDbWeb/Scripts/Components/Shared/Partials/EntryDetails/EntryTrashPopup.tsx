import EntryDeletePopupBase from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopupBase';
import DeleteEntryStore from '@/Stores/DeleteEntryStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EntryTrashPopupProps {
	confirmText: string;
	deleteEntryStore: DeleteEntryStore;
	onDelete?: () => void;
}

const EntryTrashPopup = ({
	confirmText,
	deleteEntryStore,
	onDelete,
}: EntryTrashPopupProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	return (
		<EntryDeletePopupBase
			confirmText={confirmText}
			deleteEntryStore={deleteEntryStore}
			title={t('ViewRes:EntryEdit.MoveToTrash')}
			deleteButtonProps={{ text: t('ViewRes:EntryEdit.MoveToTrash') }}
			onDelete={onDelete}
		/>
	);
};

export default EntryTrashPopup;
