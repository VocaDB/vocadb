import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { ArtistLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/ArtistLockingAutoComplete';
import { MergeEntryInfo } from '@/Components/Shared/Partials/Shared/MergeEntryInfo';
import { showErrorMessage } from '@/Components/ui';
import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { EntryType } from '@/Models/EntryType';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { ArtistMergeStore } from '@/Stores/Artist/ArtistMergeStore';
import { useVdb } from '@/VdbContext';
import { getReasonPhrase } from 'http-status-codes';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

interface ArtistMergeLayoutProps {
	artist: ArtistContract;
	artistMergeStore: ArtistMergeStore;
}

const ArtistMergeLayout = observer(
	({
		artist,
		artistMergeStore,
	}: ArtistMergeLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = `Merge artist - ${artist.name}`; /* LOC */

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
								to: '/Artist',
							}}
							divider
						>
							{t(`ViewRes:Shared.Artists`)}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details(EntryType.Artist, artist.id),
							}}
							divider
						>
							{artist.name}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Artist/Edit/${artist.id}`,
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
							if (!artistMergeStore.target.id) return;

							await artistMergeStore.submit(
								artistMergeStore.target.id,
							);

							navigate(`/Artist/Edit/${artistMergeStore.target.id}`);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to merge artist.' /* LOC */,
							);

							throw error;
						}
					}}
				>
					<MergeEntryInfo />

					<br />
					<div className="clearfix">
						<div className="pull-left">
							<ArtistLockingAutoComplete
								basicEntryLinkStore={artistMergeStore.target}
								properties={{ ignoreId: artist.id }}
							/>
						</div>
						<div className="pull-left entry-field-inline-warning">
							{artistMergeStore.validationError_targetIsNewer && (
								<Alert>
									<span className="icon-line errorIcon" />{' '}
									{t('ViewRes:EntryMerge.ValidationErrorTargetIsNewer')}
								</Alert>
							)}

							{artistMergeStore.validationError_targetIsLessComplete && (
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
						disabled={
							!artistMergeStore.target.id || artistMergeStore.submitting
						}
					>
						Merge{/* LOC */}
					</Button>
				</form>
			</Layout>
		);
	},
);

const ArtistMerge = (): React.ReactElement => {
	const vdb = useVdb();

	const { id } = useParams();

	const [model, setModel] = React.useState<{
		artist: ArtistContract;
		artistMergeStore: ArtistMergeStore;
	}>();

	React.useEffect(() => {
		artistRepo
			.getOne({ id: Number(id), lang: vdb.values.languagePreference })
			.then((artist) =>
				setModel({
					artist: artist,
					artistMergeStore: new ArtistMergeStore(
						vdb.values,
						artistRepo,
						artist,
					),
				}),
			);
	}, [vdb, id]);

	return model ? (
		<ArtistMergeLayout
			artist={model.artist}
			artistMergeStore={model.artistMergeStore}
		/>
	) : (
		<></>
	);
};

export default ArtistMerge;
