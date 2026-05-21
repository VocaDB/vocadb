import { GenerateStars } from '@/Components/Shared/Partials/Shared/Stars';
import React from 'react';

interface StarsMetaProps {
	current: number;
	max: number;
}

export const StarsMeta = React.memo(
	({ current, max }: StarsMetaProps): React.ReactElement => {
		return <GenerateStars current={current} max={max} meta={true} />;
	},
);
