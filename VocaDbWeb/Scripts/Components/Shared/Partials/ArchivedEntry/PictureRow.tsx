import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import React from 'react';

interface PictureRowProps {
	name: string;
	url: string;
	compareToUrl?: string;
}

export const PictureRow = React.memo(
	({ name, url, compareToUrl }: PictureRowProps): React.ReactElement => {
		return (
			<tr>
				<td>{name}</td>
				<td colSpan={compareToUrl !== undefined ? 1 : 2}>
					<img src={url} alt={name} className="coverPic" />
				</td>
				{compareToUrl !== undefined && (
					<td>
						<img src={compareToUrl} alt={name} className="coverPic" />
					</td>
				)}
			</tr>
		);
	},
);

interface PictureRow_ComparedVersionsContractProps<T> {
	name: string;
	comparedVersions: ComparedVersionsContract<T>;
	urlGetter: (id: number) => string;
}

export const PictureRow_ComparedVersionsContract = <T,>({
	name,
	comparedVersions,
	urlGetter,
}: PictureRow_ComparedVersionsContractProps<T>): React.ReactElement => {
	const url = urlGetter(comparedVersions.firstId);
	const compareToUrl = comparedVersions.secondId
		? urlGetter(comparedVersions.secondId)
		: undefined;

	return <PictureRow name={name} url={url} compareToUrl={compareToUrl} />;
};
