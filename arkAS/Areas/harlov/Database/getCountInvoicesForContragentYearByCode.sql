USE arkAS;
GO

CREATE PROC getCountInvoicesForContragentYearByCode
@fullName NVARCHAR(100), @year INT , @code NVARCHAR(30)
AS
SELECT COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_contragents AS contr ON (inv.contragentID = contr.id)
WHERE YEAR(inv.date) = @year AND contr.name = @fullName AND inv.invStatusID IN (
	SELECT ins.id
	FROM h_invoiceStatuses AS ins
	WHERE ins.code = @code
	)

;
GO


