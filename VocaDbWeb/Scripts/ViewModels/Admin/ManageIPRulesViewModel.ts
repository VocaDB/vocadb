/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

module vdb.viewModels {

    export class ManageIPRulesViewModel {

        public add = () => {
            this.rules.push(new IPRule({ address: this.newRule.address(), notes: "", created: new Date() }));
        }

        public bannedIPs = ko.observableArray<string>();

        public newRule = new IPRule({});

        public remove = (rule: IPRule) => {
            this.rules.remove(rule);
        };

        public rules: KnockoutObservableArray<IPRule>;

        public save = () => {
            var json = ko.toJS(this);
            ko.utils.postJson(location.href, json, null);
        };

        constructor(data: IPRuleContract[]) {

            this.rules = ko.observableArray(_.map(data, (item) => new IPRule(item)));

            $.getJSON(vdb.functions.mapAbsoluteUrl("/Admin/BannedIPs"), null, (result: string[]) => {
                this.bannedIPs(result);
            });

        }

    }

    export interface IPRuleContract {

        address?: string;

        created?: Date;

        id?: number;

        notes?: string;

    }

    export class IPRule {

        private padStr(i: number): string {
            return (i < 10) ? "0" + i : "" + i;
        }

        address: KnockoutObservable<string>;

        created: Date;

        createdFormatted: string;

        id: number;

        notes: KnockoutObservable<string>;

        constructor(data: IPRuleContract) {

            this.address = ko.observable(data.address);
            this.created = data.created;

            if (data.created)
                this.createdFormatted = data.created.toDateString() + " " + this.padStr(data.created.getHours()) + ":" + this.padStr(data.created.getMinutes());

            this.id = data.id;
            this.notes = ko.observable(data.notes);

        }

    };

}