import SafeAnchor from '@Bootstrap/SafeAnchor';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import SongListsBaseStore from '@Stores/SongList/SongListsBaseStore';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import React from 'react';

interface SongListsKnockoutProps {
	songListsBaseStore: SongListsBaseStore;
	groupByYear: boolean;
}

const SongListsKnockout = observer(
	({
		songListsBaseStore,
		groupByYear,
	}: SongListsKnockoutProps): React.ReactElement => {
		return (
			<table className="table table-striped">
				<tbody>
					{songListsBaseStore.items.map((item, index) => (
						<React.Fragment key={item.id}>
							{groupByYear && songListsBaseStore.isFirstForYear(item, index) && (
								<tr>
									<td colSpan={3}>
										<h3 className="song-list-year">
											{moment(item.eventDate).format('YYYY')}
										</h3>
									</td>
								</tr>
							)}
							<tr>
								<td style={{ width: '75px' }}>
									{item.mainPicture && item.mainPicture.urlSmallThumb && (
										<a
											href={EntryUrlMapper.details(EntryType.SongList, item.id)}
										>
											{/* eslint-disable-next-line jsx-a11y/img-redundant-alt */}
											<img
												className="coverPicThumb"
												alt="Picture" /* TODO: localize */
												src={item.mainPicture.urlSmallThumb}
											/>
										</a>
									)}
								</td>
								<td>
									<a href={EntryUrlMapper.details(EntryType.SongList, item.id)}>
										{item.name}
									</a>
									{item.eventDate && (
										<div>
											<small>{moment(item.eventDate).format('l')}</small>
										</div>
									)}
								</td>
								{songListsBaseStore.showTags && item.tags && (
									<td className="search-tags-column">
										{item.tags.length > 0 && (
											<div>
												<i className="icon icon-tags fix-icon-margin" />{' '}
												{item.tags.map((tag, index) => (
													<React.Fragment key={tag.tag.id}>
														{index > 0 && ', '}
														<SafeAnchor
															onClick={(): void =>
																songListsBaseStore.selectTag(tag.tag)
															}
														>
															{tag.tag.name}
														</SafeAnchor>
													</React.Fragment>
												))}
											</div>
										)}
									</td>
								)}
							</tr>
						</React.Fragment>
					))}
				</tbody>
			</table>
		);
	},
);

export default SongListsKnockout;
