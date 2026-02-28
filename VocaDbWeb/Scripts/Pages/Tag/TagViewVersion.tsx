import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import { ArchivedObjectVersionProperties } from '@/Components/Shared/Partials/ArchivedEntry/ArchivedObjectVersionProperties';
import { HiddenBanner } from '@/Components/Shared/Partials/EntryDetails/HiddenBanner';
import { ReportEntryVersionPopupKnockout } from '@/Components/Shared/Partials/EntryDetails/ReportEntryVersionPopupKnockout';
import { PrintArchivedTagData } from '@/Components/Shared/Partials/Tag/PrintArchivedTagData';
import { useChangedFieldNames } from '@/Components/useChangedFieldNames';
import { ArchivedTagVersionDetailsContract } from '@/DataContracts/Tag/ArchivedTagVersionDetailsContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import { tagRepo } from '@/Repositories/TagRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { ArchivedEntryStore } from '@/Stores/ArchivedEntryStore';
import { useLocationStateStore } from '@/route-sphere';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams, useSearchParams } from 'react-router-dom';

interface TagViewVersionLayoutProps {
	contract: ArchivedTagVersionDetailsContract;
	archivedEntryStore: ArchivedEntryStore;
}

const TagViewVersionLayout = observer(
	({
		contract,
		archivedEntryStore,
	}: TagViewVersionLayoutProps): React.ReactElement => {
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
								to: '/Tag',
							}}
							divider
						>
							{t(`ViewRes:Shared.Tags`)}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: EntryUrlMapper.details(EntryType.Tag, contract.tag.id),
							}}
							divider
						>
							{contract.tag.name}
						</Breadcrumb.Item>
						<Breadcrumb.Item
							linkAs={Link}
							linkProps={{
								to: `/Tag/Versions/${contract.tag.id}`,
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
							href={`/Tag/ArchivedVersionXml/${contract.archivedVersion.id}`}
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
											await tagRepo.updateVersionVisibility({
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
											await tagRepo.updateVersionVisibility({
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
										changedFieldNames(EntryType.Tag, changedField),
									)
									.join(', ')} by ${comparableVersion.agentName})`}</option>
							))}
						</select>
					</form>
				)}

				<ArchivedObjectVersionProperties
					version={contract.archivedVersion}
					compareTo={contract.comparedVersion}
					entryType={EntryType.Tag}
				/>

				<PrintArchivedTagData comparedTags={contract.versions} />

				<ReportEntryVersionPopupKnockout
					reportEntryStore={archivedEntryStore.reportStore}
				/>
			</Layout>
		);
	},
);

const TagViewVersion = (): React.ReactElement => {
	const { id } = useParams();
	const [searchParams] = useSearchParams();
	const comparedVersionId = searchParams.get('comparedVersionId');

	const [model, setModel] = React.useState<{
		contract: ArchivedTagVersionDetailsContract;
		archivedEntryStore: ArchivedEntryStore;
	}>();

	React.useEffect(() => {
		tagRepo
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
						contract.tag.id,
						contract.archivedVersion.version,
						tagRepo,
					),
				}),
			);
	}, [id, comparedVersionId]);

	return model ? (
		<TagViewVersionLayout
			contract={model.contract}
			archivedEntryStore={model.archivedEntryStore}
		/>
	) : (
		<></>
	);
};

export default TagViewVersion;
