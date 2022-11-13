import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { TagLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/TagLockingAutoComplete';
import { MergeEntryInfo } from '@/Components/Shared/Partials/Shared/MergeEntryInfo';
import { showErrorMessage } from '@/Components/ui';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { EntryType } from '@/Models/EntryType';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { tagRepo } from '@/Repositories/TagRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { TagMergeStore } from '@/Stores/Tag/TagMergeStore';
import { getReasonPhrase } from 'http-status-codes';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

interface TagMergeLayoutProps {
	tag: TagBaseContract;
	tagMergeStore: TagMergeStore;
}

const TagMergeLayout = observer(
	({ tag, tagMergeStore }: TagMergeLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = `Merge tag - ${tag.name}`; /* LOC */

		useVdbTitle(title, true);

		const navigate = useNavigate();

		return (
			<Layout
				title={title}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: '/Tag',
							}}
							divider
						>
							{t(`ViewRes:Shared.Tags`)}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details(EntryType.Tag, tag.id),
							}}
							divider
						>
							{tag.name}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Tag/Edit/${tag.id}`,
							}}
						>
							Edit{/* LOC */}
						</Breadcrumb.Item>
					</>
				}
			>
				<form
					onSubmit={async (e): Promise<void> => {
						e.preventDefault();

						try {
							if (!tagMergeStore.target.id) return;

							const requestToken = await antiforgeryRepo.getToken();

							await tagMergeStore.submit(requestToken, tagMergeStore.target.id);

							navigate(`/Tag/Edit/${tagMergeStore.target.id}`);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to merge tag.' /* LOC */,
							);

							throw error;
						}
					}}
				>
					<MergeEntryInfo />

					<br />
					<div className="clearfix">
						<div className="pull-left">
							<TagLockingAutoComplete
								basicEntryLinkStore={tagMergeStore.target}
								tagFilter={tagMergeStore.tagFilter}
							/>
						</div>
						<div className="pull-left entry-field-inline-warning">
							{tagMergeStore.validationError_targetIsNewer && (
								<Alert>
									<span className="icon-line errorIcon" />{' '}
									{t('ViewRes:EntryMerge.ValidationErrorTargetIsNewer')}
								</Alert>
							)}

							{tagMergeStore.validationError_targetIsLessComplete && (
								<Alert>
									<span className="icon-line errorIcon" />{' '}
									{t('ViewRes:EntryMerge.ValidationErrorTargetIsLessComplete')}
								</Alert>
							)}
						</div>
					</div>

					<br />

					<Button
						type="submit"
						variant="primary"
						id="mergeBtn"
						disabled={!tagMergeStore.target.id || tagMergeStore.submitting}
					>
						Merge{/* LOC */}
					</Button>
				</form>
			</Layout>
		);
	},
);

const TagMerge = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<{
		tag: TagBaseContract;
		tagMergeStore: TagMergeStore;
	}>();

	React.useEffect(() => {
		tagRepo
			.getById({ id: Number(id), lang: vdb.values.languagePreference })
			.then((tag) =>
				setModel({
					tag: tag,
					tagMergeStore: new TagMergeStore(tagRepo, tag),
				}),
			);
	}, [id]);

	return model ? (
		<TagMergeLayout tag={model.tag} tagMergeStore={model.tagMergeStore} />
	) : (
		<></>
	);
};

export default TagMerge;
