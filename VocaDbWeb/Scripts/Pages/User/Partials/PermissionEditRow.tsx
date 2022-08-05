import { PermissionTokenName } from '@/Pages/User/Partials/PermissionTokenName';
import { PermissionEditStore } from '@/Stores/User/UserEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';

interface PermissionEditRowProps {
	permissionEditStore: PermissionEditStore;
}

export const PermissionEditRow = observer(
	({ permissionEditStore }: PermissionEditRowProps): React.ReactElement => {
		return (
			<p>
				<label className="checkbox">
					<input
						type="checkbox"
						checked={permissionEditStore.hasFlag}
						onChange={(e): void =>
							runInAction(() => {
								permissionEditStore.hasFlag = e.target.checked;
							})
						}
					/>
					<PermissionTokenName token={permissionEditStore.permissionToken} />
					{permissionEditStore.hasPermission && (
						<>
							{' '}
							<span>(already granted){/* TODO: localize */}</span>
						</>
					)}
				</label>
			</p>
		);
	},
);
