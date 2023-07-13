import { formatFromMilliBpm } from '@/Helpers/BpmHelper';
import { formatNumberToTime } from '@/Helpers/DateTimeHelper';
import { apiGet, apiPost, authApiGet } from '@/Helpers/FetchApiHelper';
import AlbumLink from '@/components/Links/AlbumLink';
import ArtistLink from '@/components/Links/ArtistLink';
import TagLink from '@/components/Links/TagLink';
import PVButton from '@/components/PVButton/PVButton';
import EmbedPVPreview from '@/nostalgic-darling/EmbedPVPreview';
import { useVdbStore } from '@/stores/useVdbStore';
import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { ArtistForSongContract } from '@/types/DataContracts/Song/ArtistForSongContract';
import { SongDetailsContract } from '@/types/DataContracts/Song/SongDetailsContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import { ArtistCategories } from '@/types/Models/Artists/ArtistCategories';
import { TranslationType } from '@/types/Models/Globalization/TranslationType';
import { PVType } from '@/types/Models/PVs/PVType';
import { SongVoteRating, parseSongVoteRating } from '@/types/Models/SongVoteRating';
import { ActionIcon, Grid, Group, Stack, Tabs, Text } from '@mantine/core';
import {
	IconAffiliate,
	IconAlignJustified,
	IconHeart,
	IconInfoCircle,
	IconMessageCircle,
	IconShare,
	IconThumbUp,
} from '@tabler/icons-react';
import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import React, { useState } from 'react';
import useSWR from 'swr';
// TODO: Lazy load this
import { extendedUserLanguageCultures } from '@/Helpers/userLanguageCultures';

interface SongActionsProps {
	details: SongDetailsContract;
}

const SongActions = ({ details }: SongActionsProps) => {
	const [values] = useVdbStore((set) => [set.values]);
	const { data, mutate } = useSWR(
		'/api/users/current/ratedSongs/' + details.song.id,
		authApiGet<string>
	);

	const postNewRating = async (newRating: SongVoteRating) => {
		await apiPost(`/api/songs/${details.song.id}/ratings`, {
			rating: SongVoteRating[newRating],
		});
	};

	const setRating = async (clickedRating: SongVoteRating) => {
		const newRating =
			SongVoteRating[clickedRating] === data ? SongVoteRating.Nothing : clickedRating;
		await postNewRating(newRating);
		mutate(SongVoteRating[newRating]);
	};

	const rating = parseSongVoteRating(data ?? SongVoteRating[SongVoteRating.Nothing]);

	const SongVoteButton = (buttonRating: SongVoteRating, icon: JSX.Element) => (
		<ActionIcon
			variant={rating === buttonRating ? 'filled' : undefined}
			onClick={() => setRating(buttonRating)}
			color="default"
			radius="xl"
			size="lg"
			disabled={values !== undefined && !values.isLoggedIn}
		>
			{icon}
		</ActionIcon>
	);

	return (
		<Group mt="sm">
			{SongVoteButton(SongVoteRating.Like, <IconThumbUp />)}
			{SongVoteButton(SongVoteRating.Favorite, <IconHeart />)}
		</Group>
	);
};

const SongProperty = ({
	name,
	show,
	children,
}: {
	name: string;
	show?: boolean;
	children?: JSX.Element[] | JSX.Element;
}) => {
	if (children === undefined || (show !== undefined && !show)) {
		return <></>;
	}
	return (
		<React.Fragment>
			<Grid.Col span={4} md={2}>
				<Text>{name}</Text>
			</Grid.Col>
			<Grid.Col span={8} md={10}>
				{children}
			</Grid.Col>
		</React.Fragment>
	);
};

interface SongBasicInfoProps {
	details: SongDetailsContract;
	setPV: (pv: PVContract) => any;
}

const SongBasicInfo = ({ details, setPV }: SongBasicInfoProps) => {
	const artistIsType = (artist: ArtistForSongContract, type: ArtistCategories): boolean => {
		return artist.categories
			.split(',')
			.map((category) => category.trim())
			.includes(type);
	};

	const artists = details.artists.filter((artist) =>
		artistIsType(artist, ArtistCategories.Vocalist)
	);
	const subjects = details.artists.filter((artist) =>
		artistIsType(artist, ArtistCategories.Subject)
	);
	const producers = details.artists.filter((artist) =>
		artistIsType(artist, ArtistCategories.Producer)
	);
	const bands = details.artists.filter((artist) => artistIsType(artist, ArtistCategories.Band));
	const animators = details.artists.filter((artist) =>
		artistIsType(artist, ArtistCategories.Animator)
	);
	const otherArtists = details.artists.filter(
		(artist) =>
			artistIsType(artist, ArtistCategories.Circle) ||
			artistIsType(artist, ArtistCategories.Label) ||
			artistIsType(artist, ArtistCategories.Illustrator) ||
			artistIsType(artist, ArtistCategories.Other)
	);
	const cultureCodes =
		details.cultureCodes.length > 0
			? details.cultureCodes
			: details.lyricsFromParents.filter(
					(l) => l.translationType === TranslationType[TranslationType.Original]
			  )[0]?.cultureCodes;
	const originalPVs = details.pvs?.filter((pv) => pv.pvType === PVType.Original);
	const otherPVs = details.pvs?.filter((pv) => pv.pvType !== PVType.Original);

	const mapArtists = (artists: ArtistForSongContract[]): JSX.Element[] => {
		return artists.map((a, index) => {
			return (
				<React.Fragment key={index}>
					{index !== 0 ? ', ' : ''}
					{a.artist === undefined ? (
						<Text span>{a.name}</Text>
					) : (
						<ArtistLink artist={a.artist} />
					)}
				</React.Fragment>
			);
		});
	};

	const mapTags = (tags: TagBaseContract[]): JSX.Element[] => {
		return tags.map((t, index) => {
			return (
				<React.Fragment key={index}>
					{index !== 0 ? ', ' : ''}
					<TagLink tag={t} />
				</React.Fragment>
			);
		});
	};

	const mapAlbums = (albums: AlbumForApiContract[]): JSX.Element[] => {
		return albums.map((a, index) => {
			return (
				<React.Fragment key={index}>
					{index !== 0 ? ', ' : ''}
					<AlbumLink album={a} />
				</React.Fragment>
			);
		});
	};

	return (
		<Grid mt="md">
			<SongProperty name="Name">
				<Text>{details.song.name}</Text>
				<Text fz="small" c="dimmed" span>
					{details.additionalNames}
				</Text>
			</SongProperty>
			<SongProperty name="Vocalists" show={artists.length > 0}>
				{mapArtists(artists)}
			</SongProperty>
			<SongProperty name="Subject" show={subjects.length > 0}>
				{mapArtists(subjects)}
			</SongProperty>
			<SongProperty name="Producers" show={producers.length > 0}>
				{mapArtists(producers)}
			</SongProperty>
			<SongProperty name="Band" show={bands.length > 0}>
				{mapArtists(bands)}
			</SongProperty>
			<SongProperty name="Animators" show={animators.length > 0}>
				{mapArtists(animators)}
			</SongProperty>
			<SongProperty name="Other artists" show={otherArtists.length > 0}>
				{mapArtists(otherArtists)}
			</SongProperty>
			<SongProperty name="Type">
				<TagLink tag={details.songTypeTag} />
			</SongProperty>
			<SongProperty name="Duration">
				<Text>{formatNumberToTime(details.song.lengthSeconds)}</Text>
			</SongProperty>
			<SongProperty
				name="Languages"
				show={cultureCodes !== undefined && cultureCodes.length > 0}
			>
				{/** TODO: Culture code -> Language */}
				<Text>
					{cultureCodes
						?.map((c) => extendedUserLanguageCultures[c].englishName)
						.join(', ')}
				</Text>
			</SongProperty>
			<SongProperty name="BPM" show={!!details.minMilliBpm}>
				<Text>{formatFromMilliBpm(details.minMilliBpm, details.maxMilliBpm)}</Text>
			</SongProperty>
			<SongProperty name="Albums" show={details.albums.length > 0}>
				{mapAlbums(details.albums)}
			</SongProperty>
			<SongProperty name="Tags">{mapTags(details.tags.map((t) => t.tag))}</SongProperty>
			<SongProperty name="Pools and song lists" show={details.pools.length > 0}>
				<Text>{details.pools.map((pool) => pool.name).join(', ')}</Text>
			</SongProperty>
			<SongProperty
				name="Original media"
				show={originalPVs !== undefined && originalPVs.length > 0}
			>
				<Stack align="flex-start">
					{originalPVs.map((pv) => (
						<PVButton onClick={() => setPV(pv)} pv={pv} />
					))}
				</Stack>
			</SongProperty>
			<SongProperty name="Other media" show={otherPVs !== undefined && otherPVs.length > 0}>
				<Stack align="flex-start">
					{otherPVs.map((pv) => (
						<PVButton onClick={() => setPV(pv)} pv={pv} />
					))}
				</Stack>
			</SongProperty>
		</Grid>
	);
};

interface SongTabsProps {
	details: SongDetailsContract;
	setPV: (pv: PVContract) => any;
}

const SongTabs = ({ details, setPV }: SongTabsProps) => {
	return (
		<Tabs mt="md" defaultValue="info">
			<Tabs.List>
				<Tabs.Tab value="info" icon={<IconInfoCircle size="0.8rem" />}>
					Basic Info
				</Tabs.Tab>
				<Tabs.Tab value="lyrics" icon={<IconAlignJustified size="0.8rem" />}>
					Lyrics
				</Tabs.Tab>
				<Tabs.Tab value="discussion" icon={<IconMessageCircle size="0.8rem" />}>
					Discussion
				</Tabs.Tab>
				<Tabs.Tab value="related" icon={<IconAffiliate size="0.8rem" />}>
					Related Songs
				</Tabs.Tab>
				<Tabs.Tab value="share" icon={<IconShare size="0.8rem" />}>
					Share
				</Tabs.Tab>
			</Tabs.List>

			<Tabs.Panel value="info">
				<SongBasicInfo setPV={setPV} details={details} />
			</Tabs.Panel>
			<Tabs.Panel value="lyrics">Lyrics</Tabs.Panel>
			<Tabs.Panel value="discussion">Discussion</Tabs.Panel>
			<Tabs.Panel value="related">Related Songs</Tabs.Panel>
			<Tabs.Panel value="share">Share</Tabs.Panel>
		</Tabs>
	);
};

export default function Page({ song }: InferGetServerSidePropsType<typeof getServerSideProps>) {
	const [pv, setPV] = useState<PVContract | undefined>(song.pvs[0]);
	return (
		<>
			<div style={{ marginRight: 'auto', marginLeft: 'auto', maxWidth: 'max-content' }}>
				{pv !== undefined && (
					<EmbedPVPreview song={{ ...song.song, pvs: song.pvs }} pv={pv} />
				)}
				<SongActions details={song} />
			</div>
			<SongTabs setPV={setPV} details={song} />
		</>
	);
}

export const getServerSideProps: GetServerSideProps<{
	song: SongDetailsContract;
}> = async ({ query }) => {
	const song = await apiGet<SongDetailsContract>(`/api/songs/${query.id}/details`);
	return { props: { song } };
};

