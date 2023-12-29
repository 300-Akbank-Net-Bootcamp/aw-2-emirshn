using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vb.Base.Entity;

namespace Vb.Data.Entity;

[Table("EftTransaction", Schema = "dbo")]
public class EftTransaction : BaseEntity
{
    public int AccountId { get; set; }
    public virtual Account Account { get; set; }
    
    public string ReferenceNumber { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    
    public string SenderAccount { get; set; }
    public string SenderIban { get; set; }
    public string SenderName { get; set; }
}
public class EftTransactionConfiguration : IEntityTypeConfiguration<EftTransaction>
{
    public void Configure(EntityTypeBuilder<EftTransaction> builder)
    {
        builder.Property(x => x.InsertDate).IsRequired(true);
        builder.Property(x => x.InsertUserId).IsRequired(true);
        builder.Property(x => x.UpdateDate).IsRequired(false);
        builder.Property(x => x.UpdateUserId).IsRequired(false);
        builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

        builder.Property(x => x.AccountId).IsRequired(true);
        builder.Property(x => x.TransactionDate).IsRequired(true);
        builder.Property(x => x.Amount).IsRequired(true).HasPrecision(18, 4);
        builder.Property(x => x.Description).IsRequired(false).HasMaxLength(300);
        builder.Property(x => x.ReferenceNumber).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.SenderAccount).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.SenderIban).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.SenderName).IsRequired(true).HasMaxLength(50);
        
        builder.HasIndex(x => x.ReferenceNumber);
    }
}