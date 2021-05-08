import DateTimeHelper from './DateTimeHelper';
import Decimal from 'decimal.js-light';

export default class KnockoutHelper {
  public static stringEnum<T>(
    observable: KnockoutObservable<T>,
    enumType: any,
  ): KnockoutComputed<string> {
    return ko.computed({
      read: () => {
        var val: any = observable();
        return enumType[val];
      },
      write: (val: string) => observable(enumType[val]),
    });
  }

  public static bpm(
    observable: KnockoutObservable<number>,
  ): KnockoutComputed<string> {
    return ko.computed({
      read: () => {
        var val: any = observable();
        return val ? new Decimal(val).div(1000).toString() : null!;
      },
      write: (val: string) => {
        observable(
          val ? new Decimal(val).mul(1000).toInteger().toNumber() : null!,
        );
      },
    });
  }

  public static lengthFormatted(
    observable: KnockoutObservable<number>,
  ): KnockoutComputed<string> {
    return ko.computed({
      read: () => {
        var val: any = observable();
        return DateTimeHelper.formatFromSeconds(val);
      },
      write: (val: string) => {
        observable(DateTimeHelper.parseToSeconds(val));
      },
    });
  }
}
