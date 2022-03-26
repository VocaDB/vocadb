import SafeAnchor from '@Bootstrap/SafeAnchor';
import Layout from '@Components/Shared/Layout';
import ActivityEntryKnockout from '@Components/Shared/Partials/Activityfeed/ActivityEntryKnockout';
import useVocaDbTitle from '@Components/useVocaDbTitle';
import LoginManager from '@Models/LoginManager';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import ActivityEntryListStore from '@Stores/ActivityEntry/ActivityEntryListStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const activityEntryListStore = new ActivityEntryListStore(
	vdb.values,
	httpClient,
	urlMapper,
);

const ActivityEntryIndex = observer(
	(): React.ReactElement => {
		const { t, ready } = useTranslation([
			'VocaDb.Web.Resources.Views.ActivityEntry',
		]);

		const title = t(
			'VocaDb.Web.Resources.Views.ActivityEntry:Index.RecentActivity',
		);

		useVocaDbTitle(title, ready);

		return (
			<Layout title={title}>
				<ul className="nav nav-pills">
					<li className="active">
						<Link to="/ActivityEntry">
							{t('VocaDb.Web.Resources.Views.ActivityEntry:Index.AllActivity')}
						</Link>
					</li>
					{loginManager.isLoggedIn && (
						<li>
							<a href="/ActivityEntry/FollowedArtistActivity">
								{t(
									'VocaDb.Web.Resources.Views.ActivityEntry:Index.FollowedArtists',
								)}
							</a>
						</li>
					)}
					<li>
						<a href="/Comment">
							{t('VocaDb.Web.Resources.Views.ActivityEntry:Index.Comments')}
						</a>
					</li>
				</ul>

				<div>
					{activityEntryListStore.entries.map((entry, index) => (
						<ActivityEntryKnockout
							entry={entry}
							showDetails={true}
							key={index}
						/>
					))}
				</div>

				<hr />
				<h3>
					<SafeAnchor onClick={(): void => activityEntryListStore.loadMore()}>
						{t('VocaDb.Web.Resources.Views.ActivityEntry:Index.ViewMore')}
					</SafeAnchor>
				</h3>
			</Layout>
		);
	},
);

export default ActivityEntryIndex;
