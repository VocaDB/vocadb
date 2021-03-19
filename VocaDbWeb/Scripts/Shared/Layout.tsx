import React, { Fragment, ReactElement, ReactNode } from 'react';
import GlobalSearchBox from './GlobalSearchBox';
import Footer from './Partials/Footer';
import LeftMenu from './Partials/LeftMenu';

interface LayoutProps {
	children: ReactNode;
}

const Layout = ({
	children,
}: LayoutProps): ReactElement => {
	return (
		<Fragment>
			<div className="navbar navbar-inverse navbar-fixed-top">
				<div className="navbar-inner">
					<div id="topBar" className="container">
						<GlobalSearchBox /* TODO */ />
					</div>
				</div>
			</div>

			<div className="container-fluid">
				<div className="row-fluid">
					<LeftMenu />

					<div className="span10 rightFrame well">
						{/* TODO */}

						{children}

						{/* TODO */}
					</div>
				</div>
			</div>

			<Footer />
		</Fragment>
	);
};

export default Layout;
