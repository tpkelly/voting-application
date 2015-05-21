﻿(function () {
    'use strict';

    angular
        .module('GVA.Creation', ['ngRoute', 'ngDialog', 'ngStorage', 'zeroclipboard', 'toggle-switch', 'GVA.Common', 'GVA.Poll'])
        .config(['$routeProvider',
        function ($routeProvider) {
            $routeProvider
                .when('/Manage/:manageId', {
                    templateUrl: function (params) {
                        return '../Routes/Manage/' + params['manageId'];
                    }
                })
                .when('/Manage/:manageId/Name', {
                    templateUrl: function (params) {
                        return '../Routes/ManageName/' + params['manageId'];
                    }
                })
                .when('/Manage/:manageId/Choices', {
                    templateUrl: function (params) {
                        return '../Routes/ManageChoices/' + params['manageId'];
                    }
                })
                .when('/Manage/:manageId/Invitees', {
                    templateUrl: function (params) {
                        return '../Routes/ManageInvitees/' + params['manageId'];
                    }
                })
                .when('/Manage/:manageId/Misc', {
                    templateUrl: function (params) {
                        return '../Routes/ManageMisc/' + params['manageId'];
                    }
                })
                .when('/Manage/:manageId/Voters', {
                    templateUrl: function (params) {
                        return '../Routes/ManageVoters/' + params['manageId'];
                    }
                })
                .when('/Manage/:manageId/PollType', {
                    templateUrl: function (params) {
                        return '../Routes/ManagePollType/' + params['manageId'];
                    }
                })
                .when('/Manage/:manageId/Expiry', {
                    templateUrl: function (params) {
                        return '../Routes/ManageExpiry/' + params['manageId'];
                    }
                })
                .when('/Account/ResetPassword', {
                    templateUrl: '../Routes/AccountResetPassword'
                })
                .otherwise({
                    templateUrl: '../Routes/HomePage'
                });
        }])
        .config(['uiZeroclipConfigProvider', function (uiZeroclipConfigProvider) {
            uiZeroclipConfigProvider.setZcConf({
                swfPath: '/Static/Lib/ZeroClipboard.swf'
            });
        }]);
})();
