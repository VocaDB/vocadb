import { StarsMeta } from '@/Components/Shared/Partials/Shared/StarsMeta';
import React from 'react';

interface StarsMetaSpanProps {
	current: number;
	max: number;
}

export const StarsMetaSpan = React.memo(
	({ current, max }: StarsMetaSpanProps): React.ReactElement => {
		return (
			<span title={`${Math.round(current * 100) / 100}`}>
				<StarsMeta current={Math.round(current)} max={max} />
			</span>
		);
	},
);
