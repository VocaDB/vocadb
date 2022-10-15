import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { TagUsagesManageTable } from '@/Components/Shared/Partials/Tag/TagUsagesManageTable';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface EventManageTagUsagesLayoutProps {
	event: EntryWithTagUsagesContract;
}

const EventManageTagUsagesLayout = ({
	event,
}: EventManageTagUsagesLayoutProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	const title = `Manage tag usages - ${event.defaultName}`; /* LOCALIZE */

	useVdbTitle(title, true);

	return (
		<Layout
			title={title}
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
				canRemove={event.canRemoveTagUsages}
			/>
		</Layout>
	);
};

const EventManageTagUsages = (): React.ReactElement => {
	return <EventManageTagUsagesLayout event={{} /* TODO */} />;
};

export default EventManageTagUsages;
