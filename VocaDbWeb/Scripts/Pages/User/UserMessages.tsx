import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { UserLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/UserLockingAutoComplete';
import { NotificationIcon } from '@/Components/Shared/Partials/Shared/NotificationIcon';
import { IconAndNameKnockout } from '@/Components/Shared/Partials/User/IconAndNameKnockout';
import { useVdbTitle } from '@/Components/useVdbTitle';
import JQueryUITab from '@/JQueryUI/JQueryUITab';
import JQueryUITabs from '@/JQueryUI/JQueryUITabs';
import ComposeMessage from '@/Pages/User/Partials/ComposeMessage';
import UserMessageKnockout from '@/Pages/User/Partials/UserMessageKnockout';
import { UserInboxType, UserRepository } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import {
	UserMessageFolderStore,
	UserMessagesStore,
} from '@/Stores/User/UserMessagesStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useSearchParams } from 'react-router-dom';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);

const userMessagesStore = new UserMessagesStore(
	vdb.values,
	userRepo,
	vdb.values.loggedUserId,
	UserInboxType.Received,
);

interface MessageFolderTabContentProps {
	userMessageFolderStore: UserMessageFolderStore;
	id: string;
	colFrom: boolean;
	colTo: boolean;
	showUnread: boolean;
	allowDelete: boolean;
}

const MessageFolderTabContent = observer(
	({
		userMessageFolderStore,
		id,
		colFrom,
		colTo,
		showUnread,
		allowDelete,
	}: MessageFolderTabContentProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.User']);

		return (
			<div id={id}>
				{userMessageFolderStore.canFilterByUser && (
					<div className="form-horizontal pull-left">
						<div className="control-group">
							{userMessageFolderStore.inbox === UserInboxType.Received && (
								<label className="control-label">
									{t('ViewRes.User:Messages.From')}
								</label>
							)}
							{userMessageFolderStore.inbox === UserInboxType.Sent && (
								<label className="control-label">
									{t('ViewRes.User:Messages.To')}
								</label>
							)}
							<div className="controls">
								<UserLockingAutoComplete
									basicEntryLinkStore={userMessageFolderStore.anotherUser}
								/>
							</div>
						</div>
					</div>
				)}
				<Button
					variant="danger"
					className="pull-right clearfix"
					onClick={userMessageFolderStore.deleteSelected}
				>
					{t('ViewRes.User:Messages.DeleteSelected')}
				</Button>

				<table className="table messages">
					{userMessageFolderStore.items.length > 0 && (
						<>
							<thead>
								<tr>
									<th>
										{allowDelete && (
											<input
												type="checkbox"
												checked={userMessageFolderStore.selectAll}
												onChange={(e): void => {
													runInAction(() => {
														userMessageFolderStore.selectAll = e.target.checked;
													});
												}}
											/>
										)}
									</th>
									<th>{t('ViewRes.User:Messages.Date')}</th>
									{colFrom && <th>{t('ViewRes.User:Messages.From')}</th>}
									{colTo && <th>{t('ViewRes.User:Messages.To')}</th>}
									<th>{t('ViewRes.User:Messages.Subject')}</th>
									<th></th>
								</tr>
							</thead>
							<tbody>
								{userMessageFolderStore.items.map((item) => (
									<tr
										className={classNames(item.selected && 'info')}
										key={item.id}
									>
										<td>
											<input
												type="checkbox"
												checked={item.checked}
												onChange={(e): void => {
													runInAction(() => {
														item.checked = e.target.checked;
													});
												}}
											/>
										</td>
										<td
											onClick={(): void =>
												userMessagesStore.selectMessage(item)
											}
										>
											<span title="UTC">{item.created}</span>
										</td>
										{colFrom && (
											<td
												onClick={(): void =>
													userMessagesStore.selectMessage(item)
												}
											>
												<IconAndNameKnockout
													icon={item.sender.mainPicture?.urlTinyThumb}
													name={item.sender.name}
												/>
											</td>
										)}
										{colTo && (
											<td
												onClick={(): void =>
													userMessagesStore.selectMessage(item)
												}
											>
												<IconAndNameKnockout
													icon={item.receiver.mainPicture?.urlTinyThumb}
													name={item.receiver.name}
												/>
											</td>
										)}
										<td
											onClick={(): void =>
												userMessagesStore.selectMessage(item)
											}
										>
											{item.highPriority && (
												<span>
													<NotificationIcon />
													&nbsp;
												</span>
											)}
											{showUnread && !item.read && (
												<span>
													<img
														src="/Content/star5.png"
														alt="unread" /* LOC */
													/>
													&nbsp;
												</span>
											)}
											<span>{item.subject}</span>
										</td>
										<td>
											<Button
												onClick={(): void =>
													userMessageFolderStore.deleteMessage(item)
												}
												href="#"
												className="btn-mini"
												variant="danger"
											>
												{t('ViewRes:Shared.Delete')}
											</Button>
										</td>
									</tr>
								))}
							</tbody>
						</>
					)}
				</table>

				{userMessageFolderStore.hasMore && (
					<h3>
						<SafeAnchor onClick={userMessageFolderStore.loadMore}>
							{t('ViewRes:Shared.ShowMore')}
						</SafeAnchor>
					</h3>
				)}
			</div>
		);
	},
);

const UserMessages = observer(
	(): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes.User']);

		useVdbTitle(t('ViewRes.User:Messages.Messages'), ready);

		const [searchParams] = useSearchParams();
		const messageId = searchParams.get('messageId');
		const inbox = searchParams.get('inbox');
		const receiverName = searchParams.get('receiverName');

		React.useEffect(() => {
			if (!messageId) return;

			const isNotification = inbox === 'Notifications';
			const userMessageFolderStore = isNotification
				? userMessagesStore.notifications
				: userMessagesStore.receivedMessages;

			userMessageFolderStore.init().then(() => {
				userMessagesStore.selectMessageById(
					Number(messageId),
					userMessageFolderStore,
				);
			});
		}, [messageId, inbox]);

		React.useEffect(() => {
			if (!receiverName) return;

			userMessagesStore.selectTab('composeTab');
			userRepo.getOneByName({ username: receiverName }).then((result) =>
				runInAction(() => {
					userMessagesStore.newMessageStore.receiver.id = result?.id;
				}),
			);
		}, [receiverName]);

		return (
			<Layout
				title={t('ViewRes.User:Messages.Messages')}
				parents={
					<>
						<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/User' }} divider>
							{t('ViewRes:Shared.Users')}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details_user_byName(
									vdb.values.loggedUser?.name,
								),
							}}
						>
							{vdb.values.loggedUser?.name}
						</Breadcrumb.Item>
					</>
				}
			>
				{userMessagesStore.selectedMessage && (
					<div id="viewMessage">
						<UserMessageKnockout
							userMessagesStore={userMessagesStore}
							userMessageStore={userMessagesStore.selectedMessage}
						/>
					</div>
				)}

				<JQueryUITabs
					activeKey={userMessagesStore.tabName}
					onSelect={(eventKey): void => {
						const tabName = eventKey as typeof userMessagesStore.tabName;
						userMessagesStore.selectTab(tabName);

						const inboxes = {
							receivedTab: userMessagesStore.receivedMessages,
							sentTab: userMessagesStore.sentMessages,
							notificationsTab: userMessagesStore.notifications,
							composeTab: undefined,
						};
						inboxes[tabName]?.init();
					}}
				>
					<JQueryUITab
						eventKey="receivedTab"
						title={
							<>
								{t('ViewRes.User:Messages.Received')}
								{!!userMessagesStore.receivedMessages.unread &&
									userMessagesStore.receivedMessages.unread > 0 && (
										<>
											{' '}
											<span className="badge badge-small badge-important">
												{userMessagesStore.receivedMessages.unread}
											</span>
										</>
									)}
							</>
						}
					>
						<MessageFolderTabContent
							id="receivedTabContent"
							userMessageFolderStore={userMessagesStore.receivedMessages}
							colFrom={true}
							colTo={false}
							showUnread={true}
							allowDelete={false}
						/>
					</JQueryUITab>
					<JQueryUITab
						eventKey="sentTab"
						title={t('ViewRes.User:Messages.Sent')}
					>
						<MessageFolderTabContent
							id="sentTabContent"
							userMessageFolderStore={userMessagesStore.sentMessages}
							colFrom={false}
							colTo={true}
							showUnread={false}
							allowDelete={false}
						/>
					</JQueryUITab>
					<JQueryUITab
						eventKey="notificationsTab"
						title={
							<>
								{t('ViewRes.User:Messages.Notifications')}
								{!!userMessagesStore.notifications.unread &&
									userMessagesStore.notifications.unread > 0 && (
										<>
											{' '}
											<span className="badge badge-small badge-important">
												{userMessagesStore.notifications.unread}
											</span>
										</>
									)}
							</>
						}
					>
						<MessageFolderTabContent
							id="notificationsTabContent"
							userMessageFolderStore={userMessagesStore.notifications}
							colFrom={false}
							colTo={false}
							showUnread={true}
							allowDelete={true}
						/>
					</JQueryUITab>
					<JQueryUITab
						eventKey="composeTab"
						title={t('ViewRes.User:Messages.Compose')}
					>
						<div id="composeTab">
							<ComposeMessage userMessagesStore={userMessagesStore} />
						</div>
					</JQueryUITab>
				</JQueryUITabs>
			</Layout>
		);
	},
);

export default UserMessages;
