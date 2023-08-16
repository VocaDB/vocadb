import { Container, Paper, Tooltip } from '@mantine/core';

interface EntryToolTipCardProps {
	children: JSX.Element;
	tooltip?: JSX.Element;
}

export default function EntryToolTipCard({ children, tooltip }: EntryToolTipCardProps) {
	return (
		<Tooltip
			w="300px"
			multiline
			color="white"
			openDelay={150}
			styles={{
				tooltip: {
					padding: 0,
					border: '1px solid gray',
				},
			}}
			label={<Container p="md">{tooltip}</Container>}
		>
			{children}
		</Tooltip>
	);
}

