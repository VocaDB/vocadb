import Button from '@/Bootstrap/Button';
import { EntryRefContract } from '@/DataContracts/EntryRefContract';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { TopBarStore } from '@/Stores/TopBarStore';
import React from 'react';
import { useNavigate } from 'react-router-dom';

interface ShowRandomPageButtonProps {
	entryType: (typeof TopBarStore.entryTypes)[number];
	// HACK: Replace this with a normal property after removing jQuery UI's Autocomplete.
	globalSearchTermRef: React.MutableRefObject<HTMLInputElement>;
}

export const ShowRandomPageButton = React.memo(
	({
		entryType,
		globalSearchTermRef,
	}: ShowRandomPageButtonProps): React.ReactElement => {
		const [clicked, setClicked] = React.useState(false);

		const navigate = useNavigate();

		const handleClick = React.useCallback(async () => {
			try {
				setClicked(true);

				const entry = await httpClient.get<EntryRefContract>(
					urlMapper.mapRelative(
						`/api/entries/random?entryType=${EntryType[entryType]}`,
					),
				);

				navigate(EntryUrlMapper.details(entry.entryType, entry.id));
			} finally {
				setClicked(false);
			}
		}, [entryType, navigate]);

		return (
			<Button
				variant="info"
				href="#"
				className="navbar-languageBtn"
				disabled={clicked}
				onClick={handleClick}
				title="Show random page" /* LOC */
			>
				<i className="icon-random" />
			</Button>
		);
	},
);
