USE [arkAS]
GO

CREATE PROC getBillsAllTimeByCode
@code NVARCHAR(30)
AS
SELECT MONTH(inv.date),YEAR(inv.date),COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (ins.code = @code)
GROUP BY MONTH(inv.date),YEAR(inv.date)
ORDER BY YEAR(inv.date)
;
