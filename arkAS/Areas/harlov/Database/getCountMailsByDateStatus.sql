USE arkAS;
GO

CREATE PROC getCountMailsByDateStatus
@startDate DATETIME, @endDate DATETIME , @code NVARCHAR(30)
AS
SELECT COUNT(*)
FROM h_mails AS m
INNER JOIN h_mailStatuses AS ms ON (m.mailStatusID = ms.id)
INNER JOIN h_deliverySystems AS ds ON (m.deliverySystemID = ds.id)
WHERE (m.date BETWEEN @startDate AND @endDate) AND (ms.code = @code)

;
GO
