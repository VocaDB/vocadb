import MomentJsTimeAgo from '@Components/KnockoutExtensions/MomentJsTimeAgo';
import CommentContract from '@DataContracts/CommentContract';
import LoginManager from '@Models/LoginManager';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

import FormatMarkdown from '../Html/FormatMarkdown';
import ProfileIcon from '../User/ProfileIcon';
import UserLink from '../User/UserLink';

const loginManager = new LoginManager(vdb.values);

interface CommentBodyLargeProps {
	contract: CommentContract;
	allowDelete: boolean;
	alwaysAllowDelete?: boolean;
}

const CommentBodyLarge = React.memo(
	({
		contract,
		allowDelete,
		alwaysAllowDelete = false,
	}: CommentBodyLargeProps): React.ReactElement => {
		return (
			<div className="comment media comment-large">
				<a
					className="pull-left"
					href={EntryUrlMapper.details_user_byName(contract.author.name)}
				>
					<ProfileIcon url={contract.author.mainPicture?.urlThumb} size={70} />
				</a>

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
						<UserLink user={contract.author} />
					</h3>
					<FormatMarkdown text={contract.message} />
				</div>
			</div>
		);
	},
);

export default CommentBodyLarge;
