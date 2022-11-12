import { EventSeriesContract } from '@/DataContracts/ReleaseEvents/EventSeriesContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { EntryType } from '@/Models/EntryType';
import { EventCategory } from '@/Models/Events/EventCategory';
import { ImageSize } from '@/Models/Images/ImageSize';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface EventSeriesThumbsProps {
	events: EventSeriesContract[];
}

export const EventSeriesThumbs = ({
	events,
}: EventSeriesThumbsProps): React.ReactElement => {
	const { t } = useTranslation(['VocaDb.Web.Resources.Domain.ReleaseEvents']);

	return (
		<div className="clearfix" css={{ display: 'flex', flexWrap: 'wrap' }}>
			{events.map((event) => (
				<div className="well well-transparent event-teaser-tiny" key={event.id}>
					{event.mainPicture && (
						<Link
							to={EntryUrlMapper.details(
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
								alt="Thumb" /* LOC */
							/>
						</Link>
					)}
					<div>
						<Link
							to={EntryUrlMapper.details(
								EntryType.ReleaseEventSeries,
								event.id,
								event.urlSlug,
							)}
							title={event.additionalNames}
						>
							{event.name}
						</Link>
						{event.category !== EventCategory.Unspecified &&
							event.category !== EventCategory.Other && (
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
