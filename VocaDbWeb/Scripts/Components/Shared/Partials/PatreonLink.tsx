import { useBrandableTranslation } from '@/Hooks/useBrandableTranslation';
import { useVdb } from '@/VdbContext';
import React from 'react';

export const PatreonLink = React.memo((): React.ReactElement => {
	const vdb = useVdb();
	const { t: tBrand } = useBrandableTranslation();

	return (
		<>
			<p>
				<small>{tBrand('LayoutRes.PaypalDonateTitle')}</small>
			</p>

			<a href={vdb.values.patreonLink} target="_blank" rel="noreferrer">
				<img
					src={'/Content/patreon.png'}
					alt="Support on Patreon"
					title="Support on Patreon"
				/>
			</a>
		</>
	);
});
