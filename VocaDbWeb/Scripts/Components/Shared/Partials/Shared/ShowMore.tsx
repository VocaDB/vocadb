import { BsPrefixRefForwardingComponent } from '@Bootstrap/helpers';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ShowMoreProps {
	as: React.ElementType;
}

const ShowMore: BsPrefixRefForwardingComponent<
	'a',
	ShowMoreProps
> = React.forwardRef<HTMLAnchorElement, ShowMoreProps>(
	({ as: Component, ...props }: ShowMoreProps, ref): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<div className="pull-right">
				<small>
					<Component {...props} ref={ref}>
						{t('ViewRes:Shared.ShowMore')}
					</Component>
				</small>
			</div>
		);
	},
);

export default ShowMore;
