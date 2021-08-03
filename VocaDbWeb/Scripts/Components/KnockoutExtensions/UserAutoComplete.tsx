import UserApiContract from '@DataContracts/User/UserApiContract';
import functions from '@Shared/GlobalFunctions';
import React from 'react';

import EntryAutoComplete, {
	EntryAutoCompleteParams,
} from './EntryAutoComplete';

interface UserAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	onAcceptSelection: (entry: UserApiContract) => void;
}

const UserAutoComplete = ({
	onAcceptSelection,
	...props
}: UserAutoCompleteProps): React.ReactElement => {
	const params: EntryAutoCompleteParams<UserApiContract> = {
		acceptSelection: (id, term, itemType, item) => {
			onAcceptSelection(item!);
		},
		createNewItem: null!,
		createOptionFirstRow: (item) => item.name!,
		extraQueryParams: {},
		termParamName: 'query',
		singleRow: true,
	};

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/users')}
			params={params}
			{...props}
		/>
	);
};

export default UserAutoComplete;
