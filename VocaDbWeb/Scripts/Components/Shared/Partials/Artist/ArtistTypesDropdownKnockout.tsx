import ArtistCategories from '@Models/Artists/ArtistCategories';
import ArtistType from '@Models/Artists/ArtistType';
import _ from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';

// Corresponds to ArtistHelper.CategoriesForTypes in C#.
const categoriesForTypes: Record<string, ArtistCategories> = {
	[ArtistType[ArtistType.Animator]]: ArtistCategories.Animator,
	[ArtistType[ArtistType.Character]]: ArtistCategories.Subject,
	[ArtistType[ArtistType.Circle]]: ArtistCategories.Circle,
	[ArtistType[ArtistType.Band]]: ArtistCategories.Band,
	[ArtistType[ArtistType.Illustrator]]: ArtistCategories.Illustrator,
	[ArtistType[ArtistType.Label]]: ArtistCategories.Label,
	[ArtistType[ArtistType.Lyricist]]: ArtistCategories.Other,
	[ArtistType[ArtistType.OtherGroup]]: ArtistCategories.Circle,
	[ArtistType[ArtistType.OtherIndividual]]: ArtistCategories.Other,
	[ArtistType[ArtistType.OtherVocalist]]: ArtistCategories.Vocalist,
	[ArtistType[ArtistType.OtherVoiceSynthesizer]]: ArtistCategories.Vocalist,
	[ArtistType[ArtistType.Producer]]: ArtistCategories.Producer,
	[ArtistType[ArtistType.Unknown]]: ArtistCategories.Other,
	[ArtistType[ArtistType.Utaite]]: ArtistCategories.Vocalist,
	[ArtistType[ArtistType.UTAU]]: ArtistCategories.Vocalist,
	[ArtistType[ArtistType.CeVIO]]: ArtistCategories.Vocalist,
	[ArtistType[ArtistType.Vocaloid]]: ArtistCategories.Vocalist,
	[ArtistType[ArtistType.Vocalist]]: ArtistCategories.Vocalist,
	[ArtistType[ArtistType.SynthesizerV]]: ArtistCategories.Vocalist,
	[ArtistType[ArtistType.CoverArtist]]: ArtistCategories.Producer,
};

const artistTypeGroups = _.chain(vdb.values.artistTypes)
	.orderBy((artistType) =>
		Object.values(ArtistCategories).indexOf(categoriesForTypes[artistType]),
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
				{Object.entries(artistTypeGroups).map(([key, value]) => (
					<optgroup label={key} key={key}>
						{value.map((artistType) => (
							<option value={artistType} key={artistType}>
								{t(`VocaDb.Model.Resources:ArtistTypeNames.${artistType}`)}
							</option>
						))}
					</optgroup>
				))}
			</select>
		);
	},
);

export default ArtistTypesDropdownKnockout;
