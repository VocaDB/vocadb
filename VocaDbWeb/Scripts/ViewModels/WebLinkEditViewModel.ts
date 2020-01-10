/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../DataContracts/WebLinkContract.ts" />
/// <reference path="../Models/WebLinkCategory.ts" />
/// <reference path="../Shared/WebLinkMatcher.ts" />

import WebLinkCategory from '../Models/WebLinkCategory';
import WebLinkContract from '../DataContracts/WebLinkContract';
import WebLinkMatcher from '../Shared/WebLinkMatcher';

//module vdb.viewModels {

    export default class WebLinkEditViewModel {
        
        public category: KnockoutObservable<string>;

        public description: KnockoutObservable<string>;

        public id: number;

        public url: KnockoutObservable<string>;

        constructor(data?: WebLinkContract) {
            
            if (data) {

                this.category = ko.observable(data.category);
                this.description = ko.observable(data.description);
                this.id = data.id;
                this.url = ko.observable(data.url);

            } else {

                this.category = ko.observable(WebLinkCategory[WebLinkCategory.Other]);
                this.description = ko.observable("");
                this.id = 0;
                this.url = ko.observable("");

            }

            this.url.subscribe(url => {

                if (!this.description()) {

                    var matcher = WebLinkMatcher.matchWebLink(url);

                    if (matcher) {
                        this.description(matcher.desc);
                        this.category(WebLinkCategory[matcher.cat]);
                    }

                }

            }); 
                               
        }
    
    }

//}