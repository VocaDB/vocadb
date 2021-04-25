import AdminRepository from '../../Repositories/AdminRepository';
import ui from '../../Shared/MessagesTyped';

export default class ManageIPRulesViewModel {
  public add = () => {
    const addr = this.newAddress().trim();

    if (!addr) return;

    if (_.some(this.rules(), (r) => r.address() === addr)) {
      ui.showErrorMessage('Address already added');
      return;
    }

    this.rules.unshift(
      new IPRule({ address: addr, notes: '', created: new Date() }),
    );
    this.newAddress('');
  };

  public bannedIPs = ko.observableArray<string>();

  public deleteOldRules = () => {
    var cutOff = moment().subtract(1, 'years').toDate();

    var toBeRemoved = _.filter(this.rules(), (r) => r.created < cutOff);
    this.rules.removeAll(toBeRemoved);
  };

  public newAddress = ko.observable('');

  public remove = (rule: IPRule) => {
    this.rules.remove(rule);
  };

  public rules: KnockoutObservableArray<IPRule>;

  public save = () => {
    var json = ko.toJS(this);
    ko.utils.postJson(location.href, json, null);
  };

  constructor(data: IPRuleContract[], repo: AdminRepository) {
    const rules = _.chain(data)
      .sortBy('created')
      .reverse()
      .map((r) => new IPRule(r))
      .value();
    this.rules = ko.observableArray(rules);

    repo.getTempBannedIps((result) => this.bannedIPs(result));
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
    return i < 10 ? '0' + i : '' + i;
  }

  address: KnockoutObservable<string>;

  created: Date;

  id: number;

  notes: KnockoutObservable<string>;

  constructor(data: IPRuleContract) {
    this.address = ko.observable(data.address);
    this.created = data.created;
    this.id = data.id;
    this.notes = ko.observable(data.notes);
  }
}
