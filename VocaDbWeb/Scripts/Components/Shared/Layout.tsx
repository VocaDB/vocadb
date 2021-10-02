import Alert from '@Bootstrap/Alert';
import Breadcrumb from '@Bootstrap/Breadcrumb';
import React from 'react';

interface LayoutProps {
	children?: React.ReactNode;
	parents?: React.ReactNode;
	subtitle?: string;
	title?: string;
	toolbar?: React.ReactNode;
}

const Layout = ({
	children,
	parents,
	subtitle,
	title,
	toolbar,
}: LayoutProps): React.ReactElement => {
	return (
		<>
			{parents && <Breadcrumb>{parents}</Breadcrumb>}

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
