import React from 'react';

interface LargeShareLinksProps {
	title: string;
	url: string;
}

export const LargeShareLinks = ({
	title,
	url,
}: LargeShareLinksProps): React.ReactElement => {
	const urlEncodedTitle = encodeURIComponent(title);

	return (
		<>
			<a
				href={`https://www.facebook.com/sharer/sharer.php?u=${url}`}
				target="_blank"
				rel="noreferrer"
			>
				<img
					alt="Share on Facebook" /* LOC */
					src="/Content/ExtIcons/facebook32.png"
					style={{ border: 0, height: '32px', width: '32px', margin: '0 1px' }}
					title="Share on Facebook" /* LOC */
				/>
			</a>{' '}
			<a
				href={`https://reddit.com/submit?url=${url}&title=${urlEncodedTitle}`}
				target="_blank"
				rel="noreferrer"
				title="Reddit!"
			>
				<img
					alt="Reddit!"
					src="/Content/ExtIcons/reddit32.png"
					style={{ border: 0, height: '32px', width: '32px', margin: '0 1px' }}
					title="Reddit!"
				/>
			</a>{' '}
			<a
				href={`https://twitter.com/home/?status=${urlEncodedTitle}: ${url}`}
				target="_blank"
				rel="noreferrer"
				title="Share on Twitter" /* LOC */
			>
				<img
					alt="Twitter"
					src="/Content/ExtIcons/twitter32.png"
					style={{
						border: 0,
						height: '32px',
						width: '32px',
						margin: '0 1px',
					}}
					title="Share on Twitter" /* LOC */
				/>
			</a>{' '}
			<a
				href={`https://plus.google.com/share?url=${url}`}
				onClick={(e): void => {
					e.preventDefault();

					window.open(
						`https://plus.google.com/share?url=${url}`,
						'',
						'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600',
					);
				}}
				title="Share on Google+" /* LOC */
			>
				<img
					alt="Google+"
					src="https://www.gstatic.com/images/icons/gplus-32.png"
					style={{ border: 0, height: '32px', width: '32px', margin: '0 1px' }}
					title="Share on Google+" /* LOC */
				/>
			</a>
		</>
	);
};
