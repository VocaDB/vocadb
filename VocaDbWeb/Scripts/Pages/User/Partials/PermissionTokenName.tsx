import { PermissionToken } from '@/Models/LoginManager';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface PermissionTokenNameProps {
	token: PermissionToken;
}

export const PermissionTokenName = React.memo(
	({ token }: PermissionTokenNameProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		const tokenName = Object.entries(PermissionToken)
			.filter(([_, value]) => value === token)
			.map(([key, _]) => key)[0];

		return t([`Resources:PermissionTokenNames.${tokenName}`, tokenName]);
	},
);
