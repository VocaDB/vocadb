import LocalizedStringContract from './LocalizedStringContract';

export default interface LocalizedStringWithIdContract
	extends LocalizedStringContract {
	id: number;
}
