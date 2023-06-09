import { Burger, Header, MediaQuery, useMantineTheme } from '@mantine/core';
import Image from 'next/image';
import DarkLogo from '../../public/VocaDB_Logo_White_Transparent_No_Outline.png';
import LightLogo from '../../public/VocaDB_Logo_Black_Transparent_No_Outline.png';

interface CustomHeaderProps {
	opened: boolean;
	setOpened: React.Dispatch<React.SetStateAction<boolean>>;
}

const Customheader = ({ opened, setOpened }: CustomHeaderProps): React.ReactElement => {
	const theme = useMantineTheme();

	return (
		<Header height={{ base: 50, md: 70 }} style={{ display: 'flex' }} px="md">
			<MediaQuery smallerThan="sm" styles={{ display: 'none' }}>
				<>
					{theme.colorScheme === 'dark' && (
						<Image
							style={{ objectFit: 'contain', height: '100%' }}
							height={69}
							src={DarkLogo}
							alt=""
						/>
					)}
					{theme.colorScheme === 'light' && (
						<Image
							style={{ objectFit: 'contain', height: '100%' }}
							height={69}
							src={LightLogo}
							alt=""
						/>
					)}
				</>
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

