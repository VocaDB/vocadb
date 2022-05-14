import Breadcrumb from '@Bootstrap/Breadcrumb';
import Button from '@Bootstrap/Button';
import React from 'react';
import { Link } from 'react-router-dom';

import Layout from '../Shared/Layout';
import useVocaDbTitle from '../useVocaDbTitle';

const AdminPVsByAuthor = (): React.ReactElement => {
	const title = 'PVs by author'; /* TODO: localize */

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
			<form
				/* TODO */
				method="post"
				className="form form-inline"
				/* TODO */
			>
				{/* TODO */}

				<Button type="submit" variant="primary">
					Apply{/* TODO: localize */}
				</Button>
			</form>
		</Layout>
	);
};

export default AdminPVsByAuthor;
