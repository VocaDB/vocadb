import Alert from '@Bootstrap/Alert';
import usePageTracking from '@Components/usePageTracking';
import React from 'react';
import { useTitle } from 'react-use';

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
	useTitle(title ? `${title} - ${vdb.values.siteTitle}` : vdb.values.siteTitle);

	usePageTracking();

	return (
		<>
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
