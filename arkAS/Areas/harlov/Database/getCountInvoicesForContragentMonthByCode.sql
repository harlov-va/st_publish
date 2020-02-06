USE arkAS;
GO

CREATE PROC getCountInvoicesForContragentMonthByCode
@fullName NVARCHAR(100), @month INT, @year INT, @code NVARCHAR(30)
AS
SELECT COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_contragents AS contr ON (inv.contragentID = contr.id)
WHERE MONTH(inv.date)= @month AND YEAR(inv.date) = @year AND contr.name = @fullName AND inv.invStatusID IN (
	SELECT ins.id
	FROM h_invoiceStatuses AS ins
	WHERE ins.code = @code
	)

;
GO


