import DateTimeHelper from '../../Helpers/DateTimeHelper';

QUnit.module('DateTimeHelper');

QUnit.test('convertToLocal', () => {
  var date = new Date(2016, 9, 3);
  date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
  var result = DateTimeHelper.convertToLocal(date);
  var expected = new Date(2016, 9, 3);

  QUnit.equal(result!.toString(), expected.toString());
});

QUnit.test('convertToLocal null', () => {
  var result = DateTimeHelper.convertToLocal(null!);

  QUnit.equal(result, null, 'result');
});

QUnit.test('convertToUtc', () => {
  var date = new Date(2016, 9, 3);
  var result = DateTimeHelper.convertToUtc(date);
  date.setMinutes(date.getMinutes() - date.getTimezoneOffset());

  QUnit.equal(result!.toString(), date.toString());
});

QUnit.test('convertToUtc null', () => {
  var result = DateTimeHelper.convertToUtc(null!);

  QUnit.equal(result, null, 'result');
});
