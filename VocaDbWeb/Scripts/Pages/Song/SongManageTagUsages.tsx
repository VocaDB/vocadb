import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { TagUsagesManageTable } from '@/Components/Shared/Partials/Tag/TagUsagesManageTable';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface SongManageTagUsagesLayoutProps {
	song: EntryWithTagUsagesContract;
}

const SongManageTagUsagesLayout = ({
	song,
}: SongManageTagUsagesLayoutProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	const title = `Manage tag usages - ${song.defaultName}`; /* LOCALIZE */

	useVdbTitle(title, true);

	return (
		<Layout
			title={title}
			parents={
				<>
					<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/Song' }} divider>
						{t('ViewRes:Shared.Songs')}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.Song, song.id),
						}}
					>
						{song.defaultName}
					</Breadcrumb.Item>
				</>
			}
		>
			<TagUsagesManageTable
				entryType={EntryType.Song}
				tagUsages={song.tagUsages}
				canRemove={song.canRemoveTagUsages}
			/>
		</Layout>
	);
};

const SongManageTagUsages = (): React.ReactElement => {
	return <SongManageTagUsagesLayout song={{} /* TODO */} />;
};

export default SongManageTagUsages;
