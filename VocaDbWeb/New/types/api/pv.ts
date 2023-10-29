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

export enum PVType {
  Original = "Original",
  Reprint = "Reprint",
  Other = "Other",
}

export interface PVContractBase {
  author?: string
  createdBy?: number
  disabled?: boolean
  id: number
  length?: number
  name?: string
  pvId: string
  publishDate?: string
  pvType: PVType
  thumbUrl?: string
  url?: string
}

export interface PVExtendedMetadata {
  json?: string
}

export interface BandcampPVContract extends PVContractBase {
  service: PVService.Bandcamp
  extendedMetadata?: PVExtendedMetadata
}

export interface PiaproPVContract extends PVContractBase {
  service: PVService.Piapro
  extendedMetadata?: PVExtendedMetadata
}

export interface SoundCloudPVContract extends PVContractBase {
  service: PVService.SoundCloud
}

interface DefaultPVContract extends PVContractBase {
  service: Exclude<
    PVService,
    | BandcampPVContract["service"]
    | PiaproPVContract["service"]
    | SoundCloudPVContract["service"]
  >
}

export type PVContract =
  | BandcampPVContract
  | PiaproPVContract
  | SoundCloudPVContract
  | DefaultPVContract
