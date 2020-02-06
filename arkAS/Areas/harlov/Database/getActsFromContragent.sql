USE arkAS;
GO

--DECLARE @Back INT;

CREATE PROC getActsFromContragent
@fullName NVARCHAR(100), @startDate DATETIME, @endDate DATETIME, @countBack INT OUTPUT--, @typeDoc NVARCHAR(30), @statusDoc NVARCHAR(30)
AS
SELECT c.name, d.link, d.date,d.number, d.sum, d.description
FROM h_documents AS d
INNER JOIN h_contragents AS c ON (d.contragentID = c.id)
INNER JOIN h_docTypes AS dt ON (d.docTypeID = dt.id)
WHERE c.name = @fullName AND dt.code = 'Act' AND d.date BETWEEN @startDate AND @endDate
--GROUP BY c.name
;
SET @countBack = @@ROWCOUNT;
GO


--'20191001' AND '20191231'
--@startDate AND @endDate
