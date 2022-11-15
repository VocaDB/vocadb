import Button from '@/Bootstrap/Button';
import { EmbedPVPreview } from '@/Components/Shared/Partials/PV/EmbedPVPreview';
import { showSuccessMessage } from '@/Components/ui';
import { SongWithPVAndVoteContract } from '@/DataContracts/Song/SongWithPVAndVoteContract';
import { PVHelper } from '@/Helpers/PVHelper';
import { EntryType } from '@/Models/EntryType';
import { loginManager } from '@/Models/LoginManager';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { FrontPagePVPlayerStore } from '@/Stores/FrontPageStore';
import { PVRatingButtonsStore } from '@/Stores/PVRatingButtonsStore';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface RatingBarProps {
	pvRatingButtonsStore: PVRatingButtonsStore | undefined;
}

const RatingBar = observer(
	({ pvRatingButtonsStore }: RatingBarProps): React.ReactElement => {
		const { t } = useTranslation(['AjaxRes', 'ViewRes.Song']);

		if (!pvRatingButtonsStore) {
			return (
				<span className="ratingButtons">
					<Button
						href="#"
						disabled
						variant="inverse"
						style={{ color: 'white' }}
					>
						{t('ViewRes.Song:Details.Like')}
					</Button>{' '}
					<Button
						href="#"
						disabled
						variant="inverse"
						style={{ color: 'white' }}
					>
						{t('ViewRes.Song:Details.AddToFavorites')}
					</Button>
				</span>
			);
		}

		return pvRatingButtonsStore.isRated ? (
			<Button
				href="#"
				onClick={pvRatingButtonsStore.setRating_nothing}
				className={classNames(
					(!loginManager.isLoggedIn || pvRatingButtonsStore.ratingInProgress) &&
						'disabled',
					'ratingButtons',
				)}
				variant="inverse"
				style={{ color: 'white' }}
			>
				{t('ViewRes.Song:Details.RemoveFromFavorites')}
			</Button>
		) : (
			<span className="ratingButtons">
				<Button
					href="#"
					onClick={async (): Promise<void> => {
						await pvRatingButtonsStore.setRating_like();

						showSuccessMessage(t('AjaxRes:Song.ThanksForRating'));
					}}
					disabled={
						!loginManager.isLoggedIn || pvRatingButtonsStore.ratingInProgress
					}
					variant="inverse"
					style={{ color: 'white' }}
				>
					{t('ViewRes.Song:Details.Like')}
				</Button>{' '}
				<Button
					href="#"
					onClick={async (): Promise<void> => {
						await pvRatingButtonsStore.setRating_favorite();

						showSuccessMessage(t('AjaxRes:Song.ThanksForRating'));
					}}
					disabled={
						!loginManager.isLoggedIn || pvRatingButtonsStore.ratingInProgress
					}
					variant="inverse"
					style={{ color: 'white' }}
				>
					{t('ViewRes.Song:Details.AddToFavorites')}
				</Button>
			</span>
		);
	},
);

interface PVContentProps {
	pvPlayerStore: FrontPagePVPlayerStore;
	selectedSong: SongWithPVAndVoteContract;
}

export const PVContent = observer(
	({ pvPlayerStore, selectedSong }: PVContentProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes.Home',
			'VocaDb.Model.Resources.Songs',
		]);

		const primaryPV = PVHelper.primaryPV(selectedSong.pvs);

		return (
			<>
				<input type="hidden" className="js-songId" value={selectedSong.id} />
				<h4 className="pvViewer-title">
					<span
						id="songPreviewName"
						className="pvViewer-songName"
						title={selectedSong.additionalNames}
					>
						<Link
							to={EntryUrlMapper.details(EntryType.Song, selectedSong.id)}
							style={{ color: 'white' }}
						>
							{selectedSong.name}
						</Link>
					</span>{' '}
					&nbsp;{' '}
					<small id="songPreviewArtists" className="pvViewer-artists">
						{selectedSong.artistString} (
						{t(
							`VocaDb.Model.Resources.Songs:SongTypeNames.${selectedSong.songType}`,
						)}
						)
					</small>
				</h4>
				<span className="pvViewer-translatedNames">
					{selectedSong.additionalNames}
				</span>
				<div className="pull-right" id="rating-bar">
					<RatingBar pvRatingButtonsStore={pvPlayerStore.ratingButtonsStore} />
				</div>
				<div className="pv-embed-wrapper">
					{primaryPV && (
						<EmbedPVPreview
							entry={{ ...selectedSong, entryType: EntryType.Song }}
							pv={primaryPV}
							width={560}
							height={340}
							allowInline
						/>
					)}
				</div>
				<Button
					as={Link}
					variant="info"
					className="songInfoButton pull-right"
					to={EntryUrlMapper.details(EntryType.Song, selectedSong.id)}
					style={{ color: 'white' }}
				>
					<i className="icon icon-info-sign" />{' '}
					{t('ViewRes.Home:Index.ViewSongInfo')}
				</Button>
			</>
		);
	},
);
