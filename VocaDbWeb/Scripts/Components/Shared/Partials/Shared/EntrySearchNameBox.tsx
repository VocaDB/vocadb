import React from 'react';
import { useTranslation } from 'react-i18next';

interface EntrySearchNameBoxProps
	extends React.InputHTMLAttributes<HTMLInputElement> {}

export const EntrySearchNameBox = React.forwardRef<
	HTMLInputElement,
	EntrySearchNameBoxProps
>(
	(
		{
			maxLength = 128,
			className = 'input-xlarge',
			...props
		}: EntrySearchNameBoxProps,
		ref,
	): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<input
				ref={ref}
				{...props}
				type="text"
				maxLength={maxLength}
				className={className}
				placeholder={t('ViewRes:Shared.Search')}
			/>
		);
	},
);
