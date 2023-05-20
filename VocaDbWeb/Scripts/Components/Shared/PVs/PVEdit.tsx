import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { PVType } from '@/Models/PVs/PVType';
import { PVEditStore, PVListEditStore } from '@/Stores/PVs/PVListEditStore';
import dayjs from '@/dayjs';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface PVEditProps {
	pvListEditStore: PVListEditStore;
	pvEditStore: PVEditStore;
}

export const PVEdit = observer(
	({ pvListEditStore, pvEditStore }: PVEditProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes']);

		const { contract } = pvEditStore;

		return (
			<tr>
				<td>
					<a href={contract.url}>
						<img
							src={pvListEditStore.pvServiceIcons.getIconUrl(contract.service)}
							alt={contract.service}
						/>{' '}
						{contract.service}
					</a>
				</td>
				<td>{t(`Resources:PVTypeNames.${pvEditStore.pvType}`)}</td>
				<td>
					<input
						type="text"
						value={pvEditStore.name}
						onChange={(e): void =>
							runInAction(() => {
								pvEditStore.name = e.target.value;
							})
						}
						size={40}
						maxLength={200}
						className="input-xlarge"
					/>
				</td>
				<td>{pvEditStore.lengthFormatted}</td>
				{pvListEditStore.showPublishDates && (
					<td>
						{pvListEditStore.showPublishDates && (
							<>{dayjs(contract.publishDate).format('l')}</>
						)}
					</td>
				)}
				<td>
					{contract.author && pvListEditStore.canBulkDeletePVs ? (
						<a href={`/Admin/PVsByAuthor?author=${contract.author}`}>
							{contract.author}
						</a>
					) : (
						contract.author
					)}
				</td>
				{pvListEditStore.allowDisabled && (
					<td>
						{pvEditStore.pvType === PVType.Original && (
							<span>
								<input
									type="checkbox"
									checked={pvEditStore.disabled}
									onChange={(e): void =>
										runInAction(() => {
											pvEditStore.disabled = e.target.checked;
										})
									}
								/>{' '}
								{t('ViewRes:EntryEdit.PVUnavailable')}
							</span>
						)}
					</td>
				)}
				<td>
					<SafeAnchor
						onClick={(): void => pvListEditStore.remove(pvEditStore)}
						className="textLink deleteLink"
						href="#"
					>
						{t('ViewRes:Shared.Delete')}
					</SafeAnchor>
				</td>
			</tr>
		);
	},
);
