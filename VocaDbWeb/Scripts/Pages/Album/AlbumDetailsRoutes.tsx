import AlbumDetailsForApi from '@/DataContracts/Album/AlbumDetailsForApi';
import { JQueryUINavItemComponent } from '@/JQueryUI/JQueryUITabs';
import EntryType from '@/Models/EntryType';
import AlbumBasicInfo from '@/Pages/Album/AlbumBasicInfo';
import AlbumDiscussion from '@/Pages/Album/AlbumDiscussion';
import AlbumPictures from '@/Pages/Album/AlbumPictures';
import AlbumRelated from '@/Pages/Album/AlbumRelated';
import AlbumReviews from '@/Pages/Album/AlbumReviews';
import AlbumShare from '@/Pages/Album/AlbumShare';
import EntryUrlMapper from '@/Shared/EntryUrlMapper';
import AlbumDetailsStore from '@/Stores/Album/AlbumDetailsStore';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, Route, Routes } from 'react-router-dom';

interface AlbumDetailsTabsProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
	tab:
		| 'basicInfo'
		| 'discussion'
		| 'reviews'
		| 'pictures'
		| 'related'
		| 'share';
	children?: React.ReactNode;
}

export const AlbumDetailsTabs = React.memo(
	({
		model,
		albumDetailsStore,
		tab,
		children,
	}: AlbumDetailsTabsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Album']);

		return (
			<div className="js-cloak ui-tabs ui-widget ui-widget-content ui-corner-all">
				<ul
					className="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all"
					role="tablist"
				>
					<JQueryUINavItemComponent active={tab === 'basicInfo'}>
						<Link to={`${EntryUrlMapper.details(EntryType.Album, model.id)}`}>
							{t('ViewRes:EntryDetails.BasicInfoTab')}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'discussion'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Album,
								model.id,
							)}/discussion`}
						>
							{`${t('ViewRes:EntryDetails.DiscussionTab')} (${
								model.commentCount
							})`}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'reviews'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Album,
								model.id,
							)}/reviews`}
						>
							{`${t('ViewRes.Album:Details.ReviewsTab')} (${
								model.reviewCount
							})`}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'pictures'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Album,
								model.id,
							)}/pictures`}
						>
							{`${t('ViewRes:EntryDetails.PicturesTab')} (${
								model.pictures.length + 1
							})`}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'related'}>
						<Link
							to={`${EntryUrlMapper.details(
								EntryType.Album,
								model.id,
							)}/related`}
						>
							{t('ViewRes.Album:Details.RelatedAlbums')}
						</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'share'}>
						<Link
							to={`${EntryUrlMapper.details(EntryType.Album, model.id)}/share`}
						>
							{t('ViewRes.Album:Details.Share')}
						</Link>
					</JQueryUINavItemComponent>
				</ul>

				<div
					className="ui-tabs-panel ui-widget-content ui-corner-bottom"
					role="tabpanel"
				>
					{children}
				</div>
			</div>
		);
	},
);

interface AlbumDetailsRoutesProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumDetailsRoutes = ({
	model,
	albumDetailsStore,
}: AlbumDetailsRoutesProps): React.ReactElement => {
	return (
		<Routes>
			<Route
				path="discussion"
				element={
					<AlbumDiscussion
						model={model}
						albumDetailsStore={albumDetailsStore}
					/>
				}
			/>
			<Route
				path="reviews"
				element={
					<AlbumReviews model={model} albumDetailsStore={albumDetailsStore} />
				}
			/>
			<Route
				path="pictures"
				element={
					<AlbumPictures model={model} albumDetailsStore={albumDetailsStore} />
				}
			/>
			<Route
				path="related"
				element={
					<AlbumRelated model={model} albumDetailsStore={albumDetailsStore} />
				}
			/>
			<Route
				path="share"
				element={
					<AlbumShare model={model} albumDetailsStore={albumDetailsStore} />
				}
			/>
			<Route
				path="*"
				element={
					<AlbumBasicInfo model={model} albumDetailsStore={albumDetailsStore} />
				}
			/>
		</Routes>
	);
};

export default AlbumDetailsRoutes;
