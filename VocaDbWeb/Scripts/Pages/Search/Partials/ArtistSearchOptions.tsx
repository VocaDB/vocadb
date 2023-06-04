import { ArtistTypesDropdownKnockout } from '@/Components/Shared/Partials/Artist/ArtistTypesDropdownKnockout';
import { UserLanguageCultureDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { ArtistAdvancedFilters } from '@/Components/Shared/Partials/Search/AdvancedFilters';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { ArtistSearchStore } from '@/Stores/Search/ArtistSearchStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistSearchOptionsProps {
	artistSearchStore: ArtistSearchStore;
}

const ArtistSearchOptions = observer(
	({ artistSearchStore }: ArtistSearchOptionsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Search']);

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
								runInAction(() => {
									artistSearchStore.artistType = e.target.value as ArtistType;
								})
							}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.Search:Index.Language')}
					</div>
					<div className="controls">
						<UserLanguageCultureDropdownList
							value={artistSearchStore.languages}
							placeholder="(Show all)"
							onChange={(e): void => {
								artistSearchStore.languages = [e.target.value];
							}}
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
