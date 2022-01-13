enum SongReportType {
	BrokenPV = 'BrokenPV',
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export default SongReportType;

export const reportTypesWithRequiredNotes = [
	SongReportType.InvalidInfo,
	SongReportType.Other,
];
