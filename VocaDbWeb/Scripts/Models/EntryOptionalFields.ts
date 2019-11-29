
//module vdb.models {
	
	export class EntryOptionalFields<T> {

		// TODO: better way for generic constraints?
		constructor(type: (val: T) => string, fields: T[]) {
			this.fields = _.map(fields, field => type(field)).join(",");
		}

		public fields: string;

	}


	export class SongOptionalFields extends EntryOptionalFields<SongOptionalField> {

		public static create(...fields: SongOptionalField[]) {
			return new SongOptionalFields(fields);
		}

		constructor(fields: SongOptionalField[]) {
			super(f => SongOptionalField[f], fields);
		}

	}

	export enum SongOptionalField {
		
		None = 0,
		AdditionalNames = 1,
		Albums = 2,
		Artists = 4,
		Names = 8,
		PVs = 16,
		Tags = 32,
		ThumbUrl = 64,
		WebLinks = 128

	}

//}