import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EventCategory } from '@/Models/Events/EventCategory';
import _ from 'lodash';
import moment from 'moment';
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
							alt="Thumb" /* TODO: localize */
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
							{_.truncate(event.description, {
								length: 100,
							})}
						</Markdown>
					</p>
				)}

				{event.date && (
					<p>
						{t('ViewRes.Event:Details:OccurrenceDate')}{' '}
						{moment(event.date).format('l')}
						{event.endDate && ` - ${moment(event.endDate).format('l')}`}
					</p>
				)}
			</>
		);
	},
);
