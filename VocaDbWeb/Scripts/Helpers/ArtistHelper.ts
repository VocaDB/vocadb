import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';
import { ArtistRoles } from '@/Models/Artists/ArtistRoles';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { ContentFocus } from '@/Models/ContentFocus';

export class ArtistHelper {
	/// <summary>
	/// The roles of these artists can be customized
	/// </summary>
	static customizableTypes = [
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
		ArtistType.Instrumentalist,
		ArtistType.Designer,
	];

	// Artist types that are groups (excluding Unknown)
	static groupTypes = [
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
		ArtistType.NEUTRINO,
		ArtistType.VoiSona,
		ArtistType.NewType,
		ArtistType.Voiceroid,
		ArtistType.VOICEVOX,
		ArtistType.ACEVirtualSinger,
		ArtistType.AIVOICE,
	];

	private static voiceSynthesizerTypes = [
		ArtistType.Vocaloid,
		ArtistType.UTAU,
		ArtistType.CeVIO,
		ArtistType.OtherVoiceSynthesizer,
		ArtistType.SynthesizerV,
		ArtistType.NEUTRINO,
		ArtistType.VoiSona,
		ArtistType.NewType,
		ArtistType.Voiceroid,
		ArtistType.VOICEVOX,
		ArtistType.ACEVirtualSinger,
		ArtistType.AIVOICE,
	];

	static canHaveChildVoicebanks(at?: ArtistType): boolean {
		if (at == null) return false;

		return (
			(ArtistHelper.isVocalistType(at) || at === ArtistType.Unknown) &&
			at !== ArtistType.Vocalist
		);
	}

	static getRolesArray(roles: string[] | string): ArtistRoles[] {
		const stringArr = typeof roles === 'string' ? roles.split(',') : roles;
		return stringArr.map((s) => ArtistRoles[s as keyof typeof ArtistRoles]);
	}

	static getRolesList(roles: ArtistRoles | ArtistRoles[]): string {
		if (Array.isArray(roles)) {
			return roles.map((r) => ArtistRoles[r]).join(',');
		} else {
			return ArtistRoles[roles];
		}
	}

	// Whether the roles for an artist type can be customized
	static isCustomizable(at: ArtistType): boolean {
		return ArtistHelper.customizableTypes.includes(at);
	}

	// Whether roles array indicates default roles
	static isDefaultRoles(roles: ArtistRoles[]): boolean {
		return roles.length === 0 || roles[0] === ArtistRoles.Default;
	}

	// Checks whether an artist type with possible custom roles is to be considered a producer
	static isProducerRoleType(
		artistType: ArtistType,
		roles: ArtistRoles[],
		focus: ContentFocus,
	): boolean {
		// eslint-disable-next-line react-hooks/rules-of-hooks
		if (ArtistHelper.useDefaultRoles(artistType, roles)) {
			return ArtistHelper.isProducerType(artistType, focus);
		}

		let res =
			roles.includes(ArtistRoles.Arranger) ||
			roles.includes(ArtistRoles.Composer) ||
			roles.includes(ArtistRoles.VoiceManipulator);

		if (focus === ContentFocus.Video)
			res = res || roles.includes(ArtistRoles.Animator);

		if (focus === ContentFocus.Illustration)
			res = res || roles.includes(ArtistRoles.Illustrator);

		return res;
	}

	static isProducerRole(
		artist: ArtistContract,
		roles: ArtistRoles[],
		focus: ContentFocus,
	): boolean {
		return ArtistHelper.isProducerRoleType(
			artist != null ? artist.artistType : ArtistType.Unknown,
			roles,
			focus,
		);
	}

	// Whether an artist type with default roles is to be considered a producer
	static isProducerType(artistType: ArtistType, focus: ContentFocus): boolean {
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

	static isValidForPersonalDescription(
		artistLink: ArtistForAlbumContract,
	): boolean {
		if (!artistLink.artist || artistLink.isSupport) return false;

		const artistType = artistLink.artist.artistType;
		const rolesArray = ArtistHelper.getRolesArray(artistLink.roles);

		// eslint-disable-next-line react-hooks/rules-of-hooks
		if (ArtistHelper.useDefaultRoles(artistType, rolesArray)) {
			const validTypes = [
				ArtistType.Producer,
				ArtistType.Circle,
				ArtistType.Label,
				ArtistType.OtherGroup,
				ArtistType.Band,
				ArtistType.Animator,
			];
			return validTypes.includes(artistType);
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

		return validRoles.some((r) => rolesArray.includes(r));
	}

	static isVocalistRoleType(
		artistType: ArtistType,
		roles: ArtistRoles[],
	): boolean {
		// eslint-disable-next-line react-hooks/rules-of-hooks
		if (ArtistHelper.useDefaultRoles(artistType, roles)) {
			return ArtistHelper.isVocalistType(artistType);
		}

		var res =
			roles.includes(ArtistRoles.Vocalist) ||
			roles.includes(ArtistRoles.Chorus);

		return res;
	}

	static isVocalistRole(artist: ArtistContract, roles: ArtistRoles[]): boolean {
		return ArtistHelper.isVocalistRoleType(
			artist != null ? artist.artistType : ArtistType.Unknown,
			roles,
		);
	}

	static isVocalistType(artistType: ArtistType): boolean {
		return ArtistHelper.vocalistTypes.includes(artistType);
	}

	static isVoiceSynthesizerType(artistType: ArtistType): boolean {
		return ArtistHelper.voiceSynthesizerTypes.includes(artistType);
	}
	// Whether default roles should be used for an artist type and roles combination.
	// Some artist types do not allow customization. If no custom roles are specified default roles will be used.
	private static useDefaultRoles(
		artistType: ArtistType,
		roles: ArtistRoles[],
	): boolean {
		return (
			roles == null ||
			ArtistHelper.isDefaultRoles(roles) ||
			!ArtistHelper.isCustomizable(artistType)
		);
	}
}
