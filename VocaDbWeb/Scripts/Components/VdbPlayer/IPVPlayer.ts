import PVContract from '@DataContracts/PVs/PVContract';

export default interface IPVPlayer {
	load: (pv: PVContract) => Promise<void>;
	play: () => void;
	pause: () => void;
}
