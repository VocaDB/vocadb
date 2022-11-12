import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { SongListsKnockout } from '@/Components/Shared/Partials/Song/SongListsKnockout';
import { SongListsFilters } from '@/Components/Shared/Partials/SongListsFilters';
import { useVdbTitle } from '@/Components/useVdbTitle';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { LoginManager } from '@/Models/LoginManager';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { FeaturedSongListsStore } from '@/Stores/SongList/FeaturedSongListsStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const songListRepo = new SongListRepository(httpClient, urlMapper);
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

const categories = Object.values(SongListFeaturedCategory).filter(
	(value) => value !== SongListFeaturedCategory.Nothing,
);
const featuredSongListsStore = new FeaturedSongListsStore(
	vdb.values,
	songListRepo,
	tagRepo,
	[],
	categories,
);

const SongListFeatured = observer(
	(): React.ReactElement => {
		const { t, ready } = useTranslation([
			'Resources',
			'ViewRes',
			'ViewRes.SongList',
			'ViewRes.User',
		]);

		const title = t('ViewRes:Shared.FeaturedSongLists');

		useVdbTitle(title, ready);

		useLocationStateStore(featuredSongListsStore);

		return (
			<Layout
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
