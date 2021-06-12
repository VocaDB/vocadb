import ArtistContract from '@DataContracts/Artist/ArtistContract';
import { ArtistAutoCompleteParams } from '@KnockoutExtensions/AutoCompleteParams';
import ArtistRepository from '@Repositories/ArtistRepository';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import ko, { Observable } from 'knockout';

const vocaDbContext = container.get(VocaDbContext);

export default class RequestVerificationViewModel {
	public constructor(private readonly artistRepository: ArtistRepository) {}

	public clearArtist = (): void => {
		this.selectedArtist(null!);
	};

	public privateMessage = ko.observable(false);

	public selectedArtist: Observable<ArtistContract | null> = ko.observable(
		null!,
	);

	public setArtist = (targetArtistId?: number): void => {
		this.artistRepository
			.getOne(targetArtistId!, vocaDbContext.languagePreference)
			.then((artist) => {
				this.selectedArtist(artist);
			});
	};

	public artistSearchParams: ArtistAutoCompleteParams = {
		acceptSelection: this.setArtist,
	};
}
