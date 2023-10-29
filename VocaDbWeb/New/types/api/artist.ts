import { CommonEntryContract, EntryThumbContract } from "./entry"
import { TagUsageForApiContract } from "./tag-usage"

export enum ArtistType {
  Unknown = "Unknown",
  /// <summary>
  /// Doujin circle. A group of doujin producers that also releases music (acts as a label).
  /// </summary>
  Circle = "Circle",
  /// <summary>
  /// Commercial music label. Does not produce music by itself.
  /// </summary>
  Label = "Label",
  /// <summary>
  /// Producer is the maker or the song (usually an individual, for example doriko)
  /// </summary>
  Producer = "Producer",
  Animator = "Animator",
  Illustrator = "Illustrator",
  Lyricist = "Lyricist",
  Vocaloid = "Vocaloid",
  UTAU = "UTAU",
  CeVIO = "CeVIO",
  OtherVoiceSynthesizer = "OtherVoiceSynthesizer",
  OtherVocalist = "OtherVocalist",
  OtherGroup = "OtherGroup",
  OtherIndividual = "OtherIndividual",
  Utaite = "Utaite",
  Band = "Band",
  Vocalist = "Vocalist",
  Character = "Character",
  SynthesizerV = "SynthesizerV",
  CoverArtist = "CoverArtist",
  NEUTRINO = "NEUTRINO",
  VoiSona = "VoiSona",
  NewType = "NewType",
  Voiceroid = "Voiceroid",
}

export interface ArtistContract extends CommonEntryContract {
  additionalNames?: string
  artistType: ArtistType
  mainPicture?: EntryThumbContract
  releaseDate?: string
  tags?: TagUsageForApiContract[]
}

export interface ArtistForAlbumContract {
  artist?: ArtistContract
  categories?: string /* TODO: enum */
  effectiveRoles?: string /* TODO: enum */
  id?: number
  isCustomName?: boolean
  isSupport?: boolean
  name?: string
  roles: string
}
