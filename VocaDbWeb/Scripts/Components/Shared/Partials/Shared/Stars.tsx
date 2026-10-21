import { range } from 'lodash-es';
import React from 'react';

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
							{current >= i ? (
								<img src="/Content/star.png" alt="*" />
							) : (
								<img src="/Content/star_disabled.png" alt="" />
							)}
						</React.Fragment>
					))}
				</>
			);
		}

		const shown = hovered ?? current;

		return (
			<span
				role="radiogroup"
				onMouseLeave={(): void => setHovered(undefined)}
				style={{ whiteSpace: 'nowrap' }}
			>
				{range(1, max + 1).map((i) => (
					<img
						key={i}
						src={
							shown >= i ? '/Content/star.png' : '/Content/star_disabled.png'
						}
						alt={`${i}`}
						role="radio"
						aria-checked={current === i}
						tabIndex={current === i || (current === 0 && i === 1) ? 0 : -1}
						onMouseEnter={(): void => setHovered(i)}
						onClick={(): void => onChange(i)}
						onKeyDown={(e): void => {
							if (e.key === 'Enter' || e.key === ' ') {
								e.preventDefault();
								onChange(i);
							}
						}}
						style={{ cursor: 'pointer' }}
					/>
				))}
			</span>
		);
	},
);
