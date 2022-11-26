import Container from '@/Bootstrap/Container';
import Navbar from '@/Bootstrap/Navbar';
import { GlobalSearchBox } from '@/Components/Shared/GlobalSearchBox';
import { useLoginManager } from '@/LoginManagerContext';
import { entryReportRepo } from '@/Repositories/EntryReportRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { TopBarStore } from '@/Stores/TopBarStore';
import React from 'react';

export const Header = (): React.ReactElement => {
	const loginManager = useLoginManager();

	const [topBarStore] = React.useState(
		() => new TopBarStore(loginManager, entryReportRepo, userRepo),
	);

	return (
		<Navbar className="navbar-inverse" fixed="top" collapseOnSelect>
			<Container id="topBar">
				<GlobalSearchBox topBarStore={topBarStore} />
			</Container>
		</Navbar>
	);
};
