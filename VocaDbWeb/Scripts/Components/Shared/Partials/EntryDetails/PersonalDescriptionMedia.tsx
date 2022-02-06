import Button from '@Bootstrap/Button';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import SelfDescriptionStore from '@Stores/SelfDescriptionStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

import Markdown from '../../../KnockoutExtensions/Markdown';

interface PersonalDescriptionMediaProps {
	personalDescription: SelfDescriptionStore;
	canEditPersonalDescription: boolean;
}

const PersonalDescriptionMedia = observer(
	({
		personalDescription,
		canEditPersonalDescription,
	}: PersonalDescriptionMediaProps) => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<div className="media">
				{!personalDescription.author.isEmpty &&
					personalDescription.author.entry?.mainPicture && (
						<Link
							to={EntryUrlMapper.details(
								EntryType.Artist,
								personalDescription.author.id!,
							)}
							className="pull-left"
						>
							<img
								className="coverPicThumb"
								alt="Thumb" /* TODO: localize */
								src={personalDescription.author.entry.mainPicture.urlSmallThumb}
								referrerPolicy="same-origin"
							/>
						</Link>
					)}

				<div className="media-body">
					{canEditPersonalDescription && (
						<div className="pull-right">
							<SafeAnchor
								onClick={personalDescription.beginEdit}
								href="#"
								className="textLink editLink"
							>
								{t('ViewRes:Shared.Edit')}
							</SafeAnchor>
						</div>
					)}
					{!personalDescription.author.isEmpty && (
						<h3 className="media-heading">
							<Link
								to={EntryUrlMapper.details(
									EntryType.Artist,
									personalDescription.author.id!,
								)}
							>
								{personalDescription.author.name}
							</Link>
						</h3>
					)}
					{personalDescription.editing ? (
						<div>
							<select
								value={personalDescription.author.id}
								onChange={(e): void =>
									runInAction(() => {
										personalDescription.author.id = Number(e.target.value);
									})
								}
							>
								<option>
									{t('ViewRes:EntryDetails.PersonalDescriptionSelectAuthor')}
								</option>
								{personalDescription.artists.map((artist) => (
									<option value={artist.id} key={artist.id}>
										{artist.name}
									</option>
								))}
							</select>
							<br />
							<textarea
								value={personalDescription.text}
								onChange={(e): void =>
									runInAction(() => {
										personalDescription.text = e.target.value;
									})
								}
								rows={5}
								cols={100}
								maxLength={2000}
								className="input-xxlarge"
							/>
							<br />
							<Button onClick={personalDescription.save} variant="primary">
								{t('ViewRes:Shared.Save')}
							</Button>{' '}
							<Button onClick={personalDescription.cancelEdit}>
								{t('ViewRes:Shared.Cancel')}
							</Button>
						</div>
					) : (
						<div>
							<p>
								<Markdown>{personalDescription.text!}</Markdown>
							</p>
						</div>
					)}
				</div>
			</div>
		);
	},
);

export default PersonalDescriptionMedia;
