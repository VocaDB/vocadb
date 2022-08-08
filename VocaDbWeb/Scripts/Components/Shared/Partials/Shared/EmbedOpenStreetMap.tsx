import OptionalGeoPointContract from '@/DataContracts/OptionalGeoPointContract';
import qs from 'qs';
import React from 'react';

interface EmbedOpenStreetMapProps {
	coordinates: OptionalGeoPointContract;
}

const EmbedOpenStreetMap = React.memo(
	({ coordinates }: EmbedOpenStreetMapProps): React.ReactElement => {
		return (
			<div>
				{/* eslint-disable-next-line jsx-a11y/iframe-has-title */}
				<iframe
					width="425"
					height="350"
					frameBorder="0"
					scrolling="no"
					marginHeight={0}
					marginWidth={0}
					src={`https://www.openstreetmap.org/export/embed.html?${qs.stringify({
						bbox: `${coordinates.longitude},${coordinates.latitude},${coordinates.longitude},${coordinates.latitude}`,
						layer: 'mapnik',
						marker: `${coordinates.latitude},${coordinates.longitude}`,
					})}`}
					style={{ border: '1px solid black' }}
				/>
				<br />
				<small>
					<a
						href={`https://www.openstreetmap.org/#map=19/${coordinates.latitude}/${coordinates.longitude}`}
					>
						View Larger Map
					</a>
				</small>
			</div>
		);
	},
);

export default EmbedOpenStreetMap;
