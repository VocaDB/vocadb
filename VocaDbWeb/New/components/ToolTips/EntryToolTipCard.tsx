import { Paper, Tooltip } from '@mantine/core';

interface EntryToolTipCardProps {
	children: JSX.Element;
	tooltip?: JSX.Element;
}

export default function EntryToolTipCard({ children, tooltip }: EntryToolTipCardProps) {
	return (
		<Tooltip
			w="300px"
			multiline
			openDelay={150}
			styles={{
				tooltip: {
					padding: 0,
				},
			}}
			label={
				<Paper shadow="xs" withBorder radius="xs" p="md">
					{tooltip}
				</Paper>
			}
		>
			{children}
		</Tooltip>
	);
}

