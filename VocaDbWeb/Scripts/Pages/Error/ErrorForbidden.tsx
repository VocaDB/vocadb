import React from 'react';

const ErrorForbidden = (): React.ReactElement => {

	return <>
		<h1>403 - Forbidden</h1>
		<h3>Sorry, you don't have access to this page/resource</h3>
        <p>This error might be caused by input that was rejected by the system. We're sorry for the inconvenience. </p>
	</>;
};

export default ErrorForbidden;
