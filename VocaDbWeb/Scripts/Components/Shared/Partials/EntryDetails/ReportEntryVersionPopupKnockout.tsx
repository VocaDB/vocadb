import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ReportEntryVersionPopupKnockoutProps {
	reportEntryStore: ReportEntryStore;
}

export const ReportEntryVersionPopupKnockout = observer(
	({
		reportEntryStore,
	}: ReportEntryVersionPopupKnockoutProps): React.ReactElement => {
		const { t } = useTranslation(['AjaxRes', 'ViewRes']);

		return (
			<JQueryUIDialog
				title={t('ViewRes:EntryDetails.ReportAnError')}
				autoOpen={reportEntryStore.dialogVisible}
				width={315}
				buttons={[
					{
						text: t('ViewRes:Shared.Save'),
						click: async (): Promise<void> => {
							try {
								await reportEntryStore.send();

								showSuccessMessage(t('AjaxRes:Shared.ReportSent'));
							} catch {
								showErrorMessage(t('AjaxRes:Shared.UnableToSendReport'));
							}
						},
						disabled: !reportEntryStore.isValid,
					},
				]}
				close={(): void =>
					runInAction(() => {
						reportEntryStore.dialogVisible = false;
					})
				}
			>
				<p>{t('ViewRes:EntryDetails.ReportArchivedVersionExplanation')}</p>

				<label>{t('ViewRes:EntryDetails.ReportArchivedVersionMessage')}</label>
				<textarea
					value={reportEntryStore.notes}
					onChange={(e): void =>
						runInAction(() => {
							reportEntryStore.notes = e.target.value;
						})
					}
					className="input-xlarge"
					maxLength={400}
					rows={3}
				/>
			</JQueryUIDialog>
		);
	},
);
