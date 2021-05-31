import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import ArtistCreateViewModel from '@ViewModels/ArtistCreateViewModel';
import $ from 'jquery';
import ko from 'knockout';

const ArtistCreate = (model: any): void => {
  $(document).ready(function () {
    ko.punches.enableAll();
    const httpClient = new HttpClient();
    var repoFactory = new RepositoryFactory(httpClient);
    var repo = repoFactory.artistRepository();
    var tagRepo = repoFactory.tagRepository();
    var json = model;
    ko.applyBindings(new ArtistCreateViewModel(repo, tagRepo, json));
  });
};

export default ArtistCreate;
