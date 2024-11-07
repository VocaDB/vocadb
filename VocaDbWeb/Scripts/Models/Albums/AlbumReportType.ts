export enum AlbumReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
	InvalidTag = 'InvalidTag',
}

export const albumReportTypesWithRequiredNotes = [
	AlbumReportType.InvalidInfo,
	AlbumReportType.Other,
	AlbumReportType.InvalidTag,
];
