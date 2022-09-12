import Alert from '@/Bootstrap/Alert';
import { EntryLink } from '@/Components/Shared/Partials/Shared/EntryLink';
import { NotificationIcon } from '@/Components/Shared/Partials/Shared/NotificationIcon';
import { DuplicateEntryResultContract } from '@/DataContracts/DuplicateEntryResultContract';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface DuplicateEntriesMessageProps {
	dupeEntries: DuplicateEntryResultContract[];
}

// Shows a message for duplicate instances of an entry.
export const DuplicateEntriesMessage = React.memo(
	({ dupeEntries }: DuplicateEntriesMessageProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return dupeEntries && dupeEntries.length > 0 ? (
			<Alert>
				<NotificationIcon />
				{t('ViewRes:EntryCreate.FoundPossibleDuplicates')}
				<ul>
					{dupeEntries.map((entry, index) => (
						<li key={index}>
							<EntryLink
								entry={entry.entry}
								tooltip
								/* TODO: target="_blank" */
							>
								{entry.entry.name.displayName}
							</EntryLink>{' '}
							(<span>{entry.entry.entryTypeName}</span>)
							{entry.entry.artistString && (
								<div>
									<span>{entry.entry.artistString}</span>
								</div>
							)}
						</li>
					))}
				</ul>
			</Alert>
		) : (
			<></>
		);
	},
);
