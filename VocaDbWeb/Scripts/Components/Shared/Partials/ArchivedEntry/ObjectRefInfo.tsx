import { DataRowList_ComparedVersionsContract } from '@/Components/Shared/Partials/ArchivedEntry/DataRowList';
import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import React from 'react';

interface ObjectRefInfoProps {
	objRef?: ObjectRefContract;
}

export const ObjectRefInfo = React.memo(
	({ objRef }: ObjectRefInfoProps): React.ReactElement => {
		return objRef ? (
			<>
				{objRef.nameHint} [{objRef.id}]
			</>
		) : (
			<></>
		);
	},
);

interface ObjectRefList_ComparedVersionsContractProps<T> {
	name: string;
	comparedVersions: ComparedVersionsContract<T>;
	valGetter: (data: T) => ObjectRefContract[];
}

export const ObjectRefList_ComparedVersionsContract = <T,>({
	name,
	comparedVersions,
	valGetter,
}: ObjectRefList_ComparedVersionsContractProps<T>): React.ReactElement => {
	return (
		// eslint-disable-next-line react/jsx-pascal-case
		<DataRowList_ComparedVersionsContract
			name={name}
			comparedVersions={comparedVersions}
			valGetter={(data): React.ReactNode[] =>
				valGetter(data).map((objRef, index) => (
					<ObjectRefInfo objRef={objRef} key={index} />
				))
			}
		/>
	);
};
