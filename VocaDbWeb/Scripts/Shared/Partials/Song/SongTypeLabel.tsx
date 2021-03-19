import React, { ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import SongType from '../../../../wwwroot/Scripts/Models/Songs/SongType';

interface SongTypeLabelProps {
	songType: string;
}

const SongTypeLabel = ({
	songType,
}: SongTypeLabelProps): ReactElement => {
	const { t } = useTranslation(['VocaDb.Model.Resources.Songs']);

	const title = t(`VocaDb.Model.Resources.Songs:SongTypeNames.${songType}`);

	switch (SongType[songType]) {
		case SongType.Arrangement:
			return <span className="label" title={title}>A</span>;

		case SongType.Cover:
			return <span className="label" title={title}>C</span>;

		case SongType.DramaPV:
			return <span className="label label-success" title={title}>D</span>;

		case SongType.Instrumental:
			return <span className="label label-inverse" title={title}>I</span>;

		case SongType.Mashup:
			return <span className="label" title={title}>M</span>;

		case SongType.Original:
			return <span className="label label-info" title={title}>O</span>;

		case SongType.Other:
			return <span className="label" title={title}>O</span>;

		case SongType.Remaster:
			return <span className="label label-info" title={title}>R</span>;

		case SongType.Remix:
			return <span className="label" title={title}>R</span>;

		case SongType.MusicPV:
			return <span className="label label-success" title={title}>PV</span>;

		default:
			return null;
	}
};

export default SongTypeLabel;
