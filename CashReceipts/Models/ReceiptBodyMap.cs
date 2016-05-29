using System.Data.Entity.ModelConfiguration;

namespace CashReceipts.Models
{
    internal class ReceiptBodyMap : EntityTypeConfiguration<ReceiptBody>
    {
        public ReceiptBodyMap()
        {
            //HasMany<Tender>(b => b.Tenders)
            //.WithMany(t => t.ReceiptBodies)
            //.Map(cs =>
            //{
            //    cs.MapLeftKey("ReceiptBodyID");
            //    cs.MapRightKey("TenderID");
            //    cs.ToTable("ReceiptBodiesTenders");
            //});

        }
    }
}