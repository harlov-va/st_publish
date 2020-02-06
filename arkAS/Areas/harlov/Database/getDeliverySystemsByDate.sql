USE arkAS;
GO

CREATE PROC getDeliverySystemsByDate
@startDate DATETIME, @endDate DATETIME --, @code NVARCHAR(30)
AS
SELECT ds.name, COUNT(*)
FROM h_mails AS m
--INNER JOIN h_mailStatuses AS ms ON (m.mailStatusID = ms.id)
INNER JOIN h_deliverySystems AS ds ON (m.deliverySystemID = ds.id)
WHERE (m.date BETWEEN @startDate AND @endDate)
GROUP BY ds.name
;
GO

--'20191001' AND '20191231'
