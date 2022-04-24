import Button from '@Bootstrap/Button';
import { showSuccessMessage } from '@Components/ui';
import SongWithPVAndVoteContract from '@DataContracts/Song/SongWithPVAndVoteContract';
import PVHelper from '@Helpers/PVHelper';
import EntryType from '@Models/EntryType';
import LoginManager from '@Models/LoginManager';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import { PVPlayerStore } from '@Stores/FrontPageStore';
import PVRatingButtonsStore from '@Stores/PVRatingButtonsStore';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

import EmbedPV from '../Partials/PV/EmbedPV';

const loginManager = new LoginManager(vdb.values);

interface RatingBarProps {
	pvRatingButtonsStore: PVRatingButtonsStore;
}

const RatingBar = observer(
	({ pvRatingButtonsStore }: RatingBarProps): React.ReactElement => {
		const { t } = useTranslation(['AjaxRes', 'ViewRes.Song']);

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
					className={classNames(
						(!loginManager.isLoggedIn ||
							pvRatingButtonsStore.ratingInProgress) &&
							'disabled',
					)}
					variant="inverse"
				>
					{t('ViewRes.Song:Details.Like')}
				</Button>{' '}
				<Button
					href="#"
					onClick={async (): Promise<void> => {
						await pvRatingButtonsStore.setRating_favorite();

						showSuccessMessage(t('AjaxRes:Song.ThanksForRating'));
					}}
					className={classNames(
						(!loginManager.isLoggedIn ||
							pvRatingButtonsStore.ratingInProgress) &&
							'disabled',
					)}
					variant="inverse"
				>
					{t('ViewRes.Song:Details.AddToFavorites')}
				</Button>
			</span>
		);
	},
);

interface PVContentProps {
	pvPlayerStore: PVPlayerStore;
	selectedSong: SongWithPVAndVoteContract;
}

const PVContent = observer(
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
						<Link to={EntryUrlMapper.details(EntryType.Song, selectedSong.id)}>
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
					{pvPlayerStore.ratingButtonsStore && (
						<RatingBar
							pvRatingButtonsStore={pvPlayerStore.ratingButtonsStore}
						/>
					)}
				</div>
				<div className="pv-embed-wrapper">
					{primaryPV && <EmbedPV pv={primaryPV} width={560} height={340} />}
				</div>
				<Button
					as={Link}
					variant="info"
					className="songInfoButton pull-right"
					to={EntryUrlMapper.details(EntryType.Song, selectedSong.id)}
				>
					<i className="icon icon-info-sign" />{' '}
					{t('ViewRes.Home:Index.ViewSongInfo')}
				</Button>
			</>
		);
	},
);

export default PVContent;
