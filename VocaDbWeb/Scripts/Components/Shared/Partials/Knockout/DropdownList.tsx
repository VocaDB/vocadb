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
