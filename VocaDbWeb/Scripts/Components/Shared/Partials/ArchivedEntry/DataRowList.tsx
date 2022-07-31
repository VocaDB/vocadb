import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import classNames from 'classnames';
import React from 'react';

interface DataRowListProps {
	name: string;
	rows: React.ReactNode[];
	compareRows?: React.ReactNode[];
}

export const DataRowList = React.memo(
	({ name, rows, compareRows }: DataRowListProps): React.ReactElement => {
		return (
			<tr>
				<td>{name}</td>
				<td colSpan={compareRows !== undefined ? 1 : 2}>
					<ul>
						{rows.map((row, index) => (
							<li
								className={classNames(
									compareRows !== undefined &&
										typeof row === 'string' &&
										!compareRows.some(
											(compareRow) =>
												typeof compareRow === 'string' && row === compareRow,
										) &&
										'archived-entry-compare-modified',
								)}
								key={index}
							>
								{row}
							</li>
						))}
					</ul>
				</td>
				{compareRows !== undefined && (
					<td>
						<ul>
							{compareRows.map((compareRow, index) => (
								<li
									className={classNames(
										typeof compareRow === 'string' &&
											!rows.some(
												(row) => typeof row === 'string' && compareRow === row,
											) &&
											'archived-entry-compare-modified',
									)}
									key={index}
								>
									{compareRow}
								</li>
							))}
						</ul>
					</td>
				)}
			</tr>
		);
	},
);

interface DataRowList_ComparedVersionsContractProps<T> {
	name: string;
	comparedVersions: ComparedVersionsContract<T>;
	valGetter: (data: T) => React.ReactNode[];
}

export const DataRowList_ComparedVersionsContract = <T,>({
	name,
	comparedVersions,
	valGetter,
}: DataRowList_ComparedVersionsContractProps<T>): React.ReactElement => {
	const rows = valGetter(comparedVersions.firstData);
	const compareRows =
		comparedVersions.secondData !== undefined
			? valGetter(comparedVersions.secondData)
			: undefined;

	return <DataRowList name={name} rows={rows} compareRows={compareRows} />;
};
