import Breadcrumb from '@Bootstrap/Breadcrumb';
import React from 'react';
import { Link } from 'react-router-dom';

import Layout from '../Shared/Layout';
import useVocaDbTitle from '../useVocaDbTitle';

const AdminActiveEdits = (): React.ReactElement => {
	const title = 'Active editors'; /* TODO: localize */

	useVocaDbTitle(title, true);

	return (
		<Layout
			title={title}
			parents={
				<>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: '/Admin',
						}}
					>
						Manage{/* TODO: localize */}
					</Breadcrumb.Item>
				</>
			}
		>
			<table>
				<thead>
					<tr>
						<th>Entry{/* TODO: localize */}</th>
						<th>Editor{/* TODO: localize */}</th>
						<th>Time{/* TODO: localize */}</th>
					</tr>
				</thead>
				<tbody>{/* TODO */}</tbody>
			</table>
		</Layout>
	);
};

export default AdminActiveEdits;
