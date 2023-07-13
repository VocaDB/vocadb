import { Paper, Tooltip } from '@mantine/core';

interface EntryToolTipCardProps {
	children: JSX.Element;
	tooltip?: JSX.Element;
}

export default function EntryToolTipCard({ children, tooltip }: EntryToolTipCardProps) {
	return (
		<Tooltip
			multiline
			styles={{
				tooltip: {
					padding: 0,
				},
			}}
			label={
				<Paper radius="xs" p="md">
					{tooltip}
				</Paper>
			}
		>
			{children}
		</Tooltip>
	);
}

