import Button from '@/Bootstrap/Button';
import { ArtistAutoComplete } from '@/Components/KnockoutExtensions/ArtistAutoComplete';
import { ArtistParticipationStatusOptionsKnockout } from '@/Components/Shared/Partials/Song/ArtistParticipationStatusOptionsKnockout';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { ArtistFilters as ArtistFiltersStore } from '@/Stores/Search/ArtistFilters';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface ArtistFiltersProps {
	artistFilters: ArtistFiltersStore;
	artistParticipationStatus: boolean;
	showChildVoicebanks?: boolean;
}

export const ArtistFilters = observer(
	({
		artistFilters,
		artistParticipationStatus,
		showChildVoicebanks = true,
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
							<Button
								as={Link}
								className="btn-nomargin"
								to={EntryUrlMapper.details(EntryType.Artist, artist.id)}
							>
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
									Clear{/* LOC */}
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

				{showChildVoicebanks && artistFilters.showChildVoicebanks && (
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
						properties={{ acceptSelection: artistFilters.selectArtist }}
						placeholder={t('ViewRes:Shared.Search')}
					/>
				</div>
			</>
		);
	},
);
