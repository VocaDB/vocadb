import React, { ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import ArtistType from '../../../../wwwroot/Scripts/Models/Artists/ArtistType';

interface ArtistTypeLabelProps {
	artistType: string;
}

const ArtistTypeLabel = ({
	artistType,
}: ArtistTypeLabelProps): ReactElement => {
	const { t } = useTranslation(['VocaDb.Model.Resources']);

	const title = t(`VocaDb.Model.Resources:ArtistTypeNames.${artistType}`)

	switch (ArtistType[artistType]) {
		case ArtistType.Vocaloid:
			return <span className="label label-info" title={title}>V</span>;

		case ArtistType.UTAU:
			return <span className="label label-important" title={title}>U</span>;

		case ArtistType.CeVIO:
			return <span className="label label-success" title={title}>C</span>;

		case ArtistType.OtherVoiceSynthesizer:
			return <span className="label label-inverse" title={title}>O</span>;

		case ArtistType.Utaite:
			return <span className="label label-info" title={title}>U</span>;

		case ArtistType.OtherVocalist:
			return <span className="label" title={title}>O</span>;

		case ArtistType.SynthesizerV:
			return <span className="label" title={title}>SV</span>;

		default:
			return null;
	}
};

export default ArtistTypeLabel;
