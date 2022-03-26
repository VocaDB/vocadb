import ButtonGroup from '@Bootstrap/ButtonGroup';
import Dropdown from '@Bootstrap/Dropdown';
import ArtistType from '@Models/Artists/ArtistType';
import AdvancedSearchFilter, {
	AdvancedFilterType,
} from '@Stores/Search/AdvancedSearchFilter';
import AdvancedSearchFilters from '@Stores/Search/AdvancedSearchFilters';
import { observer } from 'mobx-react-lite';
import React from 'react';

import AdvancedFilter from './AdvancedFilter';

interface AdvancedFiltersProps {
	advancedFilters: AdvancedSearchFilters;
	filters: AdvancedSearchFilter[];
}

const AdvancedFilters = observer(
	({ advancedFilters, filters }: AdvancedFiltersProps): React.ReactElement => {
		return (
			<>
				<Dropdown as={ButtonGroup}>
					<Dropdown.Toggle>
						Advanced filters{/* TODO: localize */} <span className="caret" />
					</Dropdown.Toggle>
					<Dropdown.Menu>
						{filters.map((filter) => (
							<AdvancedFilter
								advancedFilters={advancedFilters}
								description={filter.description}
								filterType={filter.filterType}
								param={filter.param}
								negate={filter.negate}
								key={filter.description}
							/>
						))}
					</Dropdown.Menu>
				</Dropdown>

				{advancedFilters.filters.length > 0 && (
					<div className="search-advanced-filters-list">
						{advancedFilters.filters.map((filter, index) => (
							<React.Fragment key={filter.description}>
								{index > 0 && ' '}
								<div
									className="label label-info"
									onClick={(): void => advancedFilters.remove(filter)}
								>
									<button type="button" className="close">
										×
									</button>
									<span>{filter.description}</span>
								</div>
							</React.Fragment>
						))}
					</div>
				)}
			</>
		);
	},
);

// Corresponds to AdvancedSearchFilters.AlbumFilters in C#.
const albumFilters: AdvancedSearchFilter[] = [
	{
		description: 'Artist type: Vocaloid' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.Vocaloid}`,
		negate: false,
	},
	{
		description: 'Artist type: UTAU' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.UTAU}`,
		negate: false,
	},
	{
		description: 'Artist type: CeVIO' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.CeVIO}`,
		negate: false,
	},
	{
		description: 'Artist type: Synthesizer V' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.SynthesizerV}`,
		negate: false,
	},
	{
		description: 'Artist type: other voice synthesizer' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.OtherVoiceSynthesizer}`,
		negate: false,
	},
	{
		description: 'No cover picture' /* TODO: localize */,
		filterType: AdvancedFilterType.NoCoverPicture,
		param: '',
		negate: false,
	},
	{
		description: 'With store link' /* TODO: localize */,
		filterType: AdvancedFilterType.HasStoreLink,
		param: '',
		negate: false,
	},
	{
		description: 'No tracks' /* TODO: localize */,
		filterType: AdvancedFilterType.HasTracks,
		param: '',
		negate: true,
	},
];

interface AlbumAdvancedFiltersProps {
	advancedFilters: AdvancedSearchFilters;
}

export const AlbumAdvancedFilters = React.memo(
	({ advancedFilters }: AlbumAdvancedFiltersProps): React.ReactElement => {
		return (
			<AdvancedFilters
				advancedFilters={advancedFilters}
				filters={albumFilters}
			/>
		);
	},
);

// Corresponds to AdvancedSearchFilters.ArtistFilters in C#.
const artistFilters: AdvancedSearchFilter[] = [
	{
		description: 'Voice provider of: any voicebank' /* TODO: localize */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: '',
		negate: false,
	},
	{
		description: 'Voice provider of: Vocaloid' /* TODO: localize */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.Vocaloid}`,
		negate: false,
	},
	{
		description: 'Voice provider of: UTAU' /* TODO: localize */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.UTAU}`,
		negate: false,
	},
	{
		description: 'Voice provider of: CeVIO' /* TODO: localize */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.CeVIO}`,
		negate: false,
	},
	{
		description: 'Voice provider of: Synthesizer V' /* TODO: localize */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.SynthesizerV}`,
		negate: false,
	},
	{
		description:
			'Voice provider of: other voice synthesizer' /* TODO: localize */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.OtherVoiceSynthesizer}`,
		negate: false,
	},
	{
		description: 'Root voicebank (no base)' /* TODO: localize */,
		filterType: AdvancedFilterType.RootVoicebank,
		param: '',
		negate: false,
	},
	{
		description: 'Derived voicebank' /* TODO: localize */,
		filterType: AdvancedFilterType.RootVoicebank,
		param: '',
		negate: true,
	},
	{
		description: 'User account on VocaDB' /* TODO: localize */,
		filterType: AdvancedFilterType.HasUserAccount,
		param: '',
		negate: false,
	},
];

interface ArtistAdvancedFiltersProps {
	advancedFilters: AdvancedSearchFilters;
}

export const ArtistAdvancedFilters = React.memo(
	({ advancedFilters }: ArtistAdvancedFiltersProps): React.ReactElement => {
		return (
			<AdvancedFilters
				advancedFilters={advancedFilters}
				filters={artistFilters}
			/>
		);
	},
);

// Corresponds to AdvancedSearchFilters.SongFilters in C#.
const songFilters: AdvancedSearchFilter[] = [
	{
		description: 'Artist type: Vocaloid' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.Vocaloid}`,
		negate: false,
	},
	{
		description: 'Artist type: UTAU' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.UTAU}`,
		negate: false,
	},
	{
		description: 'Artist type: CeVIO' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.CeVIO}`,
		negate: false,
	},
	{
		description: 'Artist type: Synthesizer V' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.SynthesizerV}`,
		negate: false,
	},
	{
		description: 'Artist type: other voice synthesizer' /* TODO: localize */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.OtherVoiceSynthesizer}`,
		negate: false,
	},
	{
		description: 'Multiple voicebanks' /* TODO: localize */,
		filterType: AdvancedFilterType.HasMultipleVoicebanks,
		param: '',
		negate: false,
	},
	{
		description: 'Lyrics: Any language' /* TODO: localize */,
		filterType: AdvancedFilterType.Lyrics,
		param: '*',
		negate: false,
	},
	{
		description: 'Lyrics: Japanese' /* TODO: localize */,
		filterType: AdvancedFilterType.Lyrics,
		param: 'ja',
		negate: false,
	},
	{
		description: 'Lyrics: Chinese' /* TODO: localize */,
		filterType: AdvancedFilterType.Lyrics,
		param: 'zh',
		negate: false,
	},
	{
		description: 'Lyrics: English' /* TODO: localize */,
		filterType: AdvancedFilterType.Lyrics,
		param: 'en',
		negate: false,
	},
	{
		description: 'Lyrics: Other/unspecified language' /* TODO: localize */,
		filterType: AdvancedFilterType.Lyrics,
		param: '',
		negate: false,
	},
	{
		description: 'Has publish date' /* TODO: localize */,
		filterType: AdvancedFilterType.HasPublishDate,
		param: '',
		negate: false,
	},
	{
		description: 'Album song' /* TODO: localize */,
		filterType: AdvancedFilterType.HasAlbum,
		param: '',
		negate: false,
	},
	{
		description: 'Standalone (no album)' /* TODO: localize */,
		filterType: AdvancedFilterType.HasAlbum,
		param: '',
		negate: true,
	},
	{
		description: 'No original media' /* TODO: localize */,
		filterType: AdvancedFilterType.HasOriginalMedia,
		param: '',
		negate: true,
	},
	{
		description: 'No media' /* TODO: localize */,
		filterType: AdvancedFilterType.HasMedia,
		param: '',
		negate: true,
	},
];

interface SongAdvancedFiltersProps {
	advancedFilters: AdvancedSearchFilters;
}

export const SongAdvancedFilters = React.memo(
	({ advancedFilters }: SongAdvancedFiltersProps): React.ReactElement => {
		return (
			<AdvancedFilters
				advancedFilters={advancedFilters}
				filters={songFilters}
			/>
		);
	},
);
