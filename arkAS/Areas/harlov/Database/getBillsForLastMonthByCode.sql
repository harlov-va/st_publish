USE [arkAS]
GO

CREATE PROC getBillsForLastMonthByCode
@code NVARCHAR(30)
AS
SELECT inv.id, inv.uniqueCode, inv.date, inv.number, inv.description
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (MONTH(inv.date) = MONTH(GETDATE()) AND (YEAR(inv.date) = YEAR(GETDATE())) AND (ins.code = @code))
;
