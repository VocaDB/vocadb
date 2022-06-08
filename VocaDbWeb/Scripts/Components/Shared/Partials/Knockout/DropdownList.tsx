import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import EventCategory from '@Models/Events/EventCategory';
import ContentLanguageSelection from '@Models/Globalization/ContentLanguageSelection';
import SongListFeaturedCategory from '@Models/SongLists/SongListFeaturedCategory';
import UserGroup from '@Models/Users/UserGroup';
import React from 'react';
import { useTranslation } from 'react-i18next';

import { regions } from '../../../regions';
import { userLanguageCultures } from '../../../userLanguageCultures';

interface DropdownListProps
	extends React.DetailedHTMLProps<
		React.SelectHTMLAttributes<HTMLSelectElement>,
		HTMLSelectElement
	> {}

export const ReleaseEventCategoryDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
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
	(props: DropdownListProps): React.ReactElement => {
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
	(props: DropdownListProps): React.ReactElement => {
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
	(props: DropdownListProps): React.ReactElement => {
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

export const LanguageSelectionDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<select {...props}>
				{Object.values(ContentLanguageSelection)
					.filter((languageSelection) => isNaN(Number(languageSelection)))
					.map((languageSelection) => (
						<option value={languageSelection} key={languageSelection}>
							{t(
								`Resources:ContentLanguageSelectionNames.${languageSelection}`,
							)}
						</option>
					))}
			</select>
		);
	},
);

export const RegionDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Domain.Globalization']);

		return (
			<select {...props}>
				<option value="">
					{t(
						'VocaDb.Web.Resources.Domain.Globalization:InterfaceLanguage.Other',
					)}
				</option>
				{Object.entries(regions).map(([key, value]) => (
					<option value={key} key={key}>
						{value}
					</option>
				))}
			</select>
		);
	},
);

export const EntryStatusDropdownList = React.memo(
	({
		allowedEntryStatuses,
		...props
	}: DropdownListProps & {
		allowedEntryStatuses: EntryStatus[];
	}): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<select {...props}>
				{allowedEntryStatuses
					.map((status) => EntryStatus[status])
					.map((status) => (
						<option value={status} key={status}>
							{t(`Resources:EntryStatusNames.${status}`)}
						</option>
					))}
			</select>
		);
	},
);

export const SongListFeaturedCategoryDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<select {...props}>
				{Object.values(SongListFeaturedCategory).map((featuredCategory) => (
					<option value={featuredCategory} key={featuredCategory}>
						{t(`Resources:SongListFeaturedCategoryNames.${featuredCategory}`)}
					</option>
				))}
			</select>
		);
	},
);
