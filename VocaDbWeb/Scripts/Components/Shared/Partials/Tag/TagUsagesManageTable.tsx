import Alert from '@/Bootstrap/Alert';
import { TagUsageWithVotesContract } from '@/DataContracts/Tag/TagUsageWithVotesContract';
import { EntryType } from '@/Models/EntryType';
import { LoginManager } from '@/Models/LoginManager';
import React from 'react';

const loginManager = new LoginManager(vdb.values);

interface TagUsagesManageTableProps {
	entryType: EntryType;
	tagUsages: TagUsageWithVotesContract[];
	canRemove: boolean;
}

export const TagUsagesManageTable = React.memo(
	({
		entryType,
		tagUsages,
		canRemove,
	}: TagUsagesManageTableProps): React.ReactElement => {
		return (
			<>
				<Alert>
					You can use this to deassociate tags that are no longer valid for this
					entry. Be careful, this cannot be undone.{/* LOCALIZE */}
				</Alert>

				{tagUsages.length > 0 ? (
					<table>
						<thead>
							<tr>
								<th>Tag{/* LOCALIZE */}</th>
								<th>Count{/* LOCALIZE */}</th>
								{loginManager.canManageUserPermissions && (
									<th>Votes{/* LOCALIZE */}</th>
								)}
								<th>Date{/* LOCALIZE */}</th>
								<th></th>
							</tr>
						</thead>
						<tbody>
							{tagUsages.map((tagUsage, index) => (
								<tr key={index}>{/* TODO */}</tr>
							))}
						</tbody>
					</table>
				) : (
					<>No tags{/* LOCALIZE */}</>
				)}
			</>
		);
	},
);
