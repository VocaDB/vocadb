import { EntryLink } from '@/Components/Shared/Partials/Shared/EntryLink';
import { UserIconLink_UserForApiContract } from '@/Components/Shared/Partials/User/UserIconLink_UserForApiContract';
import JQueryUIDialog from '@/JQueryUI/JQueryUIDialog';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { SongInListsStore } from '@/Stores/Song/SongDetailsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface SongInListsDialogProps {
	songInListsStore: SongInListsStore;
}

const SongInListsDialog = observer(
	({ songInListsStore }: SongInListsDialogProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Song', 'Resources']);

		const byCategory = songInListsStore.listsForSong
			.filter((l) => l.featuredCategory !== SongListFeaturedCategory.Nothing)
			.groupBy((l) => l.featuredCategory);
		const customLists = songInListsStore.listsForSong.filter(
			(l) => l.featuredCategory === SongListFeaturedCategory.Nothing,
		);

		return (
			<JQueryUIDialog
				title={t('ViewRes.Song:Details.SongInLists')}
				autoOpen={songInListsStore.dialogVisible}
				width={400}
				close={(): void =>
					runInAction(() => {
						songInListsStore.dialogVisible = false;
					})
				}
			>
				{Object.keys(byCategory).map((category, index) => (
					<React.Fragment key={index}>
						<h4>{t(`Resources:SongListFeaturedCategoryNames.${category}`)}</h4>
						{byCategory[category].map((song, index) => (
							<div>
								<Link to={`/L/${song.id}`} key={index}>
									{song.name}
								</Link>
							</div>
						))}
					</React.Fragment>
				))}
				{customLists.length > 0 && (
					<>
						<h4>{t(`ViewRes.Song:Details.CustomLists`)}</h4>
						{customLists.map((list, index) => (
							<div key={index}>
								<Link to={`/L/${list.id}`}>{list.name}</Link>
								{' ('}
								{/* eslint-disable-next-line react/jsx-pascal-case */}
								<UserIconLink_UserForApiContract user={list.author} />
								{' )'}
							</div>
						))}
					</>
				)}
			</JQueryUIDialog>
		);
	},
);

export default SongInListsDialog;
