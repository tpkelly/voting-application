﻿/// <reference path="../Services/AccountService.js" />
(function () {
    angular
        .module('GVA.Creation')
        .controller('CreatePageController', CreatePageController);

    CreatePageController.$inject = ['$scope', 'AccountService'];

    function CreatePageController($scope, AccountService) {

        $scope.account = AccountService.account;
        $scope.openLoginDialog = showLoginDialog;
        $scope.openRegisterDialog = openRegisterDialog;
        $scope.signOut = AccountService.clearAccount;
        
        activate();

        function showLoginDialog() {
            AccountService.openLoginDialog($scope);
        }

        function openRegisterDialog() {
            AccountService.openRegisterDialog($scope);
        }

        function activate() {
            AccountService.registerAccountObserver(function () {
                $scope.account = AccountService.account;
            });
        }
    }
})();
