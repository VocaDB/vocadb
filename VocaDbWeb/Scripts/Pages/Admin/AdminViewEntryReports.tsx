import Alert from '@/Bootstrap/Alert';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { UserIconLink_UserForApiContract } from '@/Components/Shared/Partials/User/UserIconLink_UserForApiContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { adminRepo } from '@/Repositories/AdminRepository';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import classNames from 'classnames';
import React from 'react';

enum ReportStatus {
	Open = 'Open',
	Closed = 'Closed',
}

interface EntryReportContract {
	closedAt: string;
	closedBy: UserApiContract;
	created: string;
	id: number;
	notes: string;
	reportTypeName: string;
}

const AdminViewEntryReports = (): React.ReactElement => {
	const status = ReportStatus.Open as ReportStatus; /* TODO */

	const [entryReports] = React.useState<EntryReportContract[]>([]);

	const title = 'View entry reports'; /* LOC */
	return (
		<Layout pageTitle={title} ready={true} title={title}>
			{/* TODO */}

			<ul className="nav nav-pills">
				<li className={classNames(status === ReportStatus.Open && 'active')}>
					<a href="#" /* TODO */>Open{/* LOC */}</a>
				</li>
				<li className={classNames(status === ReportStatus.Closed && 'active')}>
					<a href="#" /* TODO */>Closed{/* LOC */}</a>
				</li>
			</ul>

			<Alert variant="info">
				This list contains entries that have been reported for errors.{' '}
				<b>
					The list is shared between all trusted users and moderators, and
					anyone can take action based on these reported issues.
				</b>{' '}
				If you have time, please check that the reports are valid, and either
				notify the user who created the entry in the first place, or correct the
				errors yourself. After the issue has been resolved you can delete the
				report, but not before.{/* LOC */}
			</Alert>

			<table className="table table-striped">
				<thead>
					<tr>
						<th>Time{/* LOC */}</th>
						<th>Author{/* LOC */}</th>
						<th>Entry{/* LOC */}</th>
						<th>Type{/* LOC */}</th>
						<th>Notes{/* LOC */}</th>
						{status === ReportStatus.Open && (
							<>
								<th>Actions{/* LOC */}</th>
							</>
						)}
						{status === ReportStatus.Closed && (
							<>
								<th>Closed by{/* LOC */}</th>
								<th>Closed at{/* LOC */}</th>
							</>
						)}
					</tr>
				</thead>
				<tbody>
					{entryReports.map((entryReport) => (
						<tr key={entryReport.id}>
							<td>{entryReport.created}</td>
							{/* TODO */}
							<td>{entryReport.reportTypeName}</td>
							<td>
								<div className="entry-report-notes">{entryReport.notes}</div>
							</td>
							{status === ReportStatus.Open && (
								<>
									<td>
										<SafeAnchor
											onClick={async (): Promise<void> => {
												const requestToken = await antiforgeryRepo.getToken();

												await adminRepo.deleteEntryReport(requestToken, {
													id: entryReport.id,
												});

												// TODO
											}}
											className="textLink deleteLink"
										>
											Close{/* LOC */}
										</SafeAnchor>
									</td>
								</>
							)}
							{status === ReportStatus.Closed && (
								<>
									<td>
										{/* eslint-disable-next-line react/jsx-pascal-case */}
										<UserIconLink_UserForApiContract
											user={entryReport.closedBy}
										/>
									</td>
									<td>{entryReport.closedAt}</td>
								</>
							)}
						</tr>
					))}
				</tbody>
			</table>
		</Layout>
	);
};

export default AdminViewEntryReports;
