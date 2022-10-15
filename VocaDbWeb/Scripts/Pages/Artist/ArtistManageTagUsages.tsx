import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { TagUsagesManageTable } from '@/Components/Shared/Partials/Tag/TagUsagesManageTable';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface ArtistManageTagUsagesLayoutProps {
	artist: EntryWithTagUsagesContract;
}

const ArtistManageTagUsagesLayout = ({
	artist,
}: ArtistManageTagUsagesLayoutProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	const title = `Manage tag usages - ${artist.defaultName}`; /* LOCALIZE */

	useVdbTitle(title, true);

	return (
		<Layout
			title={title}
			parents={
				<>
					<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/Artist' }} divider>
						{t('ViewRes:Shared.Artists')}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.Artist, artist.id),
						}}
					>
						{artist.defaultName}
					</Breadcrumb.Item>
				</>
			}
		>
			<TagUsagesManageTable
				entryType={EntryType.Artist}
				tagUsages={artist.tagUsages}
				canRemove={artist.canRemoveTagUsages}
			/>
		</Layout>
	);
};

const ArtistManageTagUsages = (): React.ReactElement => {
	return <ArtistManageTagUsagesLayout artist={{} /* TODO */} />;
};

export default ArtistManageTagUsages;
