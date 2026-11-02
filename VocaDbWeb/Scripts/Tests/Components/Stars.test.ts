import { starFill, valueAt } from '@/Components/Shared/Partials/Shared/Stars';
import React from 'react';

// Fake mouse event over a 24px star at x = 100.
const mouseAt = (offsetX: number): React.MouseEvent<HTMLElement> =>
	({
		clientX: 100 + offsetX,
		currentTarget: { getBoundingClientRect: () => ({ left: 100, width: 24 }) },
	} as unknown as React.MouseEvent<HTMLElement>);

test('starFill full, half and empty', () => {
	expect(starFill(3.5, 3), 'star 3').toBe(1);
	expect(starFill(3.5, 4), 'star 4').toBe(0.5);
	expect(starFill(3.5, 5), 'star 5').toBe(0);
});

test('starFill fractional average', () => {
	expect(starFill(4.66, 5), 'star 5').toBeCloseTo(0.66);
});

test('valueAt left half', () => {
	expect(valueAt(mouseAt(6), 4), 'value').toBe(3.5);
});

test('valueAt right half', () => {
	expect(valueAt(mouseAt(18), 4), 'value').toBe(4);
});
