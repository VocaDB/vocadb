import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersionProperties } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersionProperties';
import { HiddenBanner } from '@/Components/Shared/Partials/EntryDetails/HiddenBanner';
import { ReportEntryVersionPopupKnockout } from '@/Components/Shared/Partials/EntryDetails/ReportEntryVersionPopupKnockout';
import { PrintArchivedEventData } from '@/Components/Shared/Partials/Event/PrintArchivedEventData';
import { useChangedFieldNames } from '@/Components/useChangedFieldNames';
import { ArchivedEventVersionDetailsContract } from '@/DataContracts/ReleaseEvents/ArchivedEventVersionDetailsContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import { antiforgeryRepo } from '@/Repositories/AntiforgeryRepository';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { ArchivedEntryStore } from '@/Stores/ArchivedEntryStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams, useSearchParams } from 'react-router-dom';

interface EventViewVersionLayoutProps {
	contract: ArchivedEventVersionDetailsContract;
	archivedEntryStore: ArchivedEntryStore;
}

const EventViewVersionLayout = observer(
	({
		contract,
		archivedEntryStore,
	}: EventViewVersionLayoutProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation(['ViewRes']);

		const title = `Revision ${contract.archivedVersion.version} for ${contract.name}`; /* LOC */

		const changedFieldNames = useChangedFieldNames();

		useLocationStateStore(archivedEntryStore);

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
								to: '/Event',
							}}
							divider
						>
							{t(`ViewRes:Shared.ReleaseEvents`)}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details(
									EntryType.ReleaseEvent,
									contract.releaseEvent.id,
								),
							}}
							divider
						>
							{contract.releaseEvent.name}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Event/Versions/${contract.releaseEvent.id}`,
							}}
						>
							Revisions{/* LOC */}
						</Breadcrumb.Item>
					</>
				}
				toolbar={
					<>
						<JQueryUIButton
							as="a"
							href={`/Event/ArchivedVersionXml/${contract.archivedVersion.id}`}
							icons={{ primary: 'ui-icon-arrowthickstop-1-s' }}
						>
							Download XML{/* LOC */}
						</JQueryUIButton>{' '}
						<JQueryUIButton
							as={SafeAnchor}
							href="#"
							onClick={archivedEntryStore.reportStore.show}
							icons={{ primary: 'ui-icon-alert' }}
						>
							{t('ViewRes:EntryDetails.ReportAnError')}
						</JQueryUIButton>{' '}
						{loginManager.canViewHiddenRevisions &&
							(contract.archivedVersion.hidden ? (
								<JQueryUIButton
									as="a"
									onClick={async (e): Promise<void> => {
										if (
											window.confirm(t('ViewRes:ViewVersion.ConfirmUnhide'))
										) {
											const requestToken = await antiforgeryRepo.getToken();

											await eventRepo.updateVersionVisibility(requestToken, {
												archivedVersionId: contract.archivedVersion.id,
												hidden: false,
											});

											window.location.reload();
										}
									}}
									icons={{ primary: 'ui-icon-unlocked' }}
								>
									{t('ViewRes:ViewVersion.UnhideVersion')}
								</JQueryUIButton>
							) : (
								<JQueryUIButton
									as="a"
									onClick={async (e): Promise<void> => {
										if (window.confirm(t('ViewRes:ViewVersion.ConfirmHide'))) {
											const requestToken = await antiforgeryRepo.getToken();

											await eventRepo.updateVersionVisibility(requestToken, {
												archivedVersionId: contract.archivedVersion.id,
												hidden: true,
											});

											window.location.reload();
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
							value={archivedEntryStore.comparedVersionId}
							onChange={(e): void =>
								runInAction(() => {
									archivedEntryStore.comparedVersionId = e.target.value
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
										changedFieldNames(EntryType.ReleaseEvent, changedField),
									)
									.join(', ')} by ${comparableVersion.agentName})`}</option>
							))}
						</select>
					</form>
				)}

				<ArchivedObjectVersionProperties
					version={contract.archivedVersion}
					compareTo={contract.comparedVersion}
					entryType={EntryType.ReleaseEvent}
				/>

				<PrintArchivedEventData comparedEvents={contract.versions} />

				<ReportEntryVersionPopupKnockout
					reportEntryStore={archivedEntryStore.reportStore}
				/>
			</Layout>
		);
	},
);

const EventViewVersion = (): React.ReactElement => {
	const { id } = useParams();
	const [searchParams] = useSearchParams();
	const comparedVersionId = searchParams.get('comparedVersionId');

	const [model, setModel] = React.useState<{
		contract: ArchivedEventVersionDetailsContract;
		archivedEntryStore: ArchivedEntryStore;
	}>();

	React.useEffect(() => {
		eventRepo
			.getVersionDetails({
				id: Number(id),
				comparedVersionId: comparedVersionId
					? Number(comparedVersionId)
					: undefined,
			})
			.then((contract) =>
				setModel({
					contract: contract,
					archivedEntryStore: new ArchivedEntryStore(
						contract.releaseEvent.id,
						contract.archivedVersion.version,
						eventRepo,
					),
				}),
			);
	}, [id, comparedVersionId]);

	return model ? (
		<EventViewVersionLayout
			contract={model.contract}
			archivedEntryStore={model.archivedEntryStore}
		/>
	) : (
		<></>
	);
};

export default EventViewVersion;
