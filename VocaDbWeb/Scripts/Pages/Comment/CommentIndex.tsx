import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { CommentBodyLarge } from '@/Components/Shared/Partials/Comment/CommentBodyLarge';
import { CommentEntryItem } from '@/Components/Shared/Partials/Comment/CommentEntryItem';
import { Dropdown } from '@/Components/Shared/Partials/Knockout/Dropdown';
import { CommentTargetTypeDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { UserLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/UserLockingAutoComplete';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { EntryWithCommentsContract } from '@/DataContracts/EntryWithCommentsContract';
import { LoginManager } from '@/Models/LoginManager';
import { useMutedUsers } from '@/MutedUsersContext';
import { UserRepository } from '@/Repositories/UserRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import {
	CommentListStore,
	CommentSortRule,
} from '@/Stores/Comment/CommentListStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const userRepo = new UserRepository(httpClient, urlMapper);

const commentListStore = new CommentListStore(
	vdb.values,
	httpClient,
	urlMapper,
	userRepo,
);

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
									commentListStore.entryType = e.target.value;
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
		const { t, ready } = useTranslation([
			'ViewRes.Comment',
			'VocaDb.Web.Resources.Views.ActivityEntry',
		]);

		const title = t('ViewRes.Comment:Index.RecentComments');

		useVdbTitle(title, ready);

		useLocationStateStore(commentListStore);

		return (
			<Layout title={title}>
				<ul className="nav nav-pills">
					<li>
						<Link to="/ActivityEntry">All activity{/* LOC */}</Link>
					</li>
					{loginManager.isLoggedIn && (
						<li>
							<a href="/ActivityEntry/FollowedArtistActivity">
								Only followed artists
							</a>
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
