import { Burger, Header, MediaQuery, createStyles } from '@mantine/core';
import Image from 'next/image';
import DarkLogo from '../../public/VocaDB_Logo_White_Transparent_No_Outline.png';
import LightLogo from '../../public/VocaDB_Logo_Black_Transparent_No_Outline.png';
import dynamic from 'next/dynamic';
import { useStyles } from './Header.styles';
// TODO: Properly fix the ColorSchemeToggle hydration issues
const ColorSchemeToggle = dynamic(() => import('../ColorSchemeToggle/ColorSchemeToggle'), {
	ssr: false,
});

interface CustomHeaderProps {
	opened: boolean;
	setOpened: React.Dispatch<React.SetStateAction<boolean>>;
}

const Customheader = ({ opened, setOpened }: CustomHeaderProps): React.ReactElement => {
	const { classes, theme } = useStyles();

	return (
		<Header height={{ base: 50, sm: 70 }} className={classes.header} px="md">
			<Image
				className={classes.image}
				width={167}
				height={69}
				src={theme.colorScheme === 'dark' ? DarkLogo : LightLogo}
				alt=""
			/>
			<div className={classes.rightSectionWrapper}>
				<ColorSchemeToggle />
				<Burger
					opened={opened}
					onClick={(): void => setOpened((o) => !o)}
					size="sm"
					color={theme.colors.gray[6]}
					title="Open navigation menu"
					className={classes.burger}
				/>
			</div>
		</Header>
	);
};

export default Customheader;

