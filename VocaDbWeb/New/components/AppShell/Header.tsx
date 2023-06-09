import { Burger, Header, MediaQuery, useMantineTheme } from '@mantine/core';

interface CustomHeaderProps {
	opened: boolean;
	setOpened: React.Dispatch<React.SetStateAction<boolean>>;
}

const Customheader = ({ opened, setOpened }: CustomHeaderProps): React.ReactElement => {
	const theme = useMantineTheme();

	return (
		<Header height={{ base: 50, md: 70 }} style={{ display: 'flex' }} px="md">
			<MediaQuery smallerThan="sm" styles={{ display: 'none' }}>
				<img
					style={{ maxHeight: '100%' }}
					src="/New/VocaDB_Logo_White_Transparent_No_Outline.png"
					alt=""
				/>
			</MediaQuery>
			<div style={{ display: 'flex', alignItems: 'center', height: '100%' }}>
				<MediaQuery largerThan="sm" styles={{ display: 'none' }}>
					<Burger
						opened={opened}
						onClick={(): void => setOpened((o) => !o)}
						size="sm"
						color={theme.colors.gray[6]}
						mr="xl"
					/>
				</MediaQuery>
			</div>
		</Header>
	);
};

export default Customheader;

