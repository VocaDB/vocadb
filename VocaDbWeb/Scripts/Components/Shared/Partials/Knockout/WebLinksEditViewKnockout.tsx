import SafeAnchor from '@/Bootstrap/SafeAnchor';
import RequiredField from '@/Components/Shared/Partials/Shared/RequiredField';
import WebLinkCategory from '@/Models/WebLinkCategory';
import WebLinkEditStore from '@/Stores/WebLinkEditStore';
import WebLinksEditStore from '@/Stores/WebLinksEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface WebLinkEditViewKnockoutProps {
	webLinksEditStore: WebLinksEditStore;
	webLinkEditStore: WebLinkEditStore;
}

const WebLinkEditViewKnockout = observer(
	({
		webLinksEditStore,
		webLinkEditStore,
	}: WebLinkEditViewKnockoutProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes']);

		return (
			<tr>
				<td>
					<input
						type="text"
						value={webLinkEditStore.url}
						onChange={(e): void =>
							runInAction(() => {
								webLinkEditStore.url = e.target.value;
							})
						}
						maxLength={512}
						className="input-xlarge"
					/>
				</td>
				<td>
					<input
						type="text"
						value={webLinkEditStore.description}
						onChange={(e): void =>
							runInAction(() => {
								webLinkEditStore.description = e.target.value;
							})
						}
						maxLength={512}
					/>
				</td>
				{webLinksEditStore.categories && (
					<td>
						<select
							value={webLinkEditStore.category}
							onChange={(e): void =>
								runInAction(() => {
									webLinkEditStore.category = e.target.value as WebLinkCategory;
								})
							}
							className="input-medium"
						>
							{webLinksEditStore.categories.map((category) => (
								<option value={category} key={category}>
									{t(`Resources:WebLinkCategoryNames.${category}`)}
								</option>
							))}
						</select>
					</td>
				)}
				<td>
					<label className="checkbox">
						<input
							type="checkbox"
							checked={webLinkEditStore.disabled}
							onChange={(e): void =>
								runInAction(() => {
									webLinkEditStore.disabled = e.target.checked;
								})
							}
						/>
						{t('ViewRes:EntryEdit.PVUnavailable')}
					</label>
				</td>
				<td>
					<SafeAnchor
						href="#"
						className="textLink deleteLink"
						onClick={(): void => webLinksEditStore.remove(webLinkEditStore)}
					>
						{t('ViewRes:Shared.Delete')}
					</SafeAnchor>
				</td>
			</tr>
		);
	},
);

interface WebLinksEditViewKnockoutProps {
	webLinksEditStore: WebLinksEditStore;
}

const WebLinksEditViewKnockout = observer(
	({
		webLinksEditStore,
	}: WebLinksEditViewKnockoutProps): React.ReactElement => {
		const { t } = useTranslation(['HelperRes', 'ViewRes.Song']);

		return (
			<>
				<table>
					<thead>
						{webLinksEditStore.items.length > 0 && (
							<tr>
								<th>
									{t('HelperRes:Helper.WebLinkUrlTitle')} <RequiredField />
								</th>
								<th>{t('HelperRes:Helper.WebLinkDescriptionTitle')}</th>
								{webLinksEditStore.categories && (
									<th>{t('HelperRes:Helper.WebLinkCategoryTitle')}</th>
								)}
								<th>{t('ViewRes.Song:Edit.PvStatus')}</th>
								<th />
							</tr>
						)}
					</thead>
					<tbody>
						{webLinksEditStore.items.map((item, index) => (
							<WebLinkEditViewKnockout
								webLinksEditStore={webLinksEditStore}
								webLinkEditStore={item}
								key={index}
							/>
						))}
					</tbody>
				</table>

				<SafeAnchor
					href="#"
					className="textLink addLink"
					onClick={webLinksEditStore.add}
				>
					{t('HelperRes:Helper.WebLinkNewRow')}
				</SafeAnchor>
			</>
		);
	},
);

export default WebLinksEditViewKnockout;
