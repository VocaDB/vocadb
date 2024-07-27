import { UserToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { UserBaseContract } from '@/DataContracts/User/UserBaseContract';
import { UserGroup } from '@/Models/Users/UserGroup';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link, LinkProps } from 'react-router-dom';

interface UserLinkBaseProps extends Omit<LinkProps, 'to'> {
	user: UserBaseContract;
	children?: React.ReactNode;
}

const UserLinkBase = ({
	user,
	children,
	...props
}: UserLinkBaseProps): React.ReactElement => {
	return (
		<Link {...props} to={EntryUrlMapper.details_user_byName(user.name)}>
			{children ?? user.name}
		</Link>
	);
};

export const USER_GROUP_COLOR = {
	[UserGroup.Limited]: 'gray',
	[UserGroup.Nothing]: undefined,
	[UserGroup.Regular]: undefined,
	[UserGroup.Trusted]: 'yellow',
	[UserGroup.Moderator]: 'orange',
	[UserGroup.Admin]: 'red',
};

export const getUserLinkStyle = (
	user: UserBaseContract | UserApiContract,
	indicateUserGroup?: boolean,
) => {
	return {
		color:
			indicateUserGroup && hasGroupId(user)
				? USER_GROUP_COLOR[user.groupId ?? UserGroup.Regular]
				: undefined,
	};
};

interface UserLinkProps extends Omit<LinkProps, 'to'> {
	user: UserBaseContract | UserApiContract;
	children?: React.ReactNode;
	tooltip?: boolean;
	indicateUserGroup?: boolean;
}

function hasGroupId(
	user: UserBaseContract | UserApiContract,
): user is UserApiContract {
	return (user as UserApiContract).groupId !== undefined;
}

export const UserLink = React.memo(
	({
		user,
		children,
		tooltip,
		indicateUserGroup,
		...props
	}: UserLinkProps): React.ReactElement => {
		const style = getUserLinkStyle(user, indicateUserGroup);

		props = { style, ...props };

		return tooltip ? (
			<UserToolTip id={user.id}>
				<UserLinkBase {...props} user={user}>
					{children}
				</UserLinkBase>
			</UserToolTip>
		) : (
			<UserLinkBase {...props} user={user}>
				{children}
			</UserLinkBase>
		);
	},
);
