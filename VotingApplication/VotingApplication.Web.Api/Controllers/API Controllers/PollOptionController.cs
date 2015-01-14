﻿using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using VotingApplication.Data.Context;
using VotingApplication.Data.Model;
using VotingApplication.Web.Api.Filters;

namespace VotingApplication.Web.Api.Controllers.API_Controllers
{
    public class PollOptionController : WebApiController
    {
        public PollOptionController() : base() { }
        public PollOptionController(IContextFactory contextFactory) : base(contextFactory) { }

        #region GET

        public virtual HttpResponseMessage Get(Guid pollId)
        {
            using (var context = _contextFactory.CreateContext())
            {
                Poll matchingPoll = context.Polls.Where(s => s.UUID == pollId).Include(s => s.Options).FirstOrDefault();
                if (matchingPoll == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Poll {0} not found", pollId));
                }

                // Hide UUIDs of any other polls that are linked through options
                if (matchingPoll.Options != null)
                {
                    foreach (Option matchingPollOptions in matchingPoll.Options)
                    {
                        if (matchingPollOptions.Polls != null)
                        {
                            foreach (Poll poll in matchingPollOptions.Polls)
                            {
                                poll.UUID = Guid.Empty;
                                poll.ManageId = Guid.Empty;
                            }
                        }

                    }
                }

                return this.Request.CreateResponse(HttpStatusCode.OK, matchingPoll.Options);
            }
        }

        public virtual HttpResponseMessage Get(Guid pollId, long optionId)
        {
            return this.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Cannot use GET by id on this controller");
        }

        #endregion

        #region POST

        public virtual HttpResponseMessage Post(Guid pollId, Option option)
        {
            using (var context = _contextFactory.CreateContext())
            {
                if (option == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Option required");
                }

                Poll matchingPoll = context.Polls.Where(p => p.UUID == pollId).FirstOrDefault();
                if (matchingPoll == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Poll {0} does not exist", pollId));
                }

                if (!matchingPoll.OptionAdding)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, string.Format("Option adding not allowed for poll {0}", pollId));
                }

                if (option.Name == null || option.Name.Length == 0)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cannot create an option without a name");
                }

                if (matchingPoll.Options == null)
                {
                    matchingPoll.Options = new List<Option>();
                }

                matchingPoll.Options.Add(option);
                context.SaveChanges();

                return this.Request.CreateResponse(HttpStatusCode.OK, option.Id);
            }
        }

        public virtual HttpResponseMessage Post(Guid pollId, long optionId, Option option)
        {
            return this.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Cannot use POST by id on this controller");
        }

        #endregion

        #region PUT

        public virtual HttpResponseMessage Put(Guid pollId, Option option)
        {
            return this.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Cannot use PUT on this controller");
        }

        public virtual HttpResponseMessage Put(Guid pollId, long optionId, Option option)
        {
            return this.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Cannot use PUT by id on this controller");
        }

        #endregion

        #region DELETE

        public virtual HttpResponseMessage Delete(Guid pollId)
        {
            return this.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Cannot use DELETE on this controller");
        }

        public virtual HttpResponseMessage Delete(Guid pollId, long optionId)
        {
            return this.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Cannot use DELETE by id on this controller");
        }

        #endregion

    }
}