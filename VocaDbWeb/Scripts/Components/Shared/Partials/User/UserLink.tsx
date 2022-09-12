import { UserBaseContract } from '@/DataContracts/User/UserBaseContract';
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

interface UserLinkProps extends Omit<LinkProps, 'to'> {
	user: UserBaseContract;
	children?: React.ReactNode;
	tooltip?: boolean;
}

export const UserLink = React.memo(
	({
		user,
		children,
		tooltip,
		...props
	}: UserLinkProps): React.ReactElement => {
		return tooltip ? (
			/*<UserToolTip
				{...props}
				as={Link}
				to={EntryUrlMapper.details_user_byName(user.name)}
				id={user.id}
			>
				{children ?? user.name}
			</UserToolTip>*/
			<UserLinkBase {...props} user={user}>
				{children}
			</UserLinkBase>
		) : (
			<UserLinkBase {...props} user={user}>
				{children}
			</UserLinkBase>
		);
	},
);
