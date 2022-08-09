import { ArtistLink } from '@/Components/Shared/Partials/Artist/ArtistLink';
import { ArtistTypeLabel } from '@/Components/Shared/Partials/Artist/ArtistTypeLabel';
import { ArtistLinkContract } from '@/DataContracts/Song/ArtistLinkContract';
import { ArtistCategories } from '@/Models/Artists/ArtistCategories';
import { ArtistRoles } from '@/Models/Artists/ArtistRoles';
import _ from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';

export enum ShowRolesMode {
	Never,
	IfNotDefault,
	IfNotVocalist,
}

const shouldShowRoles = (
	artist: ArtistLinkContract,
	showRoles: ShowRolesMode,
): boolean => {
	switch (showRoles) {
		case ShowRolesMode.IfNotDefault:
			return artist.effectiveRoles !== ArtistRoles[ArtistRoles.Default];

		case ShowRolesMode.IfNotVocalist:
			return (
				artist.effectiveRoles !== ArtistRoles[ArtistRoles.Default] &&
				artist.effectiveRoles !== ArtistRoles[ArtistRoles.Vocalist] &&
				!artist.categories
					.split(',')
					.map((category) => category.trim())
					.includes(ArtistCategories.Subject) &&
				!artist.categories
					.split(',')
					.map((category) => category.trim())
					.includes(ArtistCategories.Producer)
			);

		default:
			return false;
	}
};

interface ArtistListProps {
	artists: ArtistLinkContract[];
	showRoles?: ShowRolesMode;
	showType?: boolean;
	tooltip?: boolean;
}

export const ArtistList = ({
	artists,
	showRoles = ShowRolesMode.Never,
	showType = false,
	tooltip = false,
}: ArtistListProps): React.ReactElement => {
	const { t } = useTranslation(['Resources']);

	const ordered = React.useMemo(() => _.sortBy(artists, (a) => a.isSupport), [
		artists,
	]);

	return (
		<>
			{ordered.map((artist, index) => (
				<React.Fragment key={index}>
					{index > 0 && ', '}

					{artist.isSupport && '('}

					{artist.artist ? (
						<>
							{showType && (
								<ArtistTypeLabel artistType={artist.artist.artistType} />
							)}
							{showType && ' '}
							<ArtistLink
								artist={artist.artist}
								name={artist.name}
								tooltip={tooltip}
							/>
						</>
					) : (
						artist.name
					)}

					{shouldShowRoles(artist, showRoles) && (
						<>
							{' '}
							<small className="muted">
								(
								{artist.effectiveRoles
									.split(',')
									.map((role) => role.trim())
									.map((role, index) => (
										<React.Fragment key={role}>
											{index > 0 && ', '}
											{t(`Resources:ArtistRoleNames.${role}`)}
										</React.Fragment>
									))}
								)
							</small>
						</>
					)}

					{artist.isSupport && ')'}
				</React.Fragment>
			))}
		</>
	);
};
