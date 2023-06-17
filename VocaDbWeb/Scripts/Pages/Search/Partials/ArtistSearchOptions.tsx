import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistTypesDropdownKnockout } from '@/Components/Shared/Partials/Artist/ArtistTypesDropdownKnockout';
import { UserLanguageCultureDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { ArtistAdvancedFilters } from '@/Components/Shared/Partials/Search/AdvancedFilters';
import { useCultureCodes } from '@/CultureCodesContext';
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
		const { t } = useTranslation(['ViewRes', 'ViewRes.Search']);
		const { getCodeGroup } = useCultureCodes();

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
							value={
								artistSearchStore.languages
									? artistSearchStore.languages[0]
									: undefined
							}
							placeholder="(Show all)"
							extended={artistSearchStore.languagesExtended}
							onChange={(e): void => {
								artistSearchStore.languages = getCodeGroup(
									e.target.value,
									artistSearchStore.languagesExtended,
								);
							}}
						/>

						{!artistSearchStore.languagesExtended && (
							<SafeAnchor
								onClick={(): void => {
									artistSearchStore.languagesExtended = true;
									// Required for a refresh of the results
									if (
										artistSearchStore.languages &&
										artistSearchStore.languages.length > 0
									) {
										artistSearchStore.languages = getCodeGroup(
											artistSearchStore.languages[0],
											artistSearchStore.languagesExtended,
										);
									}
								}}
								href="#"
								className="textLink addLink"
							>
								{t('ViewRes:EntryEdit.LyExtendLanguages')}
							</SafeAnchor>
						)}
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
