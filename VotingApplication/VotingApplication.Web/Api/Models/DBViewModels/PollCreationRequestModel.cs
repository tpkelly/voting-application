﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VotingApplication.Data.Model;

namespace VotingApplication.Web.Api.Models.DBViewModels
{
    public class PollCreationRequestModel
    {
        [Required]
        public string PollName { get; set; }

        public List<Choice> Choices { get; set; }
        //public bool ChoiceAdding { get; set; }
        //public string PollType { get; set; }
        //public bool NamedVoting { get; set; }
        //public bool ElectionMode { get; set; }
        //public DateTime? ExpiryDateUtc { get; set; }
        //public List<string> Invitations { get; set; }
        //public bool InviteOnly { get; set; }

    }
}