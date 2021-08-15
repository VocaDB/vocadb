import Button from '@Bootstrap/Button';
import ArtistAutoComplete from '@Components/KnockoutExtensions/ArtistAutoComplete';
import ArtistFiltersStore from '@Stores/Search/ArtistFilters';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import ArtistParticipationStatusOptionsKnockout from '../Song/ArtistParticipationStatusOptionsKnockout';

interface ArtistFiltersProps {
	artistFilters: ArtistFiltersStore;
	artistParticipationStatus: boolean;
}

const ArtistFilters = observer(
	({
		artistFilters,
		artistParticipationStatus,
	}: ArtistFiltersProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Search']);

		return (
			<>
				{artistFilters.artists.map((artist, index) => (
					<div className="control-group" key={index}>
						<div
							style={{ display: 'inline-block' }}
							className="input-append input-prepend"
						>
							<Button className="btn-nomargin" href={`/Ar/${artist.id}`}>
								<i className="icon icon-info-sign" />
							</Button>
							<div className="input-append">
								<input
									type="text"
									className="input-large"
									readOnly
									value={artist.name ?? ''}
								/>
								<Button
									variant="danger"
									onClick={(): void => artistFilters.removeArtist(artist)}
								>
									Clear{/* TODO: localize */}
								</Button>
							</div>
						</div>
						{artistParticipationStatus && artistFilters.hasSingleArtist && (
							<>
								{' '}
								<ArtistParticipationStatusOptionsKnockout
									activeKey={artistFilters.artistParticipationStatus}
									onSelect={(eventKey): void =>
										runInAction(() => {
											artistFilters.artistParticipationStatus = eventKey;
										})
									}
								/>
							</>
						)}
					</div>
				))}

				{artistFilters.showChildVoicebanks && (
					<div className="control-group">
						<label className="checkbox">
							<input
								type="checkbox"
								checked={artistFilters.childVoicebanks}
								onChange={(e): void =>
									runInAction(() => {
										artistFilters.childVoicebanks = e.target.checked;
									})
								}
							/>
							{t('ViewRes.Search:Index.IncludeDerivedVoicebanks')}
						</label>
					</div>
				)}

				{artistFilters.showMembers && (
					<div className="control-group">
						<label className="checkbox">
							<input
								type="checkbox"
								checked={artistFilters.includeMembers}
								onChange={(e): void =>
									runInAction(() => {
										artistFilters.includeMembers = e.target.checked;
									})
								}
							/>
							{t('ViewRes.Search:Index.IncludeGroupMembers')}
						</label>
					</div>
				)}

				<div>
					<ArtistAutoComplete
						type="text"
						className="input-large"
						properties={artistFilters.artistSearchParams}
						placeholder={t('ViewRes:Shared.Search')}
					/>
				</div>
			</>
		);
	},
);

export default ArtistFilters;
