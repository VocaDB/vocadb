import Button from '@/Bootstrap/Button';
import React from 'react';

interface CookieConcentBannerProps {
	width?: number;
	height?: number;
}

export const CookieConcentBanner = React.memo(
	({
		width = 560,
		height = 315,
	}: CookieConcentBannerProps): React.ReactElement => {
		return (
			<div
				className="pv-embed-preview"
				css={{
					display: 'inline-block',
					position: 'relative',
					width: width,
					height: height,
				}}
			>
				<div
					css={{
						width: '100%',
						height: '100%',
						backgroundColor: 'rgb(28, 28, 28)',
						backgroundSize: 'cover',
						backgroundPosition: 'center',
						display: 'flex',
						flexDirection: 'column',
						justifyContent: 'center',
						alignItems: 'center',
					}}
				>
					<div
						css={{
							textAlign: 'center',
							maxWidth: 420,
							marginBottom: 20,
							color: 'white',
						}}
					>
						This content is hosted by a third party. By showing the external
						content you accept the{' '}
						<a href="#" style={{ color: '#5fb3fb' }}>
							terms and conditions
						</a>{' '}
						of youtube.com.
					</div>
					<div>
						<Button variant="primary">
							<i className="icon-play icon-white" /> Load video
							{/* TODO: localize */}
						</Button>{' '}
						<Button>Don't ask again{/* TODO: localize */}</Button>
					</div>
				</div>
			</div>
		);
	},
);
