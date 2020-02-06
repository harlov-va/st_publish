USE [arkAS]
GO

CREATE PROC getBillsForLastYearByCode
@code NVARCHAR(30)
AS
SELECT inv.id, inv.uniqueCode, inv.date, inv.number, inv.description
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (YEAR(inv.date) = YEAR(GETDATE()) AND (ins.code = @code))
;
