import { Layout } from '@/Components/Shared/Layout';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { LoginManager } from '@/Models/LoginManager';
import React from 'react';
import { Link } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const AdminIndex = (): React.ReactElement => {
	const title = 'Site management'; /* LOC */

	useVdbTitle(title, true);

	return (
		<Layout title={title}>
			<h3>Common tasks{/* LOC */}</h3>

			<p>
				<Link to="/User">View users list{/* LOC */}</Link>
			</p>

			<p>
				<Link to="/Comment">View recent comments{/* LOC */}</Link>
			</p>

			{loginManager.canViewAuditLog && (
				<p>
					<Link to="/Admin/ViewAuditLog">View audit log{/* LOC */}</Link>
				</p>
			)}

			{loginManager.canManageTagMappings && (
				<>
					<p>
						<Link to="/Admin/ManageTagMappings">
							Manage tag mappings{/* LOC */}
						</Link>
					</p>
					<p>
						<Link to="/Admin/ManageEntryTagMappings">
							Manage entry type to tag mappings{/* LOC */}
						</Link>
					</p>
				</>
			)}

			{loginManager.canManageEntryReports && (
				<p>
					<a href="/Admin/ViewEntryReports">Manage entry reports{/* LOC */}</a>
				</p>
			)}

			{loginManager.canBulkDeletePVs && (
				<p>
					<a href="/Admin/PVsByAuthor">Delete PVs by author{/* LOC */}</a>
				</p>
			)}

			{loginManager.canMoveToTrash && (
				<p>
					<Link to="/Album/Deleted">Manage deleted albums{/* LOC */}</Link>
				</p>
			)}

			{loginManager.canManageIPRules && (
				<p>
					<Link to="/Admin/ManageIPRules">Manage IP rules{/* LOC */}</Link>
				</p>
			)}

			{loginManager.canManageWebhooks && (
				<p>
					<Link to="/Admin/ManageWebhooks">Manage webhooks{/* LOC */}</Link>
				</p>
			)}

			<br />

			{loginManager.canAdmin && (
				<>
					<h3>Database maintenance tasks{/* LOC */}</h3>
					<p>
						<a href="/Admin/ActiveEdits">View active editors{/* LOC */}</a>
					</p>
					<p>
						<a href="/Admin/CreateJsonDump">Create JSON dump{/* LOC */}</a>
					</p>
					<p>
						<a href="/Admin/RefreshDbCache">
							Refresh NHibernate 2nd level cache{/* LOC */}
						</a>
					</p>
					<p>
						<a href="/Admin/ClearCaches">
							Refresh .NET memory cache{/* LOC */}
						</a>
					</p>
				</>
			)}
		</Layout>
	);
};

export default AdminIndex;
