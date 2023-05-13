import { Layout } from '@/Components/Shared/Layout';
import { EntryLink } from '@/Components/Shared/Partials/Shared/EntryLink';
import { UserLink } from '@/Components/Shared/Partials/User/UserLink';
import { ActiveEditorContract } from '@/DataContracts/ActiveEditorContract';
import { httpClient } from '@/Shared/HttpClient';
import { useEffect, useState } from 'react';

const AdminActiveEdits = (): React.ReactElement => {
	const title = 'Active Editors'; /* LOC */
	const [activeEdits, setActiveEdits] = useState<
		ActiveEditorContract[] | undefined
	>(undefined);

	useEffect(() => {
		httpClient
			.get<ActiveEditorContract[]>('/api/admin/activeEditors')
			.then((resp) => setActiveEdits(resp));
	}, []);

	return (
		<Layout title={title} pageTitle={title} ready>
			<table>
				<thead>
					<tr>
						<th>Entry</th>
						<th>Editor</th>
						<th>Time</th>
					</tr>
				</thead>
				<tbody>
					{activeEdits?.map((edit, index) => (
						<tr key={index}>
							<td>
								<EntryLink entry={edit.entry} />
							</td>
							<td>
								<UserLink user={edit.user} />
							</td>
							<td>
								<p>{edit.time}</p>
							</td>
						</tr>
					))}
				</tbody>
			</table>
		</Layout>
	);
};

export default AdminActiveEdits;
