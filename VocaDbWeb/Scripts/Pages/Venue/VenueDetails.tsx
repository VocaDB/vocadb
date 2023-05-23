import Breadcrumb from '@/Bootstrap/Breadcrumb';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { Layout } from '@/Components/Shared/Layout';
import { DeletedBanner } from '@/Components/Shared/Partials/EntryDetails/DeletedBanner';
import { ExternalLinksList } from '@/Components/Shared/Partials/EntryDetails/ExternalLinksList';
import { ReportEntryPopupKnockout } from '@/Components/Shared/Partials/EntryDetails/ReportEntryPopupKnockout';
import { EmbedOpenStreetMap } from '@/Components/Shared/Partials/Shared/EmbedOpenStreetMap';
import { EntryStatusMessage } from '@/Components/Shared/Partials/Shared/EntryStatusMessage';
import { useRegionNames } from '@/Components/useRegionNames';
import { VenueForApiContract } from '@/DataContracts/Venue/VenueForApiContract';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import { EntryType } from '@/Models/EntryType';
import {
	VenueReportType,
	venueReportTypesWithRequiredNotes,
} from '@/Models/Venues/VenueReportType';
import { venueRepo } from '@/Repositories/VenueRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { VenueDetailsStore } from '@/Stores/Venue/VenueDetailsStore';
import dayjs from '@/dayjs';
import NProgress from 'nprogress';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface VenueDetailsLayoutProps {
	venue: VenueForApiContract;
	venueDetailsStore: VenueDetailsStore;
}

const VenueDetailsLayout = ({
	venue,
	venueDetailsStore,
}: VenueDetailsLayoutProps): React.ReactElement => {
	const loginManager = useLoginManager();

	const { t } = useTranslation(['ViewRes', 'ViewRes.Event', 'ViewRes.Venue']);

	const regionNames = useRegionNames();

	const title = venue.name;

	return (
		<Layout
			pageTitle={title}
			ready={true}
			title={title}
			subtitle={t('ViewRes.Venue:Details.Venue')}
			parents={
				<>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{ to: '/Event/EventsByVenue' }}
					>
						{t('ViewRes:Shared.Venues')}
					</Breadcrumb.Item>
				</>
			}
			toolbar={
				<>
					<JQueryUIButton
						as={Link}
						to={`/Venue/Edit/${venue.id}`}
						disabled={
							!loginManager.canEdit({
								...venue,
								entryType: EntryType.Venue,
							})
						}
						icons={{ primary: 'ui-icon-wrench' }}
					>
						{t('ViewRes:Shared.Edit')}
					</JQueryUIButton>{' '}
					<JQueryUIButton
						as={Link}
						to={`/Venue/Versions/${venue.id}`}
						icons={{ primary: 'ui-icon-clock' }}
					>
						{t('ViewRes:EntryDetails.ViewModifications')}
					</JQueryUIButton>
					{loginManager.canManageDatabase && (
						<>
							{' '}
							<JQueryUIButton
								as={Link}
								to={`/Event/Edit?${qs.stringify({ venueId: venue.id })}`}
								icons={{ primary: 'ui-icon-plus' }}
							>
								{t('ViewRes.Event:EventsBySeries.CreateEvent')}
							</JQueryUIButton>
						</>
					)}{' '}
					<JQueryUIButton
						as={SafeAnchor}
						href="#"
						onClick={venueDetailsStore.reportStore.show}
						icons={{ primary: 'ui-icon-alert' }}
					>
						{t('ViewRes:EntryDetails.ReportAnError')}
					</JQueryUIButton>{' '}
					<EntryStatusMessage status={venue.status} />
				</>
			}
		>
			{venue.deleted && <DeletedBanner />}

			{venue.coordinates.hasValue && (
				<EmbedOpenStreetMap coordinates={venue.coordinates} />
			)}

			<div className="media">
				<div className="media-body">
					{venue.description && (
						<p>
							<Markdown>{venue.description}</Markdown>
						</p>
					)}
				</div>

				{venue.webLinks.length > 0 && (
					<>
						<ExternalLinksList webLinks={venue.webLinks} showCategory={false} />
						<br />
					</>
				)}

				{venue.additionalNames && (
					<p>
						{t('ViewRes.Venue:Details.Aliases')}: {venue.additionalNames}
					</p>
				)}

				{venue.addressCountryCode && (
					<p>
						{t('ViewRes.Venue:Details.Country')}:{' '}
						{regionNames.of(venue.addressCountryCode)}
					</p>
				)}

				{venue.address && (
					<p>
						{t('ViewRes.Venue:Details.Address')}: {venue.address}
					</p>
				)}
			</div>

			<h3>{t('ViewRes:Shared.ReleaseEvents')}</h3>
			<ul>
				{venue.events.map((event) => (
					<li key={event.id}>
						<Link to={EntryUrlMapper.details(EntryType.ReleaseEvent, event.id)}>
							{event.name}
						</Link>
						{event.date && (
							<>
								{' '}
								<small>({dayjs(event.date).format('l')})</small>
							</>
						)}
					</li>
				))}
			</ul>

			<ReportEntryPopupKnockout
				reportEntryStore={venueDetailsStore.reportStore}
				reportTypes={Object.values(VenueReportType).map((r) => ({
					id: r,
					name: t(`VocaDb.Web.Resources.Domain:EntryReportTypeNames.${r}`),
					notesRequired: venueReportTypesWithRequiredNotes.includes(r),
				}))}
			/>
		</Layout>
	);
};

const VenueDetails = (): React.ReactElement => {
	const [model, setModel] = React.useState<
		| { venue: VenueForApiContract; venueDetailsStore: VenueDetailsStore }
		| undefined
	>();

	const { id } = useParams();

	React.useEffect(() => {
		NProgress.start();

		venueRepo
			.getDetails({ id: Number(id) })
			.then((venue) => {
				setModel({
					venue: venue,
					venueDetailsStore: new VenueDetailsStore(venueRepo, venue.id),
				});

				NProgress.done();
			})
			.catch((error) => {
				if (error.response) {
					if (error.response.status === 404)
						window.location.href = '/Error/NotFound';
				}

				throw error;
			});
	}, [id]);

	return model ? (
		<VenueDetailsLayout
			venue={model.venue}
			venueDetailsStore={model.venueDetailsStore}
		/>
	) : (
		<></>
	);
};

export default VenueDetails;
