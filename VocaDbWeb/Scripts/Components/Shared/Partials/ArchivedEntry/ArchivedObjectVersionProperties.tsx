import { UniversalTimeLabel } from '@/Components/Shared/Partials/Shared/UniversalTimeLabel';
import { UserIconLinkOrName_UserForApiContract } from '@/Components/Shared/Partials/User/UserIconLinkOrName_UserForApiContract';
import { useChangedFieldNames } from '@/Components/useChangedFieldNames';
import { useReasonNames } from '@/Components/useReasonNames';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { EntryType } from '@/Models/EntryType';
import React from 'react';

interface ArchivedObjectVersionPropertiesProps {
	version: ArchivedVersionContract;
	compareTo?: ArchivedVersionContract;
	entryType: EntryType;
}

export const ArchivedObjectVersionProperties = React.memo(
	({
		version,
		compareTo,
		entryType,
	}: ArchivedObjectVersionPropertiesProps): React.ReactElement => {
		const reasonNames = useReasonNames();
		const changedFieldNames = useChangedFieldNames();

		return (
			<div className="well well-transparent">
				<h4>Properties{/* LOCALIZE */}</h4>

				<table className="table-bordered">
					<tbody>
						<tr>
							<td>Version{/* LOCALIZE */}</td>
							<td>{version.version}</td>
							<td>{compareTo?.version}</td>
						</tr>
						<tr>
							<td>Status{/* LOCALIZE */}</td>
							<td>{version.status}</td>
							<td>{compareTo?.status}</td>
						</tr>
						<tr>
							<td>Snapshot{/* LOCALIZE */}</td>
							<td>{version.isSnapshot ? 'True' : 'False'}</td>
							<td>{compareTo && (compareTo.isSnapshot ? 'True' : 'False')}</td>
						</tr>
						<tr>
							<td>Created{/* LOCALIZE */}</td>
							<td>
								<UniversalTimeLabel dateTime={version.created} />
							</td>
							<td>
								{compareTo && (
									<UniversalTimeLabel dateTime={compareTo.created} />
								)}
							</td>
						</tr>
						<tr>
							<td>Author{/* LOCALIZE */}</td>
							<td>
								{/* eslint-disable-next-line react/jsx-pascal-case */}
								<UserIconLinkOrName_UserForApiContract
									user={version.author}
									name={version.agentName}
								/>
							</td>
							<td>
								{compareTo && (
									// eslint-disable-next-line react/jsx-pascal-case
									<UserIconLinkOrName_UserForApiContract
										user={compareTo.author}
										name={compareTo.agentName}
									/>
								)}
							</td>
						</tr>
						<tr>
							<td>Reason{/* LOCALIZE */}</td>
							<td>{reasonNames(entryType, version)}</td>
							<td>{compareTo && reasonNames(entryType, compareTo)}</td>
						</tr>
						<tr>
							<td>Changed{/* LOCALIZE */}</td>
							<td>
								{version.changedFields.length > 0 && (
									<>
										{version.changedFields
											.map((changedField) =>
												changedFieldNames(entryType, changedField),
											)
											.join(', ')}
									</>
								)}
							</td>
							<td>
								{compareTo && compareTo.changedFields.length > 0 && (
									<>
										{compareTo.changedFields
											.map((changedField) =>
												changedFieldNames(entryType, changedField),
											)
											.join(', ')}
									</>
								)}
							</td>
						</tr>
						<tr>
							<td>Notes{/* LOCALIZE */}</td>
							<td>{version.notes}</td>
							<td>{compareTo?.notes}</td>
						</tr>
					</tbody>
				</table>
			</div>
		);
	},
);
