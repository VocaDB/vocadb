import { AlbumPopupContent } from '@/Components/Shared/AlbumPopupContent';
import { ArtistPopupContent } from '@/Components/Shared/ArtistPopupContent';
import { EventPopupContent } from '@/Components/Shared/EventPopupContent';
import { SongPopupContent } from '@/Components/Shared/SongPopupContent';
import { TagPopupContent } from '@/Components/Shared/TagPopupContent';
import { UserPopupContent } from '@/Components/Shared/UserPopupContent';
import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { EntryRefContract } from '@/DataContracts/EntryRefContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { EntryType } from '@/Models/EntryType';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import {
	ArtistOptionalField,
	ArtistRepository,
} from '@/Repositories/ArtistRepository';
import {
	ReleaseEventOptionalField,
	ReleaseEventRepository,
} from '@/Repositories/ReleaseEventRepository';
import {
	SongOptionalField,
	SongRepository,
} from '@/Repositories/SongRepository';
import { TagOptionalField, TagRepository } from '@/Repositories/TagRepository';
import {
	UserOptionalField,
	UserRepository,
} from '@/Repositories/UserRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import React from 'react';
import Overlay from 'react-overlays/Overlay';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);
const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);

type QTipProps = React.HTMLAttributes<HTMLDivElement>;

const QTip = React.forwardRef<HTMLDivElement>(
	({ children, ...props }: QTipProps, ref): React.ReactElement => {
		return (
			<div
				{...props}
				ref={ref}
				className="qtip qtip-default tooltip-wide qtip-pos-tl qtip-focus"
				style={{
					...props.style,
					display: 'block',
					zIndex: 15001,
				}}
			>
				<div className="qtip-content">{children}</div>
			</div>
		);
	},
);

interface AlbumToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const AlbumToolTip = React.memo(
	({ id, children }: AlbumToolTipProps): React.ReactElement => {
		const triggerRef = React.useRef<HTMLDivElement>(undefined!);
		const containerRef = React.useRef<HTMLDivElement>(undefined!);

		const [show, setShow] = React.useState(false);

		const [album, setAlbum] = React.useState<AlbumContract>();

		React.useEffect(() => {
			if (album) return;

			if (!show) return;

			albumRepo
				.getOne({ id: id, lang: vdb.values.languagePreference })
				.then((album) => setAlbum(album));
		}, [album, show, id]);

		return (
			<span ref={containerRef}>
				<span
					ref={triggerRef}
					onMouseEnter={(): void => setShow(true)}
					onMouseLeave={(): void => setShow(false)}
				>
					{children}
				</span>
				{show && album && (
					<Overlay
						show={show}
						offset={[0, 8]}
						onHide={(): void => setShow(false)}
						placement="bottom-start"
						container={containerRef}
						target={triggerRef}
						flip
					>
						{({ props }): React.ReactElement => (
							<QTip {...props}>
								<AlbumPopupContent album={album} />
							</QTip>
						)}
					</Overlay>
				)}
			</span>
		);
	},
);

interface ArtistToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const ArtistToolTip = React.memo(
	({ id, children }: ArtistToolTipProps): React.ReactElement => {
		const triggerRef = React.useRef<HTMLDivElement>(undefined!);
		const containerRef = React.useRef<HTMLDivElement>(undefined!);

		const [show, setShow] = React.useState(false);

		const [artist, setArtist] = React.useState<ArtistContract>();

		React.useEffect(() => {
			if (artist) return;

			if (!show) return;

			artistRepo
				.getOneWithComponents({
					id: id,
					fields: [
						ArtistOptionalField.AdditionalNames,
						ArtistOptionalField.MainPicture,
					],
					lang: vdb.values.languagePreference,
				})
				.then((artist) => setArtist(artist));
		}, [artist, show, id]);

		return (
			<span ref={containerRef}>
				<span
					ref={triggerRef}
					onMouseEnter={(): void => setShow(true)}
					onMouseLeave={(): void => setShow(false)}
				>
					{children}
				</span>
				{show && artist && (
					<Overlay
						show={show}
						offset={[0, 8]}
						onHide={(): void => setShow(false)}
						placement="bottom-start"
						container={containerRef}
						target={triggerRef}
						flip
					>
						{({ props }): React.ReactElement => (
							<QTip {...props}>
								<ArtistPopupContent artist={artist} />
							</QTip>
						)}
					</Overlay>
				)}
			</span>
		);
	},
);

interface EventToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const EventToolTip = React.memo(
	({ id, children }: EventToolTipProps): React.ReactElement => {
		const triggerRef = React.useRef<HTMLDivElement>(undefined!);
		const containerRef = React.useRef<HTMLDivElement>(undefined!);

		const [show, setShow] = React.useState(false);

		const [event, setEvent] = React.useState<ReleaseEventContract>();

		React.useEffect(() => {
			if (event) return;

			if (!show) return;

			eventRepo
				.getOne({
					id: id,
					fields: [
						ReleaseEventOptionalField.AdditionalNames,
						ReleaseEventOptionalField.Description,
						ReleaseEventOptionalField.MainPicture,
						ReleaseEventOptionalField.Series,
					],
				})
				.then((event) => setEvent(event));
		}, [event, show, id]);

		return (
			<span ref={containerRef}>
				<span
					ref={triggerRef}
					onMouseEnter={(): void => setShow(true)}
					onMouseLeave={(): void => setShow(false)}
				>
					{children}
				</span>
				{show && event && (
					<Overlay
						show={show}
						offset={[0, 8]}
						onHide={(): void => setShow(false)}
						placement="bottom-start"
						container={containerRef}
						target={triggerRef}
						flip
					>
						{({ props }): React.ReactElement => (
							<QTip {...props}>
								<EventPopupContent event={event} />
							</QTip>
						)}
					</Overlay>
				)}
			</span>
		);
	},
);

interface SongToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const SongToolTip = React.memo(
	({ id, children }: SongToolTipProps): React.ReactElement => {
		const triggerRef = React.useRef<HTMLDivElement>(undefined!);
		const containerRef = React.useRef<HTMLDivElement>(undefined!);

		const [show, setShow] = React.useState(false);

		const [song, setSong] = React.useState<SongContract>();

		React.useEffect(() => {
			if (song) return;

			if (!show) return;

			songRepo
				.getOneWithComponents({
					id: id,
					fields: [
						SongOptionalField.AdditionalNames,
						SongOptionalField.ThumbUrl,
					],
					lang: vdb.values.languagePreference,
				})
				.then((song) => setSong(song));
		}, [song, show, id]);

		return (
			<span ref={containerRef}>
				<span
					ref={triggerRef}
					onMouseEnter={(): void => setShow(true)}
					onMouseLeave={(): void => setShow(false)}
				>
					{children}
				</span>
				{show && song && (
					<Overlay
						show={show}
						offset={[0, 8]}
						onHide={(): void => setShow(false)}
						placement="bottom-start"
						container={containerRef}
						target={triggerRef}
						flip
					>
						{({ props }): React.ReactElement => (
							<QTip {...props}>
								<SongPopupContent song={song} />
							</QTip>
						)}
					</Overlay>
				)}
			</span>
		);
	},
);

interface TagToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const TagToolTip = React.memo(
	({ id, children }: TagToolTipProps): React.ReactElement => {
		const triggerRef = React.useRef<HTMLDivElement>(undefined!);
		const containerRef = React.useRef<HTMLDivElement>(undefined!);

		const [show, setShow] = React.useState(false);

		const [tag, setTag] = React.useState<TagApiContract>();

		React.useEffect(() => {
			if (tag) return;

			if (!show) return;

			tagRepo
				.getById({
					id: id,
					fields: [
						TagOptionalField.AdditionalNames,
						TagOptionalField.Description,
						TagOptionalField.MainPicture,
					],
					lang: vdb.values.languagePreference,
				})
				.then((tag) => setTag(tag));
		}, [tag, show, id]);

		return (
			<span ref={containerRef}>
				<span
					ref={triggerRef}
					onMouseEnter={(): void => setShow(true)}
					onMouseLeave={(): void => setShow(false)}
				>
					{children}
				</span>
				{show && tag && (
					<Overlay
						show={show}
						offset={[0, 8]}
						onHide={(): void => setShow(false)}
						placement="bottom-start"
						container={containerRef}
						target={triggerRef}
						flip
					>
						{({ props }): React.ReactElement => (
							<QTip {...props}>
								<TagPopupContent tag={tag} />
							</QTip>
						)}
					</Overlay>
				)}
			</span>
		);
	},
);

interface UserToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const UserToolTip = React.memo(
	({ id, children }: UserToolTipProps): React.ReactElement => {
		const triggerRef = React.useRef<HTMLDivElement>(undefined!);
		const containerRef = React.useRef<HTMLDivElement>(undefined!);

		const [show, setShow] = React.useState(false);

		const [user, setUser] = React.useState<UserApiContract>();

		React.useEffect(() => {
			if (user) return;

			if (!show) return;

			userRepo
				.getOne({ id: id, fields: [UserOptionalField.MainPicture] })
				.then((user) => setUser(user));
		}, [user, show, id]);

		return (
			<span ref={containerRef}>
				<span
					ref={triggerRef}
					onMouseEnter={(): void => setShow(true)}
					onMouseLeave={(): void => setShow(false)}
				>
					{children}
				</span>
				{show && user && (
					<Overlay
						show={show}
						offset={[0, 8]}
						onHide={(): void => setShow(false)}
						placement="bottom-start"
						container={containerRef}
						target={triggerRef}
						flip
					>
						{({ props }): React.ReactElement => (
							<QTip {...props}>
								<UserPopupContent user={user} />
							</QTip>
						)}
					</Overlay>
				)}
			</span>
		);
	},
);

interface EntryToolTipProps {
	entry: EntryRefContract;
	children?: React.ReactNode;
}

export const EntryToolTip = ({
	entry,
	children,
}: EntryToolTipProps): React.ReactElement => {
	switch (entry.entryType) {
		case EntryType[EntryType.Album]:
			return <AlbumToolTip id={entry.id}>{children}</AlbumToolTip>;

		case EntryType[EntryType.Artist]:
			return <ArtistToolTip id={entry.id}>{children}</ArtistToolTip>;

		case EntryType[EntryType.ReleaseEvent]:
			return <EventToolTip id={entry.id}>{children}</EventToolTip>;

		case EntryType[EntryType.Song]:
			return <SongToolTip id={entry.id}>{children}</SongToolTip>;

		case EntryType[EntryType.Tag]:
			return <TagToolTip id={entry.id}>{children}</TagToolTip>;

		case EntryType[EntryType.User]:
			return <UserToolTip id={entry.id}>{children}</UserToolTip>;

		default:
			return <></>;
	}
};
