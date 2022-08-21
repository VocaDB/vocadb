import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { ReactSortable } from 'react-sortablejs';

const PlaylistIndex = observer(
	(): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes', 'ViewRes.Search']);

		const title = t('ViewRes.Search:Index.Playlist');

		useVocaDbTitle(title, ready);

		const { vdbPlayer } = useVdbPlayer();

		return (
			<Layout title={title}>
				<table>
					<ReactSortable
						tag="tbody"
						list={vdbPlayer.playQueue.items}
						setList={(items): void =>
							runInAction(() => {
								vdbPlayer.playQueue.items = items;
							})
						}
						handle=".handle"
					>
						{vdbPlayer.playQueue.items.map((item) => (
							<tr className="ui-state-default" key={item.id}>
								<td style={{ cursor: 'move' }} className="handle">
									<span className="ui-icon ui-icon-arrowthick-2-n-s" />
								</td>
								<td>
									<SafeAnchor
										onClick={(): void => vdbPlayer.play(item)}
										href="#"
										className="iconLink playLink"
										title="Play" /* TODO: localize */
									>
										Play{/* TODO: localize */}
									</SafeAnchor>
								</td>
								<td>
									<Link to={EntryUrlMapper.details_entry(item.entry)}>
										{item.entry.name}
									</Link>
								</td>
								<td>
									<SafeAnchor
										onClick={(): void => vdbPlayer.removeFromQueue(item)}
										href="#"
										className="iconLink removeLink"
										title="Remove from playlist" /* TODO: localize */
									>
										{t('ViewRes:Shared.Remove')}
									</SafeAnchor>
								</td>
							</tr>
						))}
					</ReactSortable>
				</table>
			</Layout>
		);
	},
);

export default PlaylistIndex;
