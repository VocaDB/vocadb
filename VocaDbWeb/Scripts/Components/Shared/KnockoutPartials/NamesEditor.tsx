import SafeAnchor from '@/Bootstrap/SafeAnchor';
import LocalizedStringWithIdEditStore from '@/Stores/Globalization/LocalizedStringWithIdEditStore';
import NamesEditStore from '@/Stores/Globalization/NamesEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import HelpLabel from '../Partials/Shared/HelpLabel';

interface NameEditorProps {
	namesEditStore: NamesEditStore;
	nameEditStore: LocalizedStringWithIdEditStore;
}

const NameEditor = observer(
	({ namesEditStore, nameEditStore }: NameEditorProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<tr>
				<td>
					<input
						type="text"
						value={nameEditStore.value}
						onChange={(e): void =>
							runInAction(() => {
								nameEditStore.value = e.target.value;
							})
						}
						maxLength={255}
						className="nameEdit"
					/>
				</td>
				<td>
					<SafeAnchor
						onClick={(): void => namesEditStore.deleteAlias(nameEditStore)}
						href="#"
						className="nameDelete textLink deleteLink"
					>
						{t('ViewRes:Shared.Delete')}
					</SafeAnchor>
				</td>
			</tr>
		);
	},
);

interface NamesEditorProps {
	namesEditStore: NamesEditStore;
	showAliases?: boolean;
}

const NamesEditor = observer(
	({
		namesEditStore,
		showAliases = true,
	}: NamesEditorProps): React.ReactElement => {
		const { t } = useTranslation(['Helper', 'Resources', 'ViewRes']);

		return (
			<>
				<table>
					<tbody>
						<tr>
							<td>{t('Resources:ContentLanguageSelectionNames.Japanese')}</td>
							<td>
								<input
									type="text"
									value={namesEditStore.originalName.value}
									onChange={(e): void =>
										runInAction(() => {
											namesEditStore.originalName.value = e.target.value;
										})
									}
									maxLength={255}
									className="nameEdit"
								/>
							</td>
						</tr>
						<tr>
							<td>{t('Resources:ContentLanguageSelectionNames.Romaji')}</td>
							<td>
								<input
									type="text"
									value={namesEditStore.romajiName.value}
									onChange={(e): void =>
										runInAction(() => {
											namesEditStore.romajiName.value = e.target.value;
										})
									}
									maxLength={255}
									className="nameEdit"
								/>
							</td>
						</tr>
						<tr>
							<td>{t('Resources:ContentLanguageSelectionNames.English')}</td>
							<td>
								<input
									type="text"
									value={namesEditStore.englishName.value}
									onChange={(e): void =>
										runInAction(() => {
											namesEditStore.englishName.value = e.target.value;
										})
									}
									maxLength={255}
									className="nameEdit"
								/>
							</td>
						</tr>
					</tbody>
				</table>

				{showAliases && (
					<>
						<HelpLabel
							title={t('ViewRes:EntryEdit.AliasesDesc')}
							label={t('ViewRes:EntryEdit.Aliases')}
						/>

						<table>
							<tbody>
								{namesEditStore.aliases.map((alias, index) => (
									<NameEditor
										namesEditStore={namesEditStore}
										nameEditStore={alias}
										key={index}
									/>
								))}
							</tbody>
						</table>

						<SafeAnchor
							onClick={namesEditStore.createAlias}
							href="#"
							className="textLink addLink"
						>
							{t('HelperRes:Helper.NameNewRow')}
						</SafeAnchor>
					</>
				)}
			</>
		);
	},
);

export default NamesEditor;
