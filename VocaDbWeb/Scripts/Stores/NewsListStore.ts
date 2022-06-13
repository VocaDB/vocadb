import UrlMapper from '@Shared/UrlMapper';
import $ from 'jquery';
import { makeObservable, observable, runInAction } from 'mobx';

export interface WordpressResponse {
	posts: WordpressPost[];
}

export interface WordpressPost {
	abstract: string;

	author: WordpressAuthor;

	content: string;

	date: string;

	title: string;

	URL: string;
}

export interface WordpressAuthor {
	avatar_URL: string;

	name: string;
}

export default class NewsListStore {
	@observable public loaded = false;
	@observable public posts: WordpressPost[] = [];

	public constructor(blogUrl?: string) {
		makeObservable(this);

		if (!blogUrl) {
			this.loaded = true;
			return;
		}

		const url = UrlMapper.buildUrl(
			'https://public-api.wordpress.com/rest/v1.1/sites/',
			blogUrl,
			'/posts/',
		);

		$.ajax({ dataType: 'json', url: url, data: { number: 3 } })
			.done((response: WordpressResponse) => {
				for (const post of response.posts) {
					if (post.content.length > 400) {
						post.content = post.content.substring(0, 400) + '...';
						post.date = new Date(post.date).toLocaleString();
					}

					if (post.author && post.author.avatar_URL) {
						// Is there a way to get HTTPS URLs by default?
						post.author.avatar_URL = post.author.avatar_URL.replace(
							'http://',
							'https://',
						);
					}
				}

				runInAction(() => {
					this.posts = response.posts;
				});
			})
			.always(() =>
				runInAction(() => {
					this.loaded = true;
				}),
			);
	}
}
