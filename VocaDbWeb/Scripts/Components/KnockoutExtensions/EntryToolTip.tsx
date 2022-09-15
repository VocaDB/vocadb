import OverlayTrigger from '@/Bootstrap/OverlayTrigger';
import { AlbumPopupContent } from '@/Components/Shared/AlbumPopupContent';
import { AlbumWithCoverPopupContent } from '@/Components/Shared/AlbumWithCoverPopupContent';
import { ArtistPopupContent } from '@/Components/Shared/ArtistPopupContent';
import { EventPopupContent } from '@/Components/Shared/EventPopupContent';
import { SongWithVotePopupContent } from '@/Components/Shared/SongWithVotePopupContent';
import { TagPopupContent } from '@/Components/Shared/TagPopupContent';
import { UserPopupContent } from '@/Components/Shared/UserPopupContent';
import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { EntryRefContract } from '@/DataContracts/EntryRefContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongWithPVAndVoteContract } from '@/DataContracts/Song/SongWithPVAndVoteContract';
import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { EntryType } from '@/Models/EntryType';
import { QTipToolTip } from '@/QTip/QTipToolTip';
import {
	AlbumOptionalField,
	AlbumRepository,
} from '@/Repositories/AlbumRepository';
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

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);
const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);

interface AlbumToolTipProps {
	id: number;
	children?: React.ReactNode;
	withCover?: boolean;
}

export const AlbumToolTip = React.memo(
	({ id, children, withCover }: AlbumToolTipProps): React.ReactElement => {
		const [show, setShow] = React.useState(false);

		const [album, setAlbum] = React.useState<AlbumContract>();

		React.useEffect(() => {
			if (album) return;

			if (!show) return;

			albumRepo
				.getOneWithComponents({
					id: id,
					fields: [
						AlbumOptionalField.AdditionalNames,
						AlbumOptionalField.MainPicture,
					],
					lang: vdb.values.languagePreference,
				})
				.then((album) => setAlbum(album));
		}, [album, show, id]);

		return (
			<OverlayTrigger
				placement="bottom-start"
				delay={{ show: 250, hide: 0 }}
				flip
				offset={[0, 8]}
				overlay={
					album ? (
						<QTipToolTip style={{ opacity: 1 }}>
							{withCover ? (
								<AlbumWithCoverPopupContent album={album} />
							) : (
								<AlbumPopupContent album={album} />
							)}
						</QTipToolTip>
					) : (
						<></>
					)
				}
				onToggle={(nextShow): void => setShow(nextShow)}
			>
				<span>{children}</span>
			</OverlayTrigger>
		);
	},
);

interface ArtistToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const ArtistToolTip = React.memo(
	({ id, children }: ArtistToolTipProps): React.ReactElement => {
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
			<OverlayTrigger
				placement="bottom-start"
				delay={{ show: 250, hide: 0 }}
				flip
				offset={[0, 8]}
				overlay={
					artist ? (
						<QTipToolTip style={{ opacity: 1 }}>
							<ArtistPopupContent artist={artist} />
						</QTipToolTip>
					) : (
						<></>
					)
				}
				onToggle={(nextShow): void => setShow(nextShow)}
			>
				<span>{children}</span>
			</OverlayTrigger>
		);
	},
);

interface EventToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const EventToolTip = React.memo(
	({ id, children }: EventToolTipProps): React.ReactElement => {
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
			<OverlayTrigger
				placement="bottom-start"
				delay={{ show: 250, hide: 0 }}
				flip
				offset={[0, 8]}
				overlay={
					event ? (
						<QTipToolTip style={{ opacity: 1 }}>
							<EventPopupContent event={event} />
						</QTipToolTip>
					) : (
						<></>
					)
				}
				onToggle={(nextShow): void => setShow(nextShow)}
			>
				<span>{children}</span>
			</OverlayTrigger>
		);
	},
);

const allowedDomains = [
	'http://vocadb.net',
	'https://vocadb.net',
	'http://utaitedb.net',
	'https://utaitedb.net',
	'https://touhoudb.com',
];

interface SongToolTipProps {
	id: number;
	children?: React.ReactNode;
	foreignDomain?: string;
}

export const SongToolTip = React.memo(
	({ id, children, foreignDomain }: SongToolTipProps): React.ReactElement => {
		const [show, setShow] = React.useState(false);

		const [song, setSong] = React.useState<SongWithPVAndVoteContract>();

		React.useEffect(() => {
			if (song) return;

			if (!show) return;

			const baseUrl =
				foreignDomain &&
				allowedDomains.some((domain) =>
					foreignDomain.toLocaleLowerCase().includes(domain),
				)
					? foreignDomain
					: undefined;

			songRepo
				.getOneWithComponents({
					baseUrl: baseUrl,
					id: id,
					fields: [
						SongOptionalField.AdditionalNames,
						SongOptionalField.ThumbUrl,
					],
					lang: vdb.values.languagePreference,
				})
				.then(async (song) => {
					const vote = vdb.values.loggedUser
						? await userRepo.getSongRating({
								userId: vdb.values.loggedUser.id,
								songId: song.id,
						  })
						: 'Nothing';

					setSong({ ...song, pvs: [], vote: vote });
				});
		}, [song, show, id, foreignDomain]);

		return (
			<OverlayTrigger
				placement="bottom-start"
				delay={{ show: 250, hide: 0 }}
				flip
				offset={[0, 8]}
				overlay={
					song ? (
						<QTipToolTip style={{ opacity: 1 }}>
							<SongWithVotePopupContent song={song} />
						</QTipToolTip>
					) : (
						<></>
					)
				}
				onToggle={(nextShow): void => setShow(nextShow)}
			>
				<span>{children}</span>
			</OverlayTrigger>
		);
	},
);

interface TagToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const TagToolTip = React.memo(
	({ id, children }: TagToolTipProps): React.ReactElement => {
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
			<OverlayTrigger
				placement="bottom-start"
				delay={{ show: 250, hide: 0 }}
				flip
				offset={[0, 8]}
				overlay={
					tag ? (
						<QTipToolTip style={{ opacity: 1 }}>
							<TagPopupContent tag={tag} />
						</QTipToolTip>
					) : (
						<></>
					)
				}
				onToggle={(nextShow): void => setShow(nextShow)}
			>
				<span>{children}</span>
			</OverlayTrigger>
		);
	},
);

interface UserToolTipProps {
	id: number;
	children?: React.ReactNode;
}

export const UserToolTip = React.memo(
	({ id, children }: UserToolTipProps): React.ReactElement => {
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
			<OverlayTrigger
				placement="bottom-start"
				delay={{ show: 250, hide: 0 }}
				flip
				offset={[0, 8]}
				overlay={
					user ? (
						<QTipToolTip style={{ opacity: 1 }}>
							<UserPopupContent user={user} />
						</QTipToolTip>
					) : (
						<></>
					)
				}
				onToggle={(nextShow): void => setShow(nextShow)}
			>
				<span>{children}</span>
			</OverlayTrigger>
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
