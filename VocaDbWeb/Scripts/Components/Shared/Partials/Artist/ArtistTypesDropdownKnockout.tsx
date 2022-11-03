import { ArtistType } from '@/Models/Artists/ArtistType';
import React from 'react';
import { useTranslation } from 'react-i18next';

enum ArtistTypeGroup {
	Nothing = 'Nothing',
	Vocalist = 'Vocalist',
	Producer = 'Producer',
	Group = 'Group',
	Other = 'Other',
}

const groupsForTypes: Record<ArtistType, ArtistTypeGroup> = {
	[ArtistType.Animator]: ArtistTypeGroup.Producer,
	[ArtistType.Character]: ArtistTypeGroup.Other,
	[ArtistType.Circle]: ArtistTypeGroup.Group,
	[ArtistType.Band]: ArtistTypeGroup.Group,
	[ArtistType.Illustrator]: ArtistTypeGroup.Producer,
	[ArtistType.Label]: ArtistTypeGroup.Group,
	[ArtistType.Lyricist]: ArtistTypeGroup.Other,
	[ArtistType.OtherGroup]: ArtistTypeGroup.Group,
	[ArtistType.OtherIndividual]: ArtistTypeGroup.Other,
	[ArtistType.OtherVocalist]: ArtistTypeGroup.Vocalist,
	[ArtistType.OtherVoiceSynthesizer]: ArtistTypeGroup.Vocalist,
	[ArtistType.Producer]: ArtistTypeGroup.Producer,
	[ArtistType.Unknown]: ArtistTypeGroup.Nothing,
	[ArtistType.Utaite]: ArtistTypeGroup.Vocalist,
	[ArtistType.UTAU]: ArtistTypeGroup.Vocalist,
	[ArtistType.CeVIO]: ArtistTypeGroup.Vocalist,
	[ArtistType.Vocaloid]: ArtistTypeGroup.Vocalist,
	[ArtistType.Vocalist]: ArtistTypeGroup.Vocalist,
	[ArtistType.SynthesizerV]: ArtistTypeGroup.Vocalist,
	[ArtistType.CoverArtist]: ArtistTypeGroup.Producer,
};

const artistTypeGroups = vdb.values.artistTypes
	.orderBy((artistType) =>
		Object.values(ArtistTypeGroup).indexOf(groupsForTypes[artistType]),
	)
	.groupBy((artistType) => groupsForTypes[artistType]);

interface ArtistTypesDropdownKnockoutProps
	extends React.DetailedHTMLProps<
		React.SelectHTMLAttributes<HTMLSelectElement>,
		HTMLSelectElement
	> {}

export const ArtistTypesDropdownKnockout = React.memo(
	({ ...props }: ArtistTypesDropdownKnockoutProps): React.ReactElement => {
		const { t } = useTranslation('VocaDb.Model.Resources');

		return (
			<select {...props}>
				{Object.entries(artistTypeGroups).map(([key, value]) =>
					key === ArtistTypeGroup.Nothing ? (
						<React.Fragment key={key}>
							{value.map((artistType) => (
								<option value={artistType} key={artistType}>
									{t(`VocaDb.Model.Resources:ArtistTypeNames.${artistType}`)}
								</option>
							))}
						</React.Fragment>
					) : (
						<optgroup label={key /* LOCALIZE */} key={key}>
							{value.map((artistType) => (
								<option value={artistType} key={artistType}>
									{t(`VocaDb.Model.Resources:ArtistTypeNames.${artistType}`)}
								</option>
							))}
						</optgroup>
					),
				)}
			</select>
		);
	},
);
