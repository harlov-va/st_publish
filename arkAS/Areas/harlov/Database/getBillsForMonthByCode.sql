USE [arkAS]
GO

CREATE PROC getBillsForMonthByCode
@month INT, @year INT, @code NVARCHAR(30)
AS
SELECT inv.id, inv.uniqueCode, inv.date, inv.number, inv.description
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (MONTH(inv.date) = @month AND (YEAR(inv.date) = @year) AND (ins.code=@code))
;
