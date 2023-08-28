// TODO: Should this be rendered on the server?
'use client';
import { apiGet } from '@/Helpers/FetchApiHelper';
import Link from 'next/link';
import { useState } from 'react';
import useSWR from 'swr';
import EntryToolTip from './EntryToolTip';

interface EntryLinkProps {
	itemUrl: string;
	children: JSX.Element | JSX.Element[] | string;
}

export default function EntryLink({ itemUrl, children }: EntryLinkProps) {
	const [hovered, setHovered] = useState(false);
	const apiUrl = itemUrl
		.replace('/Ar/', '/artists/')
		.replace('/Al/', '/albums/')
		.replace('/S/', '/songs/');
	const { data, isLoading } = useSWR(
		hovered ? `/api${apiUrl}?fields=AdditionalNames` : null,
		apiGet,
		{
			revalidateOnFocus: false,
			revalidateIfStale: false,
		}
	);
	const entry = itemUrl.includes('/Ar/')
		? 'artist'
		: itemUrl.includes('/Al/')
		? 'album'
		: itemUrl.includes('/S/')
		? 'song'
		: 'tag';

	const child = (
		<Link
			onMouseOver={() => {
				if (!hovered) setHovered(true);
			}}
			href={itemUrl}
		>
			{children}
		</Link>
	);

	return isLoading || !hovered ? (
		child
	) : (
		//@ts-ignore
		<EntryToolTip artist={data} album={data} song={data} entry={entry}>
			{child}
		</EntryToolTip>
	);
}

