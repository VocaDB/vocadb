import Container from '@Bootstrap/Container';
import Navbar from '@Bootstrap/Navbar';
import GlobalSearchBox from '@Components/Shared/GlobalSearchBox';
import LoginManager from '@Models/LoginManager';
import EntryReportRepository from '@Repositories/EntryReportRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import TopBarStore from '@Stores/TopBarStore';
import React from 'react';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const entryReportRepo = new EntryReportRepository(httpClient, urlMapper);
const userRepo = new UserRepository(httpClient, urlMapper);

const topBarStore = new TopBarStore(loginManager, entryReportRepo, userRepo);

const AppHeader = (): React.ReactElement => {
	return (
		<Navbar className="navbar-inverse" fixed="top" collapseOnSelect>
			<Container id="topBar">
				<GlobalSearchBox topBarStore={topBarStore} />
			</Container>
		</Navbar>
	);
};

export default AppHeader;
