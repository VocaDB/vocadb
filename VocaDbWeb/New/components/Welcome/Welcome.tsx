import { Title, Text, Anchor, useMantineTheme } from '@mantine/core';
import useStyles from './Welcome.styles';

export function Welcome() {
	const { classes } = useStyles();
	const theme = useMantineTheme();

	return (
		<>
			<Title className={classes.title} align="center" mt={100}>
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
			<Text color="dimmed" align="center" size="lg" sx={{ maxWidth: 580 }} mx="auto" mt="xl">
				This will be the frontpage.{' '}
				<Anchor href="https://vocadb.net" size="lg">
					Here is a link
				</Anchor>
			</Text>
		</>
	);
}

