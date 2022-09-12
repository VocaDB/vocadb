import { BsPrefixRefForwardingComponent } from '@/Bootstrap/helpers';
import { EntryLink } from '@/Components/Shared/Partials/Shared/EntryLink';
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
				<div className="pictureFrame">
					{entry ? (
						<EntryLink
							entry={entry}
							tooltip={tooltip}
							css={{ display: 'block', width: '100%', height: '100%' }}
						>
							<img
								src={thumbUrl}
								alt="Preview" /* TODO: localize */
								className="coverPic"
							/>
						</EntryLink>
					) : (
						<LinkComponent
							{...linkProps}
							css={{ display: 'block', width: '100%', height: '100%' }}
						>
							<img
								src={thumbUrl}
								alt="Preview" /* TODO: localize */
								className="coverPic"
							/>
						</LinkComponent>
					)}
				</div>

				{children}
			</div>
			{caption && <p css={{ display: 'flex', width: 150 }}>{caption}</p>}
		</div>
	);
};
