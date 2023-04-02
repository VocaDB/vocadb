import {
	AutocompleteProps,
	AutocompleteItem,
	Autocomplete,
} from '@mantine/core';
import { useDebouncedValue } from '@mantine/hooks';
import { useEffect, useState } from 'react';

type FetchDataReturn = readonly (string | AutocompleteItem)[];

type MantineAutocompleteProps = {
	fetchData: (value: string) => Promise<FetchDataReturn>;
} & Omit<AutocompleteProps, 'data'>;

const MantineAutocomplete = ({
	fetchData,
	...props
}: MantineAutocompleteProps): JSX.Element => {
	const [value, setValue] = useState('');
	const [debounced] = useDebouncedValue(value, 100);
	const [data, setData] = useState<FetchDataReturn>([]);

	useEffect(() => {
		fetchData(value).then((res) => setData(res));
	}, [debounced]);

	return (
		<Autocomplete
			{...props}
			value={value}
			onChange={(v): void => setValue(v)}
			data={data ?? []}
		/>
	);
};

export default MantineAutocomplete;
