import { showSuccessMessage } from '@Components/ui';
import JQueryUIDialog from '@JQueryUI/JQueryUIDialog';
import ReportEntryStore, { IEntryReportType } from '@Stores/ReportEntryStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ReportEntryPopupKnockoutProps {
	reportEntryStore: ReportEntryStore;
	reportTypes: IEntryReportType[];
}

const ReportEntryPopupKnockout = observer(
	({
		reportEntryStore,
		reportTypes,
	}: ReportEntryPopupKnockoutProps): React.ReactElement => {
		const { t } = useTranslation(['AjaxRes', 'ViewRes']);

		React.useEffect(() => {
			runInAction(() => {
				reportEntryStore.reportType = reportTypes[0];
			});
		}, [reportEntryStore, reportTypes]);

		return (
			<JQueryUIDialog
				title={t('ViewRes:EntryDetails.ReportAnError')}
				autoOpen={reportEntryStore.dialogVisible}
				width={315}
				buttons={[
					{
						text: t('ViewRes:Shared.Save'),
						click: async (): Promise<void> => {
							await reportEntryStore.send();

							showSuccessMessage(t('AjaxRes:Shared.ReportSent'));
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
				<label>{t('ViewRes:EntryDetails.ReportType')}</label>
				<select
					value={reportEntryStore.reportType?.id}
					onChange={(e): void =>
						runInAction(() => {
							reportEntryStore.reportType = reportTypes.find(
								(t) => t.id === e.target.value,
							);
						})
					}
				>
					{reportTypes.map((reportType) => (
						<option value={reportType.id} key={reportType.id}>
							{reportType.name}
						</option>
					))}
				</select>

				<label>
					{t('ViewRes:EntryDetails.ReportNotes')}
					{reportEntryStore.reportType &&
						reportEntryStore.reportType.notesRequired && (
							<>
								{' '}
								<span>{t('ViewRes:EntryDetails:ReportNotesRequired')}</span>
							</>
						)}
				</label>
				<textarea
					value={reportEntryStore.notes}
					onChange={(e): void =>
						runInAction(() => {
							reportEntryStore.notes = e.target.value;
						})
					}
					className="input-xlarge"
					maxLength={400}
				/>
			</JQueryUIDialog>
		);
	},
);

export default ReportEntryPopupKnockout;
