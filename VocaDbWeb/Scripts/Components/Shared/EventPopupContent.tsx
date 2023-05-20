import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EventCategory } from '@/Models/Events/EventCategory';
import dayjs from '@/dayjs';
import { truncate } from 'lodash-es';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EventPopupContentProps {
	event: ReleaseEventContract;
}

export const EventPopupContent = React.memo(
	({ event }: EventPopupContentProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Domain.ReleaseEvents']);

		const category = event.series ? event.series.category : event.category;

		return (
			<>
				{event.mainPicture && event.mainPicture.urlSmallThumb && (
					<div className="thumbnail">
						<img
							src={event.mainPicture.urlSmallThumb}
							alt="Thumb" /* LOC */
							className="coverPicThumb"
							referrerPolicy="same-origin"
						/>
					</div>
				)}

				<strong className="popupTitle">{event.name}</strong>

				{event.additionalNames && <p>{event.additionalNames}</p>}

				{category !== EventCategory.Unspecified &&
					category !== EventCategory.Other && (
						<p>
							{t(
								`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${category}`,
							)}
						</p>
					)}

				{event.description && (
					<p>
						<Markdown>
							{truncate(event.description, {
								length: 100,
							})}
						</Markdown>
					</p>
				)}

				{event.date && (
					<p>
						{t('ViewRes.Event:Details:OccurrenceDate')}{' '}
						{dayjs(event.date).format('ll')}
						{event.endDate && ` - ${dayjs(event.endDate).format('ll')}`}
					</p>
				)}
			</>
		);
	},
);
