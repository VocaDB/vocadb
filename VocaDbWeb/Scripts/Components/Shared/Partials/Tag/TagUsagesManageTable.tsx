import Alert from '@/Bootstrap/Alert';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { TagUsageWithVotesForApiContract } from '@/DataContracts/Tag/TagUsageWithVotesForApiContract';
import { EntryType } from '@/Models/EntryType';
import { userRepo } from '@/Repositories/UserRepository';
import { useVdb } from '@/VdbContext';
import dayjs from '@/dayjs';
import React from 'react';

import { UserIconLink_UserForApiContract } from '../User/UserIconLink_UserForApiContract';
import { TagLink } from './TagLink';

interface TagUsagesManageTableProps {
	entryType: EntryType;
	tagUsages: TagUsageWithVotesForApiContract[];
	setTagUsages: (arg0: TagUsageWithVotesForApiContract[]) => void;
	canRemove: boolean;
}

export const TagUsagesManageTable = React.memo(
	({
		entryType,
		tagUsages,
		setTagUsages,
		canRemove,
	}: TagUsagesManageTableProps): React.ReactElement => {
		const vdb = useVdb();
		const deleteTagUsage = (tagUsageId: number): void => {
			userRepo
				.deleteTag({
					entryType,
					tagUsageId,
				})
				.then(() => setTagUsages(tagUsages.filter((t) => t.id !== tagUsageId)));
		};

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
								<th>Votes{/* LOCALIZE */}</th>
								<th>Date{/* LOCALIZE */}</th>
								<th></th>
							</tr>
						</thead>
						<tbody>
							{tagUsages.map((tagUsage, index) => (
								<tr key={index}>
									<td>
										<TagLink tag={tagUsage.tag} />
									</td>
									<td>{tagUsage.count}</td>
									<td>
										{tagUsage.votes.map((user, index) => (
											// eslint-disable-next-line react/jsx-pascal-case
											<UserIconLink_UserForApiContract
												key={index}
												user={user}
											/>
										))}
									</td>
									<td>{dayjs(tagUsage.date).format('lll')}</td>
									<td>
										<SafeAnchor
											onClick={(): void => deleteTagUsage(tagUsage.id)}
											className="removeLink textLink"
										/>
									</td>
								</tr>
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
