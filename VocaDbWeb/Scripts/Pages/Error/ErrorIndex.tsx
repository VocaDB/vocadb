import React from 'react';
import { Link, useSearchParams } from 'react-router-dom';

import ErrorBadRequest from './ErrorBadRequest';
import ErrorForbidden from './ErrorForbidden';
import ErrorNotFound from './ErrorNotFound';

const ErrorIndex = (): React.ReactElement => {
	const [params] = useSearchParams();
	const code = params.get('code');

	if (code === '404') {
		return <ErrorNotFound />;
	} else if (code === '403') {
		return <ErrorForbidden />;
	} else if (code === '400') {
		return <ErrorBadRequest />;
	}

	return (
		<>
			<h2>Sorry, an unhandled error occurred :(</h2>

			<img
				src="http://static.vocadb.net/img/844a799896b0ffe44129bf92d2fa55a5.jpg"
				alt="ERROR, image by つむつむ"
				title="ERROR, image by つむつむ"
			/>

			<h3>Possible reasons include</h3>
			<ul>
				<li>You followed an invalid link.</li>
				<li>
					The site is temporarily down because it is being updated. Check back
					in a few minutes.
				</li>
				<li>
					You entered (possibly accidentally) characters that were automatically
					detected as HTML and thus were blocked.
				</li>
				<li>
					You have encountered a programming error (bug). If you suspect this is
					the case, please inform us about it. Please see{' '}
					<a href="//wiki.vocadb.net/docs/footer/contact">Help/Contact us</a>{' '}
					for contact information.
				</li>
			</ul>
		</>
	);
};

export default ErrorIndex;
