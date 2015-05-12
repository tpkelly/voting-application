DECLARE @voteCount INT;
SET @voteCount  = 0;

/* EDIT Below */
DECLARE @voteTotal INT;
SET @voteTotal  = 1;

DECLARE @pollId INT;
SET @pollId = 1;

DECLARE @optionCount INT;
SET @optionCount = 1;
/* EDIT Above */

While @voteCount < @voteTotal
BEGIN
	DECLARE @newGuid uniqueidentifier
	SET @newGuid = NEWID()
	
	INSERT INTO Ballots (TokenGuid) VALUES (@newGuid);
	INSERT INTO Votes (Ballot_Id, Option_Id, Poll_Id, VoteValue) VALUES (@@IDENTITY, (SELECT (FLOOR(RAND()*@optionCount)+1)), @pollId, 1);
	
	SET @voteCount = @voteCount + 1;
END;