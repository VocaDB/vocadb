import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { Layout } from '@/Components/Shared/Layout';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import RatedSongs from '@/Pages/User/Partials/RatedSongs';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { tagRepo } from '@/Repositories/TagRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { urlMapper } from '@/Shared/UrlMapper';
import { RatedSongsSearchStore } from '@/Stores/User/RatedSongsSearchStore';
import { useVdb } from '@/VdbContext';
import { useLocationStateStore } from '@vocadb/route-sphere';
import React from 'react';
import { Link, useParams } from 'react-router-dom';

import '@/styles/Styles/songlist.less'

interface UserFavoriteSongsLayoutProps {
	user: UserDetailsContract;
	ratedSongsStore: RatedSongsSearchStore;
}

const UserFavoriteSongsLayout = ({
	user,
	ratedSongsStore,
}: UserFavoriteSongsLayoutProps): React.ReactElement => {
	const title = `Songs rated by ${user.name}`; /* LOC */

	useLocationStateStore(ratedSongsStore);

	return (
		<Layout
			pageTitle={title}
			ready={true}
			title={title}
			parents={
				<>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{ to: EntryUrlMapper.details_user_byName(user.name) }}
					>
						{user.name}
					</Breadcrumb.Item>
				</>
			}
		>
			<RatedSongs ratedSongsStore={ratedSongsStore} />
		</Layout>
	);
};

const UserFavoriteSongs = (): React.ReactElement => {
	const vdb = useVdb();

	const { id } = useParams();

	const [model, setModel] = React.useState<{
		user: UserDetailsContract;
		ratedSongsStore: RatedSongsSearchStore;
	}>();

	React.useEffect(() => {
		userRepo
			.getOne({ id: Number(id) })
			.then((user) => userRepo.getDetails({ name: user.name! }))
			.then((user) =>
				setModel({
					user: user,
					ratedSongsStore: new RatedSongsSearchStore(
						vdb.values,
						urlMapper,
						userRepo,
						artistRepo,
						songRepo,
						tagRepo,
						user.id,
						true,
					),
				}),
			)
			.catch((error) => {
				if (error.response) {
					if (error.response.status === 404)
						window.location.href = '/Error/NotFound';
				}

				throw error;
			});
	}, [vdb, id]);

	return model ? (
		<UserFavoriteSongsLayout
			user={model.user}
			ratedSongsStore={model.ratedSongsStore}
		/>
	) : (
		<></>
	);
};

export default UserFavoriteSongs;
