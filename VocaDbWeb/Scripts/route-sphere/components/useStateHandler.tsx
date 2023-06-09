import { isEqual, omitBy } from 'lodash-es';
import { reaction } from 'mobx';
import React from 'react';

import { StateChangeEvent } from '../stores/StateChangeEvent';

export const useRestoreState = <TState,>(
	popStateRef: React.MutableRefObject<boolean>,
	deserializer: () => any,
	stateValidator: (state: any) => state is TState,
	stateSetter: (state: TState) => void,
): void => {
	React.useEffect(() => {
		const state = deserializer();

		if (stateValidator(state)) {
			popStateRef.current = true;

			stateSetter(state);

			popStateRef.current = false;
		}
	}, [deserializer, stateValidator, popStateRef, stateSetter]);
};

export const useHandleStateChange = <TState extends Partial<TState>>(
	popStateRef: React.MutableRefObject<boolean>,
	onStateChange: ((event: StateChangeEvent<TState>) => void) | undefined,
	stateGetter: () => TState,
): void => {
	React.useEffect(() => {
		if (!onStateChange) return;

		// Returns the disposer.
		return reaction(stateGetter, (state, previousState) => {
			// Compare the current and previous values.
			const diff = omitBy(state, (v, k) =>
				isEqual(previousState[k as keyof typeof previousState], v),
			);

			// Assuming that the current value is `{ filter: 'Miku', page: 3939, searchType: 'Artist' }`, and the previous one is `{ filter: 'Miku', page: 1 }`,
			// then the diff will be `{ page: 3939, searchType: 'Artist' }`, which results in `['page', 'searchType']`.
			const keys = Object.keys(diff) as (keyof TState)[];
			console.assert(keys.length > 0);

			onStateChange({ keys: keys, popState: popStateRef.current });
		});
	}, [stateGetter, onStateChange, popStateRef]);

	// This is called when the page is first loaded.
	React.useEffect(() => {
		if (!onStateChange) return;

		const keys = Object.keys(stateGetter()) as (keyof TState)[];

		onStateChange({ keys: keys, popState: true /* Always true. */ });
	}, [stateGetter, onStateChange]);
};

export const useSaveState = <TState,>(
	popStateRef: React.MutableRefObject<boolean>,
	stateGetter: () => TState,
	serializer: (state: TState) => void,
): void => {
	React.useEffect(() => {
		// Returns the disposer.
		return reaction(stateGetter, (state) => {
			if (popStateRef.current) return;

			serializer(state);
		});
	}, [stateGetter, popStateRef, serializer]);
};

export const useStateHandler = <TState,>(
	deserializer: () => any,
	stateValidator: (state: any) => state is TState,
	stateSetter: (state: TState) => void,
	onStateChange: ((event: StateChangeEvent<TState>) => void) | undefined,
	stateGetter: () => TState,
	serializer: (state: TState) => void,
): void => {
	// Whether currently processing popstate. This is to prevent adding the previous state to history.
	const popStateRef = React.useRef(false);

	useRestoreState(popStateRef, deserializer, stateValidator, stateSetter);

	// This must be called before `useSaveState`, so that state can be changed in the `onStateChange` callback.
	useHandleStateChange(popStateRef, onStateChange, stateGetter);

	useSaveState(popStateRef, stateGetter, serializer);
};
