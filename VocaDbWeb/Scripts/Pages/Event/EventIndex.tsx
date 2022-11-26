import { Layout } from '@/Components/Shared/Layout';
import { EventThumbs } from '@/Components/Shared/Partials/Shared/EventThumbs';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import {
	eventRepo,
	ReleaseEventOptionalField,
} from '@/Repositories/ReleaseEventRepository';
import { EventSortRule } from '@/Stores/Search/EventSearchStore';
import { useVdb } from '@/VdbContext';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface EventIndexLayoutProps {
	model: ReleaseEventContract[];
}

const EventIndexLayout = ({
	model,
}: EventIndexLayoutProps): React.ReactElement => {
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
						<li className="active">
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
			<EventThumbs events={model} />
		</Layout>
	);
};

const EventIndex = (): React.ReactElement => {
	const vdb = useVdb();

	const [model, setModel] = React.useState<ReleaseEventContract[]>();

	React.useEffect(() => {
		eventRepo
			.getList({
				queryParams: {
					lang: vdb.values.languagePreference,
					fields: [
						ReleaseEventOptionalField.AdditionalNames,
						ReleaseEventOptionalField.MainPicture,
						ReleaseEventOptionalField.Series,
						ReleaseEventOptionalField.Venue,
					],
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
	}, [vdb]);

	return model ? <EventIndexLayout model={model} /> : <></>;
};

export default EventIndex;
