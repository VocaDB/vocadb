import { useMutedUsers } from '@/AppContext';
import Alert from '@/Bootstrap/Alert';
import Button from '@/Bootstrap/Button';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { MomentJsTimeAgo } from '@/Components/KnockoutExtensions/MomentJsTimeAgo';
import { UserLanguageCultureDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { MarkdownNotice } from '@/Components/Shared/Partials/Shared/MarkdownNotice';
import { IconAndLinkKnockout } from '@/Components/Shared/Partials/User/IconAndLinkKnockout';
import { NameLinkKnockout } from '@/Components/Shared/Partials/User/NameLinkKnockout';
import { AlbumDetailsForApi } from '@/DataContracts/Album/AlbumDetailsForApi';
import { LoginManager } from '@/Models/LoginManager';
import { AlbumDetailsTabs } from '@/Pages/Album/AlbumDetailsRoutes';
import { functions } from '@/Shared/GlobalFunctions';
import {
	AlbumDetailsStore,
	AlbumReviewStore,
} from '@/Stores/Album/AlbumDetailsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

const loginManager = new LoginManager(vdb.values);

interface AlbumReviewProps {
	albumDetailsStore: AlbumDetailsStore;
	review: AlbumReviewStore;
}

const AlbumReview = observer(
	({ albumDetailsStore, review }: AlbumReviewProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const mutedUsers = useMutedUsers();
		if (mutedUsers.includes(review.user)) return <></>;

		return (
			<div className="album-review media">
				<IconAndLinkKnockout user={review.user} />

				<div className="media-body">
					<div className="pull-right">
						<img
							src={functions.mergeUrls(
								vdb.values.staticContentHost,
								`/img/languageFlags/${review.languageCode}.png`,
							)}
							title={review.languageCode}
							alt={review.languageCode}
						/>{' '}
						| <MomentJsTimeAgo as="span">{review.date}</MomentJsTimeAgo>
						{review.canBeEdited && (
							<>
								&nbsp;&nbsp;
								<SafeAnchor
									onClick={(): void =>
										albumDetailsStore.reviewsStore.beginEditReview(review)
									}
									href="#"
									className="textLink editLink"
								>
									{t('ViewRes:Shared.Edit')}
								</SafeAnchor>
							</>
						)}
						{review.canBeDeleted && (
							<>
								&nbsp;&nbsp;
								<SafeAnchor
									// TODO: confirmClick
									onClick={(): void => {
										if (
											window.confirm(
												'Are you sure you want to delete this review?' /* TODO: localize */,
											)
										) {
											albumDetailsStore.reviewsStore.deleteReview(review);
										}
									}}
									href="#"
									className="textLink deleteLink"
								>
									{t('ViewRes:Shared.Delete')}
								</SafeAnchor>
							</>
						)}
					</div>
					<h3 className="media-heading">
						<NameLinkKnockout user={review.user} />
					</h3>

					<span>
						{albumDetailsStore.reviewsStore
							.ratingStars(
								albumDetailsStore.reviewsStore.getRatingForUser(review.user.id),
							)
							.map((ratingStar, index) => (
								<React.Fragment key={index}>
									{index > 0 && ' '}
									{/* eslint-disable-next-line jsx-a11y/alt-text */}
									<img
										src={
											ratingStar.enabled
												? '/Content/star.png'
												: '/Content/star_disabled.png'
										}
									/>
								</React.Fragment>
							))}
					</span>

					{albumDetailsStore.reviewsStore.editReviewStore === review ? (
						<form
							onSubmit={(e): void => {
								e.preventDefault();

								albumDetailsStore.reviewsStore.saveEditedReview();
							}}
						>
							<input
								value={review.editedTitle}
								onChange={(e): void =>
									runInAction(() => {
										review.editedTitle = e.target.value;
									})
								}
								type="text"
								className="input-xlarge"
								maxLength={200}
								placeholder="Title" /* TODO: localize */
							/>
							<br />
							<textarea
								value={review.editedText}
								onChange={(e): void =>
									runInAction(() => {
										review.editedText = e.target.value;
									})
								}
								rows={6}
								cols={100}
								maxLength={4000}
								className="input-xxlarge"
								placeholder="Review" /* TODO: localize */
								required
							/>
							<br />
							<Button type="submit" variant="primary">
								{t('ViewRes:Shared.Save')}
							</Button>{' '}
							<Button onClick={albumDetailsStore.reviewsStore.cancelEditReview}>
								{t('ViewRes:Shared.Cancel')}
							</Button>
						</form>
					) : (
						<div>
							{review.title && (
								<h4 className="album-review-title">{review.title}</h4>
							)}
							<p>
								<Markdown>{review.text}</Markdown>
							</p>
						</div>
					)}
				</div>
			</div>
		);
	},
);

interface AlbumReviewsProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumReviews = observer(
	({ model, albumDetailsStore }: AlbumReviewsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Album']);

		React.useEffect(() => {
			albumDetailsStore.reviewsStore.loadReviews();
		}, [albumDetailsStore]);

		return (
			<AlbumDetailsTabs
				model={model}
				albumDetailsStore={albumDetailsStore}
				tab="reviews"
			>
				{loginManager.canCreateComments ? (
					!albumDetailsStore.reviewsStore.showCreateNewReview && (
						<Button
							onClick={(): void =>
								runInAction(() => {
									albumDetailsStore.reviewsStore.showCreateNewReview = true;
								})
							}
							className="create-topic"
						>
							<i className="icon-comment" />{' '}
							{t('ViewRes.Album:Details.ReviewWrite')}
						</Button>
					)
				) : (
					<Alert variant="info">
						{t('ViewRes.Album:Details.ReviewLoginToReview')}
					</Alert>
				)}

				{albumDetailsStore.reviewsStore.showCreateNewReview && (
					<form
						onSubmit={(e): void => {
							e.preventDefault();

							albumDetailsStore.reviewsStore.createNewReview();
						}}
					>
						<div>
							<label>{t('ViewRes.Album:Details.ReviewLanguage')}</label>
							<UserLanguageCultureDropdownList
								placeholder="Choose" /* TODO: localize */
								value={albumDetailsStore.reviewsStore.languageCode}
								onChange={(e): void =>
									runInAction(() => {
										albumDetailsStore.reviewsStore.languageCode =
											e.target.value;
									})
								}
								className="input-xlarge"
								required
							/>

							{albumDetailsStore.reviewsStore.reviewAlreadySubmitted && (
								<Alert variant="danger">
									{t('ViewRes.Album:Details.ReviewAlreadySubmitted')}
								</Alert>
							)}

							<label>{t('ViewRes.Album:Details.ReviewTitle')}</label>
							<input
								value={albumDetailsStore.reviewsStore.newReviewTitle}
								onChange={(e): void =>
									runInAction(() => {
										albumDetailsStore.reviewsStore.newReviewTitle =
											e.target.value;
									})
								}
								type="text"
								className="input-xlarge"
								maxLength={200}
							/>

							<label>{t('ViewRes.Album:Details.ReviewText')}</label>
							<MarkdownNotice />
							<br />
							<textarea
								value={albumDetailsStore.reviewsStore.newReviewText}
								onChange={(e): void =>
									runInAction(() => {
										albumDetailsStore.reviewsStore.newReviewText =
											e.target.value;
									})
								}
								className="input-xxlarge"
								cols={100}
								rows={6}
								maxLength={4000}
								minLength={50}
								required
							/>

							<div>
								<label>{t('ViewRes.Album:Details.ReviewPreview')}</label>
								<div>
									<Markdown>
										{albumDetailsStore.reviewsStore.newReviewText}
									</Markdown>
								</div>
							</div>
						</div>

						<Button
							type="submit"
							variant="primary"
							disabled={albumDetailsStore.reviewsStore.reviewAlreadySubmitted}
						>
							{t('ViewRes.Album:Details.ReviewPost')}
						</Button>
					</form>
				)}

				<div className="album-reviews">
					{albumDetailsStore.reviewsStore.reviews.map((review, index) => (
						<AlbumReview
							albumDetailsStore={albumDetailsStore}
							review={review}
							key={index}
						/>
					))}
				</div>
			</AlbumDetailsTabs>
		);
	},
);

export default AlbumReviews;
