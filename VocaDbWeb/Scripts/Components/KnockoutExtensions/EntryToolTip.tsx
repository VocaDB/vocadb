import { BsPrefixRefForwardingComponent } from '@Bootstrap/helpers';
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
	as?: React.ElementType;
	children?: React.ReactNode;
	relativeUrl: string;
	id: number;
	params?: any;
	foreignDomain?: string;
}

const ToolTip = React.forwardRef<HTMLElement, ToolTipProps>(
	(
		{
			as: Component = 'div',
			children,
			relativeUrl,
			id,
			params,
			foreignDomain,
			...props
		}: ToolTipProps,
		ref,
	): React.ReactElement => {
		const el = React.useRef<HTMLElement>(undefined!);
		React.useImperativeHandle<HTMLElement, HTMLElement>(ref, () => el.current);

		React.useEffect(() => {
			const url =
				foreignDomain &&
				_.some(whitelistedDomains, (domain) =>
					_.includes(foreignDomain.toLocaleLowerCase(), domain),
				)
					? functions.mergeUrls(foreignDomain, relativeUrl)
					: functions.mapAbsoluteUrl(relativeUrl);
			const data = _.assign({ id: id }, params);

			$(el.current).qtip({
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

		return (
			<Component {...props} ref={el}>
				{children}
			</Component>
		);
	},
);

interface AlbumToolTipProps {
	as?: React.ElementType;
	children?: React.ReactNode;
	id: number;
}

export const AlbumToolTip: BsPrefixRefForwardingComponent<
	'div',
	AlbumToolTipProps
> = React.forwardRef<HTMLElement, AlbumToolTipProps>(
	(
		{ as, children, id, ...props }: AlbumToolTipProps,
		ref,
	): React.ReactElement => {
		return (
			<ToolTip
				{...props}
				as={as}
				relativeUrl="/Album/PopupContent"
				id={id}
				ref={ref}
			>
				{children}
			</ToolTip>
		);
	},
);

interface ArtistToolTipProps {
	as?: React.ElementType;
	children?: React.ReactNode;
	id: number;
}

export const ArtistToolTip: BsPrefixRefForwardingComponent<
	'div',
	ArtistToolTipProps
> = React.forwardRef<HTMLElement, ArtistToolTipProps>(
	(
		{ as, children, id, ...props }: ArtistToolTipProps,
		ref,
	): React.ReactElement => {
		return (
			<ToolTip
				{...props}
				as={as}
				relativeUrl="/Artist/PopupContent"
				id={id}
				ref={ref}
			>
				{children}
			</ToolTip>
		);
	},
);

interface EventToolTipProps {
	as?: React.ElementType;
	children?: React.ReactNode;
	id: number;
}

export const EventToolTip: BsPrefixRefForwardingComponent<
	'div',
	EventToolTipProps
> = React.forwardRef<HTMLElement, EventToolTipProps>(
	(
		{ as, children, id, ...props }: EventToolTipProps,
		ref,
	): React.ReactElement => {
		const culture = vdb.values.uiCulture || undefined;

		return (
			<ToolTip
				{...props}
				as={as}
				relativeUrl="/Event/PopupContent"
				id={id}
				params={{ culture: culture }}
				ref={ref}
			>
				{children}
			</ToolTip>
		);
	},
);

interface SongToolTipProps {
	as?: React.ElementType;
	children?: React.ReactNode;
	id: number;
	toolTipDomain?: string;
	version?: number;
}

export const SongToolTip: BsPrefixRefForwardingComponent<
	'div',
	SongToolTipProps
> = React.forwardRef<HTMLElement, SongToolTipProps>(
	(
		{ as, children, id, toolTipDomain, version, ...props }: SongToolTipProps,
		ref,
	): React.ReactElement => {
		return (
			<ToolTip
				{...props}
				as={as}
				relativeUrl="/Song/PopupContentWithVote"
				id={id}
				params={{ version: version }}
				foreignDomain={toolTipDomain}
				ref={ref}
			>
				{children}
			</ToolTip>
		);
	},
);

interface TagToolTipProps {
	as?: React.ElementType;
	children?: React.ReactNode;
	id: number;
}

export const TagToolTip: BsPrefixRefForwardingComponent<
	'div',
	TagToolTipProps
> = React.forwardRef<HTMLElement, TagToolTipProps>(
	(
		{ as, children, id, ...props }: TagToolTipProps,
		ref,
	): React.ReactElement => {
		const culture = vdb.values.uiCulture || undefined;
		const lang = vdb.values.languagePreference;

		return (
			<ToolTip
				{...props}
				as={as}
				relativeUrl="/Tag/PopupContent"
				id={id}
				params={{ culture: culture, lang: lang }}
				ref={ref}
			>
				{children}
			</ToolTip>
		);
	},
);

interface UserToolTipProps {
	as?: React.ElementType;
	children?: React.ReactNode;
	id: number;
}

export const UserToolTip: BsPrefixRefForwardingComponent<
	'div',
	UserToolTipProps
> = React.forwardRef<HTMLElement, UserToolTipProps>(
	(
		{ as, children, id, ...props }: UserToolTipProps,
		ref,
	): React.ReactElement => {
		var culture = vdb.values.uiCulture || undefined;

		return (
			<ToolTip
				{...props}
				as={as}
				relativeUrl="/User/PopupContent"
				id={id}
				params={{ culture: culture }}
				ref={ref}
			>
				{children}
			</ToolTip>
		);
	},
);

interface EntryToolTipProps {
	as?: React.ElementType;
	children?: React.ReactNode;
	value: EntryRefContract;
}

export const EntryToolTip: BsPrefixRefForwardingComponent<
	'div',
	EntryToolTipProps
> = React.forwardRef<HTMLElement, EntryToolTipProps>(
	(
		{ as, children, value, ...props }: EntryToolTipProps,
		ref,
	): React.ReactElement => {
		switch (value.entryType) {
			case 'Album' /* TODO: enum */:
				return (
					<AlbumToolTip
						{...props}
						as={as}
						children={children}
						id={value.id}
						ref={ref}
					/>
				);

			case 'Artist' /* TODO: enum */:
				return (
					<ArtistToolTip
						{...props}
						as={as}
						children={children}
						id={value.id}
						ref={ref}
					/>
				);

			case 'ReleaseEvent' /* TODO: enum */:
				return (
					<EventToolTip
						{...props}
						as={as}
						children={children}
						id={value.id}
						ref={ref}
					/>
				);

			case 'Song' /* TODO: enum */:
				return (
					<SongToolTip
						{...props}
						as={as}
						children={children}
						id={value.id}
						ref={ref}
					/>
				);

			case 'Tag' /* TODO: enum */:
				return (
					<TagToolTip
						{...props}
						as={as}
						children={children}
						id={value.id}
						ref={ref}
					/>
				);

			case 'User' /* TODO: enum */:
				return (
					<UserToolTip
						{...props}
						as={as}
						children={children}
						id={value.id}
						ref={ref}
					/>
				);

			default:
				return <></>;
		}
	},
);
