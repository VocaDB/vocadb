import EntryType from '@Models/EntryType';
import React from 'react';
import { useTranslation } from 'react-i18next';

const useChangedFieldNames = (): ((
	entryType: EntryType,
	changedField: string,
) => string | undefined) => {
	const { t } = useTranslation([
		'Resources',
		'VocaDb.Web.Resources.Domain.ReleaseEvents',
	]);

	return React.useCallback(
		(entryType: EntryType, changedField: string): string | undefined => {
			switch (entryType) {
				case EntryType.Album:
					return t(`Resources:AlbumEditableFieldNames.${changedField}`);

				case EntryType.Artist:
					return t(`Resources:ArtistEditableFieldNames.${changedField}`);

				case EntryType.ReleaseEvent:
					return t(`Resources:ReleaseEventEditableFieldNames.${changedField}`);

				case EntryType.ReleaseEventSeries:
					return t(
						`VocaDb.Web.Resources.Domain.ReleaseEvents:ReleaseEventSeriesEditableFieldNames.${changedField}`,
					);

				case EntryType.Song:
					return t(`Resources:SongEditableFieldNames.${changedField}`);

				case EntryType.SongList:
					return t(`Resources:SongListEditableFieldNames.${changedField}`);

				case EntryType.Tag:
					return t(`Resources:TagEditableFieldNames.${changedField}`);
			}
		},
		[t],
	);
};

export default useChangedFieldNames;
