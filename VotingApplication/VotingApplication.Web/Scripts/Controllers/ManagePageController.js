﻿/// <reference path="../Services/AccountService.js" />
/// <reference path="../Services/ManageService.js" />
/// <reference path="../Services/RoutingService.js" />
(function () {
    'use strict';

    angular
        .module('GVA.Creation')
        .controller('ManagePageController', ManagePageController);

    ManagePageController.$inject = ['$scope', '$routeParams', 'ManageService', 'RoutingService'];

    function ManagePageController($scope, $routeParams, ManageService, RoutingService) {

        var manageId = $routeParams.manageId;

        $scope.poll = {};
        $scope.manageId = manageId;
        $scope.updateQuestion = updateQuestion;
        $scope.discardNameChanges = discardNameChanges;
        $scope.formatPollExpiry = formatPollExpiryDate;
        $scope.selectText = selectTargetText;
        $scope.dateFilter = dateFilter;
        $scope.pollUrl = pollUrl;
        $scope.fullPollUrl = fullPollUrl;
        $scope.manageSubPageUrl = manageSubPageUrl;
        $scope.visited = false;

        activate();

        function dateFilter(date) {
            var startOfDay = new Date();
            startOfDay.setHours(0);
            return date >= startOfDay;
        }

        function activate() {
            ManageService.getPoll(manageId, function (data) {
                $scope.poll = data;
                $scope.Question = data.Name;
            });
            $scope.visited = ManageService.getVisited(manageId);
            if (!$scope.visited) {
                ManageService.setVisited(manageId);
            }
        }

        function updateQuestion() {
            var successCallback = function () {
                $scope.poll.Name = $scope.Question;
            };

            var failureCallback = function () {
                $scope.Question = $scope.poll.Name;
            };

            ManageService.updateQuestion($routeParams.manageId, $scope.Question, successCallback, failureCallback);
        }

        function discardNameChanges() {
            ManageService.getPoll(manageId, function (data) {
                $scope.poll = data;
            });
        }

        function formatPollExpiryDate() {
            if (!$scope.poll.ExpiryDate) {
                return 'Never';
            }

            var expiryDate = new Date($scope.poll.ExpiryDate);
            return moment(expiryDate).format('ddd, MMM Do YYYY, HH:mm');
        }

        function selectTargetText($event) {
            $event.target.select();
        }

        function pollUrl() {
            return RoutingService.getVotePageUrl($scope.poll.UUID);
        }

        function fullPollUrl() {
            return location.protocol + '//' + location.host + pollUrl();
        }

        function manageSubPageUrl(subPage) {
            return RoutingService.getManagePageUrl($scope.manageId, subPage);
        }
    }
})();
