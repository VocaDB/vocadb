import SongLengthFilterStore from '@Stores/Search/SongLengthFilter';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';

interface SongLengthFilterProps {
	songLengthFilter: SongLengthFilterStore;
}

const SongLengthFilter = observer(
	({ songLengthFilter }: SongLengthFilterProps): React.ReactElement => {
		const [value, setValue] = React.useState(songLengthFilter.lengthFormatted);

		return (
			<DebounceInput
				type="text"
				value={value}
				onChange={(e): void => setValue(e.target.value)}
				onBlur={(e): void => {
					runInAction(() => {
						songLengthFilter.lengthFormatted = e.target.value;
					});
					setValue(songLengthFilter.lengthFormatted);
				}}
				className="input-small"
				maxLength={10}
				debounceTimeout={300}
			/>
		);
	},
);

export default SongLengthFilter;
