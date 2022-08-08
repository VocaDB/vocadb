import React from 'react';

const ErrorNotFound = (): React.ReactElement => {
	React.useEffect(() => {
		window.location.replace('/Error?code=404');
	}, []);

	return <></>;
};

export default ErrorNotFound;
