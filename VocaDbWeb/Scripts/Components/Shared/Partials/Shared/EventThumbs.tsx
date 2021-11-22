import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import UrlHelper from '@Helpers/UrlHelper';
import EntryType from '@Models/EntryType';
import EventCategory from '@Models/Events/EventCategory';
import ImageSize from '@Models/Images/ImageSize';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import classNames from 'classnames';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

import VenueLinkOrVenueName from '../Event/VenueLinkOrVenueName';

const getCategory = (event: ReleaseEventContract): string => {
	return event.series?.category ?? event.category;
};

interface EventThumbsProps {
	events: ReleaseEventContract[];
	imageSize?: ImageSize;
}

const EventThumbs = ({
	events,
	imageSize = ImageSize.SmallThumb,
}: EventThumbsProps): React.ReactElement => {
	const { t } = useTranslation(['VocaDb.Web.Resources.Domain.ReleaseEvents']);

	return (
		<div className="clearfix">
			{events.map((event) => (
				<div
					className={classNames(
						'well',
						'well-transparent',
						imageSize === ImageSize.SmallThumb
							? 'event-teaser'
							: 'event-teaser-tiny',
					)}
					key={event.id}
				>
					{event.mainPicture && (
						<Link
							to={EntryUrlMapper.details(
								EntryType.ReleaseEvent,
								event.id,
								event.urlSlug,
							)}
							className="event-image pull-left"
							title={event.additionalNames}
						>
							<img
								className="media-object"
								src={UrlHelper.getSmallestThumb(event.mainPicture, imageSize)}
								alt="Thumb" /* TODO: localize */
							/>
						</Link>
					)}
					<div>
						<Link
							to={EntryUrlMapper.details(
								EntryType.ReleaseEvent,
								event.id,
								event.urlSlug,
							)}
							title={event.additionalNames}
						>
							{event.name}
						</Link>
						<br />
						{getCategory(event) !== EventCategory[EventCategory.Unspecified] &&
							getCategory(event) !== EventCategory[EventCategory.Other] && (
								<>
									(
									{t(
										`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${getCategory(
											event,
										)}`,
									)}
									)
								</>
							)}
						{event.date && (
							<>
								<br />
								<small className="extraInfo">
									{moment(event.date).format('l')}
									{event.endDate && <> - {moment(event.endDate).format('l')}</>}
								</small>
							</>
						)}
						{(event.venue || event.venueName) && (
							<>
								<br />
								<small className="extraInfo">
									<VenueLinkOrVenueName event={event} />
								</small>
							</>
						)}
					</div>
				</div>
			))}
		</div>
	);
};

export default EventThumbs;
