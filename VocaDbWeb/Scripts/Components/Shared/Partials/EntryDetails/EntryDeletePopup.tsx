import { EntryDeletePopupBase } from '@/Components/Shared/Partials/EntryDetails/EntryDeletePopupBase';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EntryDeletePopupProps {
	confirmText: string;
	deleteEntryStore: DeleteEntryStore;
	onDelete?: () => void;
}

export const EntryDeletePopup = ({
	confirmText,
	deleteEntryStore,
	onDelete,
}: EntryDeletePopupProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	return (
		<EntryDeletePopupBase
			confirmText={confirmText}
			deleteEntryStore={deleteEntryStore}
			title={t('ViewRes:Shared.Delete')}
			deleteButtonProps={{ text: t('ViewRes:Shared.Delete') }}
			onDelete={onDelete}
		/>
	);
};
