import ArtistCategories from '@Models/Artists/ArtistCategories';
import _ from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';

// Corresponds to ArtistHelper.CategoriesForTypes in C#.
const categoriesForTypes: Record<string, ArtistCategories> = {
	Animator: ArtistCategories.Animator,
	Character: ArtistCategories.Subject,
	Circle: ArtistCategories.Circle,
	Band: ArtistCategories.Band,
	Illustrator: ArtistCategories.Illustrator,
	Label: ArtistCategories.Label,
	Lyricist: ArtistCategories.Other,
	OtherGroup: ArtistCategories.Circle,
	OtherIndividual: ArtistCategories.Other,
	OtherVocalist: ArtistCategories.Vocalist,
	OtherVoiceSynthesizer: ArtistCategories.Vocalist,
	Producer: ArtistCategories.Producer,
	Unknown: ArtistCategories.Other,
	Utaite: ArtistCategories.Vocalist,
	UTAU: ArtistCategories.Vocalist,
	CeVIO: ArtistCategories.Vocalist,
	Vocaloid: ArtistCategories.Vocalist,
	Vocalist: ArtistCategories.Vocalist,
	SynthesizerV: ArtistCategories.Vocalist,
	CoverArtist: ArtistCategories.Producer,
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
