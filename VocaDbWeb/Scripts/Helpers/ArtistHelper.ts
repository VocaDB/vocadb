 
module vdb.helpers {

	import dc = vdb.dataContracts;
	import cls = vdb.models;
	var ArtistType = cls.artists.ArtistType;
	var ArtistRoles = cls.artists.ArtistRoles;

	export class ArtistHelper {
		
		/// <summary>
		/// The roles of these artists can be customized
		/// </summary>
		public static customizableTypes = [
			ArtistType.Animator, ArtistType.OtherGroup, ArtistType.OtherIndividual, 
			ArtistType.OtherVocalist, ArtistType.Producer, ArtistType.Illustrator, ArtistType.Lyricist, 
			ArtistType.Utaite, ArtistType.Band, ArtistType.Unknown
		];

		private static vocalistTypes = [
			ArtistType.OtherVocalist, ArtistType.OtherVoiceSynthesizer, ArtistType.Utaite,
			ArtistType.UTAU, ArtistType.Vocaloid, ArtistType.CeVIO
		];

		public static canHaveChildVoicebanks(at: cls.artists.ArtistType) {

			if (at === null)
				return false;

			return ArtistHelper.isVocalistType(at) || at == ArtistType.Unknown;

		}

		// Whether the roles for an artist type can be customized
		public static isCustomizable(at: cls.artists.ArtistType) {

			return _.contains(ArtistHelper.customizableTypes, at);

		}

		// Checks whether an artist type with possible custom roles is to be considered a producer
		static isProducerRoleType(artistType: cls.artists.ArtistType, roles: string[], isAnimation: boolean) {
			
			if (ArtistHelper.useDefaultRoles(artistType, roles)) {
				return ArtistHelper.isProducerType(artistType, isAnimation);
			}

			var res =
				_.contains(roles, cls.artists.ArtistRoles[ArtistRoles.Arranger]) ||
				_.contains(roles, cls.artists.ArtistRoles[ArtistRoles.Composer]) ||
				_.contains(roles, cls.artists.ArtistRoles[ArtistRoles.VoiceManipulator]);

			if (isAnimation)
				res = res || _.contains(roles, cls.artists.ArtistRoles[ArtistRoles.Animator]);

			return res;

		}

		static isProducerRole(artist: dc.ArtistContract, roles: string[], isAnimation: boolean) {

			return ArtistHelper.isProducerRoleType(artist != null ? cls.artists.ArtistType[artist.artistType] : ArtistType.Unknown, roles, isAnimation);

		}

		// Whether an artist type with default roles is to be considered a producer
		static isProducerType(artistType: cls.artists.ArtistType, isAnimation: boolean) {

			return (artistType == ArtistType.Producer
				|| artistType == ArtistType.Circle
				|| artistType == ArtistType.Band
				|| (artistType == ArtistType.Animator && isAnimation));

		}

		static isVocalistRoleType(artistType: cls.artists.ArtistType, roles: string[]) {

			if (ArtistHelper.useDefaultRoles(artistType, roles)) {
				return ArtistHelper.isVocalistType(artistType);
			}

			var res =
				_.contains(roles, cls.artists.ArtistRoles[ArtistRoles.Vocalist]) ||
				_.contains(roles, cls.artists.ArtistRoles[ArtistRoles.Chorus]);

			return res;

		}

		static isVocalistRole(artist: dc.ArtistContract, roles: string[]) {

			return ArtistHelper.isVocalistRoleType(artist != null ? cls.artists.ArtistType[artist.artistType] : ArtistType.Unknown, roles);

		}

		static isVocalistType(artistType: cls.artists.ArtistType) {

			return _.contains(ArtistHelper.vocalistTypes, artistType);

		}

		// Whether default roles should be used for an artist type and roles combination.
		// Some artist types do not allow customization. If no custom roles are specified default roles will be used.
		static useDefaultRoles(artistType: cls.artists.ArtistType, roles: string[]) {

			return roles == null || roles.length == 0 || roles[0] == cls.artists.ArtistRoles[ArtistRoles.Default] || !ArtistHelper.isCustomizable(artistType);

		}

	}

}