import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersionProperties } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersionProperties';
import { PrintArchivedArtistData } from '@/Components/Shared/Partials/Artist/PrintArchivedArtistData';
import { HiddenBanner } from '@/Components/Shared/Partials/EntryDetails/HiddenBanner';
import { ReportEntryVersionPopupKnockout } from '@/Components/Shared/Partials/EntryDetails/ReportEntryVersionPopupKnockout';
import { useChangedFieldNames } from '@/Components/useChangedFieldNames';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { ArchivedArtistVersionDetailsContract } from '@/DataContracts/Artist/ArchivedArtistVersionDetailsContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { EntryType } from '@/Models/EntryType';
import { LoginManager } from '@/Models/LoginManager';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { ArchivedArtistStore } from '@/Stores/Artist/ArchivedArtistStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams, useSearchParams } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();

const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);

interface ArtistViewVersionLayoutProps {
	contract: ArchivedArtistVersionDetailsContract;
	archivedArtistStore: ArchivedArtistStore;
}

const ArtistViewVersionLayout = observer(
	({
		contract,
		archivedArtistStore,
	}: ArtistViewVersionLayoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const title = `Revision ${contract.archivedVersion.version} for ${contract.name}`; /* TODO: localize */

		useVocaDbTitle(title, true);

		const changedFieldNames = useChangedFieldNames();

		useLocationStateStore(archivedArtistStore);

		return (
			<Layout
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
								to: EntryUrlMapper.details(
									EntryType.Artist,
									contract.artist.id,
								),
							}}
							divider
						>
							{contract.artist.name}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Artist/Versions/${contract.artist.id}`,
							}}
						>
							Revisions{/* TODO: localize */}
						</Breadcrumb.Item>
					</>
				}
				toolbar={
					<>
						{loginManager.canRestoreRevisions &&
							contract.archivedVersion.version <
								contract.artist.version - 1 && (
								<JQueryUIButton
									as="a"
									href={
										`/Artist/RevertToVersion?${qs.stringify({
											archivedArtistVersionId: contract.archivedVersion.id,
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
							href={`/Artist/ArchivedVersionXml/${contract.archivedVersion.id}`}
							icons={{ primary: 'ui-icon-arrowthickstop-1-s' }}
						>
							Download XML{/* TODO: localize */}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={SafeAnchor}
							href="#"
							onClick={archivedArtistStore.reportStore.show}
							icons={{ primary: 'ui-icon-alert' }}
						>
							{t('ViewRes:EntryDetails.ReportAnError')}
						</JQueryUIButton>{' '}
						{loginManager.canViewHiddenRevisions &&
							(contract.archivedVersion.hidden ? (
								<JQueryUIButton
									as="a"
									href={`/Artist/UpdateVersionVisibility?${qs.stringify({
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
									href={`/Artist/UpdateVersionVisibility?${qs.stringify({
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
						Compare to:{/* TODO: localize */}{' '}
						<select
							className="input-xlarge"
							value={archivedArtistStore.comparedVersionId}
							onChange={(e): void =>
								runInAction(() => {
									archivedArtistStore.comparedVersionId = e.target.value
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
										changedFieldNames(EntryType.Artist, changedField),
									)
									.join(', ')} by ${comparableVersion.agentName})`}</option>
							))}
						</select>
					</form>
				)}

				<ArchivedObjectVersionProperties
					version={contract.archivedVersion}
					compareTo={contract.comparedVersion}
					entryType={EntryType.Artist}
				/>

				<PrintArchivedArtistData comparedArtists={contract.versions} />

				<ReportEntryVersionPopupKnockout
					reportEntryStore={archivedArtistStore.reportStore}
				/>
			</Layout>
		);
	},
);

const ArtistViewVersion = (): React.ReactElement => {
	const { id } = useParams();
	const [searchParams] = useSearchParams();
	const comparedVersionId = searchParams.get('comparedVersionId');

	const [model, setModel] = React.useState<{
		contract: ArchivedArtistVersionDetailsContract;
		archivedArtistStore: ArchivedArtistStore;
	}>();

	React.useEffect(() => {
		artistRepo
			.getVersionDetails({
				id: Number(id),
				comparedVersionId: comparedVersionId
					? Number(comparedVersionId)
					: undefined,
			})
			.then((contract) =>
				setModel({
					contract: contract,
					archivedArtistStore: new ArchivedArtistStore(
						contract.artist.id,
						contract.archivedVersion.version,
						artistRepo,
					),
				}),
			);
	}, [id, comparedVersionId]);

	return model ? (
		<ArtistViewVersionLayout
			contract={model.contract}
			archivedArtistStore={model.archivedArtistStore}
		/>
	) : (
		<></>
	);
};

export default ArtistViewVersion;
