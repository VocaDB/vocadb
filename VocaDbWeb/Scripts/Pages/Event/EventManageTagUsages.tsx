import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { TagUsagesManageTable } from '@/Components/Shared/Partials/Tag/TagUsagesManageTable';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryWithTagUsagesForApiContract } from '@/DataContracts/Base/EntryWithTagUsagesForApiContract';
import { EntryType } from '@/Models/EntryType';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const EventManageTagUsages = (): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);
	const { id } = useParams();
	const [event, setEvent] = useState<
		EntryWithTagUsagesForApiContract | undefined
	>(undefined);

	const title = `Manage tag usages - ${event?.defaultName}`; /* LOCALIZE */
	useVdbTitle(title);

	useEffect(() => {
		eventRepo
			.getTagUsages({ eventId: Number(id) })
			.then((resp) => setEvent(resp));
	}, [id]);

	if (!event) {
		return <></>;
	}

	return (
		<Layout
			pageTitle={title}
			title={title}
			ready={true}
			parents={
				<>
					<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/Event' }} divider>
						{t('ViewRes:Shared.ReleaseEvents')}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.ReleaseEvent, event.id),
						}}
					>
						{event.defaultName}
					</Breadcrumb.Item>
				</>
			}
		>
			<TagUsagesManageTable
				entryType={EntryType.ReleaseEvent}
				tagUsages={event.tagUsages}
				setTagUsages={(tagUsages): void =>
					setEvent({
						...event,
						tagUsages,
					})
				}
				canRemove={event.canRemoveTagUsages}
			/>
		</Layout>
	);
};

export default EventManageTagUsages;
