import EventSeriesContract from '@DataContracts/ReleaseEvents/EventSeriesContract';
import UrlHelper from '@Helpers/UrlHelper';
import EntryType from '@Models/EntryType';
import EventCategory from '@Models/Events/EventCategory';
import ImageSize from '@Models/Images/ImageSize';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EventSeriesThumbsProps {
	events: EventSeriesContract[];
}

const EventSeriesThumbs = ({
	events,
}: EventSeriesThumbsProps): React.ReactElement => {
	const { t } = useTranslation(['VocaDb.Web.Resources.Domain.ReleaseEvents']);

	return (
		<div className="clearfix">
			{events.map((event) => (
				<div className="well well-transparent event-teaser-tiny" key={event.id}>
					{event.mainPicture && (
						<a
							href={EntryUrlMapper.details(
								EntryType.ReleaseEventSeries,
								event.id,
								event.urlSlug,
							)}
							className="event-image pull-left"
							title={event.additionalNames}
						>
							<img
								className="media-object"
								src={UrlHelper.imageThumb(
									event.mainPicture,
									ImageSize.TinyThumb,
								)}
								alt="Thumb" /* TODO: localize */
							/>
						</a>
					)}
					<div>
						<a
							href={EntryUrlMapper.details(
								EntryType.ReleaseEventSeries,
								event.id,
								event.urlSlug,
							)}
							title={event.additionalNames}
						>
							{event.name}
						</a>
						{event.category !== EventCategory[EventCategory.Unspecified] &&
							event.category !== EventCategory[EventCategory.Other] && (
								<>
									{' '}
									(
									{t(
										`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${event.category}`,
									)}
									)
								</>
							)}
					</div>
				</div>
			))}
		</div>
	);
};

export default EventSeriesThumbs;
