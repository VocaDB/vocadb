import EntryType from '@Models/EntryType';
import EventCategory from '@Models/Events/EventCategory';
import UserGroup from '@Models/Users/UserGroup';
import React from 'react';
import { useTranslation } from 'react-i18next';

import { userLanguageCultures } from '../../../userLanguageCultures';

interface DropdownListProps
	extends React.DetailedHTMLProps<
		React.SelectHTMLAttributes<HTMLSelectElement>,
		HTMLSelectElement
	> {}

export const ReleaseEventCategoryDropdownList = React.memo(
	({ ...props }: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation('VocaDb.Web.Resources.Domain.ReleaseEvents');

		return (
			<select {...props}>
				{Object.values(EventCategory)
					.filter((eventCategory) => isNaN(Number(eventCategory)))
					.map((eventCategory) => (
						<option value={eventCategory} key={eventCategory}>
							{t(
								`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${eventCategory}`,
							)}
						</option>
					))}
			</select>
		);
	},
);

export const UserGroupDropdownList = React.memo(
	({ ...props }: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation('Resources');

		return (
			<select {...props}>
				{Object.values(UserGroup).map((userGroup) => (
					<option value={userGroup} key={userGroup}>
						{t(`Resources:UserGroupNames.${userGroup}`)}
					</option>
				))}
			</select>
		);
	},
);

export const UserLanguageCultureDropdownList = React.memo(
	({ ...props }: DropdownListProps): React.ReactElement => {
		return (
			<select {...props}>
				{props.placeholder !== undefined && (
					<option>{props.placeholder}</option>
				)}
				{Object.entries(userLanguageCultures).map(([key, value]) => (
					<option value={key} key={key}>
						{value.nativeName} ({value.englishName /* TODO: localize */})
					</option>
				))}
			</select>
		);
	},
);

const commentTargetTypes: string[] = [
	EntryType[EntryType.Undefined],
	EntryType[EntryType.Album],
	EntryType[EntryType.Artist],
	EntryType[EntryType.DiscussionTopic],
	EntryType[EntryType.ReleaseEvent],
	EntryType[EntryType.Song],
	EntryType[EntryType.SongList],
	EntryType[EntryType.Tag],
	EntryType[EntryType.User],
];

export const CommentTargetTypeDropdownList = React.memo(
	({ ...props }: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation('VocaDb.Web.Resources.Domain');

		return (
			<select {...props}>
				{Object.values(commentTargetTypes).map((entryType) => (
					<option value={entryType} key={entryType}>
						{t(`VocaDb.Web.Resources.Domain:EntryTypeNames.${entryType}`)}
					</option>
				))}
			</select>
		);
	},
);
