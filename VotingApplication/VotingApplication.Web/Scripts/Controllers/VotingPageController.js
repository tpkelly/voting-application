﻿(function () {
    var VotingApp = angular.module('VotingApp');
    VotingApp.controller('VotingPageController', ['$scope', '$location', 'IdentityService',  function ($scope, $location, IdentityService) {

        $scope.identityName = IdentityService.identityName;

        $scope.logoutIdentity = function () {
            IdentityService.clearIdentityName();
        }

        // Angular won't auto update this so we need to use the observer pattern
        IdentityService.registerIdentityObserver(function () {
            $scope.identityName = IdentityService.identityName;
        });

        // Turn "/#/voting/abc/123" into "/#/results/abc/123"
        var locationTokens = $location.url().split("/");
        locationTokens.splice(0, 2);
        $scope.resultsLink = '/#/results/' + locationTokens.join("/");


    }]);
})();