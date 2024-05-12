export enum PVType {
	Original = "Original",
	Reprint = "Reprint",
	Other = "Other",
}

export enum PVService {
	NicoNicoDouga = "NicoNicoDouga",
	Youtube = "Youtube",
	SoundCloud = "SoundCloud",
	Vimeo = "Vimeo",
	Piapro = "Piapro",
	Bilibili = "Bilibili",
	File = "File",
	LocalFile = "LocalFile",
	Creofuga = "Creofuga",
	Bandcamp = "Bandcamp",
}

export interface PV {
	author?: string; // TODO: Nullable?
	createdBy?: string; // TODO: Nullable?
	disabled: boolean;
	extendedMetadata?: {
		json: string;
	};
	id: number;
	length: number;
	name?: string; // TODO: Nullable?
	publishDate?: string;
	pvId?: string;
	service: PVService;
	pvType: PVType;
	url?: string;
}

