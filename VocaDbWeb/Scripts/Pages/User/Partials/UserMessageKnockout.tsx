import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { IconAndNameLinkKnockout } from '@/Components/Shared/Partials/User/IconAndNameLinkKnockout';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import {
	UserMessagesStore,
	UserMessageStore,
} from '@/Stores/User/UserMessagesStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface UserMessageKnockoutProps {
	userMessagesStore: UserMessagesStore;
	userMessageStore: UserMessageStore;
}

const UserMessageKnockout = observer(
	({
		userMessagesStore,
		userMessageStore,
	}: UserMessageKnockoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.User']);

		return (
			<div className="message ui-tabs ui-widget ui-widget-content ui-corner-all">
				<div className="messageTitle ui-widget-header ui-corner-all">
					<div className="messageTitleText">
						<span>{userMessageStore.subject}</span>
						<br />
						<span>{userMessageStore.created}</span>{' '}
						{userMessageStore.sender && (
							<span>
								<span>{t('ViewRes.User:Messages.From')}</span>{' '}
								<span>
									<IconAndNameLinkKnockout user={userMessageStore.sender} />{' '}
									<span className="razor-whitespace-fix">.</span>
								</span>{' '}
							</span>
						)}
						<span>{t('ViewRes.User:Messages.To')}</span>{' '}
						<span>
							<IconAndNameLinkKnockout user={userMessageStore.receiver} />{' '}
							<span className="razor-whitespace-fix">.</span>
						</span>
					</div>
				</div>
				<p className="messageContent">
					<Markdown>{userMessagesStore.selectedMessageBody}</Markdown>
				</p>

				{userMessageStore.sender /* TODO */ && (
					<JQueryUIButton
						as="a"
						icons={{ primary: 'ui-icon-arrowreturnthick-1-w' }}
						onClick={userMessagesStore.reply}
						href="#"
					>
						{t('ViewRes.User:Messages.Reply')}
					</JQueryUIButton>
				)}
			</div>
		);
	},
);

export default UserMessageKnockout;
