import Button from '@Bootstrap/Button';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import EntryCountBox from '@Components/Shared/Partials/EntryCountBox';
import ServerSidePaging from '@Components/Shared/Partials/Knockout/ServerSidePaging';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import ListUsersStore from '@Stores/User/ListUsersStore';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ListUsersProps {
	listUsersStore: ListUsersStore;
}

const ListUsers = observer(
	({ listUsersStore }: ListUsersProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.User']);

		return (
			<>
				<div className="form-horizontal well well-transparent">
					<div className="control-label">
						<i className="icon-search"></i>
					</div>
					<div className="control-group">
						<div className="controls">
							<div className="input-append">
								<input
									type="text"
									value={listUsersStore.searchTerm}
									onChange={(e): void =>
										listUsersStore.setSearchTerm(e.target.value)
									}
									className="input-xlarge"
									placeholder="Type something..." /* TODO: localize */
								/>
								{listUsersStore.searchTerm && (
									<Button
										variant="danger"
										onClick={(): void => listUsersStore.setSearchTerm('')}
									>
										{t('ViewRes:Shared.Clear')}
									</Button>
								)}
							</div>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">{t('ViewRes.User:Index.Group')}</div>
						<div className="controls">{/* TODO */}</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('ViewRes.User:Index.KnowsLanguage')}
						</div>
						<div className="controls">{/* TODO */}</div>
					</div>

					<div className="control-group">
						<div className="controls">
							<label className="checkbox">
								<input
									type="checkbox"
									checked={listUsersStore.onlyVerifiedArtists}
									onChange={(e): void =>
										listUsersStore.setOnlyVerifiedArtists(e.target.checked)
									}
								/>
								{t('ViewRes.User:Index.VerifiedArtists')}
							</label>
							<label className="checkbox">
								<input
									type="checkbox"
									checked={listUsersStore.disabledUsers}
									onChange={(e): void =>
										listUsersStore.setDisabledUsers(e.target.checked)
									}
								/>
								{t('ViewRes.User:Index.ShowDisabledUsers')}
							</label>
						</div>
					</div>
				</div>

				<div className={classNames(listUsersStore.loading && 'loading')}>
					<EntryCountBox pagingStore={listUsersStore.paging} />

					<ServerSidePaging pagingStore={listUsersStore.paging} />

					<table className="table table-striped">
						<thead>
							<tr>
								<th colSpan={2}>
									<SafeAnchor
										onClick={(): void => listUsersStore.setSort('Name')}
										href="#"
									>
										{t('ViewRes.User:Details.UserName')}{' '}
										{listUsersStore.sort === 'Name' && (
											<span className="sortDirection_down"></span>
										)}
									</SafeAnchor>
								</th>
								<th>
									<SafeAnchor
										onClick={(): void => listUsersStore.setSort('RegisterDate')}
										href="#"
									>
										{t('ViewRes.User:Details.MemberSince')}{' '}
										{listUsersStore.sort === 'RegisterDate' && (
											<span className="sortDirection_down"></span>
										)}
									</SafeAnchor>
								</th>
								<th>
									<SafeAnchor
										onClick={(): void => listUsersStore.setSort('Group')}
										href="#"
									>
										{t('ViewRes.User:Details.UserGroup')}{' '}
										{listUsersStore.sort === 'Group' && (
											<span className="sortDirection_down"></span>
										)}
									</SafeAnchor>
								</th>
							</tr>
						</thead>
						<tbody>
							{listUsersStore.page.map((user) => (
								<tr key={user.id}>
									<td style={{ width: '85px' }}>
										{user.mainPicture && user.mainPicture.urlThumb && (
											<SafeAnchor
												href={EntryUrlMapper.details_user_byName(user.name)}
											>
												<img
													src={user.mainPicture.urlThumb}
													title="Picture"
													className="img-rounded"
												/>
											</SafeAnchor>
										)}
									</td>
									<td>
										<SafeAnchor
											href={EntryUrlMapper.details_user_byName(user.name)}
											className={classNames(!user.active && 'muted')}
										>
											{user.name}
										</SafeAnchor>
									</td>
									<td>{moment(user.memberSince).format('L')}</td>
									<td>{t(`Resources:UserGroupNames.${user.groupId}`)}</td>
								</tr>
							))}
						</tbody>
					</table>

					<ServerSidePaging pagingStore={listUsersStore.paging} />
				</div>
			</>
		);
	},
);

export default ListUsers;
