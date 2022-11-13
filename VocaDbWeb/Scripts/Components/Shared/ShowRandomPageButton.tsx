import Button from '@/Bootstrap/Button';
import { apiEndpointsForEntryType } from '@/Components/Shared/GlobalSearchBox';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { EntryType } from '@/Models/EntryType';
import { NameMatchMode } from '@/Models/NameMatchMode';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import qs from 'qs';
import React from 'react';
import { useNavigate } from 'react-router-dom';

const random = (max: number): number => Math.floor(Math.random() * max);

interface ShowRandomPageButtonProps {
	entryType: EntryType;

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

				const apiEndpoint = apiEndpointsForEntryType[entryType];
				const params = {
					maxResults: 1,
					nameMatchMode: NameMatchMode[NameMatchMode.Auto],
					query: globalSearchTermRef.current.value,
				};

				const entry = await httpClient
					.get<{ totalCount: number }>(
						urlMapper.mapRelative(
							`${apiEndpoint}?${qs.stringify({
								...params,
								getTotalCount: true,
							})}`,
						),
					)
					.then(async (result) => {
						const index = random(result.totalCount);

						return httpClient.get<
							PartialFindResultContract<{ id: number; entryType: EntryType }>
						>(
							urlMapper.mapRelative(
								`${apiEndpoint}?${qs.stringify({ ...params, start: index })}`,
							),
						);
					})
					.then((result) => result.items[0]);

				navigate(
					EntryUrlMapper.details(
						entryType === EntryType.Undefined ? entry.entryType : entryType,
						entry.id,
					),
				);
			} finally {
				setClicked(false);
			}
		}, [entryType, globalSearchTermRef, navigate]);

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
