import { BasicListEditStore } from './BasicListEditStore';

export class CultureCodesEditStore extends BasicListEditStore<String, String> {
	constructor(cultureCodes: string[]) {
		super(String, cultureCodes);
	}
}
