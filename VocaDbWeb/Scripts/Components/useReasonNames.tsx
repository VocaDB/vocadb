import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { EntryType } from '@/Models/EntryType';
import React from 'react';
import { useTranslation } from 'react-i18next';

export const useReasonNames = (): ((
	entryType: EntryType,
	archivedVersion: ArchivedVersionContract,
) => string | undefined) => {
	const { t } = useTranslation(['Resources']);

	return React.useCallback(
		(
			entryType: EntryType,
			archivedVersion: ArchivedVersionContract,
		): string | undefined => {
			switch (entryType) {
				case EntryType.Album:
					return archivedVersion.reason === 'Unknown'
						? archivedVersion.notes
						: t(`Resources:AlbumArchiveReasonNames.${archivedVersion.reason}`);

				case EntryType.Artist:
					return archivedVersion.reason === 'Unknown'
						? archivedVersion.notes
						: t(`Resources:ArtistArchiveReasonNames.${archivedVersion.reason}`);

				case EntryType.Song:
					return archivedVersion.reason === 'Unknown'
						? archivedVersion.notes
						: t(`Resources:SongArchiveReasonNames.${archivedVersion.reason}`);

				case EntryType.ReleaseEvent:
				case EntryType.ReleaseEventSeries:
				case EntryType.Tag:
				case EntryType.SongList:
				case EntryType.Venue:
					return t(`Resources:EntryEditEventNames.${archivedVersion.reason}`);
			}
		},
		[t],
	);
};
