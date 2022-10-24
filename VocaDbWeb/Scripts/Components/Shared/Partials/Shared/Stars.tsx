import { range } from 'lodash-es';
import React from 'react';

interface StarsProps {
	current: number;
	max: number;
}

export const Stars = React.memo(
	({ current, max }: StarsProps): React.ReactElement => {
		return (
			<>
				{range(1, max + 1).map((i) => (
					<React.Fragment key={i}>
						{i > 0 && ' '}
						{current >= i ? (
							<img src="/Content/star.png" alt="*" />
						) : (
							<img src="/Content/star_disabled.png" alt="" />
						)}
					</React.Fragment>
				))}
			</>
		);
	},
);
