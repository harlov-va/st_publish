USE arkAS;
GO

CREATE PROC getCountInvoicesForContragentMonth
@fullName NVARCHAR(100), @month INT, @year INT
AS
SELECT COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_contragents AS contr ON (inv.contragentID = contr.id)
WHERE MONTH(inv.date)= @month AND YEAR(inv.date) = @year AND contr.name = @fullName

;
GO


