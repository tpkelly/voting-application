﻿define(['jquery', 'knockout', 'Common'], function ($, ko, Common) {


    return function BasicVote(options) {

        self = this;
        self.options = ko.observableArray(options);

        var highlightOption = function (optionId) {

            clearOptionHighlighting();

            var $optionRows = $("#optionTable > tbody > tr");
            $optionRows.filter(function () {
                return $(this).attr('data-id') == optionId;
            }).addClass("success");
        };

        var clearOptionHighlighting = function () {
            $("#optionTable > tbody > tr").removeClass("success");
        };

        var countVotes = function (votes) {
            var totalCounts = [];
            votes.forEach(function (vote) {
                var optionName = vote.Option.Name;
                var voter = vote.User.Name;

                // Find a vote with the same Option.Name, if it exists.
                var existingOption = totalCounts.filter(function (vote) { return vote.Name == optionName; }).pop();

                if (existingOption) {
                    existingOption.Count++;
                    existingOption.Voters.push(voter);
                }
                else {
                    totalCounts.push({
                        Name: optionName,
                        Count: 1,
                        Voters: [voter]
                    });
                }
            });
            return totalCounts;
        };

        var drawChart = function (data) {
            // Hack to fix insight's lack of data reloading
            $('#results').html('');
            var voteData = new insight.DataSet(data);

            var chart = new insight.Chart('', '#results')
            .width($("#results").width())
            .height(data.length * 50 + 100);

            var xAxis = new insight.Axis('Votes', insight.scales.linear)
                .tickFrequency(1);
            var yAxis = new insight.Axis('', insight.scales.ordinal)
                .isOrdered(true);
            chart.xAxis(xAxis);
            chart.yAxis(yAxis);

            var series = new insight.RowSeries('votes', voteData, xAxis, yAxis)
            .keyFunction(function (d) {
                return d.Name;
            })
            .valueFunction(function (d) {

                return d.Count;
            })
            .tooltipFunction(function (d) {
                var maxToDisplay = 5;
                if (d.Count <= maxToDisplay) {
                    return "Votes: " + d.Count + "<br />" + d.Voters.toString().replace(/,/g, "<br />");
                }
                else {
                    return "Votes: " + d.Count + "<br />" + d.Voters.slice(0, maxToDisplay).toString().replace(/,/g, "<br />") + "<br />" + "+ " + (d.Count - maxToDisplay) + " others";
                }
            });

            chart.series([series]);

            chart.draw();
        };

        self.doVote = function (data, event) {
            var userId = Common.currentUserId();
            var pollId = Common.getPollId();

            if (userId && pollId) {
                $.ajax({
                    type: 'PUT',
                    url: '/api/user/' + userId + '/vote',
                    contentType: 'application/json',
                    data: JSON.stringify([{
                        OptionId: data.Id,
                        PollId: pollId
                    }]),

                    success: function (returnData) {
                        var currentRow = event.currentTarget.parentElement.parentElement;
                        $('#resultSection > div')[0].click();
                    }
                });
            }
        };

        self.resetVotes = function () {
            clearOptionHighlighting();
        }

        self.getVotes = function (pollId, userId) {
            $.ajax({
                type: 'GET',
                url: '/api/user/' + userId + '/poll/' + pollId + '/vote',
                contentType: 'application/json',

                success: function (data) {
                    if (data[0]) {
                        highlightOption(data[0].OptionId);
                    }
                }
            });
        };

        self.getResults = function (pollId) {
            $.ajax({
                type: 'GET',
                url: '/api/poll/' + pollId + '/vote',

                success: function (data) {
                    var groupedVotes = countVotes(data);
                    drawChart(groupedVotes);
                }
            });
        };
    }

});