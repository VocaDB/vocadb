export enum UserLanguageProficiency {
	Nothing = 'Nothing',
	Basics = 'Basics',
	Intermediate = 'Intermediate',
	Advanced = 'Advanced',
	Native = 'Native',
}

export default interface UserKnownLanguageContract {
	cultureCode: string;
	proficiency: UserLanguageProficiency;
}
