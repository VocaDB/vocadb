import Alert from '@/Bootstrap/Alert';
import Breadcrumb from '@/Bootstrap/Breadcrumb';
import Button from '@/Bootstrap/Button';
import { Layout } from '@/Components/Shared/Layout';
import { SongLockingAutoComplete } from '@/Components/Shared/Partials/Knockout/SongLockingAutoComplete';
import { MergeEntryInfo } from '@/Components/Shared/Partials/Shared/MergeEntryInfo';
import { showErrorMessage } from '@/Components/ui';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { EntryType } from '@/Models/EntryType';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { SongMergeStore } from '@/Stores/Song/SongMergeStore';
import { getReasonPhrase } from 'http-status-codes';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

interface SongMergeLayoutProps {
	song: SongContract;
	songMergeStore: SongMergeStore;
}

const SongMergeLayout = observer(
	({ song, songMergeStore }: SongMergeLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = `Merge song - ${song.name}`; /* LOC */

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
								to: '/Song',
							}}
							divider
						>
							{t(`ViewRes:Shared.Songs`)}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details(EntryType.Song, song.id),
							}}
							divider
						>
							{song.name}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Song/Edit/${song.id}`,
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
							if (!songMergeStore.target.id) return;

							const requestToken = await antiforgeryRepo.getToken();

							await songMergeStore.submit(
								requestToken,
								songMergeStore.target.id,
							);

							navigate(`/Song/Edit/${songMergeStore.target.id}`);
						} catch (error: any) {
							showErrorMessage(
								error.response && error.response.status
									? getReasonPhrase(error.response.status)
									: 'Unable to merge song.' /* LOC */,
							);

							throw error;
						}
					}}
				>
					<MergeEntryInfo />

					<br />
					<div className="clearfix">
						<div className="pull-left">
							<SongLockingAutoComplete
								basicEntryLinkStore={songMergeStore.target}
								ignoreId={song.id}
							/>
						</div>
						<div className="pull-left entry-field-inline-warning">
							{songMergeStore.validationError_targetIsNewer && (
								<Alert>
									<span className="icon-line errorIcon" />{' '}
									{t('ViewRes:EntryMerge.ValidationErrorTargetIsNewer')}
								</Alert>
							)}

							{songMergeStore.validationError_targetIsLessComplete && (
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
						disabled={!songMergeStore.target.id || songMergeStore.submitting}
					>
						Merge{/* LOC */}
					</Button>
				</form>
			</Layout>
		);
	},
);

const SongMerge = (): React.ReactElement => {
	const { id } = useParams();

	const [model, setModel] = React.useState<{
		song: SongContract;
		songMergeStore: SongMergeStore;
	}>();

	React.useEffect(() => {
		songRepo
			.getOne({ id: Number(id), lang: vdb.values.languagePreference })
			.then((song) =>
				setModel({
					song: song,
					songMergeStore: new SongMergeStore(vdb.values, songRepo, song),
				}),
			);
	}, [id]);

	return model ? (
		<SongMergeLayout song={model.song} songMergeStore={model.songMergeStore} />
	) : (
		<></>
	);
};

export default SongMerge;
