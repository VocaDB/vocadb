import { sumDatesInOneDay } from '@/Helpers/DateTimeHelper';
import { apiGet } from '@/Helpers/FetchApiHelper';
import { SongDetailsContract } from '@/types/DataContracts/Song/SongDetailsContract';
import { RatedSongForUserForApiContract } from '@/types/DataContracts/User/RatedSongForUserForApiContract';
import { Title } from '@mantine/core';
import { Line } from '@nivo/line';
import useSWR from 'swr';

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

	if (data === undefined) return <></>;

	return (
		<>
			<Title order={4} mt="md" mb="xs">
				Rating over time
			</Title>
			<Line
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

