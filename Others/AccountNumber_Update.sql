
  update rb
  set rb.AccountNumber = t.Fund+'.'+t.Dept+'.'+t.Program+'.'+t.Project+'.'+t.BaseElementObjectDetail
  from [dbo].[ReceiptBodies] rb
  inner join dbo.templates t
  on t.TemplateId = rb.TemplateId
