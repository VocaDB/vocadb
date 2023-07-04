import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { SongEditStore } from '@/Stores/Song/SongEditStore';
import { observer } from 'mobx-react-lite';
import { useTranslation } from 'react-i18next';

import { ReleaseEventLockingAutoComplete } from './ReleaseEventLockingAutoComplete';

interface ReleaseEventsEditViewProps {
	songEditStore: SongEditStore;
}

export const ReleaseEventsEditView = observer(
	({ songEditStore }: ReleaseEventsEditViewProps) => {
		const { t } = useTranslation(['ViewRes', 'HelperRes']);

		const releaseEvents = songEditStore.releaseEvents;
		console.log(releaseEvents);

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
					<SafeAnchor
						href="#"
						className="textLink addLink"
						onClick={songEditStore.addReleaseEvent}
					>
						{t('HelperRes:Helper.WebLinkNewRow')}
					</SafeAnchor>
				</tr>
			</tbody>
		);
	},
);
