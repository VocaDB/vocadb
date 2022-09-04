import { BsPrefixRefForwardingComponent } from '@/Bootstrap/helpers';
import { EntryToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { EntryRefContract } from '@/DataContracts/EntryRefContract';
import React from 'react';

interface ThumbItemProps {
	linkAs?: React.ElementType;
	linkProps?: Record<string, any>;
	thumbUrl: string;
	caption?: string;
	entry?: EntryRefContract;
	tooltip?: boolean;
	children?: React.ReactNode;
}

export const ThumbItem: BsPrefixRefForwardingComponent/* TODO */ <
	'a',
	ThumbItemProps
> = ({
	linkAs: LinkComponent = 'a',
	linkProps,
	thumbUrl,
	caption,
	entry,
	tooltip,
	children,
	...props
}: ThumbItemProps): React.ReactElement => {
	return (
		<div css={{ marginRight: 9, lineHeight: '18px' }}>
			<div css={{ position: 'relative' }} {...props}>
				<LinkComponent {...linkProps}>
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
				</LinkComponent>

				{children}
			</div>
			{caption && <p css={{ display: 'flex', width: 150 }}>{caption}</p>}
		</div>
	);
};
