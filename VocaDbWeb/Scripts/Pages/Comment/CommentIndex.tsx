import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { CommentBodyLarge } from '@/Components/Shared/Partials/Comment/CommentBodyLarge';
import { CommentEntryItem } from '@/Components/Shared/Partials/Comment/CommentEntryItem';
import { Dropdown } from '@/Components/Shared/Partials/Knockout/Dropdown';
import { CommentTargetTypeDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { UserLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/UserLockingAutoComplete';
import { EntryWithCommentsContract } from '@/DataContracts/EntryWithCommentsContract';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import { useMutedUsers } from '@/MutedUsersContext';
import { userRepo } from '@/Repositories/UserRepository';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import {
	CommentListStore,
	CommentSortRule,
} from '@/Stores/Comment/CommentListStore';
import { useVdb } from '@/VdbContext';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface CommentsFiltersProps {
	commentListStore: CommentListStore;
}

const CommentsFilters = observer(
	({ commentListStore }: CommentsFiltersProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes', 'ViewRes.User']);

		return (
			<div className="form-horizontal well well-transparent">
				<div className="pull-right">
					<div className="inline-block">
						{t('ViewRes:EntryIndex.SortBy')}{' '}
						<Dropdown
							items={Object.fromEntries(
								Object.values(CommentSortRule).map((value) => [
									value,
									t(`Resources:CommentSortRuleNames.${value}`),
								]),
							)}
							value={commentListStore.sort}
							onChange={(value): void =>
								runInAction(() => {
									commentListStore.sort = value as CommentSortRule;
								})
							}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">
						{t('ViewRes.User:EntryEdits.EntryType')}
					</div>
					<div className="controls">
						<CommentTargetTypeDropdownList
							value={commentListStore.entryType}
							onChange={(e): void =>
								runInAction(() => {
									commentListStore.entryType = e.target.value as EntryType;
								})
							}
						/>
					</div>
				</div>

				<div className="control-group">
					<div className="control-label">User{/* LOC */}</div>
					<div className="controls">
						<UserLockingAutoComplete
							basicEntryLinkStore={commentListStore.user}
						/>
					</div>
				</div>
			</div>
		);
	},
);

interface CommentWithEntryProps {
	entry: EntryWithCommentsContract;
}

const CommentWithEntry = observer(
	({ entry }: CommentWithEntryProps): React.ReactElement => {
		const mutedUsers = useMutedUsers();
		if (
			entry.comments.every((comment) => mutedUsers.includes(comment.author.id))
		) {
			return <></>;
		}

		return (
			<div className="row-fluid comment-with-entry well well-transparent">
				<div className="span5">
					{entry.comments.map((comment) => (
						<CommentBodyLarge
							contract={comment}
							allowDelete={false}
							key={comment.id}
						/>
					))}
				</div>

				<div className="span5 item">
					<CommentEntryItem entry={entry.entry} />
				</div>
			</div>
		);
	},
);

interface CommentSearchListProps {
	commentListStore: CommentListStore;
}

const CommentSearchList = observer(
	({ commentListStore }: CommentSearchListProps): React.ReactElement => {
		return (
			<>
				{commentListStore.entries.map((entry, index) => (
					<CommentWithEntry entry={entry} key={index} />
				))}
			</>
		);
	},
);

const CommentIndex = observer(
	(): React.ReactElement => {
		const vdb = useVdb();
		const loginManager = useLoginManager();

		const [commentListStore] = React.useState(
			() => new CommentListStore(vdb.values, httpClient, urlMapper, userRepo),
		);

		const { t, ready } = useTranslation([
			'ViewRes.Comment',
			'VocaDb.Web.Resources.Views.ActivityEntry',
		]);

		const title = t('ViewRes.Comment:Index.RecentComments');

		useLocationStateStore(commentListStore);

		return (
			<Layout pageTitle={title} ready={ready} title={title}>
				<ul className="nav nav-pills">
					<li>
						<Link to="/ActivityEntry">All activity{/* LOC */}</Link>
					</li>
					{loginManager.isLoggedIn && (
						<li>
							<Link to="/ActivityEntry/FollowedArtistActivity">
								Only followed artists
							</Link>
						</li>
					)}
					<li className="active">
						<Link to="/Comment">Comments{/* LOC */}</Link>
					</li>
				</ul>

				<CommentsFilters commentListStore={commentListStore} />

				<CommentSearchList commentListStore={commentListStore} />

				<hr />
				<h3>
					<SafeAnchor onClick={commentListStore.loadMore}>
						{t('VocaDb.Web.Resources.Views.ActivityEntry:Index.ViewMore')}
					</SafeAnchor>
				</h3>
			</Layout>
		);
	},
);

export default CommentIndex;
