import { MomentJsTimeAgo } from '@/Components/KnockoutExtensions/MomentJsTimeAgo';
import { IconNameAndTypeLinkKnockout } from '@/Components/Shared/Partials/User/IconNameAndTypeLinkKnockout';
import { useChangedFieldNames } from '@/Components/useChangedFieldNames';
import { ActivityEntryContract } from '@/DataContracts/ActivityEntry/ActivityEntryContract';
import { EntryContract } from '@/DataContracts/EntryContract';
import { EntryType } from '@/Models/EntryType';
import { useMutedUsers } from '@/MutedUsersContext';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

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
					return entry.tagCategoryName /* LOC */;

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

export const ActivityEntryKnockout = observer(
	({
		entry,
		showDetails = false,
	}: ActivityEntryKnockoutProps): React.ReactElement => {
		const { t } = useTranslation(['HelperRes', 'ViewRes']);

		const activityFeedEventName = useActivityFeedEventName();
		const changedFieldNames = useChangedFieldNames();
		const entryTypeName = useEntryTypeName();

		const mutedUsers = useMutedUsers();
		if (entry.author && mutedUsers.includes(entry.author.id)) return <></>;

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
				{showDetails && entry.archivedVersion && (
					<>
						{' '}
						<span>
							{entry.archivedVersion.changedFields &&
								entry.archivedVersion.changedFields.length > 0 && (
									<span>
										(
										{entry.archivedVersion.changedFields
											.map((changedField) =>
												changedFieldNames(
													EntryType[
														entry.entry.entryType as keyof typeof EntryType
													],
													changedField,
												),
											)
											.join(', ')}
										)
									</span>
								)}
							{entry.archivedVersion.notes && (
								<>
									{' '}
									<span>"{entry.archivedVersion.notes}"</span>
								</>
							)}{' '}
							{entry.entry.entryType !== 'SongList' &&
								entry.entry.entryType !== 'ReleaseEvent' && (
									<span>
										(
										<Link
											to={`/${entry.entry.entryType}/ViewVersion/${entry.archivedVersion.id}`}
										>
											{t('ViewRes:Misc.Details')}
										</Link>
										)
									</span>
								)}
						</span>
					</>
				)}
				<MomentJsTimeAgo as="small" className="pull-right extraInfo">
					{entry.createDate}
				</MomentJsTimeAgo>
				<div className="media">
					{entry.entry.mainPicture &&
						(entry.entry.mainPicture.urlTinyThumb ||
							entry.entry.mainPicture.urlSmallThumb) && (
							<Link
								className="pull-left"
								to={getEntryUrl(entry.entry)}
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
							</Link>
						)}
					<div className="media-body">
						<h4 className="media-heading">
							<Link
								to={getEntryUrl(entry.entry)}
								title={entry.entry.additionalNames}
							>
								<strong>{entry.entry.name}</strong>
							</Link>
							{entryTypeName(entry.entry) && (
								<>
									{' '}
									<span>({entryTypeName(entry.entry)})</span>
								</>
							)}
						</h4>

						{entry.entry.artistString && (
							<span>{entry.entry.artistString}</span>
						)}
					</div>
				</div>
			</div>
		);
	},
);
