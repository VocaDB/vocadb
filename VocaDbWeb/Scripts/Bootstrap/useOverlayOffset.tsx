import { useBootstrapPrefix } from '@/Bootstrap/ThemeProvider';
import { Offset, Options } from '@restart/ui/usePopper';
import hasClass from 'dom-helpers/hasClass';
import { useMemo, useRef } from 'react';

// This is meant for internal use.
// This applies a custom offset to the overlay if it's a popover.
export default function useOverlayOffset(
	customOffset?: Offset,
): [React.RefObject<HTMLElement>, Options['modifiers']] {
	const overlayRef = useRef<HTMLDivElement | null>(null);
	const popoverClass = useBootstrapPrefix(undefined, 'popover');

	const offset = useMemo(
		() => ({
			name: 'offset',
			options: {
				offset: (): Offset => {
					if (
						overlayRef.current &&
						hasClass(overlayRef.current, popoverClass)
					) {
						return customOffset || [0, 8];
					}
					return customOffset || [0, 0];
				},
			},
		}),
		[customOffset, popoverClass],
	);

	return [overlayRef, [offset]];
}
