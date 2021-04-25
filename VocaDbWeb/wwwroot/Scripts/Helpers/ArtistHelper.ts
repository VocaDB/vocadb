import ArtistContract from '../DataContracts/Artist/ArtistContract';
import ArtistForAlbumContract from '../DataContracts/ArtistForAlbumContract';
import ArtistRoles from '../Models/Artists/ArtistRoles';
import ArtistType from '../Models/Artists/ArtistType';
import ContentFocus from '../Models/ContentFocus';

export default class ArtistHelper {
  /// <summary>
  /// The roles of these artists can be customized
  /// </summary>
  public static customizableTypes = [
    ArtistType.Animator,
    ArtistType.OtherGroup,
    ArtistType.OtherIndividual,
    ArtistType.OtherVocalist,
    ArtistType.Producer,
    ArtistType.Illustrator,
    ArtistType.Lyricist,
    ArtistType.Utaite,
    ArtistType.Band,
    ArtistType.Vocalist,
    ArtistType.Unknown,
    ArtistType.CoverArtist,
  ];

  // Artist types that are groups (excluding Unknown)
  public static groupTypes = [
    ArtistType.Band,
    ArtistType.Circle,
    ArtistType.Label,
    ArtistType.OtherGroup,
  ];

  private static vocalistTypes = [
    ArtistType.OtherVocalist,
    ArtistType.OtherVoiceSynthesizer,
    ArtistType.Utaite,
    ArtistType.UTAU,
    ArtistType.Vocaloid,
    ArtistType.CeVIO,
    ArtistType.Vocalist,
    ArtistType.SynthesizerV,
  ];

  public static canHaveChildVoicebanks(at: ArtistType) {
    if (at === null) return false;

    return (
      (ArtistHelper.isVocalistType(at) || at === ArtistType.Unknown) &&
      at !== ArtistType.Vocalist
    );
  }

  public static getRolesArray(roles: string[] | string): ArtistRoles[] {
    const stringArr = typeof roles === 'string' ? roles.split(',') : roles;
    return _.map(stringArr, (s) => ArtistRoles[s]);
  }

  public static getRolesList(roles: ArtistRoles | ArtistRoles[]): string {
    if (Array.isArray(roles)) {
      return _.map(roles, (r) => ArtistRoles[r]).join(',');
    } else {
      return ArtistRoles[roles];
    }
  }

  // Whether the roles for an artist type can be customized
  public static isCustomizable(at: ArtistType | string) {
    if (typeof at === 'string') {
      at = ArtistType[at];
    }

    return _.includes(ArtistHelper.customizableTypes, at);
  }

  // Whether roles array indicates default roles
  public static isDefaultRoles(roles: ArtistRoles[]) {
    return roles.length === 0 || roles[0] === ArtistRoles.Default;
  }

  // Checks whether an artist type with possible custom roles is to be considered a producer
  static isProducerRoleType(
    artistType: ArtistType,
    roles: ArtistRoles[],
    focus: ContentFocus,
  ) {
    if (ArtistHelper.useDefaultRoles(artistType, roles)) {
      return ArtistHelper.isProducerType(artistType, focus);
    }

    let res =
      _.includes(roles, ArtistRoles.Arranger) ||
      _.includes(roles, ArtistRoles.Composer) ||
      _.includes(roles, ArtistRoles.VoiceManipulator);

    if (focus === ContentFocus.Video)
      res = res || _.includes(roles, ArtistRoles.Animator);

    if (focus === ContentFocus.Illustration)
      res = res || _.includes(roles, ArtistRoles.Illustrator);

    return res;
  }

  static isProducerRole(
    artist: ArtistContract,
    roles: ArtistRoles[],
    focus: ContentFocus,
  ) {
    return ArtistHelper.isProducerRoleType(
      artist != null ? ArtistType[artist.artistType] : ArtistType.Unknown,
      roles,
      focus,
    );
  }

  // Whether an artist type with default roles is to be considered a producer
  static isProducerType(artistType: ArtistType | string, focus: ContentFocus) {
    if (typeof artistType === 'string') {
      artistType = ArtistType[artistType];
    }

    return (
      artistType === ArtistType.Producer ||
      artistType === ArtistType.Circle ||
      artistType === ArtistType.Band ||
      artistType === ArtistType.CoverArtist ||
      (artistType === ArtistType.Animator && focus === ContentFocus.Video) ||
      (artistType === ArtistType.Illustrator &&
        focus === ContentFocus.Illustration)
    );
  }

  static isValidForPersonalDescription(artistLink: ArtistForAlbumContract) {
    if (!artistLink.artist || artistLink.isSupport) return false;

    const artistType = ArtistType[artistLink.artist.artistType];
    const rolesArray = ArtistHelper.getRolesArray(artistLink.roles);

    if (ArtistHelper.useDefaultRoles(artistType, rolesArray)) {
      const validTypes = [
        ArtistType.Producer,
        ArtistType.Circle,
        ArtistType.Label,
        ArtistType.OtherGroup,
        ArtistType.Band,
        ArtistType.Animator,
      ];
      return _.includes(validTypes, artistType);
    }

    const validRoles = [
      ArtistRoles.Animator,
      ArtistRoles.Arranger,
      ArtistRoles.Composer,
      ArtistRoles.Distributor,
      ArtistRoles.Mastering,
      ArtistRoles.Other,
      ArtistRoles.Publisher,
      ArtistRoles.VoiceManipulator,
    ];

    return _.some(validRoles, (r) => _.includes(rolesArray, r));
  }

  static isVocalistRoleType(artistType: ArtistType, roles: ArtistRoles[]) {
    if (ArtistHelper.useDefaultRoles(artistType, roles)) {
      return ArtistHelper.isVocalistType(artistType);
    }

    var res =
      _.includes(roles, ArtistRoles.Vocalist) ||
      _.includes(roles, ArtistRoles.Chorus);

    return res;
  }

  static isVocalistRole(artist: ArtistContract, roles: ArtistRoles[]) {
    return ArtistHelper.isVocalistRoleType(
      artist != null ? ArtistType[artist.artistType] : ArtistType.Unknown,
      roles,
    );
  }

  static isVocalistType(artistType: ArtistType) {
    return _.includes(ArtistHelper.vocalistTypes, artistType);
  }

  // Whether default roles should be used for an artist type and roles combination.
  // Some artist types do not allow customization. If no custom roles are specified default roles will be used.
  private static useDefaultRoles(artistType: ArtistType, roles: ArtistRoles[]) {
    return (
      roles == null ||
      ArtistHelper.isDefaultRoles(roles) ||
      !ArtistHelper.isCustomizable(artistType)
    );
  }
}
