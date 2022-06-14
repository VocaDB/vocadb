export class EntryOptionalFields<T> {
	// TODO: better way for generic constraints?
	public constructor(type: (val: T) => string, fields: T[]) {
		this.fields = fields.map((field) => type(field)).join(',');
	}

	public fields: string;
}

export enum SongOptionalField {
	None = 0,
	AdditionalNames = 1 << 0,
	Albums = 1 << 1,
	Artists = 1 << 2,
	Names = 1 << 3,
	PVs = 1 << 4,
	Tags = 1 << 5,
	ThumbUrl = 1 << 6,
	WebLinks = 1 << 7,
	MainPicture = 1 << 8,
}

export class SongOptionalFields extends EntryOptionalFields<SongOptionalField> {
	public static create(...fields: SongOptionalField[]): SongOptionalFields {
		return new SongOptionalFields(fields);
	}

	public constructor(fields: SongOptionalField[]) {
		super((f) => SongOptionalField[f], fields);
	}
}
