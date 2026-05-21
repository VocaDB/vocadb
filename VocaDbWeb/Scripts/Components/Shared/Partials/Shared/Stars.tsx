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

// Left half of star `i` selects i - 0.5, right half selects i.
const valueAt = (e: React.MouseEvent<HTMLElement>, i: number): number => {
	const { left, width } = e.currentTarget.getBoundingClientRect();
	return e.clientX - left < width / 2 ? i - 0.5 : i;
};

interface StarsProps {
	current: number;
	max: number;
	onChange?: (value: number) => void;
}

export const Stars = React.memo(
	({ current, max, onChange }: StarsProps): React.ReactElement => {
		const [hovered, setHovered] = React.useState<number>();

		if (!onChange) {
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
		}

		const shown = hovered ?? current;
		const checked = Math.ceil(current);

		return (
			<span
				role="radiogroup"
				onMouseLeave={(): void => setHovered(undefined)}
				style={{ whiteSpace: 'nowrap' }}
			>
				{range(1, max + 1).map((i) => (
					<span
						key={i}
						role="radio"
						aria-checked={checked === i}
						tabIndex={checked === i || (checked === 0 && i === 1) ? 0 : -1}
						onMouseMove={(e): void => setHovered(valueAt(e, i))}
						onClick={(e): void => onChange(valueAt(e, i))}
						onKeyDown={(e): void => {
							if (e.key === 'Enter' || e.key === ' ') {
								e.preventDefault();
								onChange(i);
							}
						}}
						style={{ cursor: 'pointer' }}
					>
						<Star fill={starFill(shown, i)} alt={`${i}`} />
					</span>
				))}
			</span>
		);
	},
);
