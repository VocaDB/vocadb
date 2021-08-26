import Button from '@Bootstrap/Button';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import EntryCountBox from '@Components/Shared/Partials/EntryCountBox';
import {
	UserGroupDropdownList,
	UserLanguageCultureDropdownList,
} from '@Components/Shared/Partials/Knockout/DropdownList';
import ServerSidePaging from '@Components/Shared/Partials/Knockout/ServerSidePaging';
import { useRedial } from '@Components/redial';
import UserGroup from '@Models/Users/UserGroup';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import ListUsersStore, { UserSortRule } from '@Stores/User/ListUsersStore';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';

interface ListUsersProps {
	listUsersStore: ListUsersStore;
}

const ListUsers = observer(
	({ listUsersStore }: ListUsersProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.User']);
		const redial = useRedial(listUsersStore.routeParams);

		return (
			<>
				<div className="form-horizontal well well-transparent">
					<div className="control-label">
						<i className="icon-search"></i>
					</div>
					<div className="control-group">
						<div className="controls">
							<div className="input-append">
								<DebounceInput
									type="text"
									value={listUsersStore.searchTerm}
									onChange={(e): void =>
										redial({ filter: e.target.value, page: 1 })
									}
									className="input-xlarge"
									placeholder="Type something..." /* TODO: localize */
									debounceTimeout={300}
								/>
								{listUsersStore.searchTerm && (
									<Button
										variant="danger"
										onClick={(): void => redial({ filter: '', page: 1 })}
									>
										{t('ViewRes:Shared.Clear')}
									</Button>
								)}
							</div>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">{t('ViewRes.User:Index.Group')}</div>
						<div className="controls">
							<UserGroupDropdownList
								value={listUsersStore.group}
								onChange={(e): void =>
									redial({ groupId: e.target.value as UserGroup, page: 1 })
								}
							/>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('ViewRes.User:Index.KnowsLanguage')}
						</div>
						<div className="controls">
							<UserLanguageCultureDropdownList
								value={listUsersStore.knowsLanguage}
								onChange={(e): void =>
									redial({ knowsLanguage: e.target.value, page: 1 })
								}
								placeholder=""
							/>
						</div>
					</div>

					<div className="control-group">
						<div className="controls">
							<label className="checkbox">
								<input
									type="checkbox"
									checked={listUsersStore.onlyVerifiedArtists}
									onChange={(e): void =>
										redial({ onlyVerifiedArtists: e.target.checked, page: 1 })
									}
								/>
								{t('ViewRes.User:Index.VerifiedArtists')}
							</label>
							<label className="checkbox">
								<input
									type="checkbox"
									checked={listUsersStore.disabledUsers}
									onChange={(e): void =>
										redial({ disabledUsers: e.target.checked, page: 1 })
									}
								/>
								{t('ViewRes.User:Index.ShowDisabledUsers')}
							</label>
						</div>
					</div>
				</div>

				<div className={classNames(listUsersStore.loading && 'loading')}>
					<EntryCountBox
						pagingStore={listUsersStore.paging}
						onPageSizeChange={(pageSize): void =>
							redial({ pageSize: pageSize, page: 1 })
						}
					/>

					<ServerSidePaging
						pagingStore={listUsersStore.paging}
						onPageChange={(page): void => redial({ page: page })}
					/>

					<table className="table table-striped">
						<thead>
							<tr>
								<th colSpan={2}>
									<SafeAnchor
										onClick={(): void =>
											redial({ sort: UserSortRule.Name, page: 1 })
										}
										href="#"
									>
										{t('ViewRes.User:Details.UserName')}
										{listUsersStore.sort === UserSortRule.Name && (
											<>
												{' '}
												<span className="sortDirection_down"></span>
											</>
										)}
									</SafeAnchor>
								</th>
								<th>
									<SafeAnchor
										onClick={(): void =>
											redial({ sort: UserSortRule.RegisterDate, page: 1 })
										}
										href="#"
									>
										{t('ViewRes.User:Details.MemberSince')}
										{listUsersStore.sort === UserSortRule.RegisterDate && (
											<>
												{' '}
												<span className="sortDirection_down"></span>
											</>
										)}
									</SafeAnchor>
								</th>
								<th>
									<SafeAnchor
										onClick={(): void =>
											redial({ sort: UserSortRule.Group, page: 1 })
										}
										href="#"
									>
										{t('ViewRes.User:Details.UserGroup')}
										{listUsersStore.sort === UserSortRule.Group && (
											<>
												{' '}
												<span className="sortDirection_down"></span>
											</>
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
												{/* eslint-disable-next-line jsx-a11y/alt-text */}
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

					<ServerSidePaging
						pagingStore={listUsersStore.paging}
						onPageChange={(page): void => redial({ page: page })}
					/>
				</div>
			</>
		);
	},
);

export default ListUsers;
