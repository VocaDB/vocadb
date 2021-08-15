import SongBpmFilterStore from '@Stores/Search/SongBpmFilter';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';

const absoluteMinBpm = 20;
const absoluteMaxBpm = 1015;
const bpmStep = 0.01;

interface SongBpmFilterProps {
	songBpmFilter: SongBpmFilterStore;
}

const SongBpmFilter = observer(
	({ songBpmFilter }: SongBpmFilterProps): React.ReactElement => {
		const [value, setValue] = React.useState(songBpmFilter.bpmAsString ?? '');

		return (
			<input
				type="number"
				value={value}
				onChange={(e): void => {
					const value = e.target.value;

					setValue(value);

					// Update `songBpmFilter.bpmAsString` if and only if either the `value` is `undefined`, which means the input is empty,
					// or the `value` is greater than or equal to `absoluteMinBpm` and is less than or equal to `absoluteMaxBpm`.
					if (
						!value ||
						Math.max(
							absoluteMinBpm,
							Math.min(absoluteMaxBpm, Number(value)),
						) === Number(value)
					) {
						runInAction(() => {
							songBpmFilter.bpmAsString = value;
						});
					}
				}}
				className="input-small"
				maxLength={10}
				min={absoluteMinBpm}
				max={absoluteMaxBpm}
				step={bpmStep}
			/>
		);
	},
);

export default SongBpmFilter;
