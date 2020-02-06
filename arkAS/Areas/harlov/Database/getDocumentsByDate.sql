USE arkAS;
GO

--DECLARE @Back INT;

CREATE PROC getDocumentsByDate
@startDate DATETIME, @endDate DATETIME, @typeDoc NVARCHAR(30), @statusDoc NVARCHAR(30), @countBack INT OUTPUT
AS
SELECT d.uniqueCode , d.date , d.number , d.sum , d.description , d.link, dt.name, ds.name
FROM h_documents AS d
INNER JOIN h_docTypes AS dt ON (d.docTypeID = dt.id)
INNER JOIN h_docStatuses AS ds ON (d.docStatusID = ds.id)
WHERE (d.date BETWEEN @startDate AND @endDate) AND dt.code = @typeDoc AND ds.code = @statusDoc
;
SET @countBack = @@ROWCOUNT;
GO

--'20191001' AND '20191231'
--@startDate AND @endDate
