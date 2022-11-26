import { Layout } from '@/Components/Shared/Layout';
import { FormatMarkdown } from '@/Components/Shared/Partials/Html/FormatMarkdown';
import { VenueForApiContract } from '@/DataContracts/Venue/VenueForApiContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface EventEventsByVenueLayoutProps {
	model: VenueForApiContract[];
}

const EventEventsByVenueLayout = ({
	model,
}: EventEventsByVenueLayoutProps): React.ReactElement => {
	const loginManager = useLoginManager();

	const { t, ready } = useTranslation(['ViewRes', 'ViewRes.Event']);

	const title = t('ViewRes:Shared.ReleaseEvents');

	return (
		<Layout
			pageTitle={title}
			ready={ready}
			title={title}
			toolbar={
				<>
					<ul className="nav nav-pills">
						<li>
							<Link to="/Event">
								{t('ViewRes.Event:EventsBySeries.ViewList')}
							</Link>
						</li>
						<li>
							<Link to="/Event/EventsBySeries">
								{t('ViewRes.Event:EventsBySeries.ViewBySeries')}
							</Link>
						</li>
						<li className="active">
							<Link to="/Event/EventsByVenue">
								{t('ViewRes.Event:EventsBySeries.ViewByVenue')}
							</Link>
						</li>
						<li>
							<Link to="/Event/EventsByDate">
								{t('ViewRes.Event:EventsBySeries.ViewByDate')}
							</Link>
						</li>
					</ul>

					{loginManager.canManageDatabase && (
						<>
							<JQueryUIButton
								as={Link}
								to={`/Event/Edit`}
								icons={{ primary: 'ui-icon-plus' }}
							>
								{t('ViewRes.Event:EventsBySeries.CreateEvent')}
							</JQueryUIButton>{' '}
							<JQueryUIButton
								as={Link}
								to="/Event/EditSeries"
								icons={{ primary: 'ui-icon-plus' }}
							>
								{t('ViewRes.Event:EventsBySeries.CreateSeries')}
							</JQueryUIButton>{' '}
							<JQueryUIButton
								as={Link}
								to="/Venue/Edit"
								icons={{ primary: 'ui-icon-plus' }}
							>
								{t('ViewRes.Event:EventsBySeries.CreateVenue')}
							</JQueryUIButton>
						</>
					)}
				</>
			}
		>
			{model.map((venue) => (
				<React.Fragment key={venue.id}>
					<div className="media withMargin">
						<div className="media-body">
							<h3 className="media-heading">
								{venue.name ? (
									<Link to={EntryUrlMapper.details(EntryType.Venue, venue.id)}>
										{venue.name}
									</Link>
								) : (
									t('ViewRes:Shared.Unsorted')
								)}
							</h3>

							{venue.description && (
								<p>
									<FormatMarkdown text={venue.description} />
								</p>
							)}
						</div>
					</div>

					<ul>
						{venue.events.map((event) => (
							<li key={event.id}>
								<Link
									to={EntryUrlMapper.details(
										EntryType.ReleaseEvent,
										event.id,
										event.urlSlug,
									)}
								>
									{event.name}
								</Link>
							</li>
						))}
					</ul>

					{loginManager.canManageDatabase && (
						<Link
							to={`/Event/Edit?${qs.stringify({
								venueId: venue.id !== 0 ? venue.id : undefined,
							})}`}
							className="textLink addLink"
						>
							{t('ViewRes.Event:EventsBySeries.CreateEvent')}
						</Link>
					)}
				</React.Fragment>
			))}
		</Layout>
	);
};

const EventEventsByVenue = (): React.ReactElement => {
	const [model, setModel] = React.useState<VenueForApiContract[]>();

	React.useEffect(() => {
		eventRepo.getByVenue().then((model) => setModel(model));
	}, []);

	return model ? <EventEventsByVenueLayout model={model} /> : <></>;
};

export default EventEventsByVenue;
