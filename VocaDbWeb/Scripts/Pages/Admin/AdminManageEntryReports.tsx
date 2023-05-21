import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { EntryLink } from '@/Components/Shared/Partials/Shared/EntryLink';
import { UserIconLink_UserForApiContract } from '@/Components/Shared/Partials/User/UserIconLink_UserForApiContract';
import {
	EntryReportContract,
	ReportStatus,
} from '@/DataContracts/EntryReportContract';
import { adminRepo } from '@/Repositories/AdminRepository';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import dayjs from '@/dayjs';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useSearchParams } from 'react-router-dom';

interface AdminViewEntryReportParams {
	entryReport: EntryReportContract;
	onReportDelete: (report: EntryReportContract) => any;
	status: ReportStatus;
}

const AdminViewEntryReport = ({
	entryReport,
	onReportDelete,
	status,
}: AdminViewEntryReportParams): React.ReactElement => {
	const { t } = useTranslation(['VocaDb.Web.Resources.Domain']);

	return (
		<tr>
			<td>{dayjs(entryReport.created).format('lll')}</td>
			<td>
				{entryReport.user && (
					<>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<UserIconLink_UserForApiContract user={entryReport.user} />
					</>
				)}
				{!entryReport.user && <p>System</p>}
			</td>
			<td>
				<EntryLink entry={entryReport.entry} />
			</td>
			<td>
				{t(
					`VocaDb.Web.Resources.Domain:EntryReportTypeNames.${entryReport.reportTypeName}`,
				)}
			</td>
			<td>
				<div className="entry-report-notes">{entryReport.notes}</div>
			</td>
			{status === ReportStatus.Open && (
				<td>
					<SafeAnchor
						onClick={(): void => {
							onReportDelete(entryReport);
						}}
						className="textLink deleteLink"
					>
						Close
					</SafeAnchor>
				</td>
			)}
			{status === ReportStatus.Closed && entryReport.closedBy && (
				<>
					<td>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<UserIconLink_UserForApiContract user={entryReport.closedBy} />
					</td>
					<td>{entryReport.closedAt}</td>
				</>
			)}
		</tr>
	);
};

const AdminViewEntryReports = observer(
	(): React.ReactElement => {
		const [params, setParams] = useSearchParams();
		const status = (params.get('status') as ReportStatus) ?? ReportStatus.Open;
		const [entryReports, setEntryReports] = useState<EntryReportContract[]>([]);

		useEffect(() => {
			adminRepo.getEntryReports(status).then((resp) => setEntryReports(resp));
		}, [status]);

		const changeStatus = (status: ReportStatus): void => {
			adminRepo.getEntryReports(status).then((resp) => {
				setEntryReports(resp);
				setParams({ status });
			});
		};

		// console.log(adminRepo.getEntryReports());
		const title = 'View entry reports';
		return (
			<Layout pageTitle={title} title={title} ready={true}>
				<ul className="nav nav-pills">
					<li className={classNames(status === ReportStatus.Open && 'active')}>
						<SafeAnchor onClick={(): void => changeStatus(ReportStatus.Open)}>
							Open{/* LOC */}
						</SafeAnchor>
					</li>
					<li
						className={classNames(status === ReportStatus.Closed && 'active')}
					>
						<SafeAnchor onClick={(): void => changeStatus(ReportStatus.Closed)}>
							Closed{/* LOC */}
						</SafeAnchor>
					</li>
				</ul>
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
						{entryReports.map((entryReport, index) => (
							<AdminViewEntryReport
								key={index}
								entryReport={entryReport}
								onReportDelete={(rep): void => {
									antiforgeryRepo.getToken().then((token) => {
										adminRepo
											.deleteEntryReport(token, {
												id: rep.id,
											})
											.then(() => {
												setEntryReports(
													entryReports.filter((r) => r.id !== rep.id),
												);
											});
									});
								}}
								status={status}
							/>
						))}
					</tbody>
				</table>
			</Layout>
		);
	},
);

export default AdminViewEntryReports;
