import React from 'react';

interface StarsProps {
	current: number;
	max: number;
}
interface GenerateStarsProps {
	current: number;
	max: number;
	meta: boolean;
}

interface StarProps {
	fill: number;
	current: number;
	meta: boolean;
}

function Star({ fill, current, meta }: StarProps) {
	// fill: 0.0 -> 1.0
	return (
		<span
			style={{
				position: 'relative',
				display: 'inline-block',
				width: 24,
				height: 24,
			}}
			className="rating"
		>
			<img
				src="/Content/star_disabled.png"
				style={{
					position: 'absolute',
					left: 0,
					top: 0,
					width: '100%',
				}}
				alt={meta ? current.toString() : ''}
			/>

			<div
				style={{
					position: 'absolute',
					left: 0,
					top: 0,
					width: `${fill * 100}%`,
					height: 24,
					overflow: 'hidden',
				}}
			>
				<img
					src="/Content/star.png"
					style={{
						width: 24,
						height: 24,
						maxWidth: 'none',
					}}
					alt={meta ? current.toString() : '*'}
				/>
			</div>
		</span>
	);
}

export function GenerateStars({ current, max, meta }: GenerateStarsProps) {
	return (
		<>
			{Array.from({ length: max }, (_, i) => {
				const fill = Math.max(0, Math.min(1, current - i));

				return <Star key={i} current={current} fill={fill} meta={meta} />;
			})}
		</>
	);
}

export const Stars = React.memo(
	({ current, max }: StarsProps): React.ReactElement => {
		return <GenerateStars current={current} max={max} meta={false} />;
	},
);
