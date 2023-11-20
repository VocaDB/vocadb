import { ParsedQs, parse, stringify } from 'qs';
import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

import { LocationStateStore } from '../stores/LocationStateStore';
import { StateChangeEvent } from '../stores/StateChangeEvent';
import { useStateHandler } from './useStateHandler';

export const useLocationStateDeserializer = (): (() => ParsedQs) => {
	const location = useLocation();

	// Pass `location` as deps instead of `location.search`.
	return React.useCallback(() => parse(location.search.slice(1)), [location]);
};

export const useLocationStateSerializer = <TState,>(): ((
	state: TState,
) => void) => {
	const navigate = useNavigate();

	return React.useCallback(
		(state: TState) => {
			const newUrl = `?${stringify(state)}`;
			navigate(newUrl);
		},
		[navigate],
	);
};

/** Updates a store that implements the {@link LocationStateStore} interface when a route changes, and vice versa. */
export const useLocationStateHandler = <TState,>(
	stateValidator: (state: any) => state is TState,
	stateSetter: (state: TState) => void,
	onStateChange: ((event: StateChangeEvent<TState>) => void) | undefined,
	stateGetter: () => TState,
): void => {
	const deserializer = useLocationStateDeserializer();
	const serializer = useLocationStateSerializer();
	useStateHandler(
		deserializer,
		stateValidator,
		stateSetter,
		onStateChange,
		stateGetter,
		serializer,
	);
};

export const useLocationStateSetter = <TState,>(
	store: LocationStateStore<TState>,
): ((state: TState) => void) => {
	return React.useCallback(
		(state: TState) => {
			store.locationState = state;
		},
		[store],
	);
};

export const useLocationStateGetter = <TState,>(
	store: LocationStateStore<TState>,
): (() => TState) => {
	return React.useCallback(() => store.locationState, [store]);
};

/** Updates a store that implements the {@link LocationStateStore} interface when a route changes, and vice versa. */
export const useLocationStateStore = <TState,>(
	store: LocationStateStore<TState>,
): void => {
	const stateSetter = useLocationStateSetter(store);
	const stateGetter = useLocationStateGetter(store);
	useLocationStateHandler(
		store.validateLocationState,
		stateSetter,
		store.onLocationStateChange,
		stateGetter,
	);
};
