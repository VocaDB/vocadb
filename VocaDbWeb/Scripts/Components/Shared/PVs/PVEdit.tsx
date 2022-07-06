import SafeAnchor from '@Bootstrap/SafeAnchor';
import DateTimeHelper from '@Helpers/DateTimeHelper';
import PVType from '@Models/PVs/PVType';
import PVListEditStore, { PVEditStore } from '@Stores/PVs/PVListEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface PVEditProps {
	pvListEditStore: PVListEditStore;
	pvEditStore: PVEditStore;
}

const PVEdit = observer(
	({ pvListEditStore, pvEditStore }: PVEditProps): React.ReactElement => {
		const { t } = useTranslation(['Resources', 'ViewRes']);

		return (
			<tr>
				<td>
					<a href={pvEditStore.url}>
						<img
							src={pvListEditStore.pvServiceIcons.getIconUrl(
								pvEditStore.service,
							)}
							alt={pvEditStore.service}
						/>{' '}
						{pvEditStore.service}
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
				<td>{DateTimeHelper.formatFromSeconds(pvEditStore.length)}</td>
				{pvListEditStore.showPublishDates && (
					<td>
						{pvListEditStore.showPublishDates && (
							<>{moment(pvEditStore.publishDate).format('l')}</>
						)}
					</td>
				)}
				<td>
					{pvEditStore.author && pvListEditStore.canBulkDeletePVs ? (
						<a href={`/Admin/PVsByAuthor?author=${pvEditStore.author}`}>
							{pvEditStore.author}
						</a>
					) : (
						pvEditStore.author
					)}
				</td>
				{pvListEditStore.allowDisabled && (
					<td>
						{pvEditStore.pvType === PVType[PVType.Original] && (
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

export default PVEdit;
