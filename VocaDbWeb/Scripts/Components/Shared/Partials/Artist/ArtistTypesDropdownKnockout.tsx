import ArtistType from '@Models/Artists/ArtistType';
import _ from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';

enum ArtistTypeGroup {
	Nothing = 'Nothing',
	Vocalist = 'Vocalist',
	Producer = 'Producer',
	Group = 'Group',
	Other = 'Other',
}

// Corresponds to ArtistHelper.CategoriesForTypes in C#.
const categoriesForTypes: Record<string, ArtistTypeGroup> = {
	[ArtistType[ArtistType.Animator]]: ArtistTypeGroup.Producer,
	[ArtistType[ArtistType.Character]]: ArtistTypeGroup.Other,
	[ArtistType[ArtistType.Circle]]: ArtistTypeGroup.Group,
	[ArtistType[ArtistType.Band]]: ArtistTypeGroup.Group,
	[ArtistType[ArtistType.Illustrator]]: ArtistTypeGroup.Producer,
	[ArtistType[ArtistType.Label]]: ArtistTypeGroup.Group,
	[ArtistType[ArtistType.Lyricist]]: ArtistTypeGroup.Other,
	[ArtistType[ArtistType.OtherGroup]]: ArtistTypeGroup.Group,
	[ArtistType[ArtistType.OtherIndividual]]: ArtistTypeGroup.Other,
	[ArtistType[ArtistType.OtherVocalist]]: ArtistTypeGroup.Vocalist,
	[ArtistType[ArtistType.OtherVoiceSynthesizer]]: ArtistTypeGroup.Vocalist,
	[ArtistType[ArtistType.Producer]]: ArtistTypeGroup.Producer,
	[ArtistType[ArtistType.Unknown]]: ArtistTypeGroup.Nothing,
	[ArtistType[ArtistType.Utaite]]: ArtistTypeGroup.Vocalist,
	[ArtistType[ArtistType.UTAU]]: ArtistTypeGroup.Vocalist,
	[ArtistType[ArtistType.CeVIO]]: ArtistTypeGroup.Vocalist,
	[ArtistType[ArtistType.Vocaloid]]: ArtistTypeGroup.Vocalist,
	[ArtistType[ArtistType.Vocalist]]: ArtistTypeGroup.Vocalist,
	[ArtistType[ArtistType.SynthesizerV]]: ArtistTypeGroup.Vocalist,
	[ArtistType[ArtistType.CoverArtist]]: ArtistTypeGroup.Producer,
};

const artistTypeGroups = _.chain(vdb.values.artistTypes)
	.orderBy((artistType) =>
		Object.values(ArtistTypeGroup).indexOf(categoriesForTypes[artistType]),
	)
	.groupBy((artistType) => categoriesForTypes[artistType])
	.value();

interface ArtistTypesDropdownKnockoutProps
	extends React.DetailedHTMLProps<
		React.SelectHTMLAttributes<HTMLSelectElement>,
		HTMLSelectElement
	> {}

const ArtistTypesDropdownKnockout = React.memo(
	({ ...props }: ArtistTypesDropdownKnockoutProps): React.ReactElement => {
		const { t } = useTranslation('VocaDb.Model.Resources');

		return (
			<select {...props}>
				{Object.entries(artistTypeGroups).map(([key, value]) =>
					key === ArtistTypeGroup.Nothing ? (
						<>
							{value.map((artistType) => (
								<option value={artistType} key={artistType}>
									{t(`VocaDb.Model.Resources:ArtistTypeNames.${artistType}`)}
								</option>
							))}
						</>
					) : (
						<optgroup label={key} key={key}>
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

export default ArtistTypesDropdownKnockout;
