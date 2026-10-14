import { Star, starFill } from '@/Components/Shared/Partials/Shared/Stars';
import { range } from 'lodash-es';
import React from 'react';

interface StarsMetaProps {
	current: number;
	max: number;
}

export const StarsMeta = React.memo(
	({ current, max }: StarsMetaProps): React.ReactElement => {
		return (
			<>
				{range(1, max + 1).map((i) => (
					<React.Fragment key={i}>
						{i > 0 && ' '}
						<Star
							className="rating"
							fill={starFill(current, i)}
							alt={`${current}`}
						/>
					</React.Fragment>
				))}
			</>
		);
	},
);
