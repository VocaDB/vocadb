import { ArchivedEntryPictureFileContract } from '@/DataContracts/ArchivedEntryPictureFileContract';
import React from 'react';

interface PictureFileInfoProps {
	picture: ArchivedEntryPictureFileContract;
}

export const PictureFileInfo = React.memo(
	({ picture }: PictureFileInfoProps): React.ReactElement => {
		return (
			<>
				{picture.name} ({picture.mime}) - {picture.author.nameHint} [
				{picture.author.id}], {picture.created}
			</>
		);
	},
);
