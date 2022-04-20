import ArchivedVersionContract from '@DataContracts/Versioning/ArchivedVersionContract';
import EntryType from '@Models/EntryType';
import LoginManager from '@Models/LoginManager';
import classNames from 'classnames';
import _ from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';

import useChangedFieldNames from '../../../useChangedFieldNames';
import UniversalTimeLabel from '../Shared/UniversalTimeLabel';
import UserIconLinkOrName_UserForApiContract from '../User/UserIconLinkOrName_UserForApiContract';

const loginManager = new LoginManager(vdb.values);

interface ArchivedObjectVersionRowProps {
	archivedVersion: ArchivedVersionContract;
	linkFunc?: (id: number) => string;
	entryType: EntryType;
}

const ArchivedObjectVersionRow = React.memo(
	({
		archivedVersion,
		linkFunc,
		entryType,
	}: ArchivedObjectVersionRowProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		const changedFieldNames = useChangedFieldNames();

		const anythingChanged = archivedVersion.changedFields.length > 0;

		return (
			<tr>
				<td>
					{linkFunc &&
					(loginManager.canViewHiddenRevisions || !archivedVersion.hidden) ? (
						<a
							href={linkFunc(archivedVersion.id)}
							className={classNames(!anythingChanged && 'muted')}
							style={{
								textDecoration: archivedVersion.hidden ? 'line-through' : '',
							}}
						>
							{archivedVersion.version} (
							{t(`Resources:EntryStatusNames.${archivedVersion.status}`)})
						</a>
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
					<span className={classNames(!anythingChanged && 'muted')}>
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
					<span className={classNames(!anythingChanged && 'muted')}>
						{t(`Resources:EntryEditEventNames.${archivedVersion.editEvent}`)}{' '}
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
					<span className={classNames(!anythingChanged && 'muted')}>
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

const ArchivedObjectVersions = React.memo(
	({
		archivedVersions,
		linkFunc,
		entryType,
	}: ArchivedObjectVersionsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const ordered = React.useMemo(
			() =>
				_.chain(archivedVersions)
					.orderBy((v) => v.version, 'desc')
					.value(),
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

export default ArchivedObjectVersions;
