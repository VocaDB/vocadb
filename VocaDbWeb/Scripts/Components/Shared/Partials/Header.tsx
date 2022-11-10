import Container from '@/Bootstrap/Container';
import Navbar from '@/Bootstrap/Navbar';
import { GlobalSearchBox } from '@/Components/Shared/GlobalSearchBox';
import { LoginManager } from '@/Models/LoginManager';
import { entryReportRepo } from '@/Repositories/EntryReportRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { TopBarStore } from '@/Stores/TopBarStore';
import React from 'react';

const loginManager = new LoginManager(vdb.values);

const topBarStore = new TopBarStore(loginManager, entryReportRepo, userRepo);

export const Header = (): React.ReactElement => {
	return (
		<Navbar className="navbar-inverse" fixed="top" collapseOnSelect>
			<Container id="topBar">
				<GlobalSearchBox topBarStore={topBarStore} />
			</Container>
		</Navbar>
	);
};
