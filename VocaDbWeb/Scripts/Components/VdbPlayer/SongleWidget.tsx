import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { HttpClient } from '@/Shared/HttpClient';
import { css } from '@emotion/react';
import { useNostalgicDiva } from '@vocadb/nostalgic-diva';
import { observer } from 'mobx-react-lite';
import React from 'react';

const httpClient = new HttpClient();

const paddingTop = 4;
const seekBarHeight = 12;
const seekBarSpace = 4;
const bottomControlsHeight = 32;
const paddingBottom = 4;
export const songleWidgetHeight =
	paddingTop +
	(seekBarHeight + seekBarSpace) * 6 +
	bottomControlsHeight +
	paddingBottom;

interface SongleArtist {
	id: number;
	name: string;
}

interface SongleSong {
	url: string;
	artist: SongleArtist;
	id: number;
	duration: number;
	permalink: string;
	code: string;
	rmsAmplitude: number;
	createdAt: string;
	updatedAt: string;
	recognizedAt: string;
	title: string;
}

interface SongleRepeat {
	start: number;
	duration: number;
	index: number;
}

interface SongleSegment {
	index: number;
	isChorus: boolean;
	duration: number;
	repeats: SongleRepeat[];
}

interface SongleChorus {
	chorusSegments: SongleSegment[];
	repeatSegments: SongleSegment[];
}

interface SeekBarProps {
	y: number;
	height: number;
}

const SeekBar = React.memo(
	({ y, height }: SeekBarProps): React.ReactElement => {
		const diva = useNostalgicDiva();

		const ref = React.useRef<SVGRectElement>(undefined!);
		const handleClick = React.useCallback(
			async (e: React.MouseEvent): Promise<void> => {
				const duration = await diva.getDuration();
				if (duration !== undefined) {
					const rect = ref.current.getBoundingClientRect();
					const fraction = (e.clientX - rect.left) / rect.width;
					await diva.setCurrentTime(duration * fraction);
				}
			},
			[diva],
		);

		const [hover, setHover] = React.useState(false);

		return (
			<rect
				ref={ref}
				x={0}
				y={y}
				width="100%"
				height={height}
				fill={hover ? 'rgb(50, 50, 50)' : 'rgb(43, 43, 43)'}
				onClick={handleClick}
				onMouseEnter={(): void => setHover(true)}
				onMouseLeave={(): void => setHover(false)}
			/>
		);
	},
);

interface RepeatRectProps {
	song: SongleSong;
	segment: SongleSegment;
	repeat: SongleRepeat;
}

const RepeatRect = ({
	song,
	segment,
	repeat,
}: RepeatRectProps): React.ReactElement => {
	const diva = useNostalgicDiva();

	const [hover, setHover] = React.useState(false);

	return (
		<rect
			x={`${(repeat.start / song.duration) * 100}%`}
			y={segment.index * (seekBarHeight + seekBarSpace)}
			width={`${(repeat.duration / song.duration) * 100}%`}
			height={seekBarHeight}
			stroke={hover ? undefined : 'rgb(133, 133, 133)'}
			fill={
				hover
					? segment.isChorus
						? 'rgb(255, 130, 50)'
						: 'rgb(76, 194, 255)'
					: 'rgb(78, 78, 78)'
			}
			css={{ cursor: 'pointer' }}
			onClick={(): Promise<void> => diva.setCurrentTime(repeat.start / 1000)}
			onMouseEnter={(): void => setHover(true)}
			onMouseLeave={(): void => setHover(false)}
		/>
	);
};

export const SongleIcon = React.memo(
	(): React.ReactElement => {
		return (
			<img src="/Content/songle.png" alt="Songle" width={16} height={16} />
		);
	},
);

export const SongleWidget = observer(
	(): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		const [song, setSong] = React.useState<SongleSong & SongleChorus>();

		const load = React.useCallback(async (pv: PVContract): Promise<void> => {
			const url = VideoServiceHelper.getUrlById(pv);
			const [song, chorus] = await Promise.all([
				httpClient.get<SongleSong>(
					`https://widget.songle.jp/api/v1/song.json?url=${url}`,
				),
				httpClient.get<SongleChorus>(
					`https://widget.songle.jp/api/v1/song/chorus.json?url=${url}`,
				),
			]);
			setSong({ ...chorus, ...song });
		}, []);

		React.useEffect(() => {
			setSong(undefined);

			const { currentItem } = playQueue;
			if (!currentItem) return;

			const timeoutId = setTimeout(async () => {
				await load(currentItem.pv);
			}, 1000);

			return (): void => {
				clearTimeout(timeoutId);
			};
		}, [playQueue, playQueue.currentItem?.pv, load]);

		return (
			<div
				css={{
					display: 'flex',
					flexDirection: 'column',
					backgroundColor: 'rgb(32, 32, 32)',
					height: songleWidgetHeight - (paddingTop + paddingBottom),
					paddingTop: paddingTop,
					paddingBottom: paddingBottom,
				}}
			>
				<div css={{ flexGrow: 1 }}>
					<svg
						width="100%"
						height={(seekBarHeight + seekBarSpace) * 6}
						css={{ display: 'block' }}
					>
						<g>
							{song?.repeatSegments.map((segment) => (
								<React.Fragment key={segment.index}>
									<SeekBar
										y={segment.index * (seekBarHeight + seekBarSpace)}
										height={seekBarHeight}
									/>
									{segment.repeats.map((repeat) => (
										<RepeatRect
											song={song}
											segment={segment}
											repeat={repeat}
											key={repeat.index}
										/>
									))}
								</React.Fragment>
							))}
						</g>
					</svg>
				</div>
				<div
					css={{
						display: 'flex',
						height: '100%',
						paddingLeft: 12,
						paddingRight: 12,
					}}
				>
					<div css={{ display: 'flex', alignItems: 'center' }}>
						{playQueue.currentItem && (
							<a
								href={`https://songle.jp/songs/${encodeURIComponent(
									VideoServiceHelper.getUrlById(
										playQueue.currentItem.pv,
									).replace('https://', ''),
								)}`}
								target="_blank"
								rel="noreferrer"
								css={css`
									color: white;
									&:hover {
										color: white;
									}
									&:visited {
										color: white;
									}
									font-weight: bold;
								`}
							>
								<i className="icon-pencil icon-white" /> Edit on Songle
								{/* TODO: localize */}
							</a>
						)}
					</div>
					<div css={{ display: 'flex', alignItems: 'center', flexGrow: 1 }} />
					<div css={{ display: 'flex', alignItems: 'center' }}>
						<a
							href="https://api.songle.jp/"
							target="_blank"
							rel="noreferrer"
							css={css`
								color: white;
								&:hover {
									color: white;
								}
								&:visited {
									color: white;
								}
								font-weight: bold;
							`}
						>
							<SongleIcon /> Powered by Songle API{/* TODO: localize */}
						</a>
					</div>
				</div>
			</div>
		);
	},
);
