import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { usePageTracking } from '@/Components/usePageTracking';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { useVdb } from '@/VdbContext';
import NProgress from 'nprogress';
import React from 'react';

interface LayoutProps {
	pageTitle: string | undefined;
	ready: boolean;
	children?: React.ReactNode;
	parents?: React.ReactNode;
	subtitle?: string;
	title?: string;
	toolbar?: React.ReactNode;
}

export const Layout = ({
	pageTitle,
	ready,
	children,
	parents,
	subtitle,
	title,
	toolbar,
}: LayoutProps): React.ReactElement => {
	const vdb = useVdb();

	useVdbTitle(pageTitle);

	usePageTracking(ready);

	React.useEffect(() => {
		NProgress.done();

		return (): void => {
			NProgress.start();
		};
	}, [pageTitle]);

	return (
		<>
			{parents && <Breadcrumb>{parents}</Breadcrumb>}

			{title && (
				<h1 className="page-title">
					{title}
					{subtitle && (
						<>
							{' '}
							<small>&nbsp;{subtitle}</small>
						</>
					)}
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
