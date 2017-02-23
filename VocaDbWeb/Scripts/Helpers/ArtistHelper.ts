 
module vdb.helpers {

	import dc = vdb.dataContracts;
	import cls = vdb.models;
	var ArtistType = cls.artists.ArtistType;

	export class ArtistHelper {
		
		/// <summary>
		/// The roles of these artists can be customized
		/// </summary>
		public static customizableTypes = [
			ArtistType.Animator, ArtistType.OtherGroup, ArtistType.OtherIndividual, 
			ArtistType.OtherVocalist, ArtistType.Producer, ArtistType.Illustrator, ArtistType.Lyricist, 
			ArtistType.Utaite, ArtistType.Band, ArtistType.Unknown
		];

		// Artist types that are groups (excluding Unknown)
		public static groupTypes = [
			ArtistType.Band, ArtistType.Circle,
			ArtistType.Label, ArtistType.OtherGroup
		];

		private static vocalistTypes = [
			ArtistType.OtherVocalist, ArtistType.OtherVoiceSynthesizer, ArtistType.Utaite,
			ArtistType.UTAU, ArtistType.Vocaloid, ArtistType.CeVIO
		];

		public static canHaveChildVoicebanks(at: cls.artists.ArtistType) {

			if (at === null)
				return false;

			return ArtistHelper.isVocalistType(at) || at === ArtistType.Unknown;

		}

		private static getRolesArray(roles: string[] | string): cls.artists.ArtistRoles[] {
			const stringArr = typeof roles === 'string' ? roles.split(',') : roles;
			return _.map(stringArr, s => cls.artists.ArtistRoles[s]);
		}

		// Whether the roles for an artist type can be customized
		public static isCustomizable(at: cls.artists.ArtistType) {

			return _.includes(ArtistHelper.customizableTypes, at);

		}

		// Checks whether an artist type with possible custom roles is to be considered a producer
		static isProducerRoleType(artistType: cls.artists.ArtistType, roles: string[] | string, isAnimation: boolean) {

			const rolesArray = ArtistHelper.getRolesArray(roles);

			if (ArtistHelper.useDefaultRoles(artistType, rolesArray)) {
				return ArtistHelper.isProducerType(artistType, isAnimation);
			}

			let res =
				_.includes(rolesArray, cls.artists.ArtistRoles.Arranger) ||
				_.includes(rolesArray, cls.artists.ArtistRoles.Composer) ||
				_.includes(rolesArray, cls.artists.ArtistRoles.VoiceManipulator);

			if (isAnimation)
				res = res || _.includes(rolesArray, cls.artists.ArtistRoles.Animator);

			return res;

		}

		static isProducerRole(artist: dc.ArtistContract, roles: string[], isAnimation: boolean) {

			return ArtistHelper.isProducerRoleType(artist != null ? cls.artists.ArtistType[artist.artistType] : ArtistType.Unknown, roles, isAnimation);

		}

		// Whether an artist type with default roles is to be considered a producer
		static isProducerType(artistType: cls.artists.ArtistType, isAnimation: boolean) {

			return (artistType === ArtistType.Producer
				|| artistType === ArtistType.Circle
				|| artistType === ArtistType.Band
				|| (artistType === ArtistType.Animator && isAnimation));

		}

		static isValidForPersonalDescription(artistLink: dc.ArtistForAlbumContract) {

			if (!artistLink.artist || artistLink.isSupport)
				return false;

			const artistType = cls.artists.ArtistType[artistLink.artist.artistType];
			const rolesArray = ArtistHelper.getRolesArray(artistLink.roles);

			if (ArtistHelper.useDefaultRoles(artistType, rolesArray)) {
				return ArtistHelper.isProducerType(artistType, true);
			}

			const validRoles = [
				cls.artists.ArtistRoles.Animator, cls.artists.ArtistRoles.Arranger, cls.artists.ArtistRoles.Composer,
				cls.artists.ArtistRoles.Distributor, cls.artists.ArtistRoles.Mastering, cls.artists.ArtistRoles.Other,
				cls.artists.ArtistRoles.Publisher, cls.artists.ArtistRoles.VoiceManipulator
			];

			return _.some(validRoles, r => _.includes(rolesArray, r));

		}

		static isVocalistRoleType(artistType: cls.artists.ArtistType, roles: string[]) {

			const rolesArray = ArtistHelper.getRolesArray(roles);

			if (ArtistHelper.useDefaultRoles(artistType, rolesArray)) {
				return ArtistHelper.isVocalistType(artistType);
			}

			var res =
				_.includes(rolesArray, cls.artists.ArtistRoles.Vocalist) ||
				_.includes(rolesArray, cls.artists.ArtistRoles.Chorus);

			return res;

		}

		static isVocalistRole(artist: dc.ArtistContract, roles: string[]) {

			return ArtistHelper.isVocalistRoleType(artist != null ? cls.artists.ArtistType[artist.artistType] : ArtistType.Unknown, roles);

		}

		static isVocalistType(artistType: cls.artists.ArtistType) {

			return _.includes(ArtistHelper.vocalistTypes, artistType);

		}

		// Whether default roles should be used for an artist type and roles combination.
		// Some artist types do not allow customization. If no custom roles are specified default roles will be used.
		private static useDefaultRoles(artistType: cls.artists.ArtistType, roles: cls.artists.ArtistRoles[]) {

			return roles == null || roles.length === 0 || roles[0] === cls.artists.ArtistRoles.Default || !ArtistHelper.isCustomizable(artistType);

		}

	}

}