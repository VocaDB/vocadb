import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { useLoginManager } from '@/LoginManagerContext';
import { AlbumEditStore } from '@/Stores/Album/AlbumEditStore';
import { SongEditStore } from '@/Stores/Song/SongEditStore';
import { observer } from 'mobx-react-lite';
import { useTranslation } from 'react-i18next';

import { ReleaseEventLockingAutoComplete } from './ReleaseEventLockingAutoComplete';

interface ReleaseEventsEditViewProps {
	editStore: SongEditStore | AlbumEditStore;
}

export const ReleaseEventsEditView = observer(
	({ editStore }: ReleaseEventsEditViewProps) => {
		const { t } = useTranslation(['ViewRes', 'HelperRes']);
		const loginManager = useLoginManager();

		const releaseEvents = editStore.releaseEvents;

		return (
			<tbody>
				{releaseEvents.map((e, key) => (
					<tr key={key}>
						<ReleaseEventLockingAutoComplete
							basicEntryLinkStore={e}
							// TODO: createNewItem="Create new event '{0}'" /* LOC */
						/>
					</tr>
				))}

				<tr>
					{(releaseEvents.length < 5 || loginManager.canApproveEntries) && (
						<SafeAnchor
							href="#"
							className="textLink addLink"
							onClick={editStore.addReleaseEvent}
						>
							{t('HelperRes:Helper.WebLinkNewRow')}
						</SafeAnchor>
					)}
				</tr>
			</tbody>
		);
	},
);
