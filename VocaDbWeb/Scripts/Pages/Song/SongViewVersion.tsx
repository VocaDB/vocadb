import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersionProperties } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersionProperties';
import { HiddenBanner } from '@/Components/Shared/Partials/EntryDetails/HiddenBanner';
import { ReportEntryVersionPopupKnockout } from '@/Components/Shared/Partials/EntryDetails/ReportEntryVersionPopupKnockout';
import { PrintArchivedSongData } from '@/Components/Shared/Partials/Song/PrintArchivedSongData';
import { useChangedFieldNames } from '@/Components/useChangedFieldNames';
import { useVdbTitle } from '@/Components/useVdbTitle';
import { ArchivedSongVersionDetailsContract } from '@/DataContracts/Song/ArchivedSongVersionDetailsContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { EntryType } from '@/Models/EntryType';
import { LoginManager } from '@/Models/LoginManager';
import { SongRepository } from '@/Repositories/SongRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { ArchivedSongStore } from '@/Stores/Song/ArchivedSongStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams, useSearchParams } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();

const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);

interface SongViewVersionLayoutProps {
	contract: ArchivedSongVersionDetailsContract;
	archivedSongStore: ArchivedSongStore;
}

const SongViewVersionLayout = observer(
	({
		contract,
		archivedSongStore,
	}: SongViewVersionLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = `Revision ${contract.archivedVersion.version} for ${contract.name}`; /* LOC */

		useVdbTitle(title, true);

		const changedFieldNames = useChangedFieldNames();

		useLocationStateStore(archivedSongStore);

		return (
			<Layout
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
								to: EntryUrlMapper.details(EntryType.Song, contract.song.id),
							}}
							divider
						>
							{contract.song.name}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Song/Versions/${contract.song.id}`,
							}}
						>
							Revisions{/* LOC */}
						</Breadcrumb.Item>
					</>
				}
				toolbar={
					<>
						{loginManager.canRestoreRevisions &&
							contract.archivedVersion.version < contract.song.version - 1 && (
								<JQueryUIButton
									as="a"
									href={
										`/Song/RevertToVersion?${qs.stringify({
											archivedSongVersionId: contract.archivedVersion.id,
										})}` /* TODO: Convert to POST. */
									}
									onClick={(e): void => {
										if (
											!window.confirm(
												t('ViewRes:ViewVersion.ConfirmRevertToVersion'),
											)
										) {
											e.preventDefault();
										}
									}}
									icons={{ primary: 'ui-icon-arrowrefresh-1-w' }}
								>
									{t('ViewRes:ViewVersion.RevertToVersion')}
								</JQueryUIButton>
							)}{' '}
						&nbsp;{' '}
						<JQueryUIButton
							as="a"
							href={`/Song/ArchivedVersionXml/${contract.archivedVersion.id}`}
							icons={{ primary: 'ui-icon-arrowthickstop-1-s' }}
						>
							Download XML{/* LOC */}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={SafeAnchor}
							href="#"
							onClick={archivedSongStore.reportStore.show}
							icons={{ primary: 'ui-icon-alert' }}
						>
							{t('ViewRes:EntryDetails.ReportAnError')}
						</JQueryUIButton>{' '}
						{loginManager.canViewHiddenRevisions &&
							(contract.archivedVersion.hidden ? (
								<JQueryUIButton
									as="a"
									href={`/Song/UpdateVersionVisibility?${qs.stringify({
										archivedVersionId: contract.archivedVersion.id,
										hidden: false,
									})}`} /* TODO: Convert to POST. */
									onClick={(e): void => {
										if (
											!window.confirm(t('ViewRes:ViewVersion.ConfirmUnhide'))
										) {
											e.preventDefault();
										}
									}}
									icons={{ primary: 'ui-icon-unlocked' }}
								>
									{t('ViewRes:ViewVersion.UnhideVersion')}
								</JQueryUIButton>
							) : (
								<JQueryUIButton
									as="a"
									href={`/Song/UpdateVersionVisibility?${qs.stringify({
										archivedVersionId: contract.archivedVersion.id,
										hidden: true,
									})}`} /* TODO: Convert to POST. */
									onClick={(e): void => {
										if (!window.confirm(t('ViewRes:ViewVersion.ConfirmHide'))) {
											e.preventDefault();
										}
									}}
									icons={{ primary: 'ui-icon-locked' }}
								>
									{t('ViewRes:ViewVersion.HideVersion')}
								</JQueryUIButton>
							))}
					</>
				}
			>
				{contract.archivedVersion.hidden && <HiddenBanner />}

				{contract.comparableVersions.length > 0 && (
					<form className="form form-inline">
						Compare to:{/* LOC */}{' '}
						<select
							className="input-xlarge"
							value={archivedSongStore.comparedVersionId}
							onChange={(e): void =>
								runInAction(() => {
									archivedSongStore.comparedVersionId = e.target.value
										? Number(e.target.value)
										: undefined;
								})
							}
						>
							<option value="" />
							{contract.comparableVersions.map((comparableVersion) => (
								<option
									value={comparableVersion.id}
									key={comparableVersion.id}
								>{`${
									comparableVersion.version
								} (${comparableVersion.changedFields
									.map((changedField) =>
										changedFieldNames(EntryType.Song, changedField),
									)
									.join(', ')} by ${comparableVersion.agentName})`}</option>
							))}
						</select>
					</form>
				)}

				<ArchivedObjectVersionProperties
					version={contract.archivedVersion}
					compareTo={contract.comparedVersion}
					entryType={EntryType.Song}
				/>

				<PrintArchivedSongData comparedSongs={contract.versions} />

				<ReportEntryVersionPopupKnockout
					reportEntryStore={archivedSongStore.reportStore}
				/>
			</Layout>
		);
	},
);

const SongViewVersion = (): React.ReactElement => {
	const { id } = useParams();
	const [searchParams] = useSearchParams();
	const comparedVersionId = searchParams.get('comparedVersionId');

	const [model, setModel] = React.useState<{
		contract: ArchivedSongVersionDetailsContract;
		archivedSongStore: ArchivedSongStore;
	}>();

	React.useEffect(() => {
		songRepo
			.getVersionDetails({
				id: Number(id),
				comparedVersionId: comparedVersionId
					? Number(comparedVersionId)
					: undefined,
			})
			.then((contract) =>
				setModel({
					contract: contract,
					archivedSongStore: new ArchivedSongStore(
						contract.song.id,
						contract.archivedVersion.version,
						songRepo,
					),
				}),
			);
	}, [id, comparedVersionId]);

	return model ? (
		<SongViewVersionLayout
			contract={model.contract}
			archivedSongStore={model.archivedSongStore}
		/>
	) : (
		<></>
	);
};

export default SongViewVersion;
