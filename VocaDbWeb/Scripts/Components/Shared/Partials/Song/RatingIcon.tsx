import SongVoteRating from '@/Models/SongVoteRating';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface RatingIconProps {
	rating: SongVoteRating;
}

const RatingIcon = React.memo(
	({ rating }: RatingIconProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		switch (rating) {
			case SongVoteRating.Like:
				return (
					<span
						className="icon starIcon"
						title={t(
							`Resources:SongVoteRatingNames.${
								SongVoteRating[SongVoteRating.Like]
							}`,
						)}
					/>
				);

			case SongVoteRating.Favorite:
				return (
					<span
						className="icon heartIcon"
						title={t(
							`Resources:SongVoteRatingNames.${
								SongVoteRating[SongVoteRating.Favorite]
							}`,
						)}
					/>
				);

			default:
				return <></>;
		}
	},
);

export default RatingIcon;
