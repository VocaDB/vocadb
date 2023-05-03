import React from 'react';

const ErrorBadRequest = (): React.ReactElement => {
	return (
		<>
			<h1>400 - Bad Request</h1>
			<h3>Sorry, the server rejected your input.</h3>
			<p>
				The most common reason for this error is following a link with invalid
				characters. We're sorry for the inconvenience.
			</p>
		</>
	);
};

export default ErrorBadRequest;
