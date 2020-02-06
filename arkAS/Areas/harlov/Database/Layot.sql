USE [arkAS]
GO

CREATE PROC getBillsForYearByCode
@year INT, @code NVARCHAR(30)
AS
SELECT inv.id, inv.uniqueCode, inv.date, inv.number, inv.description
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE ((YEAR(inv.date) = @year) AND (ins.code = @code))
;

INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (contr.name = @fullName AND ins.code = @code)
