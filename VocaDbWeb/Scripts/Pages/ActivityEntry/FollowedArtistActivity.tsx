import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { ActivityEntryKnockout } from '@/Components/Shared/Partials/Activityfeed/ActivityEntryKnockout';
import { useLoginManager } from '@/LoginManagerContext';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { ActivityEntryListStore } from '@/Stores/ActivityEntry/ActivityEntryListStore';
import { FollowedArtistsActivityListStore } from '@/Stores/ActivityEntry/FollowedArtistActivityListStore';
import { useVdb } from '@/VdbContext';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const ActivityEntryIndex = observer(
	(): React.ReactElement => {
		const vdb = useVdb();
		const loginManager = useLoginManager();

		const [followedArtistActivityListStore] = React.useState(
			() =>
				new FollowedArtistsActivityListStore(vdb.values, httpClient, urlMapper),
		);

		const { t, ready } = useTranslation([
			'VocaDb.Web.Resources.Views.ActivityEntry',
		]);

		const title = t(
			'VocaDb.Web.Resources.Views.ActivityEntry:Index.FollowedArtists',
		);

		React.useEffect(() => {
			followedArtistActivityListStore.loadMore();
		}, [followedArtistActivityListStore]);

		return (
			<Layout pageTitle={title} ready={ready} title={title}>
				<ul className="nav nav-pills">
					<li>
						<Link to="/ActivityEntry">
							{t('VocaDb.Web.Resources.Views.ActivityEntry:Index.AllActivity')}
						</Link>
					</li>
					{loginManager.isLoggedIn && (
						<li className="active">
							<Link to="/ActivityEntry/FollowedArtistActivity">
								{t(
									'VocaDb.Web.Resources.Views.ActivityEntry:Index.FollowedArtists',
								)}
							</Link>
						</li>
					)}
					<li>
						<Link to="/Comment">
							{t('VocaDb.Web.Resources.Views.ActivityEntry:Index.Comments')}
						</Link>
					</li>
				</ul>

				<div>
					{followedArtistActivityListStore.entries.map((entry, index) => (
						<ActivityEntryKnockout
							entry={entry}
							showDetails={true}
							key={index}
						/>
					))}
				</div>
			</Layout>
		);
	},
);

export default ActivityEntryIndex;
