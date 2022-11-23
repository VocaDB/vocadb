import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { ActivityEntryKnockout } from '@/Components/Shared/Partials/Activityfeed/ActivityEntryKnockout';
import { loginManager } from '@/Models/LoginManager';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { ActivityEntryListStore } from '@/Stores/ActivityEntry/ActivityEntryListStore';
import { useVdb } from '@/VdbContext';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const ActivityEntryIndex = observer(
	(): React.ReactElement => {
		const vdb = useVdb();

		const [activityEntryListStore] = React.useState(
			() => new ActivityEntryListStore(vdb.values, httpClient, urlMapper),
		);

		const { t, ready } = useTranslation([
			'VocaDb.Web.Resources.Views.ActivityEntry',
		]);

		const title = t(
			'VocaDb.Web.Resources.Views.ActivityEntry:Index.RecentActivity',
		);

		React.useEffect(() => {
			activityEntryListStore.loadMore();
		}, [activityEntryListStore]);

		return (
			<Layout pageTitle={title} ready={ready} title={title}>
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
						<Link to="/Comment">
							{t('VocaDb.Web.Resources.Views.ActivityEntry:Index.Comments')}
						</Link>
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
					<SafeAnchor onClick={activityEntryListStore.loadMore}>
						{t('VocaDb.Web.Resources.Views.ActivityEntry:Index.ViewMore')}
					</SafeAnchor>
				</h3>
			</Layout>
		);
	},
);

export default ActivityEntryIndex;
