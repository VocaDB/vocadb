import { Title, Text, Anchor } from '@mantine/core';
import useStyles from './Welcome.styles';

export function Welcome() {
	const { classes } = useStyles();

	return (
		<>
			<Title className={classes.title} align="center" mt={100}>
				Welcome to{' '}
				<Text inherit variant="gradient" component="span">
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

