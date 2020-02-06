USE [core]
GO
/****** Object:  StoredProcedure [dbo].[getActsFromContragent]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--DECLARE @Back INT;

CREATE PROC [dbo].[getActsFromContragent]
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
/****** Object:  StoredProcedure [dbo].[getBillsAllTime]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsAllTime]
AS
SELECT MONTH(inv.date),YEAR(inv.date),COUNT(*)
FROM h_invoices AS inv
GROUP BY MONTH(inv.date),YEAR(inv.date)
ORDER BY YEAR(inv.date)
;
GO
/****** Object:  StoredProcedure [dbo].[getBillsAllTimeByCode]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsAllTimeByCode]
@code NVARCHAR(30)
AS
SELECT MONTH(inv.date),YEAR(inv.date),COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (ins.code = @code)
GROUP BY MONTH(inv.date),YEAR(inv.date)
ORDER BY YEAR(inv.date)
;
GO
/****** Object:  StoredProcedure [dbo].[getBillsForLastMonth]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsForLastMonth]
AS
SELECT *
FROM h_invoices AS inv
WHERE (MONTH(inv.date) = MONTH(GETDATE()) AND (YEAR(inv.date) = YEAR(GETDATE())))
;
GO
/****** Object:  StoredProcedure [dbo].[getBillsForLastMonthByCode]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsForLastMonthByCode]
@code NVARCHAR(30)
AS
SELECT inv.id, inv.uniqueCode, inv.date, inv.number, inv.description
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (MONTH(inv.date) = MONTH(GETDATE()) AND (YEAR(inv.date) = YEAR(GETDATE())) AND (ins.code = @code))
;
GO
/****** Object:  StoredProcedure [dbo].[getBillsForLastYear]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsForLastYear]
--@month INT, @year INT
AS
SELECT *
FROM h_invoices AS inv
WHERE (YEAR(inv.date) = YEAR(GETDATE()))
;
GO
/****** Object:  StoredProcedure [dbo].[getBillsForLastYearByCode]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsForLastYearByCode]
@code NVARCHAR(30)
AS
SELECT inv.id, inv.uniqueCode, inv.date, inv.number, inv.description
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (YEAR(inv.date) = YEAR(GETDATE()) AND (ins.code = @code))
;
GO
/****** Object:  StoredProcedure [dbo].[getBillsForMonth]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsForMonth]
@month INT, @year INT
AS
SELECT *
FROM h_invoices AS inv
WHERE (MONTH(inv.date) = @month AND (YEAR(inv.date) = @year))
;
GO
/****** Object:  StoredProcedure [dbo].[getBillsForMonthByCode]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsForMonthByCode]
@month INT, @year INT, @code NVARCHAR(30)
AS
SELECT inv.id, inv.uniqueCode, inv.date, inv.number, inv.description
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (MONTH(inv.date) = @month AND (YEAR(inv.date) = @year) AND (ins.code=@code))
;
GO
/****** Object:  StoredProcedure [dbo].[getBillsForYear]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsForYear]
@year INT --@month INT,
AS
SELECT *
FROM h_invoices AS inv
WHERE (YEAR(inv.date) = @year)
;
GO
/****** Object:  StoredProcedure [dbo].[getBillsForYearByCode]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getBillsForYearByCode]
@year INT, @code NVARCHAR(30)
AS
SELECT inv.id, inv.uniqueCode, inv.date, inv.number, inv.description
FROM h_invoices AS inv
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE ((YEAR(inv.date) = @year) AND (ins.code = @code))
;
GO
/****** Object:  StoredProcedure [dbo].[getCountInvoicesForContragentAllTime]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getCountInvoicesForContragentAllTime]
@fullName NVARCHAR(100)
AS
SELECT COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_contragents AS contr ON (inv.contragentID = contr.id)
WHERE contr.name = @fullName
;
GO
/****** Object:  StoredProcedure [dbo].[getCountInvoicesForContragentAllTimeByCode]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getCountInvoicesForContragentAllTimeByCode]
@fullName NVARCHAR(100), @code NVARCHAR(30)
AS
SELECT COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_contragents AS contr ON (inv.contragentID = contr.id)
INNER JOIN h_invoiceStatuses AS ins ON (inv.invStatusID = ins.id)
WHERE (contr.name = @fullName AND ins.code = @code)

;
GO
/****** Object:  StoredProcedure [dbo].[getCountInvoicesForContragentMonth]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getCountInvoicesForContragentMonth]
@fullName NVARCHAR(100), @month INT, @year INT
AS
SELECT COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_contragents AS contr ON (inv.contragentID = contr.id)
WHERE MONTH(inv.date)= @month AND YEAR(inv.date) = @year AND contr.name = @fullName

;
GO
/****** Object:  StoredProcedure [dbo].[getCountInvoicesForContragentMonthByCode]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getCountInvoicesForContragentMonthByCode]
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
/****** Object:  StoredProcedure [dbo].[getCountInvoicesForContragentYear]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getCountInvoicesForContragentYear]
@fullName NVARCHAR(100), @year INT --, @code NVARCHAR(30)
AS
SELECT COUNT(*)
FROM h_invoices AS inv
INNER JOIN h_contragents AS contr ON (inv.contragentID = contr.id)
WHERE YEAR(inv.date) = @year AND contr.name = @fullName 

;
GO
/****** Object:  StoredProcedure [dbo].[getCountInvoicesForContragentYearByCode]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getCountInvoicesForContragentYearByCode]
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
/****** Object:  StoredProcedure [dbo].[getCountMailsByDateStatus]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getCountMailsByDateStatus]
@startDate DATETIME, @endDate DATETIME , @code NVARCHAR(30)
AS
SELECT COUNT(*)
FROM h_mails AS m
INNER JOIN h_mailStatuses AS ms ON (m.mailStatusID = ms.id)
INNER JOIN h_deliverySystems AS ds ON (m.deliverySystemID = ds.id)
WHERE (m.date BETWEEN @startDate AND @endDate) AND (ms.code = @code)

;
GO
/****** Object:  StoredProcedure [dbo].[getDeliverySystemsByDate]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getDeliverySystemsByDate]
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
/****** Object:  StoredProcedure [dbo].[getDocumentsByDate]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--DECLARE @Back INT;

CREATE PROC [dbo].[getDocumentsByDate]
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
/****** Object:  StoredProcedure [dbo].[getLinkesFromContragent]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--DECLARE @Back INT;

CREATE PROC [dbo].[getLinkesFromContragent]
@fullName NVARCHAR(100), @codeDoc NVARCHAR(30)--@startDate DATETIME, @endDate DATETIME, @typeDoc NVARCHAR(30), @statusDoc NVARCHAR(30), @countBack INT OUTPUT
AS
SELECT c.name, d.link
FROM h_documents AS d
INNER JOIN h_contragents AS c ON (d.contragentID = c.id)
INNER JOIN h_docTypes AS dt ON (d.docTypeID = dt.id)
WHERE c.name = @fullName AND dt.code = @codeDoc
--GROUP BY c.name
;
--SET @countBack = @@ROWCOUNT;
GO
/****** Object:  StoredProcedure [dbo].[getMailsByBackDateRecipient]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--DECLARE @countBack INT
CREATE PROC [dbo].[getMailsByBackDateRecipient]
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
/****** Object:  StoredProcedure [dbo].[getMailsByDateStatus]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getMailsByDateStatus]
@startDate DATETIME, @endDate DATETIME , @code NVARCHAR(30)
AS
SELECT m.uniqueCode, m.date, m.fromSender, m.toRecipient, m.description, m.trackNumber, ms.name, ds.name
FROM h_mails AS m
INNER JOIN h_mailStatuses AS ms ON (m.mailStatusID = ms.id)
INNER JOIN h_deliverySystems AS ds ON (m.deliverySystemID = ds.id)
WHERE (m.date BETWEEN @startDate AND @endDate) AND (ms.code = @code)

;
GO
/****** Object:  StoredProcedure [dbo].[getMoneysFromContragent]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--DECLARE @Back INT;

CREATE PROC [dbo].[getMoneysFromContragent]
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
/****** Object:  StoredProcedure [dbo].[getRecipientByDate]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getRecipientByDate]
@startDate DATETIME, @endDate DATETIME --, @code NVARCHAR(30)
AS
SELECT m.toRecipient, ms.name
FROM h_mails AS m
INNER JOIN h_mailStatuses AS ms ON (m.mailStatusID = ms.id)
INNER JOIN h_deliverySystems AS ds ON (m.deliverySystemID = ds.id)
WHERE (m.date BETWEEN @startDate AND @endDate)

;
GO
/****** Object:  StoredProcedure [dbo].[getSupplementaryAgreementByNumber]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--DECLARE @Back INT;

CREATE PROC [dbo].[getSupplementaryAgreementByNumber]
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
/****** Object:  StoredProcedure [dbo].[getTrackNumberByDate]    Script Date: 24.04.2019 13:09:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[getTrackNumberByDate]
@startDate DATETIME, @endDate DATETIME --, @code NVARCHAR(30)
AS
SELECT m.trackNumber
FROM h_mails AS m
--INNER JOIN h_mailStatuses AS ms ON (m.mailStatusID = ms.id)
--INNER JOIN h_deliverySystems AS ds ON (m.deliverySystemID = ds.id)
WHERE (m.date BETWEEN @startDate AND @endDate)
--GROUP BY ds.name
;
GO
