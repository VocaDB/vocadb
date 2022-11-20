import { Layout } from '@/Components/Shared/Layout';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EntryType } from '@/Models/EntryType';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface EventEventsByDateLayoutProps {
	model: ReleaseEventContract[];
}

const EventEventsByDateLayout = ({
	model,
}: EventEventsByDateLayoutProps): React.ReactElement => {
	const { t, ready } = useTranslation(['ViewRes', 'ViewRes.Event']);

	const title = t('ViewRes:Shared.ReleaseEvents');

	const byYear = React.useMemo(
		() =>
			model
				.filter((event) => !!event.date)
				.groupBy((event) => moment(event.date!).format('YYYY')),
		[model],
	);

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
						<li>
							<Link to="/Event/EventsByVenue">
								{t('ViewRes.Event:EventsBySeries.ViewByVenue')}
							</Link>
						</li>
						<li className="active">
							<Link to="/Event/EventsByDate">
								{t('ViewRes.Event:EventsBySeries.ViewByDate')}
							</Link>
						</li>
					</ul>
				</>
			}
		>
			<table>
				<tbody>
					{Object.entries(byYear)
						.orderBy(([year]) => year, 'desc')
						.map(([year, events], index) => (
							<React.Fragment key={index}>
								<tr>
									<td className="alginTop" colSpan={2}>
										<h3>{year}</h3>
									</td>
								</tr>

								{events.map((event, index) => (
									<tr key={index}>
										<td>{moment(event.date).format('l')}</td>
										<td>
											<Link
												to={EntryUrlMapper.details(
													EntryType.ReleaseEvent,
													event.id,
													event.urlSlug,
												)}
											>
												{event.name}
											</Link>
										</td>
									</tr>
								))}
							</React.Fragment>
						))}
				</tbody>
			</table>
		</Layout>
	);
};

const EventEventsByDate = (): React.ReactElement => {
	const [model, setModel] = React.useState<ReleaseEventContract[]>();

	React.useEffect(() => {
		eventRepo.getByDate().then((model) => setModel(model));
	}, []);

	return model ? <EventEventsByDateLayout model={model} /> : <></>;
};

export default EventEventsByDate;
