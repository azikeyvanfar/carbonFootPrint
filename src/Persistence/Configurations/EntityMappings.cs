using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;


namespace ContractorBackend.Persistence.Configurations
{
    public static partial class EntityMappings
    {
        public static void AddCustomEntityMappings(this ModelBuilder modelBuilder)
        {


            #region Authorization & Menu
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasIndex(e => e.OwnerId, "FK_Menus_AppUsers_OwnerId");
                entity.Property(x => x.Name).HasMaxLength(1000);
                entity.Property(x => x.Title).HasMaxLength(1000);

                entity.HasOne(x => x.Menu)
                .WithMany(x => x.MenuItems)
                .HasForeignKey(x => x.MenuId);

                entity.HasOne(x => x.ParentMenuItem)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId);

                entity.HasOne(x => x.Owner)
                .WithMany(x => x.MenuItems)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.PageRoute)
               .WithMany(x => x.MenuItems)
               .HasForeignKey(x => x.PageRouteId)
               .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RoleClaim>(entity =>
            {
                entity.HasOne(x => x.PageRouteClaim)
               .WithMany(x => x.RoleClaims)
               .HasForeignKey(x => x.PageRouteClaimId)
               .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasIndex(e => e.OwnerId, "FK_MenuItems_AppUsers_OwnerId");
                entity.Property(x => x.Title).HasMaxLength(1000).IsRequired();
                entity.Property(x => x.Name).HasMaxLength(1000).IsRequired();

                entity.HasOne(x => x.Owner)
                .WithMany(x => x.Menus)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<PageRouteClaim>(entity =>
            {
                entity.HasOne(x => x.GeneralClaim).
                WithMany(x => x.PageRouteClaims).
                HasForeignKey(x => x.GeneralClaimsId).
                OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.PageRoute).
               WithMany(x => x.PageRouteClaims).
               HasForeignKey(x => x.PageRouteId).
               OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<RolePageRouteAccess>(entity =>
            {

                entity.HasOne(x => x.Role).
                WithMany(x => x.RoleAccessItems).
                HasForeignKey(x => x.RoleId).
                OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(x => x.PageRoute)
                .WithMany(x => x.RolePageRouteAccesses)
                .HasForeignKey(c => c.PageRouteId)
                .OnDelete(DeleteBehavior.NoAction);
            });
            #endregion


            #region Shared
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasIndex(e => e.ParentId);

                entity.HasOne(p => p.Parent)
                  .WithMany(q => q!.Children)
                  .HasForeignKey(p => p.ParentId);

                entity.HasOne(x => x.Owner)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                ;
            });

            modelBuilder.Entity<SmsHistory>(entity =>
            {
                entity.HasOne(x => x.User).
                WithMany(x => x.SmsHistories).
                HasForeignKey(x => x.UserId).
                OnDelete(DeleteBehavior.NoAction)
                ;
            });
            #endregion

        }
    }
}
