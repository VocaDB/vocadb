import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { ActivityEntryKnockout } from '@/Components/Shared/Partials/Activityfeed/ActivityEntryKnockout';
import { Dropdown } from '@/Components/Shared/Partials/Knockout/Dropdown';
import { ActivityEntryTargetTypeDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import { EntryEditEvent } from '@/Models/ActivityEntries/EntryEditEvent';
import { UserRepository } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import {
	ActivityEntryListStore,
	ActivityEntrySortRule,
} from '@/Stores/ActivityEntry/ActivityEntryListStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const userRepo = new UserRepository(httpClient, urlMapper);

interface UserEntryEditsLayoutProps {
	user: UserDetailsContract;
	activityEntryListStore: ActivityEntryListStore;
}

const UserEntryEditsLayout = observer(
	({
		user,
		activityEntryListStore,
	}: UserEntryEditsLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes.User']);

		const title = `Entry edits - ${user.name}`; /* TODO: localize */

		useVocaDbTitle(title, true);

		useLocationStateStore(activityEntryListStore);

		return (
			<Layout
				title={title}
				parents={
					<>
						<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/User' }} divider>
							Users{/* TODO: localize */}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{ to: EntryUrlMapper.details_user_byName(user.name) }}
						>
							{user.name}
						</Breadcrumb.Item>
					</>
				}
			>
				<div className="form-horizontal well well-transparent">
					<div className="pull-right">
						<div className="inline-block">
							{t('ViewRes:EntryIndex.SortBy')}{' '}
							<Dropdown
								items={Object.fromEntries(
									Object.values(ActivityEntrySortRule).map((value) => [
										value,
										t(`Resources:ActivityEntrySortRuleNames.${value}`),
									]),
								)}
								value={activityEntryListStore.sort}
								onChange={(value): void =>
									runInAction(() => {
										activityEntryListStore.sort = value as ActivityEntrySortRule;
									})
								}
							/>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('ViewRes.User:EntryEdits.EntryType')}
						</div>
						<div className="controls">
							<ActivityEntryTargetTypeDropdownList
								value={activityEntryListStore.entryType}
								onChange={(e): void =>
									runInAction(() => {
										activityEntryListStore.entryType = e.target.value;
									})
								}
							/>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							Edit event{/* TODO: localize */}
						</div>
						<div className="controls">
							<Button
								disabled={activityEntryListStore.entryEditEvent === undefined}
								onClick={(): void =>
									runInAction(() => {
										activityEntryListStore.entryEditEvent = undefined;
									})
								}
							>
								All{/* TODO: localize */}
							</Button>{' '}
							<Button
								disabled={
									activityEntryListStore.entryEditEvent ===
									EntryEditEvent.Created
								}
								onClick={(): void =>
									runInAction(() => {
										activityEntryListStore.entryEditEvent =
											EntryEditEvent.Created;
									})
								}
							>
								Only additions{/* TODO: localize */}
							</Button>{' '}
							<Button
								disabled={
									activityEntryListStore.entryEditEvent ===
									EntryEditEvent.Updated
								}
								onClick={(): void =>
									runInAction(() => {
										activityEntryListStore.entryEditEvent =
											EntryEditEvent.Updated;
									})
								}
							>
								Only edits{/* TODO: localize */}
							</Button>
						</div>
					</div>
				</div>

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
						View more{/* TODO: localize */}
					</SafeAnchor>
				</h3>
			</Layout>
		);
	},
);

const UserEntryEdits = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<{
		user: UserDetailsContract;
		activityEntryListStore: ActivityEntryListStore;
	}>();

	React.useEffect(() => {
		userRepo
			.getOne({ id: Number(id) })
			.then((user) => userRepo.getDetails({ name: user.name! }))
			.then((user) =>
				setModel({
					user: user,
					activityEntryListStore: new ActivityEntryListStore(
						vdb.values,
						httpClient,
						urlMapper,
						user.id,
					),
				}),
			)
			.catch((error) => {
				if (error.response) {
					if (error.response.status === 404)
						window.location.href = '/Error/NotFound';
				}

				throw error;
			});
	}, [id]);

	return model ? (
		<UserEntryEditsLayout
			user={model.user}
			activityEntryListStore={model.activityEntryListStore}
		/>
	) : (
		<></>
	);
};

export default UserEntryEdits;
