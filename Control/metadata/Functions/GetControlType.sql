-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE FUNCTION [metadata].[GetControlType]
(
	-- Add the parameters for the function here
@pWindowControlTypeId INT
)
RETURNS VARCHAR(50)
AS
     BEGIN
	-- Declare the return variable here
         DECLARE @Result VARCHAR(50);

	-- Add the T-SQL statements to compute the return value here
         SELECT @Result = WindowControlType
         FROM control.metadata.WindowControlType
         WHERE WindowControlType = CASE @pWindowControlTypeId
                                       WHEN 1
                                       THEN 'COMBO'
                                       WHEN 2
                                       THEN 'ROWSOURCE'
                                       WHEN 3
                                       THEN 'FILTER'
                                       WHEN 4
                                       THEN 'ORDERBY'
                                   END;

	-- Return the result of the function
         RETURN @Result;
     END;

GO
GRANT EXECUTE
    ON OBJECT::[metadata].[GetControlType] TO [cUser]
    AS [dbo];

