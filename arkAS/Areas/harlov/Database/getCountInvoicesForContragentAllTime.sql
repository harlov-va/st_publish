USE arkAS;
GO

CREATE PROC getCountInvoicesForContragentAllTime
@fullName NVARCHAR(100)
AS
SELECT COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_contragents AS contr ON (inv.contragentID = contr.id)
WHERE contr.name = @fullName
;
GO