import SafeAnchor from '@/Bootstrap/SafeAnchor';
import MomentJsTimeAgo from '@/Components/KnockoutExtensions/MomentJsTimeAgo';
import truncateWithEllipsis from '@/Components/truncateWithEllipsis';
import CommentContract from '@/DataContracts/CommentContract';
import LoginManager from '@/Models/LoginManager';
import React from 'react';
import { useTranslation } from 'react-i18next';

import FormatMarkdown from '../Html/FormatMarkdown';
import UserIconLink_UserForApiContract from '../User/UserIconLink_UserForApiContract';

const loginManager = new LoginManager(vdb.values);

interface PrintCommentProps {
	contract: CommentContract;
	allowDelete: boolean;
	alwaysAllowDelete?: boolean;
	maxLength?: number;
}

const PrintComment = ({
	contract,
	allowDelete,
	alwaysAllowDelete = false,
	maxLength = 2147483647,
}: PrintCommentProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	return (
		<div className="comment">
			<h3>
				{/* eslint-disable-next-line react/jsx-pascal-case */}
				<UserIconLink_UserForApiContract user={contract.author} size={25} />

				{(alwaysAllowDelete ||
					(allowDelete && loginManager.canDeleteComment(contract))) && (
					<>
						-{' '}
						<SafeAnchor href="#" /* TODO */ className="deleteComment">
							{t('ViewRes:Shared.Delete')}
						</SafeAnchor>
					</>
				)}
				<MomentJsTimeAgo as="small" className="pull-right extraInfo">
					{contract.created}
				</MomentJsTimeAgo>
			</h3>
			<FormatMarkdown
				text={truncateWithEllipsis(contract.message, maxLength)}
			/>
		</div>
	);
};

export default PrintComment;
