import SafeAnchor from '@/Bootstrap/SafeAnchor';
import EnglishTranslatedStringEditStore from '@/Stores/Globalization/EnglishTranslatedStringEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import Markdown from '../../KnockoutExtensions/Markdown';

interface EnglishTranslatedStringEditProps {
	englishTranslatedStringEditStore: EnglishTranslatedStringEditStore;
}

const EnglishTranslatedStringEdit = observer(
	({
		englishTranslatedStringEditStore,
	}: EnglishTranslatedStringEditProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<>
				<div>
					{englishTranslatedStringEditStore.showTranslation && (
						<h4>{t('ViewRes:EntryEdit.TranslatedStringOriginal')}</h4>
					)}
					<textarea
						rows={8}
						cols={70}
						className="edit-text"
						value={englishTranslatedStringEditStore.original}
						onChange={(e): void =>
							runInAction(() => {
								englishTranslatedStringEditStore.original = e.target.value;
							})
						}
					/>
					<div className="edit-preview">
						<Markdown>{englishTranslatedStringEditStore.original}</Markdown>
					</div>
					<p>
						{!englishTranslatedStringEditStore.showTranslation && (
							<SafeAnchor
								href="#"
								className="textLink addLink"
								onClick={(): void =>
									runInAction(() => {
										englishTranslatedStringEditStore.showTranslation = true;
									})
								}
							>
								{t('ViewRes:EntryEdit.TranslatedStringAddEnglishTranslation')}
							</SafeAnchor>
						)}
					</p>
				</div>
				{englishTranslatedStringEditStore.showTranslation && (
					<div>
						<h4>{t('ViewRes:EntryEdit.TranslatedStringEnglishTranslation')}</h4>
						<textarea
							rows={8}
							cols={70}
							className="edit-text"
							value={englishTranslatedStringEditStore.english}
							onChange={(e): void =>
								runInAction(() => {
									englishTranslatedStringEditStore.english = e.target.value;
								})
							}
						/>
						<div className="edit-preview">
							<Markdown>{englishTranslatedStringEditStore.english}</Markdown>
						</div>
					</div>
				)}
			</>
		);
	},
);

export default EnglishTranslatedStringEdit;
