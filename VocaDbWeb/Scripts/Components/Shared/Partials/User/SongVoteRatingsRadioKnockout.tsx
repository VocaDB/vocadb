import Button from '@/Bootstrap/Button';
import { SongVoteRating } from '@/Stores/Search/SongSearchStore';
import classNames from 'classnames';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongVoteRatingsRadioKnockoutProps {
	rating: SongVoteRating;
	onRatingChange: (rating: SongVoteRating) => void;
}

const SongVoteRatingsRadioKnockout = React.memo(
	({
		rating,
		onRatingChange,
	}: SongVoteRatingsRadioKnockoutProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes.User']);

		return (
			<>
				<Button
					onClick={(): void => onRatingChange('Nothing')}
					className={classNames(rating === 'Nothing' && 'active')}
				>
					{t('ViewRes.User:RatedSongs.RatingAnything')}
				</Button>{' '}
				<Button
					onClick={(): void => onRatingChange('Like')}
					className={classNames(rating === 'Like' && 'active')}
				>
					<i className="icon-star" /> {t('Resources:SongVoteRatingNames.Like')}
				</Button>{' '}
				<Button
					onClick={(): void => onRatingChange('Favorite')}
					className={classNames(rating === 'Favorite' && 'active')}
				>
					<i className="icon-heart" />{' '}
					{t('Resources:SongVoteRatingNames.Favorite')}
				</Button>
			</>
		);
	},
);

export default SongVoteRatingsRadioKnockout;
