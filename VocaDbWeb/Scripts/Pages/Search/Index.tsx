import React, { ReactElement, useEffect, useState } from 'react';
import Layout from '../../Shared/Layout';
import EntryType from '../../../wwwroot/Scripts/Models/EntryType';
import { useTranslation } from 'react-i18next';
import AnythingSearchList from './Partials/AnythingSearchList';
import ArtistSearchList from './Partials/ArtistSearchList';
import AlbumSearchList from './Partials/AlbumSearchList';
import SongSearchList from './Partials/SongSearchList';
import EventSearchList from './Partials/EventSearchList';
import TagSearchList from './Partials/TagSearchList';
import classNames from 'classnames';
import { usePrevious } from 'react-use';
import { pickBy } from 'lodash';
import { Inertia } from '@inertiajs/inertia';
import PartialFindResultContract from '../../../wwwroot/Scripts/DataContracts/PartialFindResultContract';
import SafeAnchor from '../../Bootstrap/SafeAnchor';
import ArtistSearchOptions from './Partials/ArtistSearchOptions';
import AlbumSearchOptions from './Partials/AlbumSearchOptions';
import SongSearchOptions from './Partials/SongSearchOptions';
import EventSearchOptions from './Partials/EventSearchOptions';

interface SearchIndexViewModel {
	filter: string;
	page: number;
	pageSize: number;
	searchType: string;
}

interface IndexProps {
	viewModel: SearchIndexViewModel;
	result: PartialFindResultContract<any/* TODO */>;
}

const Index = ({
	viewModel,
	result,
}: IndexProps): ReactElement => {
	const { t } = useTranslation(['ViewRes', 'ViewRes.Search', 'VocaDb.Web.Resources.Domain']);

	const [showAdvancedFilters, setShowAdvancedFilters] = useState(false);

	const showAnythingSearch = (): boolean => viewModel.searchType === EntryType[EntryType.Undefined];
	const showArtistSearch = (): boolean => viewModel.searchType === EntryType[EntryType.Artist];
	const showAlbumSearch = (): boolean => viewModel.searchType === EntryType[EntryType.Album];
	const showSongSearch = (): boolean => viewModel.searchType === EntryType[EntryType.Song];
	const showEventSearch = (): boolean => viewModel.searchType === EntryType[EntryType.ReleaseEvent];
	const showTagSearch = (): boolean => viewModel.searchType === EntryType[EntryType.Tag];
	const [showTags, setShowTags] = useState(false);

	const [values, setValues] = useState<SearchIndexViewModel>({
		filter: viewModel.filter,
		page: viewModel.page,
		pageSize: viewModel.pageSize,
		searchType: viewModel.searchType,
	});

	const prevValues = usePrevious(values);

	useEffect(() => {
		if (prevValues) {
			const query = Object.keys(pickBy(values)).length ? pickBy(values) : { remember: 'forget' };
			Inertia.get('/Search', query, {
				replace: true,
				preserveState: true,
				preserveScroll: true,
			});
		}
	}, [values]);

	interface SearchCategoryProps {
		entryType: EntryType;
		title: string;
	}

	const SearchCategory = ({
		entryType,
		title,
	}: SearchCategoryProps): ReactElement => {
		return (
			<li className={classNames(viewModel.searchType === EntryType[entryType] && 'active')}>
				<SafeAnchor onClick={() => setValues({ ...values, page: undefined/* REVIEW: React */, searchType: EntryType[entryType] })}>{title}</SafeAnchor>
			</li>
		);
	};

	const handleChange = (e/* TODO */) => {
		const key = e.target.name;
		const value = e.target.value;

		setValues(values => ({
			...values,
			[key]: value,
		}));
	};

	return (
		<Layout>
			<ul className="nav nav-pills">
				<SearchCategory entryType={EntryType.Undefined} title={t('VocaDb.Web.Resources.Domain:EntryTypeNames.Undefined')} />
				<SearchCategory entryType={EntryType.Artist} title={t('ViewRes:Shared.Artists')} />
				<SearchCategory entryType={EntryType.Album} title={t('ViewRes:Shared.Albums')} />
				<SearchCategory entryType={EntryType.Song} title={t('ViewRes:Shared.Songs')} />
				<SearchCategory entryType={EntryType.ReleaseEvent} title={t('ViewRes:Shared.ReleaseEvents')} />
				<SearchCategory entryType={EntryType.Tag} title={t('ViewRes:Shared.Tags')} />
			</ul>

			<div id="anythingSearchTab" className="form-horizontal well well-transparent">
				<div className="pull-right">
					{/* TODO */}

					<div className="inline-block" /* TODO */>
						<SafeAnchor onClick={() => setShowTags(!showTags)} className={classNames('btn', 'btn-nomargin', showTags && 'active')} title={t('ViewRes.Search:Index.ShowTags')}>
							<i className="icon-tags"></i>
						</SafeAnchor>
					</div>
				</div>

				<div className="control-label">
					<i className="icon-search"></i>
				</div>
				<div className="control-group">
					<div className="controls">
						<div className="input-append">
							<input type="text" name="filter" value={values.filter} onChange={handleChange} className="input-xlarge" placeholder={t('ViewRes.Search:Index.TypeSomething')} />
							{viewModel.filter.length > 0 && (
								<button className="btn btn-danger" onClick={() => setValues({ ...values, filter: '' })}>{t('ViewRes:Shared.Clear')}</button>
							)}
						</div>
						{' '}
						<button className={classNames('btn', showAdvancedFilters && 'active')} onClick={() => setShowAdvancedFilters(!showAdvancedFilters)}>
							{t('ViewRes.Search:Index.MoreFilters')}
							{' '}
							<span className="caret"></span>
						</button>
					</div>
				</div>

				{showAdvancedFilters && (
					<div>
						<div className="control-group" /* TODO */>
							<div className="control-label">{t('ViewRes:Shared.Tag')}</div>
							<div className="controls">
								{/* TODO */}
							</div>
						</div>

						{showArtistSearch() && (
							<ArtistSearchOptions />
						)}

						{showAlbumSearch() && (
							<AlbumSearchOptions />
						)}

						{showSongSearch() && (
							<SongSearchOptions />
						)}

						{showEventSearch() && (
							<EventSearchOptions />
						)}

						{showTagSearch() && (
							<div className="control-group">
								<div className="control-label">{t('ViewRes.Search:Index.TagCategory')}</div>
								<div className="controls">
									<div className="input-append">
										{/* TODO */}
									</div>
								</div>
							</div>
						)}

						{/* TODO */}

						<div className="control-group">
							<div className="controls">
								{/* TODO */}

								<label className="checkbox" /* TODO */>
									<input type="checkbox" /* TODO */ />
									{t('ViewRes:EntryIndex.OnlyDrafts')}
								</label>
							</div>
						</div>
					</div>
				)}
			</div>

			{showAnythingSearch() && (
				<AnythingSearchList
					onPageChange={(page: number) => setValues({ ...values, page: page })}
					onPageSizeChange={(pageSize: number) => setValues({ ...values, page: undefined/* REVIEW: React */, pageSize: pageSize })}
					page={viewModel.page}
					pageSize={viewModel.pageSize}
					result={result}
				/>
			)}

			{showArtistSearch() && (
				<ArtistSearchList
					onPageChange={(page: number) => setValues({ ...values, page: page })}
					onPageSizeChange={(pageSize: number) => setValues({ ...values, page: undefined/* REVIEW: React */, pageSize: pageSize })}
					page={viewModel.page}
					pageSize={viewModel.pageSize}
					result={result}
				/>
			)}

			{showAlbumSearch() && (
				<AlbumSearchList
					onPageChange={(page: number) => setValues({ ...values, page: page })}
					onPageSizeChange={(pageSize: number) => setValues({ ...values, page: undefined/* REVIEW: React */, pageSize: pageSize })}
					page={viewModel.page}
					pageSize={viewModel.pageSize}
					result={result}
					unknownPictureUrl={'/Content/unknown.png'/* REVIEW: React */}
				/>
			)}

			{showSongSearch() && (
				<SongSearchList
					onPageChange={(page: number) => setValues({ ...values, page: page })}
					onPageSizeChange={(pageSize: number) => setValues({ ...values, page: undefined/* REVIEW: React */, pageSize: pageSize })}
					page={viewModel.page}
					pageSize={viewModel.pageSize}
					result={result}
				/>
			)}

			{showEventSearch() && (
				<EventSearchList
					onPageChange={(page: number) => setValues({ ...values, page: page })}
					onPageSizeChange={(pageSize: number) => setValues({ ...values, page: undefined/* REVIEW: React */, pageSize: pageSize })}
					page={viewModel.page}
					pageSize={viewModel.pageSize}
					result={result}
				/>
			)}

			{showTagSearch() && (
				<TagSearchList
					onPageChange={(page: number) => setValues({ ...values, page: page })}
					onPageSizeChange={(pageSize: number) => setValues({ ...values, page: undefined/* REVIEW: React */, pageSize: pageSize })}
					page={viewModel.page}
					pageSize={viewModel.pageSize}
					result={result}
				/>
			)}

			{/* TODO */}
		</Layout>
	);
};

export default Index;
