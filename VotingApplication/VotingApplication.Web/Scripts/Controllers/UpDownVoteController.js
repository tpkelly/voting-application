﻿/// <reference path="../Services/IdentityService.js" />
/// <reference path="../Services/PollService.js" />
/// <reference path="../Services/TokenService.js" />
/// <reference path="../Services/VoteService.js" />
(function () {
    angular
        .module('GVA.Voting')
        .controller('UpDownVoteController', UpDownVoteController);

    UpDownVoteController.$inject = ['$scope', '$routeParams', 'IdentityService', 'PollService', 'TokenService', 'VoteService'];

    function UpDownVoteController($scope, $routeParams, IdentityService, PollService, TokenService, VoteService) {

        var pollId = $routeParams.pollId;
        var token = null;

        // TODO: Rename this function, as it's ambiguous (i.e. 'vote' is a verb and a noun).
        $scope.vote = submitVote;

        activate();

        function activate() {
            PollService.getPoll(pollId, getPollSuccessCallback);
        }

        function getPollSuccessCallback(pollData) {
            $scope.options = pollData.Options;

            TokenService.getToken(pollId, getTokenSuccessCallback);
        }

        function getTokenSuccessCallback(tokenData) {
            token = tokenData;

            // Get Previous Votes
            VoteService.getTokenVotes(pollId, token, function (voteData) {
                voteData.forEach(function (dataItem) {

                    for (var i = 0; i < $scope.options.length; i++) {

                        var option = $scope.options[i];

                        if (option.Id === dataItem.OptionId) {
                            option.voteValue = dataItem.VoteValue;
                            break;
                        }
                    }
                });
            });
        }

        function submitVote(options) {
            if (!options) {
                return null;
            }

            if (!token) {
                // Probably invite only, tell the user
            }
            else if (!IdentityService.identity) {
                IdentityService.openLoginDialog($scope, function () {
                    $scope.vote(options);
                });
            }
            else {

                var votes = options
                    .filter(function (option) { return option.voteValue; })
                    .map(function (option) {
                        return {
                            OptionId: option.Id,
                            VoteValue: option.voteValue,
                            VoterName: IdentityService.identity.name
                        };
                    });

                VoteService.submitVote(pollId, votes, token, function (data) {
                    window.location = $scope.$parent.resultsLink;
                });
            }
        }
    }

})();
