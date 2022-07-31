import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import classNames from 'classnames';
import React from 'react';

interface DataRowProps<T> {
	name: string;
	val: T;
	compareVal?: T;
	preserveLineBreaks?: boolean;
}

export const DataRow = React.memo(
	<T,>({
		name,
		val,
		compareVal,
		preserveLineBreaks = false,
	}: DataRowProps<T>): React.ReactElement => {
		return (
			<tr className={classNames(compareVal !== undefined && 'changed')}>
				<td>{name}</td>
				<td
					colSpan={compareVal !== undefined ? 1 : 2}
					className={classNames(preserveLineBreaks && 'pre-line')}
				>
					{val}
				</td>
				{compareVal !== undefined && (
					<td className={classNames(preserveLineBreaks && 'pre-line')}>
						{compareVal}
					</td>
				)}
			</tr>
		);
	},
);

interface DataRow_ComparedVersionsContractProps<T> {
	name: string;
	comparedVersions: ComparedVersionsContract<T>;
	valGetter: (data: T) => React.ReactNode;
	preserveLineBreaks?: boolean;
}

export const DataRow_ComparedVersionsContract = <T,>({
	name,
	comparedVersions,
	valGetter,
	preserveLineBreaks = false,
}: DataRow_ComparedVersionsContractProps<T>): React.ReactElement => {
	const val = valGetter(comparedVersions.firstData);
	const compareVal =
		comparedVersions.secondData !== undefined
			? valGetter(comparedVersions.secondData)
			: undefined;

	return (
		<DataRow
			name={name}
			val={val}
			compareVal={compareVal}
			preserveLineBreaks={preserveLineBreaks}
		/>
	);
};
