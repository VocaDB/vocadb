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
		const { t } = useTranslation(['Resources', 'ViewRes']);

		const ordered = React.useMemo(
			() =>
				_.chain(archivedVersions)
					.orderBy((v) => v.version, 'desc')
					.value(),
			[archivedVersions],
		);

		const changedFieldNames = useChangedFieldNames();

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
						<tr key={archivedVersion.id}>
							<td>
								{linkFunc &&
								(loginManager.canViewHiddenRevisions ||
									!archivedVersion.hidden) ? (
									<a
										href={linkFunc(archivedVersion.id)}
										className={classNames(
											archivedVersion.changedFields.length === 0 && 'muted',
										)}
										style={{
											textDecoration: archivedVersion.hidden
												? 'line-through'
												: '',
										}}
									>
										{archivedVersion.version} (
										{t(`Resources:EntryStatusNames.${archivedVersion.status}`)})
									</a>
								) : (
									<span
										style={{
											textDecoration: archivedVersion.hidden
												? 'line-through'
												: '',
										}}
									>
										{archivedVersion.version} (
										{t(`Resources:EntryStatusNames.${archivedVersion.status}`)})
									</span>
								)}
							</td>
							<td>
								<span
									className={classNames(
										archivedVersion.changedFields.length === 0 && 'muted',
									)}
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
									className={classNames(
										archivedVersion.changedFields.length === 0 && 'muted',
									)}
								>
									{t(
										`Resources:EntryEditEventNames.${archivedVersion.editEvent}`,
									)}{' '}
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
									className={classNames(
										archivedVersion.changedFields.length === 0 && 'muted',
									)}
								>
									{archivedVersion.notes}
								</span>
							</td>
						</tr>
					))}
				</tbody>
			</table>
		);
	},
);

export default ArchivedObjectVersions;
