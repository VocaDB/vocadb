export interface ComparedVersionsContract<T> {
	firstId: number;
	firstData: T;
	secondId: number;
	secondData?: T;
}
