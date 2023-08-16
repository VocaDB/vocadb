'use client';

import { AppShell, Burger, Group } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import React from 'react';
import dynamic from 'next/dynamic';

const Logo = dynamic(() => import('./Logo'), { ssr: false });
const ColorSchemeToggle = dynamic(() => import('../ColorSchemeToggle/ColorSchemeToggle'), {
	ssr: false,
});

interface CustomAppShellProps {
	children?: React.ReactNode;
}

export default function CustomAppShell({ children }: CustomAppShellProps) {
	const [opened, { toggle }] = useDisclosure();

	return (
		<AppShell header={{ height: 60 }} padding="md">
			<AppShell.Header>
				<Group h="100%" px="md">
					<Burger opened={opened} onClick={toggle} hiddenFrom="sm" />
					<Logo />
					<ColorSchemeToggle />
				</Group>
			</AppShell.Header>
			<AppShell.Main>{children}</AppShell.Main>
		</AppShell>
	);
}

