import { MomentJsTimeAgo } from '@/Components/KnockoutExtensions/MomentJsTimeAgo';
import { FormatMarkdown } from '@/Components/Shared/Partials/Html/FormatMarkdown';
import { ProfileIcon } from '@/Components/Shared/Partials/User/ProfileIcon';
import { UserLink } from '@/Components/Shared/Partials/User/UserLink';
import { CommentContract } from '@/DataContracts/CommentContract';
import { useLoginManager } from '@/LoginManagerContext';
import { useMutedUsers } from '@/MutedUsersContext';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { Link } from 'react-router-dom';

interface CommentBodyLargeProps {
	contract: CommentContract;
	allowDelete: boolean;
	alwaysAllowDelete?: boolean;
}

export const CommentBodyLarge = observer(
	({
		contract,
		allowDelete,
		alwaysAllowDelete = false,
	}: CommentBodyLargeProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const mutedUsers = useMutedUsers();
		if (mutedUsers.includes(contract.author.id)) return <></>;

		return (
			<div className="comment media comment-large">
				<Link
					className="pull-left"
					to={EntryUrlMapper.details_user_byName(contract.author.name)}
				>
					<ProfileIcon url={contract.author.mainPicture?.urlThumb} size={70} />
				</Link>

				<div className="media-body">
					<div className="pull-right">
						<MomentJsTimeAgo as="span" className="comment-date">
							{contract.created}
						</MomentJsTimeAgo>
						{(alwaysAllowDelete ||
							(allowDelete && loginManager.canDeleteComment(contract))) && (
							<>{/* TODO */}</>
						)}
					</div>
					<h3 className="media-heading comment-large-header">
						<UserLink indicateUserGroup user={contract.author} />
					</h3>
					<FormatMarkdown text={contract.message} />
				</div>
			</div>
		);
	},
);
