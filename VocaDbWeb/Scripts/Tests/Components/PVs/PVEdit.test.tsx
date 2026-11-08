import { PVEdit } from '@/Components/Shared/PVs/PVEdit';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PVService } from '@/Models/PVs/PVService';
import { PVType } from '@/Models/PVs/PVType';
import { PVRepository } from '@/Repositories/PVRepository';
import { UrlMapper } from '@/Shared/UrlMapper';
import { PVListEditStore } from '@/Stores/PVs/PVListEditStore';
import dayjs from '@/dayjs';
import 'dayjs/locale/de';
import 'dayjs/locale/ja';
import $ from 'jquery';
import React from 'react';
import { Root, createRoot } from 'react-dom/client';
import { act } from 'react-dom/test-utils';

jest.mock('react-i18next', () => ({
	useTranslation: (): { t: (key: string) => string } => ({
		t: (key: string): string => key,
	}),
}));
jest.mock('@/JQueryUI/JQueryUIDialog', () => ({
	__esModule: true,
	default: (): null => null,
}));

let table: HTMLTableElement;
let body: HTMLTableSectionElement;
let root: Root;

beforeEach(() => {
	Object.assign(globalThis, { IS_REACT_ACT_ENVIRONMENT: true });
	table = document.createElement('table');
	body = table.createTBody();
	document.body.appendChild(table);
	root = createRoot(body);
});

afterEach(() => {
	act(() => root.unmount());
	table.remove();
	jest.restoreAllMocks();
	jest.useRealTimers();
	dayjs.locale('en');
});

const contract = (overrides: Partial<PVContract> = {}): PVContract => ({
	id: 1,
	pvId: 'test',
	service: PVService.Youtube,
	pvType: PVType.Original,
	name: 'Test PV',
	length: 123,
	disabled: false,
	...overrides,
});

const repository = {
	getPVByUrl: jest.fn(),
} as unknown as PVRepository;

const createStore = (
	pvs: PVContract[],
	showPublishDates = true,
	allowDisabled = true,
): PVListEditStore =>
	new PVListEditStore(
		repository,
		new UrlMapper(''),
		pvs,
		false,
		showPublishDates,
		allowDisabled,
	);

const render = (store: PVListEditStore): HTMLTableRowElement => {
	act(() => {
		root.render(<PVEdit pvListEditStore={store} pvEditStore={store.pvs[0]} />);
	});
	return body.rows[0];
};

describe.each([PVService.Youtube, PVService.LocalFile])(
	'%s publish dates',
	(service) => {
		test.each([
			['omitted', {}],
			['undefined', { publishDate: undefined }],
			['null', { publishDate: null as unknown as string }],
			...[
				'',
				' ',
				'Invalid Date',
				'not-a-date',
				'2024-02-30',
				'2023-02-29',
				'2100-02-29',
				'2024-13-01',
				'2024-00-10',
				'2024-01-00',
				'2024-01-32',
				'2024-02-29T25:00:00',
				'2024-02-29T12:60:00',
				'2024-02-29T12:00:60',
				'2024-02-29T12:00:00+25:00',
				'2024-02-29T12:34:56+2500',
				'2024-02-30T12:00:00.1234567Z',
				'2024-02-30T12:34Z',
				'2024-02-30T12:34',
				'2024-02-30 12:34:56',
				'2024-02-29 12:60:00',
			].map((publishDate) => [publishDate, { publishDate }]),
		] as [string, Partial<PVContract>][])(
			'leaves %s blank and keeps the table aligned',
			(_, date) => {
				const row = render(createStore([contract({ ...date, service })]));
				expect(row.cells).toHaveLength(9);
				expect(row.cells[4].textContent).toBe('');
				expect(row.cells[3].textContent).toBe('2:03');
				expect(row.textContent).not.toContain('Invalid Date');
			},
		);
	},
);

test.each([
	'2024-02-29',
	'2000-02-29',
	'2024-02-29T00:00:00',
	'2024-02-29T12:34:56Z',
	'2024-02-29T12:34:56.1234567',
	'2024-02-29T12:34:56.1234567Z',
	'2024-02-29T12:34:56+09:00',
	'2024-02-29T12:34:56-06:00',
	'2024-02-29T12:34',
	'2024-02-29T12:34Z',
	'2024-02-29T12:34:56+0900',
	'2024-02-29 12:34:56',
	'2024-03-01T00:00:00+14:00',
	'2024-03-10T02:30:00Z',
	'2024-03-10T02:30:00+09:00',
])('preserves valid date and timezone formatting for %s', (publishDate) => {
	const row = render(createStore([contract({ publishDate })]));
	expect(row.cells[4].textContent).toBe(dayjs(publishDate).format('l'));
});

test.each([
	['en', '2/29/2024'],
	['ja', '2024/02/29'],
	['de', '29.2.2024'],
])('uses the %s locale', (locale, expected) => {
	dayjs.locale(locale);
	const row = render(createStore([contract({ publishDate: '2024-02-29' })]));
	expect(row.cells[4].textContent).toBe(expected);
});

test.each([undefined, 'not-a-date', '2024-02-29'])(
	'omits the date column when disabled (%s)',
	(publishDate) => {
		const row = render(
			createStore([contract({ publishDate, author: 'Test author' })], false),
		);
		expect(row.cells).toHaveLength(8);
		expect(row.cells[4].textContent).toBe('Test author');
	},
);

test('keeps the event table aligned without the disabled-PV column', () => {
	const row = render(createStore([contract()], true, false));
	expect(row.cells).toHaveLength(8);
	expect(row.cells[4].textContent).toBe('');
});

test('does not substitute the current date when metadata is missing', () => {
	jest.useFakeTimers().setSystemTime(new Date('2024-05-20T12:00:00Z'));
	const row = render(createStore([contract()]));
	expect(row.cells[4].textContent).toBe('');
});

test.each([undefined, null])(
	'preserves a missing LocalFile date through save and reload (%s)',
	(publishDate) => {
		const original = contract({
			service: PVService.LocalFile,
			publishDate: publishDate as unknown as string,
		});
		const saved = createStore([original]).toContracts();
		expect(saved[0].publishDate).toBe(publishDate);
		const reloaded = createStore(JSON.parse(JSON.stringify(saved)));
		expect(render(reloaded).cells[4].textContent).toBe('');
	},
);

test('renders a newly uploaded MP3 and its saved/reloaded contract without a date', async () => {
	const uploaded = contract({ service: PVService.LocalFile });
	const ajax = jest
		.spyOn($, 'ajax')
		.mockImplementation((options: string | JQueryAjaxSettings) => {
			const settings = options as JQueryAjaxSettings;
			settings.success?.(uploaded, 'success', {} as JQueryXHR);
			return $.Deferred().resolve(uploaded).promise() as JQueryXHR;
		});
	const store = createStore([]);
	await act(async () => {
		await store.uploadMedia(
			new File(['mp3'], 'test.mp3', { type: 'audio/mpeg' }),
		);
	});
	expect(ajax).toHaveBeenCalledWith(
		expect.objectContaining({ url: '/Song/PostMedia/', type: 'POST' }),
	);
	expect(render(store).cells[4].textContent).toBe('');
	const reloaded = createStore(JSON.parse(JSON.stringify(store.toContracts())));
	expect(render(reloaded).cells[4].textContent).toBe('');
});

test('renders newly fetched YouTube metadata with no publish date', async () => {
	const getPVByUrl = repository.getPVByUrl as jest.Mock;
	getPVByUrl.mockResolvedValueOnce(contract());
	const store = createStore([]);
	store.newPvUrl = 'https://www.youtube.com/watch?v=test';
	await act(async () => {
		store.add();
		await Promise.resolve();
	});
	expect(getPVByUrl).toHaveBeenCalledWith({
		pvUrl: 'https://www.youtube.com/watch?v=test',
		type: PVType.Original,
	});
	expect(render(store).cells[4].textContent).toBe('');
});
