import { StateChangeEvent } from './StateChangeEvent';

export interface LocationStateStore<TState> {
	locationState: TState;
	validateLocationState(locationState: any): locationState is TState;
	onLocationStateChange?(event: StateChangeEvent<TState>): void;
}
