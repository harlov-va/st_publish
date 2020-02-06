USE arkAS;
GO

--DECLARE @countBack INT
CREATE PROC getMailsByBackDateRecipient
@startDate DATETIME, @endDate DATETIME, @countBack INT OUTPUT--, @code NVARCHAR(30)
AS
SELECT m.uniqueCode, m.date, m.fromSender, m.toRecipient, m.description, m.trackNumber, m.backTrackNumber, m.backDateReceipt
FROM h_mails AS m
--INNER JOIN h_mailStatuses AS ms ON (m.mailStatusID = ms.id)
--INNER JOIN h_deliverySystems AS ds ON (m.deliverySystemID = ds.id)
WHERE (m.backDateReceipt BETWEEN @startDate AND @endDate)
;
SET @countBack = @@ROWCOUNT
GO

--'20191001' AND '20191231'
--@startDate AND @endDate
