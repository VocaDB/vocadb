import Button from '@/Bootstrap/Button';
import { PVService } from '@/Models/PVs/PVService';
import React from 'react';

type ThirdPartyPVService = Exclude<
	PVService,
	PVService.File | PVService.LocalFile
>;

const domainNames: Record<ThirdPartyPVService, string> = {
	[PVService.Bandcamp]: 'bandcamp.com',
	[PVService.Bilibili]: 'bilibili.tv',
	[PVService.Creofuga]: 'creofuga.net',
	[PVService.NicoNicoDouga]: 'nicovideo.jp',
	[PVService.Piapro]: 'piapro.jp',
	[PVService.SoundCloud]: 'soundcloud.com',
	[PVService.Vimeo]: 'vimeo.com',
	[PVService.Youtube]: 'youtube.com',
};

const termsOfServiceUrls: Record<ThirdPartyPVService, string> = {
	[PVService.Bandcamp]: 'https://bandcamp.com/terms_of_use',
	[PVService.Bilibili]: 'https://www.bilibili.tv/en/user-agreement',
	[PVService.Creofuga]: 'https://creofuga.net/',
	[PVService.NicoNicoDouga]:
		'https://account.nicovideo.jp/rules/account?language=en-us',
	[PVService.Piapro]: 'https://piapro.jp/user_agreement/',
	[PVService.SoundCloud]: 'https://soundcloud.com/terms-of-use',
	[PVService.Vimeo]: 'https://vimeo.com/terms/',
	[PVService.Youtube]: 'https://www.youtube.com/t/terms',
};

interface CookieConsentBannerProps {
	service: ThirdPartyPVService;
	width?: string | number;
	height?: string | number;
	onLoadVideo: () => void;
	onDoNotAskAgain: () => void;
}

export const CookieConsentBanner = React.memo(
	({
		service,
		width = 560,
		height = 315,
		onLoadVideo,
		onDoNotAskAgain,
	}: CookieConsentBannerProps): React.ReactElement => {
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
						<a
							href={termsOfServiceUrls[service]}
							target="_blank"
							rel="noreferrer"
							style={{ color: '#5fb3fb' }}
						>
							terms and conditions
						</a>{' '}
						of {domainNames[service]}.
					</div>
					<div>
						<Button onClick={onLoadVideo} variant="primary">
							<i className="icon-play icon-white" /> Load video
							{/* LOC */}
						</Button>{' '}
						<Button onClick={onDoNotAskAgain}>
							Don't ask again{/* LOC */}
						</Button>
					</div>
				</div>
			</div>
		);
	},
);
