import ActivityEntryContract from '@DataContracts/ActivityEntry/ActivityEntryContract';
import EntryContract from '@DataContracts/EntryContract';
import ArchivedVersionContract from '@DataContracts/Versioning/ArchivedVersionContract';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import _ from 'lodash';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';

import IconNameAndTypeLinkKnockout from '../User/IconNameAndTypeLinkKnockout';

const useActivityFeedEventName = (): ((
	activityEntry: ActivityEntryContract,
) => string | undefined) => {
	const { t } = useTranslation(['HelperRes']);

	return React.useCallback(
		(activityEntry: ActivityEntryContract): string | undefined => {
			switch (activityEntry.editEvent) {
				case 'Created':
					return t(`HelperRes:ActivityFeedHelper.CreatedNew`, {
						0: t(
							`HelperRes:ActivityFeedHelper.Entry${activityEntry.entry.entryType}`,
						),
					});

				case 'Updated':
					return t('HelperRes:ActivityFeedHelper.Updated', {
						0: t(
							`HelperRes:ActivityFeedHelper.Entry${activityEntry.entry.entryType}`,
						),
					});

				default:
					return undefined;
			}
		},
		[t],
	);
};

const useChangedFieldNames = (): ((
	entry: EntryContract,
	archivedVersion?: ArchivedVersionContract,
) => string | undefined) => {
	const { t } = useTranslation(['Resources']);

	return React.useCallback(
		(
			entry: EntryContract,
			archivedVersion?: ArchivedVersionContract,
		): string | undefined => {
			if (
				!archivedVersion ||
				!archivedVersion.changedFields ||
				archivedVersion.changedFields.length === 0
			)
				return undefined;

			switch (EntryType[entry.entryType as keyof typeof EntryType]) {
				case EntryType.Album:
					return _.map(archivedVersion.changedFields, (f) =>
						t(`Resources:AlbumEditableFieldNames.${f}`),
					).join(', ');

				case EntryType.Artist:
					return _.map(archivedVersion.changedFields, (f) =>
						t(`Resources:ArtistEditableFieldNames.${f}`),
					).join(', ');

				case EntryType.ReleaseEvent:
					return _.map(archivedVersion.changedFields, (f) =>
						t(`Resources:ReleaseEventEditableFieldNames.${f}`),
					).join(', ');

				case EntryType.Song:
					return _.map(archivedVersion.changedFields, (f) =>
						t(`Resources:SongEditableFieldNames.${f}`),
					).join(', ');

				case EntryType.SongList:
					return _.map(archivedVersion.changedFields, (f) =>
						t(`Resources:SongListEditableFieldNames.${f}`),
					).join(', ');

				case EntryType.Tag:
					return _.map(archivedVersion.changedFields, (f) =>
						t(`Resources:TagEditableFieldNames.${f}`),
					).join(', ');

				default:
					return _.join(', ') /* TODO: localize */;
			}
		},
		[t],
	);
};

const useEntryTypeName = (): ((entry: EntryContract) => string | undefined) => {
	const { t } = useTranslation([
		'Resources',
		'VocaDb.Model.Resources',
		'VocaDb.Model.Resources.Albums',
		'VocaDb.Model.Resources.Songs',
	]);

	return React.useCallback(
		(entry: EntryContract): string | undefined => {
			switch (EntryType[entry.entryType as keyof typeof EntryType]) {
				case EntryType.Album:
					return t(
						`VocaDb.Model.Resources.Albums:DiscTypeNames.${entry.discType}`,
					);

				case EntryType.Artist:
					return t(
						`VocaDb.Model.Resources:ArtistTypeNames.${entry.artistType}`,
					);

				case EntryType.Song:
					return t(
						`VocaDb.Model.Resources.Songs:SongTypeNames.${entry.songType}`,
					);

				case EntryType.SongList:
					return t(
						`Resources:SongListFeaturedCategoryNames.${entry.songListFeaturedCategory}`,
					);

				case EntryType.Tag:
					return entry.tagCategoryName /* TODO: localize */;

				default:
					return undefined;
			}
		},
		[t],
	);
};

const getEntryUrl = (entry: EntryContract): string => {
	return EntryUrlMapper.details_entry(entry, entry.urlSlug);
};

interface ActivityEntryKnockoutProps {
	entry: ActivityEntryContract;
	showDetails?: boolean;
}

const ActivityEntryKnockout = ({
	entry,
	showDetails = false,
}: ActivityEntryKnockoutProps): React.ReactElement => {
	const { t } = useTranslation(['HelperRes', 'ViewRes']);

	const activityFeedEventName = useActivityFeedEventName();
	const changedFieldNames = useChangedFieldNames();
	const entryTypeName = useEntryTypeName();

	return (
		<div className="message activityEntry ui-tabs ui-widget ui-widget-content ui-corner-all">
			{entry.author ? (
				<span>
					<IconNameAndTypeLinkKnockout user={entry.author} />
				</span>
			) : (
				<span>{t('HelperRes:ActivityFeedHelper.Someone')}</span>
			)}{' '}
			<span>{activityFeedEventName(entry)}</span>
			{showDetails && (
				<>
					{' '}
					<span>
						{entry.archivedVersion.changedFields &&
							entry.archivedVersion.changedFields.length > 0 && (
								<span>
									({changedFieldNames(entry.entry, entry.archivedVersion)})
								</span>
							)}
						{entry.archivedVersion.notes && (
							<span>"{entry.archivedVersion.notes}"</span>
						)}{' '}
						{entry.entry.entryType !== 'SongList' &&
							entry.entry.entryType !== 'ReleaseEvent' && (
								<span>
									(
									<a
										href={`/${entry.entry.entryType}/ViewVersion/${entry.archivedVersion.id}`}
									>
										{t('ViewRes:Misc.Details')}
									</a>
									)
								</span>
							)}
					</span>
				</>
			)}
			<small
				className="pull-right extraInfo"
				title={moment(entry.createDate).format('l LT ([UTC]Z)')}
				/* TODO: timeAgo */
			>
				{moment(entry.createDate).fromNow()}
			</small>
			<div className="media">
				{entry.entry.mainPicture &&
					(entry.entry.mainPicture.urlTinyThumb ||
						entry.entry.mainPicture.urlSmallThumb) && (
						<a
							className="pull-left"
							href={getEntryUrl(entry.entry)}
							title={entry.entry.additionalNames}
						>
							<img
								src={
									entry.entry.mainPicture.urlTinyThumb ||
									entry.entry.mainPicture.urlSmallThumb
								}
								alt="thumb"
								className="media-object coverPicThumb"
								referrerPolicy="same-origin"
							/>
						</a>
					)}
				<div className="media-body">
					<h4 className="media-heading">
						<a
							href={getEntryUrl(entry.entry)}
							title={entry.entry.additionalNames}
						>
							<strong>{entry.entry.name}</strong>
						</a>
						{entryTypeName(entry.entry) && (
							<>
								{' '}
								<span>({entryTypeName(entry.entry)})</span>
							</>
						)}
					</h4>

					{entry.entry.artistString && <span>{entry.entry.artistString}</span>}
				</div>
			</div>
		</div>
	);
};

export default ActivityEntryKnockout;
