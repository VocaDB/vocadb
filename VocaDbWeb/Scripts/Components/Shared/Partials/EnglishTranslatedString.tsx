import SafeAnchor from '@Bootstrap/SafeAnchor';
import EnglishTranslatedStringContract from '@DataContracts/Globalization/EnglishTranslatedStringContract';
import EnglishTranslatedStringStore from '@Stores/Globalization/EnglishTranslatedStringStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import FormatMarkdown from './Html/FormatMarkdown';

interface TextProps {
	englishTranslatedStringStore: EnglishTranslatedStringStore;
	string: string;
	maxLength: number;
	summaryLength: number;
}

const Text = observer(
	({
		englishTranslatedStringStore,
		string,
		maxLength,
		summaryLength,
	}: TextProps): React.ReactElement => {
		const { t } = useTranslation([
			'VocaDb.Web.Resources.Views.Shared.Partials',
		]);

		return !string || string.length <= maxLength ? (
			<FormatMarkdown text={string} />
		) : englishTranslatedStringStore.isFullDescriptionShown ? (
			<div>
				<FormatMarkdown text={string} />
			</div>
		) : (
			<div>
				<p>
					<FormatMarkdown text={`${string.slice(0, summaryLength)}...`} />
					<SafeAnchor
						onClick={englishTranslatedStringStore.showFullDescription}
					>
						{t(
							'VocaDb.Web.Resources.Views.Shared.Partials:EnglishTranslatedString.ShowFullDescription',
						)}
					</SafeAnchor>
				</p>
			</div>
		);
	},
);

interface EnglishTranslatedStringProps {
	englishTranslatedStringStore: EnglishTranslatedStringStore;
	string: EnglishTranslatedStringContract;
	maxLength?: number;
	summaryLength?: number;
}

const EnglishTranslatedString = observer(
	({
		englishTranslatedStringStore,
		string,
		maxLength = 500,
		summaryLength = 400,
	}: EnglishTranslatedStringProps): React.ReactElement => {
		const { t } = useTranslation([
			'VocaDb.Web.Resources.Views.Shared.Partials',
		]);

		return string.english ? (
			<div>
				<ul className="nav nav-pills pull-right no-margin">
					<li
						className={classNames(
							!englishTranslatedStringStore.showTranslatedDescription &&
								'active',
						)}
					>
						<SafeAnchor
							onClick={(): void =>
								runInAction(() => {
									englishTranslatedStringStore.showTranslatedDescription = false;
								})
							}
							href="#"
						>
							{t(
								'VocaDb.Web.Resources.Views.Shared.Partials:EnglishTranslatedString.Original',
							)}
						</SafeAnchor>
					</li>
					<li
						className={classNames(
							englishTranslatedStringStore.showTranslatedDescription &&
								'active',
						)}
					>
						<SafeAnchor
							onClick={(): void =>
								runInAction(() => {
									englishTranslatedStringStore.showTranslatedDescription = true;
								})
							}
							href="#"
						>
							{t(
								'VocaDb.Web.Resources.Views.Shared.Partials:EnglishTranslatedString.Translated',
							)}
						</SafeAnchor>
					</li>
				</ul>
				{englishTranslatedStringStore.showTranslatedDescription ? (
					<div>
						<Text
							englishTranslatedStringStore={englishTranslatedStringStore}
							string={string.english}
							maxLength={maxLength}
							summaryLength={summaryLength}
						/>
					</div>
				) : (
					<div>
						<Text
							englishTranslatedStringStore={englishTranslatedStringStore}
							string={string.original}
							maxLength={maxLength}
							summaryLength={summaryLength}
						/>
					</div>
				)}
			</div>
		) : (
			<Text
				englishTranslatedStringStore={englishTranslatedStringStore}
				string={string.original}
				maxLength={maxLength}
				summaryLength={summaryLength}
			/>
		);
	},
);

export default EnglishTranslatedString;
