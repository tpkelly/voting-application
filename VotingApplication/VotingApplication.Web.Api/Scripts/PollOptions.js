﻿define('PollOptions', ['knockout'], function (ko) {
    return function PollOptions(pollId) {
        self = this;
        self.options = ko.observableArray();
        self.optionAdding = ko.observable(false);

        self.newName = ko.observable("");
        self.newInfo = ko.observable("");
        self.newDescription = ko.observable("");

        self.addOption = function () {
            //Don't submit without an entry in the name field
            if (self.newName() === "") return;

            $.ajax({
                type: 'POST',
                url: '/api/poll/' + pollId + '/option',
                contentType: 'application/json',

                data: JSON.stringify({
                    Name: self.newName(),
                    Description: self.newDescription(),
                    Info: self.newInfo()
                }),

                success: function () {
                    self.refreshOptions();
                }
            });

            self.newName("");
            self.newInfo("");
            self.newDescription("");
        };

        var mapOption = function (option) {
            option.highlight = ko.observable(false);
            return option;
        };

        self.refreshOptions = function () {
            $.ajax({
                type: 'GET',
                url: "/api/poll/" + pollId + "/option",

                success: function (data) {
                    data.forEach(function (dataOption) {
                        // Only append new options
                        if (self.options().filter(function (o) { return o.Id === dataOption.Id; }).length === 0) {
                            self.options.push(mapOption(dataOption));
                        }
                    });
                }
            });
        };

        // Take the grouped votes and return the winner(s) names(s)
        self.getWinners = function (groupedVotes, getOptionVotes) {
            var winners = null;
            groupedVotes.forEach(function (g) {
                // Optional callback to get the data for this grouped option
                var optionVotes = getOptionVotes ? getOptionVotes(g) : g;

                if (!winners || optionVotes.Sum > winners[0].Sum) {
                    winners = [optionVotes];
                } else if (winners[0].Sum === optionVotes.Sum) {
                    // This is a tie
                    winners.push(optionVotes);
                }
            });

            return winners ? winners.map(function (w) { return w.Name; }) : [];
        };

        self.initialise = function (pollData) {
            self.options(pollData.Options.map(mapOption));
            self.optionAdding(pollData.OptionAdding);
        };
    };
});