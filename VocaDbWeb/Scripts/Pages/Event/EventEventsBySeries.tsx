import { Layout } from '@/Components/Shared/Layout';
import { FormatMarkdown } from '@/Components/Shared/Partials/Html/FormatMarkdown';
import { ReleaseEventSeriesWithEventsContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesWithEventsContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { loginManager } from '@/Models/LoginManager';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface EventEventsBySeriesLayoutProps {
	model: ReleaseEventSeriesWithEventsContract[];
}

const EventEventsBySeriesLayout = ({
	model,
}: EventEventsBySeriesLayoutProps): React.ReactElement => {
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
						<li className="active">
							<Link to="/Event/EventsBySeries">
								{t('ViewRes.Event:EventsBySeries.ViewBySeries')}
							</Link>
						</li>
						<li>
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
			{model.map((series) => (
				<React.Fragment key={series.id}>
					<div className="media withMargin">
						{series.mainPicture && (
							<Link
								className="pull-left"
								to={EntryUrlMapper.details(
									EntryType.ReleaseEventSeries,
									series.id,
								)}
							>
								<img
									className="media-object"
									src={UrlHelper.imageThumb(
										series.mainPicture,
										ImageSize.TinyThumb,
									)}
									alt="Thumb" /* LOC */
								/>
							</Link>
						)}
						<div className="media-body">
							<h3 className="media-heading">
								{series.name ? (
									<Link
										to={EntryUrlMapper.details(
											EntryType.ReleaseEventSeries,
											series.id,
										)}
									>
										{series.name}
									</Link>
								) : (
									t('ViewRes:Shared.Unsorted')
								)}
							</h3>

							{series.description && (
								<p>
									<FormatMarkdown text={series.description} />
								</p>
							)}
						</div>
					</div>

					<ul>
						{series.events.map((event) => (
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
								seriesId: series.id !== 0 ? series.id : undefined,
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

const EventEventsBySeries = (): React.ReactElement => {
	const [model, setModel] = React.useState<
		ReleaseEventSeriesWithEventsContract[]
	>();

	React.useEffect(() => {
		eventRepo.getBySeries().then((model) => setModel(model));
	}, []);

	return model ? <EventEventsBySeriesLayout model={model} /> : <></>;
};

export default EventEventsBySeries;
