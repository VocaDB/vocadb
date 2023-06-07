import { StateChangeEvent } from './StateChangeEvent';

export interface LocalStorageStateStore<TState> {
	localStorageState: TState;
	validateLocalStorageState(
		localStorageState: any,
	): localStorageState is TState;
	onLocalStorageStateChange?(event: StateChangeEvent<TState>): void;
}
