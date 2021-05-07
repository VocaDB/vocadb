import TagBaseContract from '../DataContracts/Tag/TagBaseContract';
import TagDetailsViewModel from '../ViewModels/Tag/TagDetailsViewModel';
import UrlMapper from '../Shared/UrlMapper';
import TagRepository from '../Repositories/TagRepository';
import UserRepository from '../Repositories/UserRepository';
import CommentContract from '../DataContracts/CommentContract';
import { IEntryReportType } from '../ViewModels/ReportEntryViewModel';
import HttpClient from '../Shared/HttpClient';

function initChart(
  urlMapper: UrlMapper,
  thisTag: string,
  parent: TagBaseContract,
  siblings: TagBaseContract[],
  children: TagBaseContract[],
  hasMoreSiblings: boolean,
  hasMoreChildren: boolean,
): void {
  var tagUrl = (tag: TagBaseContract): string =>
    urlMapper.mapRelative('/T/' + tag.id + '/' + tag.urlSlug);
  var tagLink = (tag: TagBaseContract): string => {
    var link = '<a href="' + tagUrl(tag) + '">' + tag.name + '</a>';
    return link;
  };

  var tagLinks = (tagList: TagBaseContract[]): string => {
    var str = '';
    const links = _.map(tagList, (item) => tagLink(item));
    const tagsPerRow = 7;

    for (var i = 0; i < tagList.length; i += tagsPerRow) {
      str += _.reduce<string, string>(
        _.take(_.drop(links, i), tagsPerRow),
        (list, item) => list + ', ' + item,
      );

      if (i < tagList.length + tagsPerRow) str += '<br/>';
    }

    return str;
  };

  $('#hierarchyContainer').highcharts({
    credits: { enabled: false },
    chart: {
      backgroundColor: null,
      events: {
        load: function () {
          // Draw the flow chart
          var ren = this.renderer,
            colors = Highcharts.getOptions().colors,
            downArrow = [
              'M',
              0,
              0,
              'L',
              0,
              40,
              'L',
              -5,
              35,
              'M',
              0,
              40,
              'L',
              5,
              35,
            ],
            rightAndDownArrow = [
              'M',
              0,
              0,
              'L',
              70,
              0,
              'C',
              90,
              0,
              90,
              0,
              90,
              25,
              'L',
              90,
              80,
              'L',
              85,
              75,
              'M',
              90,
              80,
              'L',
              95,
              75,
            ];

          var y = 10;

          if (parent) {
            var parentLab = ren
              .label('Parent tag:<br/>' + tagLink(parent), 10, y)
              .attr({
                fill: colors[0],
                stroke: 'white',
                'stroke-width': 2,
                padding: 5,
                r: 5,
              })
              .css({
                color: 'white',
              })
              .add()
              .shadow(true);

            // Arrow from parent to this tag
            ren
              .path(downArrow)
              .translate(50, y + 60)
              .attr({
                'stroke-width': 2,
                stroke: colors[3],
              })
              .add();

            if (siblings && siblings.length) {
              // Arrow from parent to siblings
              ren
                .path(rightAndDownArrow)
                .translate(
                  parentLab.getBBox().x + parentLab.getBBox().width + 20,
                  y + 20,
                )
                .attr({
                  'stroke-width': 2,
                  stroke: colors[3],
                })
                .add();

              const siblingsText =
                'Siblings:<br/>' +
                tagLinks(siblings) +
                (hasMoreSiblings ? ' (+ more)' : '');

              ren
                .label(siblingsText, 150, y + 115)
                .attr({
                  fill: colors[4],
                  stroke: 'white',
                  'stroke-width': 2,
                  padding: 5,
                  r: 5,
                })
                .css({
                  color: 'white',
                })
                .add()
                .shadow(true);
            }

            y += 115;
          }

          ren
            .label('This tag:<br />' + thisTag, 10, y)
            .attr({
              fill: colors[1],
              stroke: 'white',
              'stroke-width': 2,
              padding: 5,
              r: 5,
            })
            .css({
              color: 'white',
            })
            .add()
            .shadow(true);

          if (children && children.length) {
            // Arrow from this to children
            ren
              .path(downArrow)
              .translate(50, y + 60)
              .attr({
                'stroke-width': 2,
                stroke: colors[3],
              })
              .add();

            const childrenText =
              'Children:<br/>' +
              tagLinks(children) +
              (hasMoreChildren ? ' (+ more)' : '');

            ren
              .label(childrenText, 10, y + 115)
              .attr({
                fill: colors[4],
                stroke: 'white',
                'stroke-width': 2,
                padding: 5,
                r: 5,
              })
              .css({
                color: 'white',
              })
              .add()
              .shadow(true);
          }
        },
      },
    },
    title: {
      text: null,
    },
  });
}

function initTagsPage(vm: TagDetailsViewModel): void {
  $('#tabs').tabs({
    activate: (event, ui) => {
      if (ui.newTab.data('tab') === 'Discussion') {
        vm.comments.initComments();
      }
    },
  });

  $('#editTagLink').button({
    disabled: $('#editTagLink').hasClass('disabled'),
    icons: { primary: 'ui-icon-wrench' },
  });
  $('#viewVersions').button({ icons: { primary: 'ui-icon-clock' } });
  $('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });

  $('#viewCommentsLink').click(() => {
    var index = $('#tabs ul [data-tab="Discussion"]').index();
    $('#tabs').tabs('option', 'active', index);
    return false;
  });
}

const TagDetails = (
  canDeleteAllComments: boolean,
  model: {
    id: number;
    isFollowing: boolean;
    jsonModel: any;
    latestComments: CommentContract[];
  },
  reportTypes: IEntryReportType[],
  showTranslatedDescription: boolean,
): void => {
  $(function () {
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var jsonModel = model.jsonModel;
    var vm;

    initChart(
      urlMapper,
      jsonModel.name,
      jsonModel.parent,
      jsonModel.siblings,
      jsonModel.children,
      jsonModel.hasMoreSiblings,
      jsonModel.hasMoreChildren,
    );

    const httpClient = new HttpClient();
    var repo = new TagRepository(httpClient, vdb.values.baseAddress);
    var userRepo = new UserRepository(httpClient, urlMapper);
    var latestComments = model.latestComments;

    vm = new TagDetailsViewModel(
      repo,
      userRepo,
      latestComments,
      reportTypes,
      vdb.values.loggedUserId,
      model.id,
      canDeleteAllComments,
      showTranslatedDescription,
      model.isFollowing,
    );
    ko.applyBindings(vm);
    initTagsPage(vm);
  });
};

export default TagDetails;
