﻿using System;
using System.Collections.Generic;
using VotingApplication.Data.Model;

namespace VotingApplication.Web.Api.Models.DBViewModels
{
    public class PollRequestResponseModel
    {
        public Guid UUID { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }
        public string PollType { get; set; }
        public int? MaxPoints { get; set; }
        public int? MaxPerVote { get; set; }
        public bool InviteOnly { get; set; }
        public bool NamedVoting { get; set; }
        public DateTime? ExpiryDateUtc { get; set; }
        public bool ChoiceAdding { get; set; }
        public List<Choice> Choices { get; set; }
        public bool HiddenResults { get; set; }
    }
}