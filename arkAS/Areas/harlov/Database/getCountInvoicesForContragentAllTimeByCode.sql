USE arkAS;
GO

CREATE PROC getCountInvoicesForContragentAllTimeByCode
@fullName NVARCHAR(100), @code NVARCHAR(30)
AS
SELECT COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_contragents AS contr ON (inv.contragentID = contr.id)
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (contr.name = @fullName AND ins.code = @code)

;
GO


