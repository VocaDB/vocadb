import Alert from '@Bootstrap/Alert';
import { EntryToolTip } from '@Components/KnockoutExtensions/EntryToolTip';
import DuplicateEntryResultContract from '@DataContracts/DuplicateEntryResultContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';

import NotificationIcon from '../Partials/Shared/NotificationIcon';

interface DuplicateEntriesMessageProps {
	dupeEntries: DuplicateEntryResultContract[];
}

// Shows a message for duplicate instances of an entry.
const DuplicateEntriesMessage = React.memo(
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
								as="a"
								value={entry.entry}
								href={EntryUrlMapper.details_entry(entry.entry)}
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

export default DuplicateEntriesMessage;
