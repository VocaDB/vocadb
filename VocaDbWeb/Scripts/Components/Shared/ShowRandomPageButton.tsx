import Button from '@Bootstrap/Button';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import qs from 'qs';
import React from 'react';
import { useNavigate } from 'react-router-dom';

import { apiEndpointsForEntryType } from './GlobalSearchBox';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const random = (max: number): number => Math.floor(Math.random() * max);

interface ShowRandomPageButtonProps {
	entryType: EntryType;
}

const ShowRandomPageButton = React.memo(
	({ entryType }: ShowRandomPageButtonProps): React.ReactElement => {
		const [clicked, setClicked] = React.useState(false);

		const navigate = useNavigate();

		const handleClick = React.useCallback(async () => {
			try {
				setClicked(true);

				const apiEndpoint = apiEndpointsForEntryType[entryType];
				const params = { maxResults: 1 };

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
							PartialFindResultContract<{ id: number; entryType: string }>
						>(
							urlMapper.mapRelative(
								`${apiEndpoint}?${qs.stringify({ ...params, start: index })}`,
							),
						);
					})
					.then((result) => result.items[0]);

				navigate(
					EntryUrlMapper.details(
						entryType === EntryType.Undefined
							? EntryType[entry.entryType as keyof typeof EntryType]
							: entryType,
						entry.id,
					),
				);
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
				title="Show random page" /* TODO: localize */
			>
				<i className="icon-random" />
			</Button>
		);
	},
);

export default ShowRandomPageButton;
