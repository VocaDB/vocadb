import React from 'react';
import { useTranslation } from 'react-i18next';

export const MarkdownNotice = React.memo(
	(): React.ReactElement => {
		const { t } = useTranslation(['HelperRes']);

		return (
			<small>
				(
				<a
					href="https://help.github.com/articles/github-flavored-markdown"
					target="_blank"
					rel="noreferrer"
				>
					{t('HelperRes:Helper.GitHubMarkdown')}
				</a>{' '}
				{t('HelperRes:Helper.IsSupportedForFormatting')})
			</small>
		);
	},
);
