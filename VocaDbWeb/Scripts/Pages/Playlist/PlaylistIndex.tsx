import { Layout } from '@/Components/Shared/Layout';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const PlaylistIndex = observer(
	(): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes.Search']);

		const title = t('ViewRes.Search:Index.Playlist');

		useVocaDbTitle(title, ready);

		const vdbPlayer = useVdbPlayer();

		return (
			<Layout title={title}>
				<table>
					<tbody>
						{vdbPlayer.playQueue.items.map((item) => (
							<tr key={item.id}>
								<td>
									<Link to={EntryUrlMapper.details_entry(item.entry)}>
										{item.entry.name}
									</Link>
								</td>
							</tr>
						))}
					</tbody>
				</table>
			</Layout>
		);
	},
);

export default PlaylistIndex;
