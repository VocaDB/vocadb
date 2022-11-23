import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { SongListsKnockout } from '@/Components/Shared/Partials/Song/SongListsKnockout';
import { SongListsFilters } from '@/Components/Shared/Partials/SongListsFilters';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { loginManager } from '@/Models/LoginManager';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { songListRepo } from '@/Repositories/SongListRepository';
import { tagRepo } from '@/Repositories/TagRepository';
import { FeaturedSongListsStore } from '@/Stores/SongList/FeaturedSongListsStore';
import { useVdb } from '@/VdbContext';
import { useLocationStateStore } from '@vocadb/route-sphere';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const categories = Object.values(SongListFeaturedCategory).filter(
	(value) => value !== SongListFeaturedCategory.Nothing,
);

const SongListFeatured = observer(
	(): React.ReactElement => {
		const vdb = useVdb();

		const [featuredSongListsStore] = React.useState(
			() =>
				new FeaturedSongListsStore(
					vdb.values,
					songListRepo,
					tagRepo,
					[],
					categories,
				),
		);

		const { t, ready } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.SongList',
			'ViewRes.User',
		]);

		const title = t('ViewRes:Shared.FeaturedSongLists');

		useLocationStateStore(featuredSongListsStore);

		return (
			<Layout
				pageTitle={title}
				ready={ready}
				title={title}
				toolbar={
					<>
						{loginManager.canEditFeaturedLists && (
							<JQueryUIButton
								as={Link}
								to="/SongList/Edit"
								icons={{
									primary: 'ui-icon-plusthick',
								}}
							>
								{t('ViewRes.User:Details.CreateNewList')}
							</JQueryUIButton>
						)}
						{loginManager.canEditProfile && (
							<>
								{' '}
								<JQueryUIButton
									as={Link}
									to="/SongList/Import"
									icons={{
										primary: 'ui-icon-plusthick',
									}}
								>
									{t('ViewRes.SongList:Featured.Import')}
								</JQueryUIButton>
							</>
						)}
					</>
				}
			>
				<ul className="nav nav-pills">
					{categories.map((category) => (
						<li
							className={classNames(
								featuredSongListsStore.category === category && 'active',
							)}
							key={category}
						>
							<SafeAnchor
								onClick={(): void =>
									featuredSongListsStore.setCategory(category)
								}
							>
								{t(`Resources:SongListFeaturedCategoryNames.${category}`)}
							</SafeAnchor>
						</li>
					))}
				</ul>

				{categories.map((category) => (
					<React.Fragment key={category}>
						{featuredSongListsStore.category === category ? (
							<div>
								<SongListsFilters
									songListsBaseStore={
										featuredSongListsStore.categories[category]
									}
								/>

								<SongListsKnockout
									songListsBaseStore={
										featuredSongListsStore.categories[category]
									}
									groupByYear={true}
								/>

								{featuredSongListsStore.categories[category].hasMore && (
									<h3>
										<SafeAnchor
											onClick={(): void => {
												featuredSongListsStore.categories[category].loadMore();
											}}
										>
											{t('ViewRes:Shared.ShowMore')}
										</SafeAnchor>
									</h3>
								)}
							</div>
						) : (
							<></>
						)}
					</React.Fragment>
				))}
			</Layout>
		);
	},
);

export default SongListFeatured;
