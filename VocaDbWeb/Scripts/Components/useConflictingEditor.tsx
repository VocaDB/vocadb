import { EntryEditDataContract } from '@/DataContracts/User/EntryEditDataContract';
import { EntryType } from '@/Models/EntryType';
import { UserRepository } from '@/Repositories/UserRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import React from 'react';
import { useParams } from 'react-router-dom';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const userRepo = new UserRepository(httpClient, urlMapper);

export const useConflictingEditor = (
	entryType: EntryType,
): EntryEditDataContract | undefined => {
	const { id } = useParams();

	const [
		conflictingEditor,
		setConflictingEditor,
	] = React.useState<EntryEditDataContract>();

	React.useEffect(() => {
		if (!id) return;

		const checkConcurrentEdits = async (): Promise<void> => {
			const conflictingEditor = await userRepo.refreshEntryEdit({
				entryType: entryType,
				entryId: Number(id),
			});

			setConflictingEditor(conflictingEditor);
		};

		checkConcurrentEdits();

		// REVIEW: Would it be better to use server-sent events or websockets rather than polling?
		const intervalId = window.setInterval(checkConcurrentEdits, 10000);

		return (): void => window.clearInterval(intervalId);
	}, [entryType, id]);

	return conflictingEditor;
};
