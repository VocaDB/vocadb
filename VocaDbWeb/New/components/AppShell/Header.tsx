import { Burger, Header, MediaQuery, createStyles, useMantineTheme } from '@mantine/core';
import Image from 'next/image';
import DarkLogo from '../../public/VocaDB_Logo_White_Transparent_No_Outline.png';
import LightLogo from '../../public/VocaDB_Logo_Black_Transparent_No_Outline.png';

interface CustomHeaderProps {
	opened: boolean;
	setOpened: React.Dispatch<React.SetStateAction<boolean>>;
}

const useStyles = createStyles((theme) => ({
	header: {
		display: 'flex',
		justifyContent: 'spaceBetween',
		[theme.fn.smallerThan('sm')]: {
			flexDirection: 'row-reverse',
		},
	},

	image: {
		objectFit: 'contain',
		height: '100%',
	},
}));

const Customheader = ({ opened, setOpened }: CustomHeaderProps): React.ReactElement => {
	const { classes, theme } = useStyles();

	return (
		<Header height={{ base: 50, md: 70 }} className={classes.header} px="md">
			<MediaQuery smallerThan="sm" styles={{ display: 'none' }}>
				<Image
					className={classes.image}
					width={167}
					height={69}
					src={theme.colorScheme === 'dark' ? DarkLogo : LightLogo}
					alt=""
				/>
			</MediaQuery>
			<div
				style={{
					display: 'flex',
					alignItems: 'center',
					height: '100%',
				}}
			>
				<MediaQuery largerThan="sm" styles={{ display: 'none' }}>
					<Burger
						opened={opened}
						onClick={(): void => setOpened((o) => !o)}
						size="sm"
						color={theme.colors.gray[6]}
					/>
				</MediaQuery>
			</div>
		</Header>
	);
};

export default Customheader;
