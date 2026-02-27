import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { AlbumLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/AlbumLockingAutoComplete';
import { MergeEntryInfo } from '@/Components/Shared/Partials/Shared/MergeEntryInfo';
import { showErrorMessage } from '@/Components/ui';
import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { EntryType } from '@/Models/EntryType';
import { albumRepo } from '@/Repositories/AlbumRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { AlbumMergeStore } from '@/Stores/Album/AlbumMergeStore';
import { useVdb } from '@/VdbContext';
import { getReasonPhrase } from 'http-status-codes';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

interface AlbumMergeLayoutProps {
	album: AlbumContract;
	albumMergeStore: AlbumMergeStore;
}

const AlbumMergeLayout = observer(
	({ album, albumMergeStore }: AlbumMergeLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = `Merge album - ${album.name}`; /* LOC */

		const navigate = useNavigate();

		return (
			<Layout
				pageTitle={title}
				ready={true}
				title={title}
				parents={
					<>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: '/Album',
							}}
							divider
						>
							{t(`ViewRes:Shared.Albums`)}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details(EntryType.Album, album.id),
							}}
							divider
						>
							{album.name}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Album/Edit/${album.id}`,
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

						if (!albumMergeStore.target.id) return;

						try {

							await albumMergeStore.submit(
								albumMergeStore.target.id,
							);

							navigate(`/Album/Edit/${albumMergeStore.target.id}`);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to merge album.' /* LOC */,
							);

							throw error;
						}
					}}
				>
					<MergeEntryInfo />

					<br />
					<div className="clearfix">
						<div className="pull-left">
							<AlbumLockingAutoComplete
								basicEntryLinkStore={albumMergeStore.target}
								properties={{ ignoreId: album.id }}
							/>
						</div>
						<div className="pull-left entry-field-inline-warning">
							{albumMergeStore.validationError_targetIsNewer && (
								<Alert>
									<span className="icon-line errorIcon" />{' '}
									{t('ViewRes:EntryMerge.ValidationErrorTargetIsNewer')}
								</Alert>
							)}

							{albumMergeStore.validationError_targetIsLessComplete && (
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
						disabled={!albumMergeStore.target.id || albumMergeStore.submitting}
					>
						Merge{/* LOC */}
					</Button>
				</form>
			</Layout>
		);
	},
);

const AlbumMerge = (): React.ReactElement => {
	const vdb = useVdb();

	const { id } = useParams();

	const [model, setModel] = React.useState<{
		album: AlbumContract;
		albumMergeStore: AlbumMergeStore;
	}>();

	React.useEffect(() => {
		albumRepo
			.getOne({ id: Number(id), lang: vdb.values.languagePreference })
			.then((album) =>
				setModel({
					album: album,
					albumMergeStore: new AlbumMergeStore(vdb.values, albumRepo, album),
				}),
			);
	}, [vdb, id]);

	return model ? (
		<AlbumMergeLayout
			album={model.album}
			albumMergeStore={model.albumMergeStore}
		/>
	) : (
		<></>
	);
};

export default AlbumMerge;
