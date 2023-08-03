import { sumDatesInOneDay } from '@/Helpers/DateTimeHelper';
import { apiGet } from '@/Helpers/FetchApiHelper';
import { SongDetailsContract } from '@/types/DataContracts/Song/SongDetailsContract';
import { RatedSongForUserForApiContract } from '@/types/DataContracts/User/RatedSongForUserForApiContract';
import { useMantineTheme, Title } from '@mantine/core';
import dynamic from 'next/dynamic';
import useSWR from 'swr';

const ResponsiveLine = dynamic(() => import('@nivo/line').then((m) => m.Line), {
	ssr: false,
});

interface CustomSymbolProps {
	size: number;
	color: string;
	borderWidth: number;
	borderColor: string;
}

const CustomSymbol = ({ size, color, borderWidth, borderColor }: CustomSymbolProps) => (
	<g>
		<circle fill="#fff" r={size / 2} strokeWidth={borderWidth} stroke={borderColor} />
		<circle
			r={size / 5}
			strokeWidth={borderWidth}
			stroke={borderColor}
			fill={color}
			fillOpacity={0.35}
		/>
	</g>
);

interface StatsTabProps {
	details: SongDetailsContract;
}

export default function StatsTab({ details }: StatsTabProps) {
	const { data } = useSWR(
		`/api/songs/${details.song.id}/ratings`,
		apiGet<RatedSongForUserForApiContract[]>
	);
	const theme = useMantineTheme();

	if (data === undefined) return <></>;

	return (
		<>
			<Title order={4} mt="md" mb="xs">
				Rating over time
			</Title>
			<ResponsiveLine
				width={900}
				height={400}
				margin={{ top: 30, right: 20, bottom: 30, left: 80 }}
				theme={{ background: 'white', tooltip: { basic: { color: 'black' } } }}
				// colors={{ scheme: theme.colorScheme === 'dark' ? 'dark2' : 'nivo' }}
				// theme={{
				// 	textColor: theme.colorScheme === 'dark' ? 'white' : 'black',
				// 	grid: {
				// 		line: {
				// 			stroke: 'white',
				// 		},
				// 	},
				// }}
				data={[
					{
						id: 'fake corp. A',
						data: sumDatesInOneDay(data.map((rating) => rating.date)).map(
							(dateStat) => ({
								x: dateStat.date,
								y: dateStat.count,
							})
						),
						// data: [
						// 	{ x: '2018-01-01', y: 7 },
						// 	{ x: '2018-01-02', y: 5 },
						// 	{ x: '2018-01-03', y: 11 },
						// 	{ x: '2018-01-04', y: 9 },
						// 	{ x: '2018-01-05', y: 12 },
						// 	{ x: '2018-01-06', y: 16 },
						// 	{ x: '2018-01-07', y: 13 },
						// 	{ x: '2018-01-08', y: 13 },
						// ],
					},
				]}
				xScale={{
					type: 'time',
					format: '%Y-%m-%d',
					useUTC: false,
					precision: 'day',
				}}
				xFormat="time:%Y-%m-%d"
				yScale={{
					type: 'linear',
				}}
				axisLeft={{
					legend: 'linear scale',
					legendOffset: 12,
				}}
				axisBottom={{
					format: '%b %d',
					legend: 'time scale',
					legendOffset: -12,
				}}
				curve="catmullRom"
				enablePointLabel={true}
				pointSymbol={CustomSymbol}
				pointSize={16}
				pointBorderWidth={1}
				pointBorderColor={{
					from: 'color',
					modifiers: [['darker', 0.3]],
				}}
				useMesh={true}
				enableSlices={false}
			/>
		</>
	);
}

