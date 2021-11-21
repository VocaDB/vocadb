import Breadcrumb from '@Bootstrap/Breadcrumb';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import Markdown from '@Components/KnockoutExtensions/Markdown';
import Layout from '@Components/Shared/Layout';
import DeletedBanner from '@Components/Shared/Partials/EntryDetails/DeletedBanner';
import ExternalLinksList from '@Components/Shared/Partials/EntryDetails/ExternalLinksList';
import ReportEntryPopupKnockout from '@Components/Shared/Partials/EntryDetails/ReportEntryPopupKnockout';
import EmbedGoogleMaps from '@Components/Shared/Partials/Shared/EmbedGoogleMaps';
import EntryStatusMessage from '@Components/Shared/Partials/Shared/EntryStatusMessage';
import useVocaDbTitle from '@Components/useVocaDbTitle';
import VenueForApiContract from '@DataContracts/Venue/VenueForApiContract';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import LoginManager from '@Models/LoginManager';
import VenueReportType, {
	reportTypesWithRequiredNotes,
} from '@Models/Venues/VenueReportType';
import VenueRepository from '@Repositories/VenueRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import VenueDetailsStore from '@Stores/Venue/VenueDetailsStore';
import moment from 'moment';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const venueRepo = new VenueRepository(httpClient, urlMapper);

interface VenueDetailsLayoutProps {
	venue: VenueForApiContract;
	venueDetailsStore: VenueDetailsStore;
}

const VenueDetailsLayout = ({
	venue,
	venueDetailsStore,
}: VenueDetailsLayoutProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes', 'ViewRes.Event', 'ViewRes.Venue']);

	const title = venue.name;

	useVocaDbTitle(title, true);

	return (
		<Layout
			title={title}
			subtitle={t('ViewRes.Venue:Details.Venue')}
			parents={
				<>
					<Breadcrumb.Item href="/Event/EventsByVenue">
						{t('ViewRes:Shared.Venues')}
					</Breadcrumb.Item>
				</>
			}
			toolbar={
				<>
					<JQueryUIButton
						as="a"
						href={`/Venue/Edit/${venue.id}`}
						disabled={
							!loginManager.canEdit({
								...venue,
								entryType: EntryType[EntryType.Venue],
							})
						}
						icons={{ primary: 'ui-icon-wrench' }}
					>
						{t('ViewRes:Shared.Edit')}
					</JQueryUIButton>{' '}
					<JQueryUIButton
						as="a"
						href={`/Venue/Versions/${venue.id}`}
						icons={{ primary: 'ui-icon-clock' }}
					>
						{t('ViewRes:EntryDetails.ViewModifications')}
					</JQueryUIButton>
					{loginManager.canManageDatabase && (
						<>
							{' '}
							<JQueryUIButton
								as="a"
								href={`/Event/Edit?${qs.stringify({ venueId: venue.id })}`}
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
					<EntryStatusMessage
						status={EntryStatus[venue.status as keyof typeof EntryStatus]}
					/>
				</>
			}
		>
			{venue.deleted && <DeletedBanner />}

			{venue.coordinates.hasValue && (
				<EmbedGoogleMaps coordinates={venue.coordinates} />
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
						{venue.addressCountryCode /* TODO */}
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
								<small>({moment(event.date).format('l')})</small>
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
					notesRequired: reportTypesWithRequiredNotes.includes(r),
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
	const navigate = useNavigate();

	React.useEffect(() => {
		venueRepo
			.getDetails({ id: Number(id) })
			.then((venue) =>
				setModel({
					venue: venue,
					venueDetailsStore: new VenueDetailsStore(venueRepo, venue.id),
				}),
			)
			.catch((error) => {
				if (error.response.status === 404) navigate('/Error/NotFound');
			});
	}, [id, navigate]);

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
