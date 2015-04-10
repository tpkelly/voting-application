﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using VotingApplication.Data.Context;
using VotingApplication.Data.Model;
using VotingApplication.Web.Api.Models.DBViewModels;

namespace VotingApplication.Web.Api.Controllers
{
    public class ManagePollTypeController : WebApiController
    {
        public ManagePollTypeController() : base() { }

        public ManagePollTypeController(IContextFactory contextFactory) : base(contextFactory) { }

        [HttpPut]
        public void Put(Guid manageId, ManagePollTypeRequest updateRequest)
        {
            using (var context = _contextFactory.CreateContext())
            {
                Poll poll = context.Polls.Where(p => p.ManageId == manageId).SingleOrDefault();

                if (poll == null)
                {
                    ThrowError(HttpStatusCode.NotFound, string.Format("Poll for manage id {0} not found", manageId));
                }

                PollType pollType;
                if (!Enum.TryParse<PollType>(updateRequest.PollType, true, out pollType))
                {
                    ModelState.AddModelError("PollType", "Invalid PollType");
                }

                if (!ModelState.IsValid)
                {
                    ThrowError(HttpStatusCode.BadRequest, ModelState);
                }

                if (updateRequest.PollType.ToLower() != poll.PollType.ToString().ToLower())
                {
                    List<Vote> removedVotes = context.Votes.Include(v => v.Poll)
                                                            .Where(v => v.Poll.UUID == poll.UUID)
                                                            .ToList();
                    foreach (Vote oldVote in removedVotes)
                    {
                        context.Votes.Remove(oldVote);
                    }

                }

                poll.PollType = pollType;
                poll.MaxPerVote = updateRequest.MaxPerVote;

                poll.MaxPoints = updateRequest.MaxPoints;

                poll.LastUpdated = DateTime.Now;

                context.SaveChanges();
            }
        }
    }
}