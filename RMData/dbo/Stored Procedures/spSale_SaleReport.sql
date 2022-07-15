CREATE PROCEDURE [dbo].[spSale_SaleReport]
as
begin 
	set nocount on;

	select 
		[s].[SalesDate], [s].[SubTotal], [s].[Tax], [s].[Total], 
		[u].[FirstName], [u].[LastName], [u].[EmailAddress]
	from dbo.Sale s
	join dbo.[User] u on s.CashierId = u.Id
end
