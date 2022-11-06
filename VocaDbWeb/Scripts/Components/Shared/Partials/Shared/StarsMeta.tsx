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
						{current >= i ? (
							<img
								className="rating"
								src="/Content/star.png"
								alt={`${current}`}
							/>
						) : (
							<img
								className="rating"
								src="/Content/star_disabled.png"
								alt={`${current}`}
							/>
						)}
					</React.Fragment>
				))}
			</>
		);
	},
);
