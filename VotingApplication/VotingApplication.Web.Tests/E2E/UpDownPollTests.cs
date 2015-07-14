﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Protractor;
using System;
using System.Collections.Generic;
using System.Linq;
using VotingApplication.Data.Model;
using VotingApplication.Web.Tests.E2E.Helpers;
using VotingApplication.Web.Tests.E2E.Helpers.Clearers;

namespace VotingApplication.Web.Tests.E2E
{
    public class UpDownPollTests
    {
        private const string ChromeDriverDir = @"..\..\";
        private const string SiteBaseUri = @"http://localhost:64205/";

        [TestClass]
        public class DefaultPollConfiguration : E2ETest
        {
            private static readonly Guid PollGuid = Guid.NewGuid();
            private static readonly string PollUrl = SiteBaseUri + "Poll/#/" + PollGuid + "/Vote";

            [TestMethod]
            [TestCategory("E2E")]
            public void PopulatedChoices_DisplaysAllChoices()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        Poll poll = CreatePoll(context);
                        GoToUrl(driver, PollUrl);

                        IReadOnlyCollection<IWebElement> choiceNames = driver.FindElements(NgBy.Binding("choice.Name"));

                        Assert.AreEqual(poll.Choices.Count, choiceNames.Count);

                        List<string> expected = poll.Choices.Select(o => o.Name).ToList();
                        List<string> actual = choiceNames.Select(o => o.Text).ToList();
                        CollectionAssert.AreEquivalent(expected, actual);
                    }
                }
            }

            [TestMethod]
            [TestCategory("E2E")]
            public void VotingOnChoice_NavigatesToResultsPage()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        Poll poll = CreatePoll(context);
                        GoToUrl(driver, PollUrl);
                        IReadOnlyCollection<IWebElement> choices = driver.FindElements(NgBy.Repeater("choice in poll.Choices"));

                        IReadOnlyCollection<IWebElement> firstChoiceButtons = choices.First().FindElements(By.TagName("Button"));
                        IWebElement downButton = firstChoiceButtons.First();
                        downButton.Click();

                        IWebElement voteButton = FindElementById(driver, "vote-button");
                        voteButton.Click();

                        string expectedUriPrefix = SiteBaseUri + "Poll/#/" + poll.UUID + "/Results";
                        Assert.IsTrue(driver.Url.StartsWith(expectedUriPrefix));
                    }
                }
            }

            [Ignore]
            [TestMethod]
            [TestCategory("E2E")]
            public void DefaultPoll_ShowsResultsButton()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        CreatePoll(context);
                        GoToUrl(driver, PollUrl);

                        IWebElement resultsLink = FindElementById(driver, "results-button");

                        Assert.IsTrue(resultsLink.IsVisible());
                    }
                }
            }

            [TestMethod, TestCategory("E2E")]
            public void AfterVoting_VoteIsRemembered()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        CreatePoll(context);
                        GoToUrl(driver, PollUrl);

                        IReadOnlyCollection<IWebElement> choices = driver.FindElements(NgBy.Repeater("choice in poll.Choices"));

                        IReadOnlyCollection<IWebElement> firstChoiceButtons = choices.First().FindElements(By.TagName("Button"));
                        IWebElement downButton = firstChoiceButtons.First();
                        downButton.Click();

                        IWebElement voteButton = FindElementById(driver, "vote-button");
                        voteButton.Click();

                        NavigateBackToVotePage(driver);

                        IReadOnlyCollection<IWebElement> selectedChoices = driver.FindElements(NgBy.Repeater("choice in poll.Choices"));
                        IWebElement selectedChoice = selectedChoices.First();
                        IWebElement selectedChoiceButton = selectedChoice.FindElement(By.CssSelector(".active-btn"));


                        Assert.IsTrue(selectedChoiceButton.IsVisible());
                    }
                }
            }

            public static Poll CreatePoll(TestVotingContext testContext)
            {
                var testPollChoices = new List<Choice>() 
                {
                    new Choice(){ Name = "Test Choice 1", Description = "Test Description 1" }
                };

                // Open, Anonymous, No Choice Adding, Shown Results
                var defaultUpDownPoll = new Poll()
                {
                    UUID = PollGuid,
                    PollType = PollType.UpDown,
                    Name = "Test Poll",
                    LastUpdatedUtc = DateTime.UtcNow,
                    CreatedDateUtc = DateTime.UtcNow,
                    Choices = testPollChoices,
                    InviteOnly = false,
                    NamedVoting = false,
                    ChoiceAdding = false,
                    ElectionMode = false
                };

                testContext.Polls.Add(defaultUpDownPoll);
                testContext.SaveChanges();

                return defaultUpDownPoll;
            }

            private static void NavigateBackToVotePage(IWebDriver driver)
            {
                // The token is on the Uri, so we can't just navigate back to
                // PollUrl, as it won't display the selected vote.
                string resultsUri = driver.Url.Replace("Results", "Vote");
                GoToUrl(driver, resultsUri);
            }
        }

        [TestClass]
        public class InviteOnlyTests : E2ETest
        {
            private static readonly Guid PollGuid = Guid.NewGuid();
            private static readonly string PollUrl = SiteBaseUri + "Poll/#/" + PollGuid + "/Vote";

            [TestMethod]
            [TestCategory("E2E")]
            public void AccessWithToken_DisplaysAllChoices()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        Poll poll = CreatePoll(context);

                        GoToUrl(driver, PollUrl + "/" + poll.Ballots[0].TokenGuid);
                        IReadOnlyCollection<IWebElement> choiceNames = driver.FindElements(NgBy.Binding("choice.Name"));

                        Assert.AreEqual(poll.Choices.Count, choiceNames.Count);

                        List<string> expected = poll.Choices.Select(o => o.Name).ToList();
                        List<string> actual = choiceNames.Select(o => o.Text).ToList();
                        CollectionAssert.AreEquivalent(expected, actual);
                    }
                }
            }

            [TestMethod]
            [TestCategory("E2E")]
            public void VoteWithToken_NavigatesToResultsPage()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        Poll poll = CreatePoll(context);

                        GoToUrl(driver, PollUrl + "/" + poll.Ballots[0].TokenGuid);

                        IReadOnlyCollection<IWebElement> voteButtons = FindElementsById(driver, "vote-button");
                        voteButtons.First().Click();

                        string expectedUrlPrefix = SiteBaseUri + "Poll/#/" + poll.UUID + "/Results";
                        Assert.IsTrue(driver.Url.StartsWith(expectedUrlPrefix));
                    }
                }
            }

            public static Poll CreatePoll(TestVotingContext testContext)
            {
                List<Choice> testPollChoices = new List<Choice>
                {
                    new Choice() { Name = "Test Choice 1", Description = "Test Description 1"},
                    new Choice() { Name = "Test Choice 2", Description = "Test Description 2"}
                };


                // Invite Only, Anonymous, No Choice Adding, Shown Results
                var inviteOnlyUpDownPoll = new Poll()
                {
                    UUID = PollGuid,
                    PollType = PollType.UpDown,
                    Name = "Test Poll",
                    LastUpdatedUtc = DateTime.UtcNow,
                    CreatedDateUtc = DateTime.UtcNow,
                    Choices = testPollChoices,
                    InviteOnly = true,
                    NamedVoting = false,
                    ChoiceAdding = false,
                    ElectionMode = false,
                    Ballots = new List<Ballot>()
                    {
                        new Ballot() { TokenGuid = Guid.NewGuid() }
                    }
                };

                testContext.Polls.Add(inviteOnlyUpDownPoll);
                testContext.SaveChanges();

                return inviteOnlyUpDownPoll;
            }
        }

        [TestClass]
        public class NamedVotingTests : E2ETest
        {
            const string VoterName = "User";
            private static readonly Guid PollGuid = Guid.NewGuid();
            private readonly string _pollVoteUrl = GetPollVoteUrl(PollGuid);
            private readonly string _pollResultsUrl = GetPollResultsUrl(PollGuid);

            [TestMethod]
            [TestCategory("E2E")]
            public void NoNameEntered_VoteNotAllowed()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        CreateNamedVotersPoll(context);
                        GoToUrl(driver, _pollVoteUrl);


                        IWebElement voteButton = FindElementById(driver, "vote-button");
                        voteButton.Click();

                        Assert.IsFalse(driver.Url.StartsWith(_pollResultsUrl));
                        Assert.IsTrue(driver.Url.StartsWith(_pollVoteUrl));
                    }
                }
            }

            [TestMethod]
            [TestCategory("E2E")]
            public void NameEntered_VoteAllowed()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        CreateNamedVotersPoll(context);
                        GoToUrl(driver, _pollVoteUrl);


                        IWebElement nameInput = FindElementById(driver, "voter-name-input");
                        nameInput.SendKeys(VoterName);

                        IWebElement voteButton = FindElementById(driver, "vote-button");
                        voteButton.Click();

                        Assert.IsTrue(driver.Url.StartsWith(_pollResultsUrl));
                    }
                }
            }

            [TestMethod]
            [TestCategory("E2E")]
            public void NoNameEntered_ShowsFailedValidationMessage()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        CreateNamedVotersPoll(context);
                        GoToUrl(driver, _pollVoteUrl);


                        IWebElement voteButton = FindElementById(driver, "vote-button");
                        voteButton.Click();

                        IWebElement requiredValidationMessage = FindElementById(driver, "voter-name-required-validation-message");

                        Assert.IsTrue(requiredValidationMessage.IsVisible());
                    }
                }
            }

            [TestMethod]
            [TestCategory("E2E")]
            public void EnteringVoterName_AllowsVoting()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        CreateNamedVotersPoll(context);
                        GoToUrl(driver, _pollVoteUrl);


                        IWebElement voteButton = FindElementById(driver, "vote-button");
                        voteButton.Click();

                        IWebElement requiredValidationMessage = FindElementById(driver, "voter-name-required-validation-message");

                        Assert.IsTrue(requiredValidationMessage.IsVisible());



                        IWebElement nameInput = FindElementById(driver, "voter-name-input");
                        nameInput.SendKeys(VoterName);

                        voteButton.Click();

                        Assert.IsTrue(driver.Url.StartsWith(_pollResultsUrl));
                    }
                }
            }

            [TestMethod]
            [TestCategory("E2E")]
            public void VotingAndReturning_RemembersVoterName()
            {
                using (IWebDriver driver = Driver)
                {
                    using (var context = new TestVotingContext())
                    {
                        CreateNamedVotersPoll(context);
                        GoToUrl(driver, _pollVoteUrl);


                        IWebElement nameInput = FindElementById(driver, "voter-name-input");
                        nameInput.SendKeys(VoterName);

                        IWebElement voteButton = FindElementById(driver, "vote-button");
                        voteButton.Click();

                        GoToUrl(driver, _pollVoteUrl);

                        IWebElement newNameInput = FindElementById(driver, "voter-name-input");

                        Assert.AreEqual(VoterName, newNameInput.GetAttribute("value"));
                    }
                }
            }

            public static Poll CreateNamedVotersPoll(TestVotingContext testContext)
            {
                var testPollChoices = new List<Choice>() 
                {
                    new Choice(){ Name = "Test Choice 1", Description = "Test Description 1" },
                };

                // Open, Anonymous, No Choice Adding, Shown Results
                var namedVotersPoll = new Poll()
                {
                    UUID = PollGuid,
                    PollType = PollType.UpDown,
                    Name = "Test Poll",
                    LastUpdatedUtc = DateTime.UtcNow,
                    CreatedDateUtc = DateTime.UtcNow,
                    Choices = testPollChoices,
                    InviteOnly = false,
                    NamedVoting = true,
                    ChoiceAdding = false,
                    ElectionMode = false
                };

                testContext.Polls.Add(namedVotersPoll);
                testContext.SaveChanges();

                return namedVotersPoll;
            }
        }

        [TestClass]
        public class ChoiceAddingPollConfiguration
        {
            private static ITestVotingContext _context;
            private static Poll _choiceAddingUpDownPoll;
            private static readonly Guid PollGuid = Guid.NewGuid();
            private IWebDriver _driver;

            [TestInitialize]
            public virtual void TestInitialise()
            {
                _driver = new NgWebDriver(new ChromeDriver(ChromeDriverDir));
                _driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(10));
                _driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
            }

            [TestCleanup]
            public void TestCleanUp()
            {
                _driver.Dispose();
            }

            [ClassInitialize]
            public static void ClassInitialise(TestContext testContext)
            {
                _context = new TestVotingContext();

                List<Choice> testPollChoices = new List<Choice>() {
                new Choice(){ Name = "Test Choice 1", Description = "Test Description 1" },
                new Choice(){ Name = "Test Choice 2", Description = "Test Description 2" }};

                // Open, Named voters, No Choice Adding, Shown Results
                _choiceAddingUpDownPoll = new Poll()
                {
                    UUID = PollGuid,
                    PollType = PollType.UpDown,
                    Name = "Test Poll",
                    LastUpdatedUtc = DateTime.UtcNow,
                    CreatedDateUtc = DateTime.UtcNow,
                    Choices = testPollChoices,
                    InviteOnly = false,
                    NamedVoting = false,
                    ChoiceAdding = true,
                    ElectionMode = false
                };

                _context.Polls.Add(_choiceAddingUpDownPoll);
                _context.SaveChanges();
            }

            [ClassCleanup]
            public static void ClassCleanup()
            {
                PollClearer pollTearDown = new PollClearer(_context);
                pollTearDown.ClearPoll(_choiceAddingUpDownPoll);

                _context.Dispose();
            }

            [TestMethod, TestCategory("E2E")]
            public void ChoiceAddingPoll_ProvidesLinkForAddingChoices()
            {
                _driver.Navigate().GoToUrl(SiteBaseUri + "Poll/#/Vote/" + _choiceAddingUpDownPoll.UUID);

                IWebElement addChoiceLink = _driver.FindElement(By.Id("add-choice-link"));

                Assert.IsTrue(addChoiceLink.IsVisible());
            }

            [TestMethod, TestCategory("E2E")]
            public void ChoiceAddingLink_PromptsForChoiceDetails()
            {
                _driver.Navigate().GoToUrl(SiteBaseUri + "Poll/#/Vote/" + _choiceAddingUpDownPoll.UUID);
                IWebElement addChoiceLink = _driver.FindElement(By.Id("add-choice-link"));
                addChoiceLink.Click();

                Assert.AreEqual(SiteBaseUri + "Poll/#/Vote/" + _choiceAddingUpDownPoll.UUID, _driver.Url);

                IWebElement formName = _driver.FindElement(NgBy.Model("addChoiceForm.name"));
                Assert.IsTrue(formName.IsVisible());
                Assert.AreEqual(String.Empty, formName.Text);

                IWebElement formDescription = _driver.FindElement(NgBy.Model("addChoiceForm.description"));
                Assert.IsTrue(formDescription.IsVisible());
                Assert.AreEqual(String.Empty, formDescription.Text);
            }

            [TestMethod, TestCategory("E2E")]
            public void ChoiceAddingPrompt_AcceptsValidName()
            {
                _driver.Navigate().GoToUrl(SiteBaseUri + "Poll/#/Vote/" + _choiceAddingUpDownPoll.UUID);
                IWebElement addChoiceLink = _driver.FindElement(By.Id("add-choice-link"));
                addChoiceLink.Click();

                IWebElement formName = _driver.FindElement(NgBy.Model("addChoiceForm.name"));
                IWebElement addButton = _driver.FindElement(By.Id("add-button"));

                Assert.IsTrue(addButton.IsVisible());
                Assert.IsFalse(addButton.Enabled);

                formName.SendKeys("New Choice");

                Assert.IsTrue(addButton.Enabled);
            }

            [TestMethod, TestCategory("E2E")]
            public void ChoiceAddingSubmission_AddsChoice()
            {
                _driver.Navigate().GoToUrl(SiteBaseUri + "Poll/#/Vote/" + _choiceAddingUpDownPoll.UUID);
                IWebElement addChoiceLink = _driver.FindElement(By.Id("add-choice-link"));
                addChoiceLink.Click();

                IWebElement formName = _driver.FindElement(NgBy.Model("addChoiceForm.name"));


                const string newChoiceName = "New Choice";
                formName.SendKeys(newChoiceName);

                IWebElement formButton = _driver.FindElement(By.Id("add-button"));
                formButton.Click();

                IReadOnlyCollection<IWebElement> choiceNames = _driver.FindElements(NgBy.Binding("choice.Name"));

                Assert.AreEqual(_choiceAddingUpDownPoll.Choices.Count + 1, choiceNames.Count);
                Assert.AreEqual(newChoiceName, choiceNames.Last().Text);

                // Refresh to ensure they new choice was stored in DB
                _driver.Navigate().GoToUrl(SiteBaseUri + "Poll/#/Vote/" + _choiceAddingUpDownPoll.UUID);

                choiceNames = _driver.FindElements(NgBy.Binding("choice.Name"));

                Assert.AreEqual(_choiceAddingUpDownPoll.Choices.Count + 1, choiceNames.Count);
                Assert.AreEqual(newChoiceName, choiceNames.Last().Text);
            }
        }

        [TestClass]
        public class ElectionModeConfiguration
        {
            private static ITestVotingContext _context;
            private static Poll _electionModeUpDownPoll;
            private static readonly Guid PollGuid = Guid.NewGuid();
            private static readonly string PollUrl = SiteBaseUri + "Poll/#/Vote/" + PollGuid;
            private IWebDriver _driver;

            [ClassInitialize]
            public static void ClassInitialise(TestContext testContext)
            {
                _context = new TestVotingContext();

                List<Choice> testPollChoices = new List<Choice>() {
                    new Choice(){ Name = "Test Choice 1", Description = "Test Description 1" }
                };

                // Open, Anonymous, No Choice Adding, Shown Results
                _electionModeUpDownPoll = new Poll()
                {
                    UUID = PollGuid,
                    PollType = PollType.UpDown,
                    Name = "Test Poll",
                    LastUpdatedUtc = DateTime.UtcNow,
                    CreatedDateUtc = DateTime.UtcNow,
                    Choices = testPollChoices,
                    InviteOnly = false,
                    NamedVoting = false,
                    ChoiceAdding = false,
                    ElectionMode = true
                };

                _context.Polls.Add(_electionModeUpDownPoll);
                _context.SaveChanges();
            }

            [ClassCleanup]
            public static void ClassCleanup()
            {
                PollClearer pollTearDown = new PollClearer(_context);
                pollTearDown.ClearPoll(_electionModeUpDownPoll);

                _context.Dispose();
            }

            [TestInitialize]
            public virtual void TestInitialise()
            {
                _driver = new NgWebDriver(new ChromeDriver(ChromeDriverDir));
                _driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(10));
                _driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
            }

            [TestCleanup]
            public void TestCleanUp()
            {
                _driver.Dispose();
            }

            [Ignore]
            [TestMethod]
            [TestCategory("E2E")]
            public void ElectionModePoll_DoesNotShowResultsButton()
            {
                _driver.Navigate().GoToUrl(PollUrl);
                IWebElement resultButton = _driver.FindElement(By.Id("results-button"));

                Assert.IsFalse(resultButton.IsVisible());
            }
        }
    }
}
