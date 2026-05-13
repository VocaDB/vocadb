import React from 'react';
import { GenerateStars } from '@/Components/Shared/Partials/Shared/Stars';

interface StarsMetaProps {
    current: number;
    max: number;
}

export const StarsMeta = React.memo(
    ({ current, max }: StarsMetaProps): React.ReactElement => {
        return <GenerateStars current={current} max={max} />;
    },
);
