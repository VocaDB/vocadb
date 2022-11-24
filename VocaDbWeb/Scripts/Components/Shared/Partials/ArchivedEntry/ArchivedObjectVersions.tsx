import { UniversalTimeLabel } from '@/Components/Shared/Partials/Shared/UniversalTimeLabel';
import { UserIconLinkOrName_UserForApiContract } from '@/Components/Shared/Partials/User/UserIconLinkOrName_UserForApiContract';
import { useChangedFieldNames } from '@/Components/useChangedFieldNames';
import { useReasonNames } from '@/Components/useReasonNames';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import { useMutedUsers } from '@/MutedUsersContext';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface ArchivedObjectVersionRowProps {
	archivedVersion: ArchivedVersionContract;
	linkFunc?: (id: number) => string;
	entryType: EntryType;
}

const ArchivedObjectVersionRow = observer(
	({
		archivedVersion,
		linkFunc,
		entryType,
	}: ArchivedObjectVersionRowProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation(['Resources']);

		const reasonNames = useReasonNames();
		const changedFieldNames = useChangedFieldNames();

		const mutedUsers = useMutedUsers();
		if (
			archivedVersion.author &&
			mutedUsers.includes(archivedVersion.author.id)
		) {
			return <></>;
		}

		return (
			<tr>
				<td>
					{linkFunc &&
					(loginManager.canViewHiddenRevisions || !archivedVersion.hidden) ? (
						<Link
							to={linkFunc(archivedVersion.id)}
							className={classNames(
								!archivedVersion.anythingChanged && 'muted',
							)}
							style={{
								textDecoration: archivedVersion.hidden ? 'line-through' : '',
							}}
						>
							{archivedVersion.version} (
							{t(`Resources:EntryStatusNames.${archivedVersion.status}`)})
						</Link>
					) : (
						<span
							style={{
								textDecoration: archivedVersion.hidden ? 'line-through' : '',
							}}
						>
							{archivedVersion.version} (
							{t(`Resources:EntryStatusNames.${archivedVersion.status}`)})
						</span>
					)}
				</td>
				<td>
					<span
						className={classNames(!archivedVersion.anythingChanged && 'muted')}
					>
						<UniversalTimeLabel dateTime={archivedVersion.created} />
					</span>
				</td>
				<td>
					{/* eslint-disable-next-line react/jsx-pascal-case */}
					<UserIconLinkOrName_UserForApiContract
						user={archivedVersion.author}
						name={archivedVersion.agentName}
					/>
				</td>
				<td>
					<span
						className={classNames(!archivedVersion.anythingChanged && 'muted')}
					>
						{reasonNames(entryType, archivedVersion)}{' '}
						{archivedVersion.changedFields.length > 0 && (
							<>
								{' '}
								(
								{archivedVersion.changedFields
									.map((changedField) =>
										changedFieldNames(entryType, changedField),
									)
									.join(', ')}
								)
							</>
						)}
					</span>
				</td>
				<td>
					<span
						className={classNames(!archivedVersion.anythingChanged && 'muted')}
					>
						{archivedVersion.notes}
					</span>
				</td>
			</tr>
		);
	},
);

interface ArchivedObjectVersionsProps {
	archivedVersions: ArchivedVersionContract[];
	linkFunc?: (id: number) => string;
	entryType: EntryType;
}

export const ArchivedObjectVersions = React.memo(
	({
		archivedVersions,
		linkFunc,
		entryType,
	}: ArchivedObjectVersionsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const ordered = React.useMemo(
			() => archivedVersions.orderBy((v) => v.version, 'desc'),
			[archivedVersions],
		);

		return (
			<table className="table table-striped table-hover">
				<thead>
					<tr>
						<th>{t('ViewRes:ArchivedObjectVersions.Version')}</th>
						<th>{t('ViewRes:ArchivedObjectVersions.Created')}</th>
						<th>{t('ViewRes:ArchivedObjectVersions.Author')}</th>
						<th>{t('ViewRes:ArchivedObjectVersions.Changes')}</th>
						<th>{t('ViewRes:ArchivedObjectVersions.Notes')}</th>
					</tr>
				</thead>
				<tbody>
					{ordered.map((archivedVersion) => (
						<ArchivedObjectVersionRow
							archivedVersion={archivedVersion}
							linkFunc={linkFunc}
							entryType={entryType}
							key={archivedVersion.id}
						/>
					))}
				</tbody>
			</table>
		);
	},
);
