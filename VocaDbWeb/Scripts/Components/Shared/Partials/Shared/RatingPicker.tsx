import React from 'react';

interface RatingPickerProps {
	value: number;
	onChange: (value: number) => void;
	max?: number;
}

interface RatingStarProps {
	fill: number;
	index: number;
	onClick: (index: number, event: React.MouseEvent<HTMLSpanElement>) => void;
	onMouseMove: (
		index: number,
		event: React.MouseEvent<HTMLSpanElement>,
	) => void;
}

export const RatingStar = ({
	fill,
	index,
	onClick,
	onMouseMove,
}: RatingStarProps): React.ReactElement => {
	return (
		<span
			className="rating"
			style={{
				position: 'relative',
				display: 'inline-block',
				width: 20,
				height: 20,
				cursor: 'pointer',
			}}
			onClick={(event): void => onClick(index, event)}
			onMouseMove={(event): void => onMouseMove(index, event)}
		>
			<img
				src="/Content/star_disabled.png"
				style={{
					position: 'absolute',
					left: 0,
					top: 0,
					width: '100%',
				}}
				alt=""
			/>
			<div
				style={{
					position: 'absolute',
					left: 0,
					top: 0,
					width: `${fill * 100}%`,
					height: 20,
					overflow: 'hidden',
				}}
			>
				<img
					src="/Content/star.png"
					style={{
						width: 20,
						height: 20,
						maxWidth: 'none',
					}}
					alt="*"
				/>
			</div>
		</span>
	);
};

export const RatingPicker = ({
	value,
	onChange,
	max = 5,
}: RatingPickerProps): React.ReactElement => {
	const [hoverValue, setHoverValue] = React.useState<number | null>(null);
	const displayValue = hoverValue ?? value;

	const handleClick = (index: number, event: React.MouseEvent<HTMLSpanElement>): void => {
		const rect = event.currentTarget.getBoundingClientRect();
		const x = event.clientX - rect.left;
		const nextValue = Math.min(max, Math.max(0.5, index + (x < rect.width / 2 ? 0.5 : 1)));
		onChange(nextValue);
	};

	const handleMouseMove = (index: number, event: React.MouseEvent<HTMLSpanElement>): void => {
		const rect = event.currentTarget.getBoundingClientRect();
		const x = event.clientX - rect.left;
		const nextValue = Math.min(max, Math.max(0.5, index + (x < rect.width / 2 ? 0.5 : 1)));
		setHoverValue(nextValue);
	};

	return (
		<span onMouseLeave={(): void => setHoverValue(null)} style={{ display: 'inline-flex' }}>
			{Array.from({ length: max }, (_, index) => {
				const fill = Math.max(0, Math.min(1, displayValue - index));
				return (
					<RatingStar
						key={index}
						fill={fill}
						index={index}
						onClick={handleClick}
						onMouseMove={handleMouseMove}
					/>
				);
			})}
		</span>
	);
};

