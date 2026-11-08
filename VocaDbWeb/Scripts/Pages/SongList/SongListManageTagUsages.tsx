import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { TagUsagesManageTable } from '@/Components/Shared/Partials/Tag/TagUsagesManageTable';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryWithTagUsagesForApiContract } from '@/DataContracts/Base/EntryWithTagUsagesForApiContract';
import { EntryType } from '@/Models/EntryType';
import { songListRepo } from '@/Repositories/SongListRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const SongListManageTagUsages = (): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);
	const { id } = useParams();
	const [songList, setSongList] = useState<
		EntryWithTagUsagesForApiContract | undefined
	>(undefined);

	const title = `Manage tag usages - ${songList?.defaultName}`;
	useVdbTitle(title);

	useEffect(() => {
		songListRepo
			.getTagUsages({ songListId: Number(id) })
			.then((resp) => setSongList(resp));
	}, [id]);

	if (!songList) {
		return <></>;
	}

	return (
		<Layout
			pageTitle={title}
			title={title}
			ready={true}
			parents={
				<>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{ to: '/SongList/Featured' }}
						divider
					>
						{t('ViewRes:Shared.FeaturedSongLists')}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: EntryUrlMapper.details(EntryType.SongList, songList.id),
						}}
					>
						{songList.defaultName}
					</Breadcrumb.Item>
				</>
			}
		>
			<TagUsagesManageTable
				entryType={EntryType.SongList}
				entryId={songList.id}
				tagUsages={songList.tagUsages}
				setTagUsages={(tagUsages): void =>
					setSongList({
						...songList,
						tagUsages,
					})
				}
				canRemove={songList.canRemoveTagUsages}
			/>
		</Layout>
	);
};

export default SongListManageTagUsages;
