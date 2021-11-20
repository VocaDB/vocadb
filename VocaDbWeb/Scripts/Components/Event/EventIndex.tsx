import Layout from '@Components/Shared/Layout';
import EventThumbs from '@Components/Shared/Partials/Shared/EventThumbs';
import useVocaDbTitle from '@Components/useVocaDbTitle';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import LoginManager from '@Models/LoginManager';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import { EventSortRule } from '@Stores/Search/EventSearchStore';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);

interface EventIndexLayoutProps {
	model: ReleaseEventContract[];
}

const EventIndexLayout = ({
	model,
}: EventIndexLayoutProps): React.ReactElement => {
	const { t, ready } = useTranslation(['ViewRes', 'ViewRes.Event']);

	const title = t('ViewRes:Shared.ReleaseEvents');

	useVocaDbTitle(title, ready);

	return (
		<Layout
			title={title}
			toolbar={
				<>
					<ul className="nav nav-pills">
						<li className="active">
							<Link to="/Event">
								{t('ViewRes.Event:EventsBySeries.ViewList')}
							</Link>
						</li>
						<li>
							<a href="/Event/EventsBySeries">
								{t('ViewRes.Event:EventsBySeries.ViewBySeries')}
							</a>
						</li>
						<li>
							<a href="/Event/EventsByVenue">
								{t('ViewRes.Event:EventsBySeries.ViewByVenue')}
							</a>
						</li>
						<li>
							<a href="/Event/EventsByDate">
								{t('ViewRes.Event:EventsBySeries.ViewByDate')}
							</a>
						</li>
					</ul>

					{loginManager.canManageDatabase && (
						<>
							<JQueryUIButton
								as="a"
								href={`/Event/Edit`}
								icons={{ primary: 'ui-icon-plus' }}
							>
								{t('ViewRes.Event:EventsBySeries.CreateEvent')}
							</JQueryUIButton>{' '}
							<JQueryUIButton
								as="a"
								href={`/Event/EditSeries`}
								icons={{ primary: 'ui-icon-plus' }}
							>
								{t('ViewRes.Event:EventsBySeries.CreateSeries')}
							</JQueryUIButton>{' '}
							<JQueryUIButton
								as="a"
								href={`/Venue/Edit`}
								icons={{ primary: 'ui-icon-plus' }}
							>
								{t('ViewRes.Event:EventsBySeries.CreateVenue')}
							</JQueryUIButton>
						</>
					)}
				</>
			}
		>
			<EventThumbs events={model} />
		</Layout>
	);
};

const EventIndex = (): React.ReactElement => {
	const [model, setModel] = React.useState<
		ReleaseEventContract[] | undefined
	>();

	React.useEffect(() => {
		eventRepo
			.getList({
				queryParams: {
					lang: vdb.values.languagePreference,
					fields: 'AdditionalNames,MainPicture,Series,Venue',
					afterDate: moment().subtract(2, 'days').toDate(),
					start: 0,
					maxResults: 15,
					sort: EventSortRule.Date,
					sortDirection: 'Ascending',
					childTags: false,
					tagIds: [],
				},
			})
			.then((result) => setModel(result.items));
	}, []);

	return model ? <EventIndexLayout model={model} /> : <></>;
};

export default EventIndex;
