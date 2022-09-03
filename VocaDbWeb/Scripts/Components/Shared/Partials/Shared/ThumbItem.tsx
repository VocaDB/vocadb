import { BsPrefixRefForwardingComponent } from '@/Bootstrap/helpers';
import { EntryToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { EmbedPVPreviewButtons } from '@/Components/Shared/Partials/PV/EmbedPVPreview';
import { EntryRefContract } from '@/DataContracts/EntryRefContract';
import React from 'react';

interface ThumbItemProps {
	as?: React.ElementType;
	thumbUrl: string;
	caption?: string;
	entry?: EntryRefContract;
	tooltip?: boolean;
}

export const ThumbItem: BsPrefixRefForwardingComponent/* TODO */ <
	'a',
	ThumbItemProps
> = ({
	as: Component = 'a',
	thumbUrl,
	caption,
	entry,
	tooltip,
	...props
}: ThumbItemProps): React.ReactElement => {
	const handlePlay = React.useCallback(() => {}, []);

	const handlePlayNext = React.useCallback(() => {}, []);

	const handleAddToPlayQueue = React.useCallback(() => {}, []);

	return (
		<div css={{ marginRight: 9 }}>
			<div css={{ position: 'relative' }}>
				<Component {...props}>
					<div className="pictureFrame">
						{entry ? (
							tooltip ? (
								<EntryToolTip
									as="img"
									src={thumbUrl}
									alt="Preview" /* TODO: localize */
									className="coverPic"
									value={entry}
								/>
							) : (
								<img
									src={thumbUrl}
									alt="Preview" /* TODO: localize */
									className="coverPic"
								/>
							)
						) : (
							<img
								src={thumbUrl}
								alt="Preview" /* TODO: localize */
								className="coverPic"
							/>
						)}
					</div>
				</Component>

				<EmbedPVPreviewButtons
					onPlay={handlePlay}
					onPlayNext={handlePlayNext}
					onAddToPlayQueue={handleAddToPlayQueue}
				/>
			</div>
			{caption && <span>{caption}</span>}
		</div>
	);
};
