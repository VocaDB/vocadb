import { Title, Text, Anchor, useMantineTheme } from '@mantine/core';
import useStyles from './Welcome.styles';

export function Welcome() {
	const { classes } = useStyles();
	const theme = useMantineTheme();

	return (
		<>
			<Title className={classes.title} align="center">
				Welcome to{' '}
				<Text
					c={theme.primaryColor}
					variant="gradient"
					gradient={{
						from: theme.colors[theme.primaryColor][9],
						to: theme.colors[theme.primaryColor][2],
					}}
					inherit
					component="span"
				>
					VocaDB
				</Text>
			</Title>
		</>
	);
}

