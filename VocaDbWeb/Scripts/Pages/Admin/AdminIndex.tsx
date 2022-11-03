import { Layout } from '@/Components/Shared/Layout';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { LoginManager } from '@/Models/LoginManager';
import React from 'react';
import { Link } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const AdminIndex = (): React.ReactElement => {
	const title = 'Site management'; /* LOCALIZE */

	useVdbTitle(title, true);

	return (
		<Layout title={title}>
			<h3>Common tasks{/* LOCALIZE */}</h3>

			<p>
				<Link to="/User">View users list{/* LOCALIZE */}</Link>
			</p>

			<p>
				<Link to="/Comment">View recent comments{/* LOCALIZE */}</Link>
			</p>

			{loginManager.canViewAuditLog && (
				<p>
					<Link to="/Admin/ViewAuditLog">View audit log{/* LOCALIZE */}</Link>
				</p>
			)}

			{loginManager.canManageTagMappings && (
				<>
					<p>
						<Link to="/Admin/ManageTagMappings">
							Manage tag mappings{/* LOCALIZE */}
						</Link>
					</p>
					<p>
						<Link to="/Admin/ManageEntryTagMappings">
							Manage entry type to tag mappings{/* LOCALIZE */}
						</Link>
					</p>
				</>
			)}

			{loginManager.canManageEntryReports && (
				<p>
					<a href="/Admin/ViewEntryReports">
						Manage entry reports{/* LOCALIZE */}
					</a>
				</p>
			)}

			{loginManager.canBulkDeletePVs && (
				<p>
					<a href="/Admin/PVsByAuthor">Delete PVs by author{/* LOCALIZE */}</a>
				</p>
			)}

			{loginManager.canMoveToTrash && (
				<p>
					<Link to="/Album/Deleted">Manage deleted albums{/* LOCALIZE */}</Link>
				</p>
			)}

			{loginManager.canManageIPRules && (
				<p>
					<Link to="/Admin/ManageIPRules">Manage IP rules{/* LOCALIZE */}</Link>
				</p>
			)}

			{loginManager.canManageWebhooks && (
				<p>
					<Link to="/Admin/ManageWebhooks">
						Manage webhooks{/* LOCALIZE */}
					</Link>
				</p>
			)}

			<br />

			{loginManager.canAdmin && (
				<>
					<h3>Database maintenance tasks{/* LOCALIZE */}</h3>
					<p>
						<a href="/Admin/ActiveEdits">View active editors{/* LOCALIZE */}</a>
					</p>
					<p>
						<a href="/Admin/CreateJsonDump">Create JSON dump{/* LOCALIZE */}</a>
					</p>
					<p>
						<a href="/Admin/RefreshDbCache">
							Refresh NHibernate 2nd level cache{/* LOCALIZE */}
						</a>
					</p>
					<p>
						<a href="/Admin/ClearCaches">
							Refresh .NET memory cache{/* LOCALIZE */}
						</a>
					</p>
				</>
			)}
		</Layout>
	);
};

export default AdminIndex;
