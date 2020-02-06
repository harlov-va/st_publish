USE arkAS;
GO

--DECLARE @Back INT;

CREATE PROC getSupplementaryAgreementByNumber
@number NVARCHAR(30)--, @startDate DATETIME, @endDate DATETIME, @countBack INT OUTPUT--, @typeDoc NVARCHAR(30), @statusDoc NVARCHAR(30)
AS
SELECT d.uniqueCode, d.date,d.number, d.sum, d.description,  d.link
FROM h_documents AS d
--INNER JOIN h_contragents AS c ON (d.contragentID = c.id)
--INNER JOIN h_docTypes AS dt ON (d.docTypeID = dt.id)
WHERE d.docParentID IN (SELECT dp.id
						FROM h_documents AS dp
						WHERE dp.number = @number)
;
--SET @countBack = @@ROWCOUNT;
GO


--'20191001' AND '20191231'
--@startDate AND @endDate
