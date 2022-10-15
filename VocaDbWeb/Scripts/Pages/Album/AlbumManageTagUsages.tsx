import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { TagUsagesManageTable } from '@/Components/Shared/Partials/Tag/TagUsagesManageTable';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface AlbumManageTagUsagesLayoutProps {
	album: EntryWithTagUsagesContract;
}

const AlbumManageTagUsagesLayout = ({
	album,
}: AlbumManageTagUsagesLayoutProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	const title = `Manage tag usages - ${album.defaultName}`; /* LOCALIZE */

	useVdbTitle(title, true);

	return (
		<Layout
			title={title}
			parents={
				<>
					<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/Album' }} divider>
						{t('ViewRes:Shared.Albums')}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.Album, album.id),
						}}
					>
						{album.defaultName}
					</Breadcrumb.Item>
				</>
			}
		>
			<TagUsagesManageTable
				entryType={EntryType.Album}
				tagUsages={album.tagUsages}
				canRemove={album.canRemoveTagUsages}
			/>
		</Layout>
	);
};

const AlbumManageTagUsages = (): React.ReactElement => {
	return <AlbumManageTagUsagesLayout album={{} /* TODO */} />;
};

export default AlbumManageTagUsages;
