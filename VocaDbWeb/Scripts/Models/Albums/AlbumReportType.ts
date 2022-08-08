enum AlbumReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export default AlbumReportType;

export const albumReportTypesWithRequiredNotes = [
	AlbumReportType.InvalidInfo,
	AlbumReportType.Other,
];
