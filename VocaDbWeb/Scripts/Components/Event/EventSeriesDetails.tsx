import Breadcrumb from '@Bootstrap/Breadcrumb';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import Markdown from '@Components/KnockoutExtensions/Markdown';
import Layout from '@Components/Shared/Layout';
import DeletedBanner from '@Components/Shared/Partials/EntryDetails/DeletedBanner';
import ExternalLinksList from '@Components/Shared/Partials/EntryDetails/ExternalLinksList';
import EntryStatusMessage from '@Components/Shared/Partials/Shared/EntryStatusMessage';
import TagList from '@Components/Shared/Partials/TagList';
import TagsEdit from '@Components/Shared/Partials/TagsEdit';
import useVocaDbTitle from '@Components/useVocaDbTitle';
import ReleaseEventSeriesDetailsContract from '@DataContracts/ReleaseEvents/ReleaseEventSeriesDetailsContract';
import UrlHelper from '@Helpers/UrlHelper';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import EventCategory from '@Models/Events/EventCategory';
import ImageSize from '@Models/Images/ImageSize';
import LoginManager from '@Models/LoginManager';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import UserRepository from '@Repositories/UserRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import EventSeriesDetailsStore from '@Stores/ReleaseEvent/EventSeriesDetailsStore';
import { SearchType } from '@Stores/Search/SearchStore';
import moment from 'moment';
import qs from 'qs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
const userRepo = new UserRepository(httpClient, urlMapper);

interface EventSeriesDetailsLayoutProps {
	series: ReleaseEventSeriesDetailsContract;
	eventSeriesDetailsStore: EventSeriesDetailsStore;
}

const EventSeriesDetailsLayout = ({
	series,
	eventSeriesDetailsStore,
}: EventSeriesDetailsLayoutProps): React.ReactElement => {
	const { t, ready } = useTranslation([
		'ViewRes',
		'ViewRes.Event',
		'VocaDb.Web.Resources.Domain.ReleaseEvents',
		'VocaDb.Web.Resources.Views.Event',
	]);

	const subtitle =
		series.category === EventCategory[EventCategory.Unspecified] ||
		series.category === EventCategory[EventCategory.Other]
			? t('ViewRes:Misc.EventSeries')
			: t(
					`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${series.category}`,
			  );

	useVocaDbTitle(`${series.name} (${subtitle})`, ready);

	return (
		<Layout
			title={series.name}
			subtitle={subtitle}
			parents={
				<>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: '/Event',
						}}
					>
						{t(`ViewRes:Shared.ReleaseEvents`)}
					</Breadcrumb.Item>
				</>
			}
			toolbar={
				<>
					{loginManager.canManageDatabase && (
						<>
							{loginManager.canEdit({
								...series,
								entryType: EntryType[EntryType.ReleaseEventSeries],
							}) && (
								<>
									<JQueryUIButton
										as="a"
										href={`/Event/EditSeries/${series.id}`}
										icons={{ primary: 'ui-icon-wrench' }}
									>
										{t('ViewRes:Shared.Edit')}
									</JQueryUIButton>{' '}
									<JQueryUIButton
										as="a"
										href={`/Event/SeriesVersions/${series.id}`}
										icons={{ primary: 'ui-icon-clock' }}
									>
										{t('ViewRes:EntryDetails.ViewModifications')}
									</JQueryUIButton>
								</>
							)}{' '}
							<JQueryUIButton
								as="a"
								href={`/Event/Edit?${qs.stringify({ seriesId: series.id })}`}
								icons={{ primary: 'ui-icon-plus' }}
							>
								{t('ViewRes.Event:EventsBySeries.CreateEvent')}
							</JQueryUIButton>
						</>
					)}{' '}
					<EntryStatusMessage
						status={EntryStatus[series.status as keyof typeof EntryStatus]}
					/>
				</>
			}
		>
			{series.deleted && <DeletedBanner />}

			<div className="media">
				{series.mainPicture && (
					<a
						className="pull-left"
						href={UrlHelper.imageThumb(series.mainPicture, ImageSize.Original)}
					>
						<img
							className="media-object"
							src={UrlHelper.imageThumb(
								series.mainPicture,
								ImageSize.SmallThumb,
							)}
							alt="Thumb" /* TODO: localize */
						/>
					</a>
				)}
				<div className="media-body">
					{series.description && (
						<p>
							<Markdown>{series.description}</Markdown>
						</p>
					)}
				</div>

				<p>
					{t('ViewRes.Event:Details.Category')}:{' '}
					<Link
						to={`/Search?${qs.stringify({
							searchType: SearchType.ReleaseEvent,
							eventCategory: series.category,
						})}`}
					>
						{t(
							`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${series.category}`,
						)}
					</Link>
				</p>

				{series.webLinks.length > 0 && (
					<p>
						<ExternalLinksList
							webLinks={series.webLinks}
							showCategory={false}
						/>
						<br />
					</p>
				)}

				{series.additionalNames && (
					<p>
						{t('VocaDb.Web.Resources.Views.Event:SeriesDetails.Aliases')}:{' '}
						{series.additionalNames}
					</p>
				)}

				<p>
					<div className="inline-block">{t('ViewRes:Shared.Tags')}:</div>
					{eventSeriesDetailsStore.tagUsages.tagUsages.length > 0 && (
						<>
							{' '}
							<div className="entry-tag-usages inline-block">
								<TagList tagListStore={eventSeriesDetailsStore.tagUsages} />
							</div>
						</>
					)}
					<div>
						<JQueryUIButton
							as={SafeAnchor}
							className="inline-block"
							disabled={!loginManager.canEditTags}
							icons={{ primary: 'ui-icon-tag' }}
							onClick={eventSeriesDetailsStore.tagsEditStore.show}
							href="#"
						>
							{t('ViewRes:EntryDetails.EditTags')}
						</JQueryUIButton>
					</div>
				</p>
			</div>

			<h3>{t('ViewRes:Shared.ReleaseEvents')}</h3>
			<ul>
				{series.events.map((event) => (
					<li key={event.id}>
						<a
							href={EntryUrlMapper.details(
								EntryType.ReleaseEvent,
								event.id,
								event.urlSlug,
							)}
						>
							{event.name}
						</a>
						{event.date && (
							<>
								{' '}
								<small>({moment(event.date).format('l')})</small>
							</>
						)}
					</li>
				))}
			</ul>

			<TagsEdit tagsEditStore={eventSeriesDetailsStore.tagsEditStore} />
		</Layout>
	);
};

const EventSeriesDetails = (): React.ReactElement => {
	const [model, setModel] = React.useState<
		| {
				series: ReleaseEventSeriesDetailsContract;
				eventSeriesDetailsStore: EventSeriesDetailsStore;
		  }
		| undefined
	>();

	const { id } = useParams();
	const navigate = useNavigate();

	React.useEffect(() => {
		eventRepo
			.getReleaseEventSeriesDetails({ id: Number(id) })
			.then((series) =>
				setModel({
					series: series,
					eventSeriesDetailsStore: new EventSeriesDetailsStore(
						userRepo,
						series.id,
						series.tags,
					),
				}),
			)
			.catch((error) => {
				if (error.response.status === 404) navigate('/Error/NotFound');
			});
	}, [id, navigate]);

	return model ? (
		<EventSeriesDetailsLayout
			series={model.series}
			eventSeriesDetailsStore={model.eventSeriesDetailsStore}
		/>
	) : (
		<></>
	);
};

export default EventSeriesDetails;
