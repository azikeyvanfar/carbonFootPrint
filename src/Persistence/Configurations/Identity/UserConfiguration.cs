using ContractorBackend.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations.Identity
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //builder.HasOne(u => u.Employee)
            //    .WithOne(e => e.User)
            //    .HasForeignKey<User>(u => u.EmployeeId)
            //    .OnDelete(DeleteBehavior.NoAction);


            builder.Property(e => e.PersonnelCode).HasMaxLength(450);
            builder.Property(e => e.UpLevel).HasMaxLength(450);
            builder.Property(e => e.NationalCode).HasMaxLength(450);
            builder.Property(e => e.LastNameEng).HasMaxLength(450);
            builder.Property(e => e.OTP).HasMaxLength(450);
            builder.Property(e => e.FirstNameEng).HasMaxLength(450);
            builder.Property(e => e.PasswordHash).HasMaxLength(5000);
            builder.Property(e => e.SecurityStamp).HasMaxLength(1000);
            builder.Property(e => e.ConcurrencyStamp).HasMaxLength(1000);
            builder.Property(e => e.PhoneNumber).HasMaxLength(450);


            builder.ToTable("AppUsers");
        }
    }
}