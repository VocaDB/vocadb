import React from 'react';

import StarsMeta from './StarsMeta';

interface StarsMetaSpanProps {
	current: number;
	max: number;
}

const StarsMetaSpan = React.memo(
	({ current, max }: StarsMetaSpanProps): React.ReactElement => {
		return (
			<span title={`${Math.round(current * 100) / 100}`}>
				<StarsMeta current={Math.round(current)} max={max} />
			</span>
		);
	},
);

export default StarsMetaSpan;
