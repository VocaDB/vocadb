import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import { EntryType } from '@/Models/EntryType';
import { UserEventRelationshipType } from '@/Models/Users/UserEventRelationshipType';
import { UserDetailsNav } from '@/Pages/User/UserDetailsRoutes';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { UserDetailsStore } from '@/Stores/User/UserDetailsStore';
import dayjs from '@/dayjs';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface EventsProps {
	userDetailsStore: UserDetailsStore;
}

const Events = observer(
	({ userDetailsStore }: EventsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.User']);

		return (
			<div className="well well-transparent">
				<ul className="nav nav-pills">
					<li
						className={classNames(
							userDetailsStore.eventsType ===
								UserEventRelationshipType[
									UserEventRelationshipType.Attending
								] && 'active',
						)}
					>
						<SafeAnchor
							href="#"
							onClick={(): void =>
								runInAction(() => {
									userDetailsStore.eventsType =
										UserEventRelationshipType[
											UserEventRelationshipType.Attending
										];
								})
							}
						>
							{t('ViewRes.User:Details.Attending')}
						</SafeAnchor>
					</li>
					<li
						className={classNames(
							userDetailsStore.eventsType ===
								UserEventRelationshipType[
									UserEventRelationshipType.Interested
								] && 'active',
						)}
					>
						<SafeAnchor
							href="#"
							onClick={(): void =>
								runInAction(() => {
									userDetailsStore.eventsType =
										UserEventRelationshipType[
											UserEventRelationshipType.Interested
										];
								})
							}
						>
							{t('ViewRes.User:Details.Interested')}
						</SafeAnchor>
					</li>
				</ul>

				<table className="table">
					<tbody>
						{userDetailsStore.events.map((event) => (
							<tr key={event.id}>
								<td width="85px">
									{event.mainPicture && event.mainPicture.urlTinyThumb && (
										<Link
											title={event.additionalNames}
											to={EntryUrlMapper.details(
												EntryType.ReleaseEvent,
												event.id,
												event.urlSlug,
											)}
										>
											{/* eslint-disable-next-line jsx-a11y/img-redundant-alt */}
											<img
												alt="Picture" /* LOC */
												src={event.mainPicture.urlTinyThumb}
											/>
										</Link>
									)}
								</td>
								<td>
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
									{event.date && (
										<span>
											<br />
											<small>{dayjs(event.date).format('l')}</small>
										</span>
									)}
								</td>
							</tr>
						))}
					</tbody>
				</table>
			</div>
		);
	},
);

interface UserEventsProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserEvents = ({
	user,
	userDetailsStore,
}: UserEventsProps): React.ReactElement => {
	React.useEffect(() => {
		userDetailsStore.initEvents();
	}, [userDetailsStore]);

	return (
		<>
			<UserDetailsNav user={user} tab="events" />

			<Events userDetailsStore={userDetailsStore} />
		</>
	);
};

export default UserEvents;
