export enum SongReportType {
	BrokenPV = 'BrokenPV',
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
	InvalidTag = 'InvalidTag',
}

export const songReportTypesWithRequiredNotes = [
	SongReportType.BrokenPV,
	SongReportType.InvalidInfo,
	SongReportType.Other,
	SongReportType.InvalidTag,
];
