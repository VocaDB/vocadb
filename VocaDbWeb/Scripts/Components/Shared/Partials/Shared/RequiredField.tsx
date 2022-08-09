import React from 'react';

export const RequiredField = React.memo(
	(): React.ReactElement => {
		return <span className="required-field">*</span>;
	},
);
