import { PVContract } from '@/types/DataContracts/PVs/PVContract';

export interface PlayerProps {
	pv: PVContract;
}

export type IPlayer = React.FC<PlayerProps>;

