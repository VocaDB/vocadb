import ArtistTypesDropdownKnockout from '@Components/Shared/Partials/Artist/ArtistTypesDropdownKnockout';
import { ArtistAdvancedFilters } from '@Components/Shared/Partials/Search/AdvancedFilters';
import useRedial from '@Components/useRedial';
import ArtistSearchStore from '@Stores/Search/ArtistSearchStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistSearchOptionsProps {
	artistSearchStore: ArtistSearchStore;
}

const ArtistSearchOptions = observer(
	({ artistSearchStore }: ArtistSearchOptionsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Search']);
		const redial = useRedial(artistSearchStore.routeParams);

		return (
			<div>
				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.ArtistType')}
					</div>
					<div className="controls">
						<ArtistTypesDropdownKnockout
							value={artistSearchStore.artistType}
							onChange={(e): void =>
								redial({ artistType: e.target.value, page: 1 })
							}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label"></div>
					<div className="controls">
						<ArtistAdvancedFilters
							advancedFilters={artistSearchStore.advancedFilters}
						/>
					</div>
				</div>
			</div>
		);
	},
);

export default ArtistSearchOptions;
