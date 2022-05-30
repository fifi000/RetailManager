CREATE PROCEDURE [dbo].[spUserLookup]
	@Id nvarchar(128)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id, FirstName, LastName, EmailAddress, CreatedDate
	FROM dbo.[User] U
	WHERE U.Id = @Id
END