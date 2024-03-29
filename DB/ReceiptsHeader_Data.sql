CREATE PROCEDURE Getreceipts(@startDate DATE, 
                             @endDate   DATE) 
AS 
  BEGIN 
      SELECT [receiptnumber]                 AS [Receipt Number], 
             [receiptdate]                   AS [Receipt Date], 
             d.NAME                          AS [Department], 
             c.lastname + ', ' + c.firstname AS [Clerk], 
             [receipttotal]                  AS [Receipt Total], 
             COALESCE([comments], '')        AS [Received From], 
             COALESCE([receivedfor], '')     AS [Received For] 
      FROM   [dbo].[receiptheaders] rh 
             INNER JOIN dbo.clerks c 
                     ON c.clerkid = rh.clerkid 
             INNER JOIN dbo.departments d 
                     ON rh.departmentid = d.departmentid 
      WHERE  rh.isdeleted = 0 
             AND CONVERT(DATE, rh.receiptdate) BETWEEN @startDate AND @endDate 

      SELECT rh.receiptnumber                  AS [Receipt Number], 
             t.fund + '.' + t.dept + '.' + t.program + '.' + t.project 
             + '.' + t.baseelementobjectdetail AS [Account Number], 
             rb.linetotal                      AS [Line Total], 
             rb.accountdescription             AS [Template] 
      FROM   dbo.receiptbodies rb 
             INNER JOIN dbo.templates t 
                     ON t.templateid = rb.templateid 
             INNER JOIN dbo.receiptheaders rh 
                     ON rh.receiptheaderid = rb.receiptheaderid 
      WHERE  rh.isdeleted = 0 
             AND CONVERT(DATE, rh.receiptdate) BETWEEN @startDate AND @endDate 
      ORDER  BY rh.receiptnumber, 
                [account number] 

      SELECT rh.receiptnumber, 
             pm.NAME, 
             t.[description], 
             t.amount 
      FROM   dbo.tenders t 
             INNER JOIN dbo.receiptheaders rh 
                     ON rh.receiptheaderid = t.receiptheaderid 
             INNER JOIN [dbo].[paymentmethods] pm 
                     ON pm.id = t.paymentmethodid 
      WHERE  rh.isdeleted = 0 
             AND CONVERT(DATE, rh.receiptdate) BETWEEN @startDate AND @endDate 
      ORDER  BY rh.receiptnumber 
  END 
--exec GetReceipts '2016-06-24', '2016-09-15' 