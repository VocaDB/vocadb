import Button from '@Bootstrap/Button';
import { showSuccessMessage } from '@Components/ui';
import LoginManager from '@Models/LoginManager';
import SongVoteRating from '@Models/SongVoteRating';
import PVRatingButtonsStore from '@Stores/PVRatingButtonsStore';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

const loginManager = new LoginManager(vdb.values);

interface PVRatingButtonsForIndexProps {
	pvRatingButtonsStore: PVRatingButtonsStore;
}

const PVRatingButtonsForIndex = observer(
	({
		pvRatingButtonsStore,
	}: PVRatingButtonsForIndexProps): React.ReactElement => {
		const { t } = useTranslation(['AjaxRes', 'Resources', 'ViewRes.Song']);

		return loginManager.isLoggedIn ? (
			<div className="pull-right">
				{pvRatingButtonsStore.isRated ? (
					<span className="pull-right">
						{pvRatingButtonsStore.isRatingFavorite && (
							<span
								className="icon heartIcon"
								title={t(
									`Resources:SongVoteRatingNames.${
										SongVoteRating[SongVoteRating.Favorite]
									}`,
								)}
							/>
						)}
						{pvRatingButtonsStore.isRatingLike && (
							<span
								className="icon starIcon"
								title={t(
									`Resources:SongVoteRatingNames.${
										SongVoteRating[SongVoteRating.Like]
									}`,
								)}
							/>
						)}{' '}
						<Button
							onClick={pvRatingButtonsStore.setRating_nothing}
							href="#"
							className={classNames(
								'btn-rateSong-remove',
								pvRatingButtonsStore.ratingInProgress && 'disabled',
							)}
						>
							{t('ViewRes.Song:Details.RemoveFromFavorites')}
						</Button>
					</span>
				) : (
					<span className="pull-right">
						<Button
							onClick={async (): Promise<void> => {
								await pvRatingButtonsStore.setRating_like();

								showSuccessMessage(t('AjaxRes:Song.ThanksForRating'));
							}}
							href="#"
							className={classNames(
								'btn-rateSong-like',
								pvRatingButtonsStore.ratingInProgress && 'disabled',
							)}
						>
							{t('ViewRes.Song:Details.Like')}
						</Button>{' '}
						<Button
							onClick={async (): Promise<void> => {
								await pvRatingButtonsStore.setRating_favorite();

								showSuccessMessage(t('AjaxRes:Song.ThanksForRating'));
							}}
							href="#"
							className={classNames(
								'btn-rateSong-favorite',
								pvRatingButtonsStore.ratingInProgress && 'disabled',
							)}
						>
							{t('ViewRes.Song:Details.AddToFavorites')}
						</Button>
					</span>
				)}
			</div>
		) : (
			<></>
		);
	},
);

export default PVRatingButtonsForIndex;
