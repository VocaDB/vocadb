import { range } from 'lodash-es';
import React from 'react';

interface StarProps {
	fill: number;
	alt: string;
	className?: string;
}

// Disabled star with the enabled star clipped on top to `fill` (0 to 1).
export const Star = ({
	fill,
	alt,
	className,
}: StarProps): React.ReactElement => {
	return (
		<span style={{ position: 'relative', display: 'inline-block' }}>
			<img className={className} src="/Content/star_disabled.png" alt={alt} />
			{fill > 0 && (
				<span
					style={{
						position: 'absolute',
						top: 0,
						left: 0,
						width: `${fill * 100}%`,
						overflow: 'hidden',
					}}
				>
					<img src="/Content/star.png" alt="" style={{ maxWidth: 'none' }} />
				</span>
			)}
		</span>
	);
};

export const starFill = (current: number, i: number): number =>
	Math.max(0, Math.min(1, current - i + 1));

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
						<Star
							fill={starFill(current, i)}
							alt={starFill(current, i) > 0 ? '*' : ''}
						/>
					</React.Fragment>
				))}
			</>
		);
	},
);
