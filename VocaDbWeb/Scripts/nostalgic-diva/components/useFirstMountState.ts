// https://github.com/streamich/react-use/blob/8ceb4c0f0c5625124f487b435a2fd0d3b3bc2a4f/src/useFirstMountState.ts
import { useRef } from 'react';

export function useFirstMountState(): boolean {
	const isFirst = useRef(true);

	if (isFirst.current) {
		isFirst.current = false;

		return true;
	}

	return isFirst.current;
}
