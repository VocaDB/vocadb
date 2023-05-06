import React from 'react';

const ErrorIPForbidden = (): React.ReactElement => {
	return (
		<>
			<h1>403 - Forbidden</h1>
			<p>
				Sorry, access from your host is restricted. It is possible this
				restriction is no longer valid. If you think this is the case, please
				contact support. We're sorry for the inconvenience.{' '}
			</p>
		</>
	);
};

export default ErrorIPForbidden;
