import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { TagUsagesManageTable } from '@/Components/Shared/Partials/Tag/TagUsagesManageTable';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryWithTagUsagesForApiContract } from '@/DataContracts/Base/EntryWithTagUsagesForApiContract';
import { EntryType } from '@/Models/EntryType';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

const ArtistManageTagUsages = (): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);
	const { id } = useParams();
	const [artist, setArtist] = useState<
		EntryWithTagUsagesForApiContract | undefined
	>(undefined);

	const title = `Manage tag usages - ${artist?.defaultName}`; /* LOCALIZE */
	useVdbTitle(title);

	useEffect(() => {
		artistRepo
			.getTagUsages({ artistId: Number(id) })
			.then((resp) => setArtist(resp));
	}, [id]);

	if (!artist) {
		return <></>;
	}

	return (
		<Layout
			pageTitle={title}
			title={title}
			ready={true}
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
				setTagUsages={(tagUsages): void =>
					setArtist({
						...artist,
						tagUsages,
					})
				}
				canRemove={artist.canRemoveTagUsages}
			/>
		</Layout>
	);
};

export default ArtistManageTagUsages;
