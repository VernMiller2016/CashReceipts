select ActNumbr_1, ActNumbr_2, ActNumbr_3, count(*) from dbo.GL00100 
group by ActNumbr_1, ActNumbr_2, ActNumbr_3
having LEN(ActNumbr_1)>3 and  count(*) > 1