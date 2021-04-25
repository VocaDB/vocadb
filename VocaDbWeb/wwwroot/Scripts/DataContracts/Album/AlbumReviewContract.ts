import UserApiContract from '../User/UserApiContract';

export default interface AlbumReviewContract {
  date: string;

  id?: number;

  languageCode: string;

  text: string;

  title: string;

  user: UserApiContract;
}
