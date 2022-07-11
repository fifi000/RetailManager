CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id int = 0,
	@CashierId nvarchar(128),
	@SalesDate datetime2,
	@SubTotal money,
	@Tax money,
	@Total money
AS
begin
	set nocount on;

	insert into Sale(CashierId, SalesDate, SubTotal, Tax, Total)
	values (@CashierId, @SalesDate, @SubTotal, @Tax, @Total);

	select cast(SCOPE_IDENTITY() as int);
	
end
