
(function () {
    angular.module('BlakWealth')
        .factory('contentItemFactory', contentItemFactory);
    contentItemFactory.$inject = ['$http', '$q'];


    function contentItemFactory($http, $q) {

        function _get(activeId) {
            settings = {
                url: '/api/contentitemcomponent/' + activeId,
                cache: false,
                method: "GET",
                responseType: "json",
            };
            return $http(settings)
                .then(_getSuccess, _getError);
        }

        function _getSuccess(response) {
            return response;
        }

        function _getError(error) {
            return $q.reject("this is a get error");
        }
        return {
            get: _get
        };
    }
})();

(function () {
    angular.module('BlakWealth')
        .controller('contentItemController', contentItemController);
    contentItemController.$inject = ['contentItemFactory', '$sce', 'alertService', '$window'];

    function contentItemController(contentItemFactory, $sce, alertService, $window) {
        var vm = this;
        vm.contentItemFactory = contentItemFactory;
        vm.data = {};
        vm.get = _getData;
        vm.$onChanges = _inIt;

        function _inIt() {
            if (vm.contentId) {
                vm.get(vm.contentId);
            }
            else {
                vm.data = {};
                makeFalse();
            }
        }

        function _getData(id) {
            vm.contentItemFactory.get(id)
                .then(_getDataSuccess, _getDataError);
        }

        function makeFalse() {
            vm.showVideo = false;
            vm.showImage = false;
            vm.showLink = false;
            vm.showAudio = false;
            vm.showAmazon = false;
            vm.showText = false;
            vm.getContentInfo({ info: null });
        }

        function _getDataSuccess(response) {

            makeFalse();

            if (response.data.item.contentType == "www.youtube.com") {
                var youtubeUrl = response.data.item.contents;
                youtubeGetID(youtubeUrl);
                vm.showVideo = true;
            } else if (response.data.item.contentType == "vimeo.com") {
                var vimeoUrl = response.data.item.contents;
                vimeoID(vimeoUrl);
                vm.showVideo = true;
            } else if (response.data.item.contentType == "image") {
                vm.data.contents = response.data.item.contents;
                vm.showImage = true;   
            } else if (response.data.item.contentType == "link") {
                vm.data.contents = $sce.trustAsResourceUrl(response.data.item.contents);
                vm.showLink = true;
            } else if (response.data.item.contentType == "audio") {
                vm.data.contents = $sce.trustAsResourceUrl(response.data.item.contents);
                vm.showAudio = true;
            } else if (response.data.item.contentType == "video") {
                vm.data.contents = response.data.item.contents;
                vm.showAmazon = true;
            } else if (response.data.item.contentType == "markdown") {

                var converter = new showdown.Converter(),
                    text = response.data.item.contents,
                    html = converter.makeHtml(text);

                vm.data.contents = $sce.trustAsHtml(html);
                vm.showText = true;
            }

            vm.getContentInfo({ info: response.data.item})

        }

        function _getDataError(error) {
            alertService.error("There was an error in retrieving the data");
        }

        function youtubeGetID(url) {
            var ID = '';
            url = url.replace(/(>|<)/gi, '').split(/(vi\/|v=|\/v\/|youtu\.be\/|\/embed\/)/);
            if (url[2] !== undefined) {
                ID = url[2].split(/[^0-9a-z_\-]/i);
                ID = ID[0];
            }
            else {
                ID = url;
            }
            showYoutube(ID);
        }

        function showYoutube(Id) {
            vm.data.contents = $sce.trustAsResourceUrl("https://www.youtube.com/embed/" + Id);
        }

        function vimeoID(url) {
            var vimeoRegex = /https?:\/\/(?:www\.)?vimeo.com\/(?:channels\/(?:\w+\/)?|groups\/([^\/]*)\/videos\/|album\/(\d+)\/video\/|)(\d+)(?:$|\/|\?)/;
            var match = url.match(vimeoRegex);
            showVimeo(match[3]);
        }

        function showVimeo(Id) {
            vm.data.contents = $sce.trustAsResourceUrl("https://player.vimeo.com/video/" + Id);
        }
    }

})();

(function () {
    "use strict";

    var app = angular.module("BlakWealth");

    app.component("contentItem", {
        templateUrl: "contentitemwidget/contentitemwidget.html",
        controller: 'contentItemController',
        bindings: {
            contentId: '<',
            getContentInfo: '&'
        }
    });
})();