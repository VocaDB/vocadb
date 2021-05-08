import FakeEntryReportRepository from '../TestSupport/FakeEntryReportRepository';
import FakeUserRepository from '../TestSupport/FakeUserRepository';
import TopBarViewModel from '../../ViewModels/TopBarViewModel';

var entryTypeTranslations: { [x: string]: string };
var entryReportRepo: FakeEntryReportRepository;
var userRepo: FakeUserRepository;

QUnit.module('TopBarViewModel', {
  setup: () => {
    entryTypeTranslations = { Album: 'Album!' };
    entryReportRepo = new FakeEntryReportRepository();
    entryReportRepo.entryReportCount = 39;
    userRepo = new FakeUserRepository();
    userRepo.messages = [
      {
        createdFormatted: '2039.3.9',
        highPriority: false,
        id: 39,
        inbox: 'Received',
        read: false,
        receiver: null!,
        subject: 'New message!',
      },
      {
        createdFormatted: '2039.3.9',
        highPriority: false,
        id: 40,
        inbox: 'Received',
        read: false,
        receiver: null!,
        subject: 'Another message',
      },
    ];
  },
});

var create = (getNewReportsCount: boolean = false): TopBarViewModel => {
  return new TopBarViewModel(
    entryTypeTranslations,
    'Album',
    '',
    0,
    getNewReportsCount,
    entryReportRepo,
    userRepo,
  );
};

test('constructor', () => {
  var result = create();

  equal(result.entryTypeName(), 'Album!', 'entryTypeName');
  equal(result.hasNotifications(), false, 'hasNotifications');
  equal(result.reportCount(), 0, 'reportCount not loaded');
});

test('constructor load report count', () => {
  var result = create(true);
  equal(result.reportCount(), 39, 'reportCount was loaded');
});

test('ensureMessagesLoaded', () => {
  var target = create();

  target.ensureMessagesLoaded();

  equal(target.isLoaded(), true, 'isLoaded');
  ok(target.unreadMessages(), 'unreadMessages()');
  equal(target.unreadMessages().length, 2, 'unreadMessages().length');
  equal(target.unreadMessages()[0].subject, 'New message!');
});
