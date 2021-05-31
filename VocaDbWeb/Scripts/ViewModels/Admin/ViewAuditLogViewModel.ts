import functions from '@Shared/GlobalFunctions';
import $ from 'jquery';
import ko from 'knockout';

export default class ViewAuditLogViewModel {
	public excludeUsers = ko.observable('');

	public filter = ko.observable('');

	public filterVisible = ko.observable(false);

	public onlyNewUsers = ko.observable(false);

	public toggleFilter = (): void => {
		this.filterVisible(!this.filterVisible());
	};

	public userName = ko.observable('');

	private split(val: string): string[] {
		return val.split(/,\s*/);
	}

	private extractLast(term: string): string {
		return this.split(term).pop()!;
	}

	constructor(data: ViewAuditLogContract) {
		this.excludeUsers(data.excludeUsers);
		this.filter(data.filter);
		this.onlyNewUsers(data.onlyNewUsers);
		this.userName(data.userName);
		this.filterVisible(
			!functions.isNullOrWhiteSpace(data.userName) ||
				!functions.isNullOrWhiteSpace(data.excludeUsers) ||
				!functions.isNullOrWhiteSpace(data.filter) ||
				data.onlyNewUsers,
		);

		$('#userNameField').autocomplete({
			source: (
				ui: { term: any },
				callback: (data: any, textStatus: string, jqXHR: JQueryXHR) => any,
			) => {
				var url = functions.mapAbsoluteUrl('/api/users/names');
				$.getJSON(url, { query: ui.term }, callback);
			},
			select: (event, ui) => {
				this.userName(ui.item.value);
				return false;
			},
		});

		$('#usersList')
			// don't navigate away from the field on tab when selecting an item
			.bind('keydown', function (this: any, event) {
				if (
					event.keyCode === $.ui.keyCode.TAB &&
					$(this).data('ui-autocomplete').menu.active
				) {
					event.preventDefault();
				}
			})
			.autocomplete({
				minLength: 1,
				source: (
					request: { term: string },
					response: {
						(arg0: {}): void;
						(data: any, textStatus: string, jqXHR: JQueryXHR): any;
					},
				) => {
					var name = this.extractLast(request.term);
					if (name.length === 0) response({});
					else
						$.getJSON(
							'/api/users/names',
							{ query: name, startsWith: true } as any,
							response,
						);
				},
				focus: function () {
					// prevent value inserted on focus
					return false;
				},
				select: (event, ui) => {
					var terms = this.split($('#usersList').val());
					// remove the current input
					terms.pop();
					// add the selected item
					terms.push(ui.item.value);
					// add placeholder to get the comma-and-space at the end
					terms.push('');
					this.excludeUsers(terms.join(', '));
					return false;
				},
			});
	}
}

export interface ViewAuditLogContract {
	excludeUsers: string;

	filter: string;

	onlyNewUsers: boolean;

	userName: string;
}
