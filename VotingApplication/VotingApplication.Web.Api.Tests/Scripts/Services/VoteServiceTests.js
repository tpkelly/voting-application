﻿'use strict';

describe('Vote Service', function () {

    beforeEach(module('GVA.Voting'));

    var voteService;
    var rootScope;
    var httpBackend;

    beforeEach(inject(function (VoteService, $rootScope, $httpBackend) {
        voteService = VoteService;
        rootScope = $rootScope;
        httpBackend = $httpBackend;
    }));

    afterEach(function () {
        httpBackend.verifyNoOutstandingExpectation();
        httpBackend.verifyNoOutstandingRequest();

        // http://stackoverflow.com/questions/27016235/angular-js-unit-testing-httpbackend-spec-has-no-expectations
        expect('Suppress SPEC HAS NO EXPECTATIONS').toBeDefined();
    });

    describe('Get Results', function () {

        it('Returns a failure for a null pollId', function () {
            var pollId = null;

            httpBackend.when(
                    'GET',
                    '/api/poll/' + pollId + '/results?lastRefreshed=0'
                    ).respond(404, '');

            var promise = voteService.getResults(pollId);

            httpBackend.flush();

            promise
                .then(function () {
                    fail('Expected the promise to be rejected, not resolved.');
                });

            rootScope.$apply();
        });

        it('Makes http call to get results', function () {

            var baseTime = new Date(2013, 9, 23);
            jasmine.clock().mockDate(baseTime);

            var pollId = '7C7CE5F8-873D-4F1F-AF3F-D24769813ABC';

            var expectedUrl = '/api/poll/' + pollId + '/results?lastRefreshed=0';

            httpBackend.expect(
                    'GET',
                    expectedUrl
                    ).respond(200, '');


            voteService.getResults(pollId);

            httpBackend.flush();
        });

        it('Passes the last requested datetime on multiple calls', function () {

            var baseTime = new Date(2013, 9, 23); // 1382482800000
            jasmine.clock().mockDate(baseTime);

            var pollId = '7C7CE5F8-873D-4F1F-AF3F-D24769813ABC';
            var initialExpectedUrl = '/api/poll/' + pollId + '/results?lastRefreshed=0';
            httpBackend.expect(
                    'GET',
                    initialExpectedUrl
                    ).respond(200, '');

            voteService.getResults(pollId);
            httpBackend.flush();



            var subesquentExpectedUrl = '/api/poll/' + pollId + '/results?lastRefreshed=1382482800000';
            httpBackend.expect(
                    'GET',
                    subesquentExpectedUrl
                    ).respond(200, '');


            voteService.getResults(pollId);

            httpBackend.flush();
        });
    });

    describe('Submit Vote', function () {

        it('Returns a failure for a null pollId', function () {
            var pollId = null;


            var promise = voteService.submitVote(pollId);

            promise
                .then(function () {
                    fail('Expected the promise to be rejected, not resolved.');
                });

            rootScope.$apply();
        });

        it('Returns a failure for null votes', function () {
            var pollId = '7C7CE5F8-873D-4F1F-AF3F-D24769813ABC';
            var votes = null;


            var promise = voteService.submitVote(pollId, votes);

            promise
                .then(function () {
                    fail('Expected the promise to be rejected, not resolved.');
                });

            rootScope.$apply();
        });

        it('Returns a failure for a null token', function () {
            var pollId = '7C7CE5F8-873D-4F1F-AF3F-D24769813ABC';
            var votes = [];
            var token = null;

            var promise = voteService.submitVote(pollId, votes, token);

            promise
                .then(function () {
                    fail('Expected the promise to be rejected, not resolved.');
                });

            rootScope.$apply();
        });

        it('Makes http call to submit vote', function () {
            var pollId = '7C7CE5F8-873D-4F1F-AF3F-D24769813ABC';
            var votes = [];
            var token = '64C66CBC-5A5F-481E-9546-26DCE015CD40';

            var expectedUrl = '/api/poll/' + pollId + '/token/' + token + '/vote';

            httpBackend.expect(
                    'PUT',
                    expectedUrl
                    ).respond(200, '');

            voteService.submitVote(pollId, votes, token);

            httpBackend.flush();
        });

    });

    describe('Add Voter Option', function () {

        it('Makes http call to add option', function () {
            var pollId = '7C7CE5F8-873D-4F1F-AF3F-D24769813ABC';
            var newOption = {};

            var expectedUrl = '/api/poll/' + pollId + '/option';

            httpBackend.expect(
                    'POST',
                    expectedUrl
                    ).respond(200, '');

            voteService.addVoterOption(pollId, newOption);

            httpBackend.flush();

        });

    });
});