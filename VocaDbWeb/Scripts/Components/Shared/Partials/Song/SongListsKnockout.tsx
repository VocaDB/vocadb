import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { SongListsBaseStore } from '@/Stores/SongList/SongListsBaseStore';
import dayjs from '@/dayjs';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { Link } from 'react-router-dom';

interface SongListsKnockoutProps {
	songListsBaseStore: SongListsBaseStore;
	groupByYear: boolean;
}

export const SongListsKnockout = observer(
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
											{dayjs(item.eventDate).format('YYYY')}
										</h3>
									</td>
								</tr>
							)}
							<tr>
								<td style={{ width: '75px' }}>
									{item.mainPicture && item.mainPicture.urlSmallThumb && (
										<Link
											to={EntryUrlMapper.details(EntryType.SongList, item.id)}
										>
											{/* eslint-disable-next-line jsx-a11y/img-redundant-alt */}
											<img
												className="coverPicThumb"
												alt="Picture" /* LOC */
												src={item.mainPicture.urlSmallThumb}
											/>
										</Link>
									)}
								</td>
								<td>
									<Link
										to={EntryUrlMapper.details(EntryType.SongList, item.id)}
									>
										{item.name}
									</Link>
									{item.eventDate && (
										<div>
											<small>{dayjs(item.eventDate).format('l')}</small>
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
