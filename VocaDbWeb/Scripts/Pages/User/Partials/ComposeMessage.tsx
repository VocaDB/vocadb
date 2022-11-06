import Alert from '@/Bootstrap/Alert';
import Button from '@/Bootstrap/Button';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { UserLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/UserLockingAutoComplete';
import { MarkdownNotice } from '@/Components/Shared/Partials/Shared/MarkdownNotice';
import { showErrorMessage, showSuccessMessage } from '@/Components/ui';
import { UserMessagesStore } from '@/Stores/User/UserMessagesStore';
import { getReasonPhrase } from 'http-status-codes';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ComposeMessageProps {
	userMessagesStore: UserMessagesStore;
}

const ComposeMessage = observer(
	({ userMessagesStore }: ComposeMessageProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.User']);

		return (
			<form
				onSubmit={async (e): Promise<void> => {
					e.preventDefault();

					try {
						if (userMessagesStore.newMessageStore.receiver.isEmpty) {
							runInAction(() => {
								userMessagesStore.newMessageStore.isReceiverInvalid = true;
							});
						} else {
							await userMessagesStore.sendMessage();

							showSuccessMessage(t('ViewRes.User:Messages.MessageSent'));
						}
					} catch (error: any) {
						showErrorMessage(
							error.response && error.response.status
								? getReasonPhrase(error.response.status)
								: t('ViewRes.User:Messages.MessageSendError'),
						);

						throw error;
					}
				}}
			>
				<div className="editor-label">{t('ViewRes.User:Messages.To')}</div>
				<div className="editor-field">
					<UserLockingAutoComplete
						basicEntryLinkStore={userMessagesStore.newMessageStore.receiver}
					/>
					{userMessagesStore.newMessageStore.isReceiverInvalid && (
						<Alert variant="danger">
							Receiver must be selected.{/* LOC */}
						</Alert>
					)}
				</div>

				<div className="editor-label">{t('ViewRes.User:Messages.Subject')}</div>
				<div className="editor-field">
					<input
						type="text"
						size={40}
						className="span3"
						value={userMessagesStore.newMessageStore.subject}
						onChange={(e): void =>
							runInAction(() => {
								userMessagesStore.newMessageStore.subject = e.target.value;
							})
						}
						maxLength={200}
						required
					/>
					<br />

					<label className="checkbox">
						<input
							type="checkbox"
							checked={userMessagesStore.newMessageStore.highPriority}
							onChange={(e): void =>
								runInAction(() => {
									userMessagesStore.newMessageStore.highPriority =
										e.target.checked;
								})
							}
						/>
						{t('ViewRes.User:Messages.HighPriority')}
					</label>
				</div>

				<div className="editor-label">
					{t('ViewRes.User:Messages.Body')} <MarkdownNotice />
				</div>
				<div className="editor-field">
					<textarea
						value={userMessagesStore.newMessageStore.body}
						onChange={(e): void => {
							runInAction(() => {
								userMessagesStore.newMessageStore.body = e.target.value;
							});
						}}
						className="span5"
						rows={10}
						cols={60}
						maxLength={10000}
						required
					/>
					<br />
					Live preview{/* LOC */}
					<Markdown>{userMessagesStore.newMessageStore.body}</Markdown>
				</div>
				<br />

				<Button
					type="submit"
					variant="primary"
					disabled={userMessagesStore.newMessageStore.isSending}
				>
					<i className="icon-envelope icon-white" /> &nbsp;
					{t('ViewRes.User:Messages.Send')}
				</Button>
			</form>
		);
	},
);

export default ComposeMessage;
