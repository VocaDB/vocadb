/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../ViewModels/TopBarViewModel.ts" />
/// <reference path="../TestSupport/FakeEntryReportRepository.ts" />
/// <reference path="../TestSupport/FakeUserRepository.ts" />

//module vdb.tests.viewModels {

    import dc = vdb.dataContracts;
    import sup = vdb.tests.testSupport;

    var entryTypeTranslations;
    var entryReportRepo: sup.FakeEntryReportRepository;
    var userRepo: sup.FakeUserRepository;

    QUnit.module("TopBarViewModel", {
        setup: () => {

            entryTypeTranslations = { 'Album': 'Album!' };
            entryReportRepo = new sup.FakeEntryReportRepository();
            entryReportRepo.entryReportCount = 39;
            userRepo = new sup.FakeUserRepository();
            userRepo.messages = [
				{ createdFormatted: "2039.3.9", highPriority: false, id: 39, inbox: 'Received', read: false, receiver: null, subject: "New message!" },
				{ createdFormatted: "2039.3.9", highPriority: false, id: 40, inbox: 'Received', read: false, receiver: null, subject: "Another message" }
			];

        }
    });

    var create = (getNewReportsCount: boolean = false) => {
        return new vdb.viewModels.TopBarViewModel(entryTypeTranslations, 'Album', '', 0, getNewReportsCount, entryReportRepo, userRepo);
    };

    test("constructor", () => {

        var result = create();

        equal(result.entryTypeName(), 'Album!', "entryTypeName");
        equal(result.hasNotifications(), false, "hasNotifications");
        equal(result.reportCount(), 0, "reportCount not loaded");

    });

    test("constructor load report count", () => {

        var result = create(true);
        equal(result.reportCount(), 39, "reportCount was loaded");

    });

    test("ensureMessagesLoaded", () => {

        var target = create();

        target.ensureMessagesLoaded();

        equal(target.isLoaded(), true, "isLoaded");
		ok(target.unreadMessages(), "unreadMessages()");
        equal(target.unreadMessages().length, 2, "unreadMessages().length");
        equal(target.unreadMessages()[0].subject, "New message!");

    });

//}