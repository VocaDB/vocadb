import Alert from '@/Bootstrap/Alert';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { TagUsageWithVotesForApiContract } from '@/DataContracts/Tag/TagUsageWithVotesForApiContract';
import { EntryType } from '@/Models/EntryType';
import { userRepo } from '@/Repositories/UserRepository';
import dayjs from '@/dayjs';
import React from 'react';

import { UserIconLink_UserForApiContract } from '../User/UserIconLink_UserForApiContract';
import { TagLink } from './TagLink';

const undoTimeoutMs = 8000;

interface PendingDeletion {
	entryId: number;
	index: number;
	tagUsage: TagUsageWithVotesForApiContract;
}

interface TagUsagesManageTableProps {
	entryType: EntryType;
	entryId: number;
	tagUsages: TagUsageWithVotesForApiContract[];
	setTagUsages: (arg0: TagUsageWithVotesForApiContract[]) => void;
	canRemove: boolean;
}

export const TagUsagesManageTable = React.memo(
	({
		entryType,
		entryId,
		tagUsages,
		setTagUsages,
		canRemove,
	}: TagUsagesManageTableProps): React.ReactElement => {
		const [pendingDeletion, setPendingDeletion] =
			React.useState<PendingDeletion | null>(null);
		const pendingDeletionRef = React.useRef<PendingDeletion | null>(null);
		const entryIdRef = React.useRef(entryId);
		entryIdRef.current = entryId;
		const tagUsagesRef = React.useRef(tagUsages);
		tagUsagesRef.current = tagUsages;
		const setTagUsagesRef = React.useRef(setTagUsages);
		setTagUsagesRef.current = setTagUsages;

		const activePending =
			pendingDeletion?.entryId === entryId ? pendingDeletion : null;

		const restoreTagUsage = React.useCallback(
			(pending: PendingDeletion): void => {
				if (pending.entryId !== entryIdRef.current) return;

				const current = tagUsagesRef.current;

				if (current.some((t) => t.id === pending.tagUsage.id)) return;

				const next = [...current];
				next.splice(Math.min(pending.index, next.length), 0, pending.tagUsage);
				setTagUsagesRef.current(next);
			},
			[],
		);

		React.useEffect(() => {
			const pending = pendingDeletionRef.current;

			if (!pending || pending.entryId === entryId) return;

			pendingDeletionRef.current = null;
			setPendingDeletion(null);
			userRepo
				.deleteTag({
					entryType,
					tagUsageId: pending.tagUsage.id,
				})
				.catch(() => {});
		}, [entryId, entryType]);

		React.useEffect(() => {
			if (!activePending) return;

			const timeoutId = window.setTimeout(() => {
				if (pendingDeletionRef.current !== activePending) return;

				pendingDeletionRef.current = null;
				setPendingDeletion(null);
				userRepo
					.deleteTag({
						entryType,
						tagUsageId: activePending.tagUsage.id,
					})
					.catch(() => restoreTagUsage(activePending));
			}, undoTimeoutMs);

			return () => window.clearTimeout(timeoutId);
		}, [activePending, entryType, restoreTagUsage]);

		React.useEffect(() => {
			const handlePageHide = (): void => {
				const pending = pendingDeletionRef.current;

				if (!pending) return;

				pendingDeletionRef.current = null;
				setPendingDeletion(null);
				userRepo.deleteTagKeepalive({
					entryType,
					tagUsageId: pending.tagUsage.id,
				});
			};

			window.addEventListener('pagehide', handlePageHide);

			return () => {
				window.removeEventListener('pagehide', handlePageHide);

				const pending = pendingDeletionRef.current;

				if (!pending) return;

				pendingDeletionRef.current = null;
				userRepo
					.deleteTag({
						entryType,
						tagUsageId: pending.tagUsage.id,
					})
					.catch(() => {});
			};
		}, [entryType]);

		const deleteTagUsage = (
			tagUsage: TagUsageWithVotesForApiContract,
			index: number,
		): void => {
			const previous = pendingDeletionRef.current;

			if (previous) {
				pendingDeletionRef.current = null;
				userRepo
					.deleteTag({
						entryType,
						tagUsageId: previous.tagUsage.id,
					})
					.catch(() => restoreTagUsage(previous));
			}

			const nextDeletion = { entryId, index, tagUsage };
			pendingDeletionRef.current = nextDeletion;
			setTagUsages(tagUsagesRef.current.filter((t) => t.id !== tagUsage.id));
			setPendingDeletion(nextDeletion);
		};

		const undoDeletion = (): void => {
			const pending = pendingDeletionRef.current;

			if (!pending || pending.entryId !== entryId) return;

			pendingDeletionRef.current = null;
			const current = tagUsagesRef.current;

			if (!current.some((t) => t.id === pending.tagUsage.id)) {
				const next = [...current];
				next.splice(Math.min(pending.index, next.length), 0, pending.tagUsage);
				setTagUsagesRef.current(next);
			}

			setPendingDeletion(null);
		};

		return (
			<>
				{activePending && (
					<Alert variant="info">
						Tag &quot;{activePending.tagUsage.tag.name}&quot; removed.{' '}
						<Button variant="link" onClick={undoDeletion}>
							Undo
						</Button>
					</Alert>
				)}

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
										{canRemove && (
											<SafeAnchor
												onClick={(): void => deleteTagUsage(tagUsage, index)}
												className="removeLink textLink"
											/>
										)}
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
