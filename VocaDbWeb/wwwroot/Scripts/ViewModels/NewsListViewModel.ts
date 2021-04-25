import UrlMapper from '../Shared/UrlMapper';

export default class NewsListViewModel {
  constructor(readonly blogUrl: string) {
    if (!blogUrl) {
      this.loaded(true);
      return;
    }

    const url = UrlMapper.buildUrl(
      'https://public-api.wordpress.com/rest/v1.1/sites/',
      blogUrl,
      '/posts/',
    );

    $.ajax({ dataType: 'json', url: url, data: { number: 3 } })
      .done((response: WordpressResponse) => {
        _.forEach(response.posts, (post) => {
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
        });

        this.posts(response.posts);
      })
      .always(() => this.loaded(true));
  }

  loaded = ko.observable(false);

  posts = ko.observableArray<WordpressPost>();
}

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
