import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Dropdown from '@/Bootstrap/Dropdown';
import { AdvancedFilter } from '@/Components/Shared/Partials/Search/AdvancedFilter';
import { ArtistType } from '@/Models/Artists/ArtistType';
import {
	AdvancedFilterType,
	AdvancedSearchFilter,
} from '@/Stores/Search/AdvancedSearchFilter';
import { AdvancedSearchFilters } from '@/Stores/Search/AdvancedSearchFilters';
import { observer } from 'mobx-react-lite';
import React from 'react';

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
						Advanced filters{/* LOC */} <span className="caret" />
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
		description: 'Artist type: Vocaloid' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.Vocaloid}`,
		negate: false,
	},
	{
		description: 'Artist type: UTAU' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.UTAU}`,
		negate: false,
	},
	{
		description: 'Artist type: CeVIO' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.CeVIO}`,
		negate: false,
	},
	{
		description: 'Artist type: Synthesizer V' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.SynthesizerV}`,
		negate: false,
	},
	{
		description: 'Artist type: NEUTRINO' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.NEUTRINO}`,
		negate: false,
	},
	{
		description: 'Artist type: VoiSona' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.VoiSona}`,
		negate: false,
	},
	{
		description: 'Artist type: New Type' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.NewType}`,
		negate: false,
	},
	{
		description: 'Artist type: Voiceroid' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.Voiceroid}`,
		negate: false,
	},
	{
		description: 'Artist type: VOICEVOX' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.VOICEVOX}`,
		negate: false,
	},
	{
		description: 'Artist type: ACE Virtual Singer' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.ACEVirtualSinger}`,
		negate: false,
	},
	{
		description: 'Artist type: A.I.VOICE' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.AIVOICE}`,
		negate: false,
	},
	{
		description: 'Artist type: other voice synthesizer' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.OtherVoiceSynthesizer}`,
		negate: false,
	},
	{
		description: 'No cover picture' /* LOC */,
		filterType: AdvancedFilterType.NoCoverPicture,
		param: '',
		negate: false,
	},
	{
		description: 'With store link' /* LOC */,
		filterType: AdvancedFilterType.HasStoreLink,
		param: '',
		negate: false,
	},
	{
		description: 'No tracks' /* LOC */,
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
		description: 'Voice provider of: any voicebank' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: '',
		negate: false,
	},
	{
		description: 'Voice provider of: Vocaloid' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.Vocaloid}`,
		negate: false,
	},
	{
		description: 'Voice provider of: UTAU' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.UTAU}`,
		negate: false,
	},
	{
		description: 'Voice provider of: CeVIO' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.CeVIO}`,
		negate: false,
	},
	{
		description: 'Voice provider of: Synthesizer V' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.SynthesizerV}`,
		negate: false,
	},
	{
		description: 'Voice provider of: NEUTRINO' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.NEUTRINO}`,
		negate: false,
	},
	{
		description: 'Voice provider of: VoiSona' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.VoiSona}`,
		negate: false,
	},
	{
		description: 'Voice provider of: New Type' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.NewType}`,
		negate: false,
	},
	{
		description: 'Voice provider of: Voiceroid' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.Voiceroid}`,
		negate: false,
	},
	{
		description: 'Voice provider of: VOICEVOX' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.VOICEVOX}`,
		negate: false,
	},
	{
		description: 'Voice provider of: ACE Virtual Singer' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.ACEVirtualSinger}`,
		negate: false,
	},
	{
		description: 'Voice provider of: A.I.VOICE' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.AIVOICE}`,
		negate: false,
	},
	{
		description: 'Voice provider of: other voice synthesizer' /* LOC */,
		filterType: AdvancedFilterType.VoiceProvider,
		param: `${ArtistType.OtherVoiceSynthesizer}`,
		negate: false,
	},
	{
		description: 'Root voicebank (no base)' /* LOC */,
		filterType: AdvancedFilterType.RootVoicebank,
		param: '',
		negate: false,
	},
	{
		description: 'Derived voicebank' /* LOC */,
		filterType: AdvancedFilterType.RootVoicebank,
		param: '',
		negate: true,
	},
	{
		description: 'User account on VocaDB' /* LOC */,
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
		description: 'Artist type: Vocaloid' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.Vocaloid}`,
		negate: false,
	},
	{
		description: 'Artist type: UTAU' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.UTAU}`,
		negate: false,
	},
	{
		description: 'Artist type: CeVIO' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.CeVIO}`,
		negate: false,
	},
	{
		description: 'Artist type: Synthesizer V' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.SynthesizerV}`,
		negate: false,
	},
	{
		description: 'Artist type: NEUTRINO' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.NEUTRINO}`,
		negate: false,
	},
	{
		description: 'Artist type: VoiSona' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.VoiSona}`,
		negate: false,
	},
	{
		description: 'Artist type: New Type' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.NewType}`,
		negate: false,
	},
	{
		description: 'Artist type: Voiceroid' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.Voiceroid}`,
		negate: false,
	},
	{
		description: 'Artist type: VOICEVOX' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.VOICEVOX}`,
		negate: false,
	},
	{
		description: 'Artist type: ACE Virtual Singer' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.ACEVirtualSinger}`,
		negate: false,
	},
	{
		description: 'Artist type: A.I.VOICE' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.AIVOICE}`,
		negate: false,
	},
	{
		description: 'Artist type: other voice synthesizer' /* LOC */,
		filterType: AdvancedFilterType.ArtistType,
		param: `${ArtistType.OtherVoiceSynthesizer}`,
		negate: false,
	},
	{
		description: 'Multiple voicebanks' /* LOC */,
		filterType: AdvancedFilterType.HasMultipleVoicebanks,
		param: '',
		negate: false,
	},
	{
		description: 'Lyrics: Any language' /* LOC */,
		filterType: AdvancedFilterType.Lyrics,
		param: '*',
		negate: false,
	},
	{
		description: 'Lyrics: Japanese' /* LOC */,
		filterType: AdvancedFilterType.Lyrics,
		param: 'ja',
		negate: false,
	},
	{
		description: 'Lyrics: Chinese' /* LOC */,
		filterType: AdvancedFilterType.Lyrics,
		param: 'zh',
		negate: false,
	},
	{
		description: 'Lyrics: English' /* LOC */,
		filterType: AdvancedFilterType.Lyrics,
		param: 'en',
		negate: false,
	},
	{
		description: 'Lyrics: Other/unspecified language' /* LOC */,
		filterType: AdvancedFilterType.Lyrics,
		param: '',
		negate: false,
	},
	{
		description: 'Has publish date' /* LOC */,
		filterType: AdvancedFilterType.HasPublishDate,
		param: '',
		negate: false,
	},
	{
		description: 'Album song' /* LOC */,
		filterType: AdvancedFilterType.HasAlbum,
		param: '',
		negate: false,
	},
	{
		description: 'Standalone (no album)' /* LOC */,
		filterType: AdvancedFilterType.HasAlbum,
		param: '',
		negate: true,
	},
	{
		description: 'No original media' /* LOC */,
		filterType: AdvancedFilterType.HasOriginalMedia,
		param: '',
		negate: true,
	},
	{
		description: 'No media' /* LOC */,
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
