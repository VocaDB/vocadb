import { ActionIcon, Button, Card, Group, Text, useMantineColorScheme } from '@mantine/core';
import { IconSun, IconMoonStars } from '@tabler/icons';
import Image from 'next/image';
import { useThemeOverride } from '../Context/ThemeContext';

export function ColorSchemeToggle() {
	const { colorScheme, toggleColorScheme } = useMantineColorScheme();
	const { setPrimaryColor } = useThemeOverride();

	return (
		<>
			<Group position="center" mt="xl">
				<ActionIcon
					onClick={() => toggleColorScheme()}
					size="xl"
					sx={(theme) => ({
						backgroundColor:
							theme.colorScheme === 'dark'
								? theme.colors.dark[6]
								: theme.colors.gray[0],
						color:
							theme.colorScheme === 'dark'
								? theme.colors.yellow[4]
								: theme.colors.blue[6],
					})}
				>
					{colorScheme === 'dark' ? (
						<IconSun size={20} stroke={1.5} />
					) : (
						<IconMoonStars size={20} stroke={1.5} />
					)}
				</ActionIcon>
			</Group>
			<Text mt="xl" align="center" fw={500}>
				Choose your color!
			</Text>
			<Text align="center" variant="dimmed">
				(Just an idea)
			</Text>
			<Group mt="md" position="center">
				<Card shadow="sm" radius="md" maw={'20vw'}>
					<Image
						src="/Hatsune Miku.png"
						style={{ marginLeft: 'auto', marginRight: 'auto', display: 'block' }}
						width={80}
						height={120}
						alt="Hatsune Miku"
					/>

					<Text mt="sm" size="sm" color="dimmed">
						Ullamco duis exercitation nulla aute id anim sit veniam. Velit sint sint sit
						do esse incididunt aliquip eu aliquip veniam ad in. Culpa cillum sunt irure
						quis nostrud qui laborum.
					</Text>

					<Button
						onClick={() => setPrimaryColor('miku')}
						mt="sm"
						variant="light"
						color="miku"
						radius="md"
						fullWidth
						title="Color scheme toggle"
					>
						Apply Color
					</Button>
				</Card>
				<Card shadow="sm" radius="md" maw={'20vw'}>
					<Image
						src="/Megurine Luka.png"
						style={{ marginLeft: 'auto', marginRight: 'auto', display: 'block' }}
						width={31}
						height={120}
						alt="Hatsune Miku"
					/>

					<Text mt="sm" size="sm" color="dimmed">
						Ullamco duis exercitation nulla aute id anim sit veniam. Velit sint sint sit
						do esse incididunt aliquip eu aliquip veniam ad in. Culpa cillum sunt irure
						quis nostrud qui laborum.
					</Text>

					<Button
						onClick={() => setPrimaryColor('luka')}
						mt="sm"
						variant="light"
						color="luka"
						radius="md"
						fullWidth
					>
						Apply Color
					</Button>
				</Card>
			</Group>
		</>
	);
}

