import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { TagUsagesManageTable } from '@/Components/Shared/Partials/Tag/TagUsagesManageTable';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryWithTagUsagesForApiContract } from '@/DataContracts/Base/EntryWithTagUsagesForApiContract';
import { EntryType } from '@/Models/EntryType';
import { songRepo } from '@/Repositories/SongRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const SongManageTagUsages = (): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);
	const { id } = useParams();
	const [song, setSong] = useState<
		EntryWithTagUsagesForApiContract | undefined
	>(undefined);

	const title = `Manage tag usages - ${song?.defaultName}`; /* LOCALIZE */
	useVdbTitle(title);

	useEffect(() => {
		songRepo.getTagUsages({ songId: Number(id) }).then((resp) => setSong(resp));
	}, [id]);

	if (!song) {
		return <></>;
	}

	return (
		<Layout
			pageTitle={title}
			title={title}
			ready={true}
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
				setTagUsages={(tagUsages): void =>
					setSong({
						...song,
						tagUsages,
					})
				}
				canRemove={song.canRemoveTagUsages}
			/>
		</Layout>
	);
};

export default SongManageTagUsages;
