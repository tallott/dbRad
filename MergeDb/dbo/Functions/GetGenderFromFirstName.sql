CREATE FUNCTION GetGenderFromFirstName
(@firstName VARCHAR(50)
)
RETURNS INT
AS
     BEGIN
         DECLARE @GenderId INT;
         SELECT @GenderId = CASE
                                WHEN FirstName IN('Ashleigh', 'Awhina', 'Charlotte', 'Christine', 'Danielle', 'Deanna', 'Debbie', 'Demie', 'Devendra', 'Ella', 'Hannah', 'Hayley', 'Heather', 'Helen', 'Inga', 'Isabella', 'Jackie', 'Jan', 'Joanne', 'Karen', 'Katene', 'Lisa', 'Melanie', 'Melinda', 'Michaela', 'Michelle', 'Natasha', 'Nicki', 'Nina', 'Pamela', 'Rachel', 'Samantha', 'Sandra', 'Sharon', 'Sheree', 'Tanya', 'Tina', 'Toni', 'Victoria', 'Wanna', 'Yasmin')
                                THEN
         (
             SELECT GenderId
             FROM wsac.fish.tblGender
             WHERE Gender = 'Female'
         )
                                ELSE
         (
             SELECT GenderId
             FROM wsac.fish.tblGender
             WHERE Gender = 'Male'
         )
                            END
         FROM Member
         WHERE FirstName = @firstname;
         RETURN @GenderId;
     END;