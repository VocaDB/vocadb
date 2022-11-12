import { NewsListStore } from '@/Stores/NewsListStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface NewsItemsProps {
	newsListStore: NewsListStore;
}

const NewsItems = observer(
	({ newsListStore }: NewsItemsProps): React.ReactElement => {
		const { t } = useTranslation(['HelperRes', 'ViewRes.Home']);

		return (
			<>
				{!newsListStore.loaded ||
					(newsListStore.posts.length > 0 && (
						<h3 className="withMargin">{t('ViewRes.Home:Index.News')}</h3>
					))}

				{newsListStore.loaded ? (
					<div>
						{newsListStore.posts.map((post) => (
							<div
								className="message newsEntry ui-tabs ui-widget ui-widget-content ui-corner-all"
								key={post.URL}
							>
								<div className="messageTitle ui-widget-header ui-corner-all">
									<div className="messageTitleText">
										<h4>
											<a href={post.URL}>{post.title}</a> &nbsp;{' '}
											<span className="news-title-author">
												<img
													src={post.author.avatar_URL || '/Content/Unknown.png'}
													width={15}
													height={15}
													alt="Avatar" /* LOC */
												/>{' '}
												<span>{post.author.name}</span>
												<span> {t('HelperRes:ActivityFeedHelper.At')} </span>
												<span>{post.date}</span>
											</span>
										</h4>
									</div>
								</div>
								<p className="messageContent">
									<span>{post.content}</span>
								</p>
							</div>
						))}
					</div>
				) : (
					<div>
						{/* eslint-disable-next-line jsx-a11y/alt-text */}
						<img src="/Content/ajax-loader.gif" /> {t('ViewRes:Shared.Loading')}
					</div>
				)}
			</>
		);
	},
);

export default NewsItems;
