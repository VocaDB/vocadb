import { regionNames } from '@/Components/regions';
import { userLanguageCultures } from '@/Components/userLanguageCultures';
import { UserLanguageProficiency } from '@/DataContracts/User/UserKnownLanguageContract';
import { ArtistLinkType } from '@/Models/Artists/ArtistLinkType';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { EventCategory } from '@/Models/Events/EventCategory';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { PVType } from '@/Models/PVs/PVType';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { UserEmailOptions } from '@/Models/Users/UserEmailOptions';
import { UserGroup } from '@/Models/Users/UserGroup';
import { useVdb } from '@/VdbContext';
import React from 'react';
import { useTranslation } from 'react-i18next';

let nextEventCategoryOrder = 1;
const eventCategoryOrders: Record<EventCategory, number> = {
	[EventCategory.Unspecified]: nextEventCategoryOrder++,
	[EventCategory.AlbumRelease]: nextEventCategoryOrder++,
	[EventCategory.Anniversary]: nextEventCategoryOrder++,
	[EventCategory.Club]: nextEventCategoryOrder++,
	[EventCategory.Concert]: nextEventCategoryOrder++,
	[EventCategory.Contest]: nextEventCategoryOrder++,
	[EventCategory.Festival]: nextEventCategoryOrder++,
	[EventCategory.Convention]: nextEventCategoryOrder++,
	[EventCategory.Other]: nextEventCategoryOrder++,
};

const eventCategories = Object.values(EventCategory).orderBy(
	(eventCategory) => eventCategoryOrders[eventCategory],
);

interface DropdownListProps
	extends React.DetailedHTMLProps<
		React.SelectHTMLAttributes<HTMLSelectElement>,
		HTMLSelectElement
	> {}

export const ReleaseEventCategoryDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation('VocaDb.Web.Resources.Domain.ReleaseEvents');

		return (
			<select {...props}>
				{eventCategories.map((eventCategory) => (
					<option value={eventCategory} key={eventCategory}>
						{t(
							`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${eventCategory}`,
						)}
					</option>
				))}
			</select>
		);
	},
);

export const UserGroupDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation('Resources');

		return (
			<select {...props}>
				{Object.values(UserGroup).map((userGroup) => (
					<option value={userGroup} key={userGroup}>
						{t(`Resources:UserGroupNames.${userGroup}`)}
					</option>
				))}
			</select>
		);
	},
);

const cultures = Object.fromEntries(
	Object.entries({
		'en-US': {
			nativeName: 'English (United States)',
			englishName: 'English (United States)',
		},
		'de-DE': {
			nativeName: 'Deutsch (Deutschland)',
			englishName: 'German (Germany)',
		},
		es: { nativeName: 'espa\u00F1ol', englishName: 'Spanish' },
		'fi-Fi': { nativeName: 'suomi (Suomi)', englishName: 'Finnish (Finland)' },
		pt: { nativeName: 'portugu\u00EAs', englishName: 'Portuguese' },
		'ru-RU': {
			nativeName:
				'\u0440\u0443\u0441\u0441\u043A\u0438\u0439 (\u0420\u043E\u0441\u0441\u0438\u044F)',
			englishName: 'Russian (Russia)',
		},
		'ja-JP': {
			nativeName: '\u65E5\u672C\u8A9E (\u65E5\u672C)',
			englishName: 'Japanese (Japan)',
		},
		'zh-Hans': {
			nativeName: '\u4E2D\u6587\uFF08\u7B80\u4F53\uFF09',
			englishName: 'Chinese (Simplified)',
		},
	}).orderBy(([_, value]) => value.nativeName),
); /* TODO */

export const CultureDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		return (
			<select {...props}>
				{props.placeholder !== undefined && (
					<option value="">{props.placeholder}</option>
				)}
				{Object.entries(cultures).map(([key, value]) => (
					<option value={key} key={key}>
						{value.nativeName} ({value.englishName /* LOC */})
					</option>
				))}
			</select>
		);
	},
);

export const UserLanguageCultureDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		return (
			<select {...props}>
				{props.placeholder !== undefined && (
					<option value="">{props.placeholder}</option>
				)}
				{Object.entries(userLanguageCultures).map(([key, value]) => (
					<option value={key} key={key}>
						{value.nativeName} ({value.englishName /* LOC */})
					</option>
				))}
			</select>
		);
	},
);

export const UserLanguageProficiencyDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Domain.Users']);

		return (
			<select {...props}>
				{props.placeholder !== undefined && (
					<option value="">{props.placeholder}</option>
				)}
				{Object.values(UserLanguageProficiency).map((value) => (
					<option value={value} key={value}>
						{t(
							`VocaDb.Web.Resources.Domain.Users:UserLanguageProficiencyNames.${value}`,
							'',
						)}
					</option>
				))}
			</select>
		);
	},
);

const commentTargetTypes: EntryType[] = [
	EntryType.Undefined,
	EntryType.Album,
	EntryType.Artist,
	EntryType.DiscussionTopic,
	EntryType.ReleaseEvent,
	EntryType.Song,
	EntryType.SongList,
	EntryType.Tag,
	EntryType.User,
];

export const CommentTargetTypeDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation('VocaDb.Web.Resources.Domain');

		return (
			<select {...props}>
				{Object.values(commentTargetTypes).map((entryType) => (
					<option value={entryType} key={entryType}>
						{t(`VocaDb.Web.Resources.Domain:EntryTypeNames.${entryType}`)}
					</option>
				))}
			</select>
		);
	},
);

export const LanguageSelectionDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<select {...props}>
				{Object.values(ContentLanguageSelection)
					.filter((languageSelection) => isNaN(Number(languageSelection)))
					.map((languageSelection) => (
						<option value={languageSelection} key={languageSelection}>
							{t(
								`Resources:ContentLanguageSelectionNames.${languageSelection}`,
							)}
						</option>
					))}
			</select>
		);
	},
);

export const LanguagePreferenceDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<select {...props}>
				{Object.values(ContentLanguagePreference)
					.filter((languagePreference) => isNaN(Number(languagePreference)))
					.map((languagePreference) => (
						<option value={languagePreference} key={languagePreference}>
							{t(
								`Resources:ContentLanguageSelectionNames.${languagePreference}`,
							)}
						</option>
					))}
			</select>
		);
	},
);

const regionCodes = [
	'AF',
	'AX',
	'AL',
	'DZ',
	'AS',
	'AD',
	'AO',
	'AI',
	'AG',
	'AR',
	'AM',
	'AW',
	'AU',
	'AT',
	'AZ',
	'BS',
	'BH',
	'BD',
	'BB',
	'BY',
	'BE',
	'BZ',
	'BJ',
	'BM',
	'BT',
	'BO',
	'BQ',
	'BA',
	'BW',
	'BR',
	'IO',
	'VG',
	'BN',
	'BG',
	'BF',
	'BI',
	'CV',
	'KH',
	'CM',
	'CA',
	//'029',
	'KY',
	'CF',
	'TD',
	'CL',
	'CN',
	'CX',
	'CC',
	'CO',
	'KM',
	'CG',
	'CD',
	'CK',
	'CR',
	'CI',
	'HR',
	'CU',
	'CW',
	'CY',
	'CZ',
	'DK',
	'DJ',
	'DM',
	'DO',
	'EC',
	'EG',
	'SV',
	'GQ',
	'ER',
	'EE',
	'SZ',
	'ET',
	//'150',
	'FK',
	'FO',
	'FJ',
	'FI',
	'FR',
	'GF',
	'PF',
	'GA',
	'GM',
	'GE',
	'DE',
	'GH',
	'GI',
	'GR',
	'GL',
	'GD',
	'GP',
	'GU',
	'GT',
	'GG',
	'GN',
	'GW',
	'GY',
	'HT',
	'HN',
	'HK',
	'HU',
	'IS',
	'IN',
	'ID',
	'IR',
	'IQ',
	'IE',
	'IM',
	'IL',
	'IT',
	'JM',
	'JP',
	'JE',
	'JO',
	'KZ',
	'KE',
	'KI',
	'KR',
	'XK',
	'KW',
	'KG',
	'LA',
	//'419',
	'LV',
	'LB',
	'LS',
	'LR',
	'LY',
	'LI',
	'LT',
	'LU',
	'MO',
	'MG',
	'MW',
	'MY',
	'MV',
	'ML',
	'MT',
	'MH',
	'MQ',
	'MR',
	'MU',
	'YT',
	'MX',
	'FM',
	'MD',
	'MC',
	'MN',
	'ME',
	'MS',
	'MA',
	'MZ',
	'MM',
	'NA',
	'NR',
	'NP',
	'NL',
	'NC',
	'NZ',
	'NI',
	'NE',
	'NG',
	'NU',
	'NF',
	'KP',
	'MK',
	'MP',
	'NO',
	'OM',
	'PK',
	'PW',
	'PS',
	'PA',
	'PG',
	'PY',
	'PE',
	'PH',
	'PN',
	'PL',
	'PT',
	'PR',
	'QA',
	'RE',
	'RO',
	'RU',
	'RW',
	'WS',
	'SM',
	'ST',
	'SA',
	'SN',
	'RS',
	'SC',
	'SL',
	'SG',
	'SX',
	'SK',
	'SI',
	'SB',
	'SO',
	'ZA',
	'SS',
	'ES',
	'LK',
	'SH',
	'BL',
	'KN',
	'LC',
	'MF',
	'PM',
	'VC',
	'SD',
	'SR',
	'SJ',
	'SE',
	'CH',
	'SY',
	'TW',
	'TJ',
	'TZ',
	'TH',
	'TL',
	'TG',
	'TK',
	'TO',
	'TT',
	'TN',
	'TR',
	'TM',
	'TC',
	'TV',
	'UM',
	'VI',
	'UG',
	'UA',
	'AE',
	'GB',
	'US',
	'UY',
	'UZ',
	'VU',
	'VA',
	'VE',
	'VN',
	'WF',
	//'001',
	'YE',
	'ZM',
	'ZW',
];

const regions: Record<string, string | undefined> = Object.fromEntries(
	regionCodes
		.map((regionCode) => [regionCode, regionNames.of(regionCode)])
		.orderBy(([, value]) => value),
);

export const RegionDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Domain.Globalization']);

		return (
			<select {...props}>
				<option value="">
					{t(
						'VocaDb.Web.Resources.Domain.Globalization:InterfaceLanguage.Other',
					)}
				</option>
				{Object.entries(regions).map(([key, value]) => (
					<option value={key} key={key}>
						{value}
					</option>
				))}
			</select>
		);
	},
);

export const EntryStatusDropdownList = React.memo(
	({
		allowedEntryStatuses,
		...props
	}: DropdownListProps & {
		allowedEntryStatuses: EntryStatus[];
	}): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<select {...props}>
				{allowedEntryStatuses
					.map((status) => EntryStatus[status])
					.map((status) => (
						<option value={status} key={status}>
							{t(`Resources:EntryStatusNames.${status}`)}
						</option>
					))}
			</select>
		);
	},
);

export const SongListFeaturedCategoryDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<select {...props}>
				{Object.values(SongListFeaturedCategory).map((featuredCategory) => (
					<option value={featuredCategory} key={featuredCategory}>
						{t(`Resources:SongListFeaturedCategoryNames.${featuredCategory}`)}
					</option>
				))}
			</select>
		);
	},
);

const associatedArtistTypes = Object.values(ArtistLinkType).filter(
	(artistLinkType) => artistLinkType !== ArtistLinkType.Group,
);

export const AssociatedArtistTypeDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Domain.Artists']);

		return (
			<select {...props}>
				{associatedArtistTypes.map((associatedArtistType) => (
					<option value={associatedArtistType} key={associatedArtistType}>
						{t(
							`VocaDb.Web.Resources.Domain.Artists:ArtistLinkTypeNames.${associatedArtistType}`,
						)}
					</option>
				))}
			</select>
		);
	},
);

export const AlbumTypeDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const vdb = useVdb();

		const { t } = useTranslation(['VocaDb.Model.Resources.Albums']);

		return (
			<select {...props}>
				{vdb.values.albumTypes.map((albumType) => (
					<option value={albumType} key={albumType}>
						{t(`VocaDb.Model.Resources.Albums:DiscTypeNames.${albumType}`)}
					</option>
				))}
			</select>
		);
	},
);

export const SongTypeDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const vdb = useVdb();

		const { t } = useTranslation(['VocaDb.Model.Resources.Songs']);

		return (
			<select {...props}>
				{vdb.values.songTypes.map((songType) => (
					<option value={songType} key={songType}>
						{t(`VocaDb.Model.Resources.Songs:SongTypeNames.${songType}`)}
					</option>
				))}
			</select>
		);
	},
);

export const PVTypeDescriptionsDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<select {...props}>
				{Object.values(PVType)
					.filter((pvType) => isNaN(Number(pvType)))
					.map((pvType) => (
						<option value={pvType} key={pvType}>
							{t(`Resources:PVTypeDescriptions.${pvType}`)}
						</option>
					))}
			</select>
		);
	},
);

const activityEntryTargetTypes: EntryType[] = [
	EntryType.Undefined,
	EntryType.Album,
	EntryType.Artist,
	EntryType.ReleaseEvent,
	EntryType.Song,
	EntryType.SongList,
	EntryType.Tag,
	EntryType.Venue,
];

export const ActivityEntryTargetTypeDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation('VocaDb.Web.Resources.Domain');

		return (
			<select {...props}>
				{Object.values(activityEntryTargetTypes).map((entryType) => (
					<option value={entryType} key={entryType}>
						{t(`VocaDb.Web.Resources.Domain:EntryTypeNames.${entryType}`)}
					</option>
				))}
			</select>
		);
	},
);

export const EmailOptionsDropdownList = React.memo(
	(props: DropdownListProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<select {...props}>
				{Object.values(UserEmailOptions).map((userEmailOption) => (
					<option value={userEmailOption} key={userEmailOption}>
						{t(`Resources:UserEmailOptionsNames.${userEmailOption}`)}
					</option>
				))}
			</select>
		);
	},
);
