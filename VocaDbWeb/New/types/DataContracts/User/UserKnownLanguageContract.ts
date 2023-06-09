export enum UserLanguageProficiency {
	Nothing = 'Nothing',
	Basics = 'Basics',
	Intermediate = 'Intermediate',
	Advanced = 'Advanced',
	Native = 'Native',
}

export interface UserKnownLanguageContract {
	cultureCode: string;
	proficiency: UserLanguageProficiency;
}
