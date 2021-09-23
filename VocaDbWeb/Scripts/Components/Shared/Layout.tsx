import Alert from '@Bootstrap/Alert';
import React from 'react';
import { Helmet } from 'react-helmet-async';

interface LayoutProps {
	children?: React.ReactNode;
	subtitle?: string;
	title?: string;
	toolbar?: React.ReactNode;
}

const Layout = ({
	children,
	subtitle,
	title,
	toolbar,
}: LayoutProps): React.ReactElement => {
	return (
		<>
			<Helmet>
				<title>
					{title ? `${title} - ${vdb.values.siteTitle}` : vdb.values.siteTitle}
				</title>
			</Helmet>

			{/* TODO */}

			{title && (
				<h1 className="page-title">
					{title}
					{subtitle && <small>&nbsp;{subtitle}</small>}
				</h1>
			)}

			{vdb.values.lockdownMessage && (
				<Alert>{vdb.values.lockdownMessage}</Alert>
			)}

			{vdb.values.sitewideAnnouncement && (
				<Alert>{vdb.values.sitewideAnnouncement}</Alert>
			)}

			{toolbar && <p>{toolbar}</p>}

			{/* TODO */}

			{children}

			{/* TODO */}
		</>
	);
};

export default Layout;
