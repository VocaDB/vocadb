import EntryRefContract from '@DataContracts/EntryRefContract';
import functions from '@Shared/GlobalFunctions';
import $ from 'jquery';
import _ from 'lodash';
import 'qtip2';
import React from 'react';

const whitelistedDomains = [
	'http://vocadb.net',
	'https://vocadb.net',
	'http://utaitedb.net',
	'https://utaitedb.net',
	'https://touhoudb.com',
];

interface ToolTipProps {
	children?: React.ReactNode;
	relativeUrl: string;
	id: number;
	params?: any;
	foreignDomain?: string;
}

const ToolTip = ({
	children,
	relativeUrl,
	id,
	params,
	foreignDomain,
}: ToolTipProps): React.ReactElement => {
	const ref = React.useRef<HTMLDivElement>(undefined!);

	React.useEffect(() => {
		const url =
			foreignDomain &&
			_.some(whitelistedDomains, (domain) =>
				_.includes(foreignDomain.toLocaleLowerCase(), domain),
			)
				? functions.mergeUrls(foreignDomain, relativeUrl)
				: functions.mapAbsoluteUrl(relativeUrl);
		const data = _.assign({ id: id }, params);

		$(ref.current).qtip({
			content: {
				text: 'Loading...' /* TODO: localize */,
				ajax: {
					url: url,
					type: 'GET',
					data: data,
					dataType: foreignDomain ? 'jsonp' : undefined,
				},
			},
			position: {
				viewport: $(window),
			},
			style: {
				classes: 'tooltip-wide',
			},
		});
	});

	return <div ref={ref}>{children}</div>;
};

interface AlbumToolTipProps {
	children?: React.ReactNode;
	id: number;
}

export const AlbumToolTip = ({
	children,
	id,
}: AlbumToolTipProps): React.ReactElement => {
	return (
		<ToolTip relativeUrl="/Album/PopupContent" id={id}>
			{children}
		</ToolTip>
	);
};

interface ArtistToolTipProps {
	children?: React.ReactNode;
	id: number;
}

export const ArtistToolTip = ({
	children,
	id,
}: ArtistToolTipProps): React.ReactElement => {
	return (
		<ToolTip relativeUrl="/Artist/PopupContent" id={id}>
			{children}
		</ToolTip>
	);
};

interface EventToolTipProps {
	children?: React.ReactNode;
	id: number;
}

export const EventToolTip = ({
	children,
	id,
}: EventToolTipProps): React.ReactElement => {
	const culture = vdb.values.uiCulture || undefined;

	return (
		<ToolTip
			relativeUrl="/Event/PopupContent"
			id={id}
			params={{ culture: culture }}
		>
			{children}
		</ToolTip>
	);
};

interface SongToolTipProps {
	children?: React.ReactNode;
	id: number;
	toolTipDomain?: string;
	version?: number;
}

export const SongToolTip = ({
	children,
	id,
	toolTipDomain,
	version,
}: SongToolTipProps): React.ReactElement => {
	return (
		<ToolTip
			relativeUrl="/Song/PopupContentWithVote"
			id={id}
			params={{ version: version }}
			foreignDomain={toolTipDomain}
		>
			{children}
		</ToolTip>
	);
};

interface TagToolTipProps {
	children?: React.ReactNode;
	id: number;
}

export const TagToolTip = ({
	children,
	id,
}: TagToolTipProps): React.ReactElement => {
	const culture = vdb.values.uiCulture || undefined;
	const lang = vdb.values.languagePreference;

	return (
		<ToolTip
			relativeUrl="/Tag/PopupContent"
			id={id}
			params={{ culture: culture, lang: lang }}
		>
			{children}
		</ToolTip>
	);
};

interface UserToolTipProps {
	children?: React.ReactNode;
	id: number;
}

export const UserToolTip = ({
	children,
	id,
}: UserToolTipProps): React.ReactElement => {
	var culture = vdb.values.uiCulture || undefined;

	return (
		<ToolTip
			relativeUrl="/User/PopupContent"
			id={id}
			params={{ culture: culture }}
		>
			{children}
		</ToolTip>
	);
};

interface EntryToolTipProps {
	children?: React.ReactNode;
	value: EntryRefContract;
}

export const EntryToolTip = ({
	children,
	value,
}: EntryToolTipProps): React.ReactElement => {
	switch (value.entryType) {
		case 'Album' /* TODO: enum */:
			return <AlbumToolTip children={children} id={value.id} />;

		case 'Artist' /* TODO: enum */:
			return <ArtistToolTip children={children} id={value.id} />;

		case 'ReleaseEvent' /* TODO: enum */:
			return <EventToolTip children={children} id={value.id} />;

		case 'Song' /* TODO: enum */:
			return <SongToolTip children={children} id={value.id} />;

		case 'Tag' /* TODO: enum */:
			return <TagToolTip children={children} id={value.id} />;

		case 'User' /* TODO: enum */:
			return <UserToolTip children={children} id={value.id} />;

		default:
			return <></>;
	}
};
