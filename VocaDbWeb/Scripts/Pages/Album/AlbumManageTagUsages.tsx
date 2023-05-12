import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { TagUsagesManageTable } from '@/Components/Shared/Partials/Tag/TagUsagesManageTable';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryWithTagUsagesForApiContract } from '@/DataContracts/Base/EntryWithTagUsagesForApiContract';
import { EntryType } from '@/Models/EntryType';
import { albumRepo } from '@/Repositories/AlbumRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const AlbumManageTagUsages = (): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);
	const { id } = useParams();
	const [album, setAlbum] = useState<
		EntryWithTagUsagesForApiContract | undefined
	>(undefined);

	const title = `Manage tag usages - ${album?.defaultName}`; /* LOCALIZE */
	useVdbTitle(title);

	useEffect(() => {
		albumRepo
			.getTagUsages({ albumId: Number(id) })
			.then((resp) => setAlbum(resp));
	}, [id]);

	if (!album) {
		return <></>;
	}

	return (
		<Layout
			pageTitle={title}
			title={title}
			ready={true}
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
				setTagUsages={(tagUsages): void =>
					setAlbum({
						...album,
						tagUsages,
					})
				}
				canRemove={album.canRemoveTagUsages}
			/>
		</Layout>
	);
};

export default AlbumManageTagUsages;
