import Alert from '@/Bootstrap/Alert';
import { EntryToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { NotificationIcon } from '@/Components/Shared/Partials/Shared/NotificationIcon';
import { DuplicateEntryResultContract } from '@/DataContracts/DuplicateEntryResultContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

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
							<EntryToolTip
								as={Link}
								value={entry.entry}
								to={EntryUrlMapper.details_entry(entry.entry)}
								/* TODO: target="_blank" */
							>
								{entry.entry.name.displayName}
							</EntryToolTip>{' '}
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
