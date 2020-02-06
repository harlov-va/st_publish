USE arkAS;
GO

--DECLARE @Back INT;

CREATE PROC getMoneysFromContragent
@fullName NVARCHAR(100)--@startDate DATETIME, @endDate DATETIME, @typeDoc NVARCHAR(30), @statusDoc NVARCHAR(30), @countBack INT OUTPUT
AS
SELECT c.name, SUM(d.sum)
FROM h_documents AS d
INNER JOIN h_contragents AS c ON (d.contragentID = c.id)
INNER JOIN h_docTypes AS dt ON (d.docTypeID = dt.id)
WHERE c.name = @fullName AND NOT dt.code IN ( 'Act')
GROUP BY c.name
;
--SET @countBack = @@ROWCOUNT;
GO


--'20191001' AND '20191231'
--@startDate AND @endDate
