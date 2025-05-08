using System;
using M3alam.Domain.Entities;
using M3alam.Identity.Infrastructure.Identity;   // ApplicationUser, ApplicationRole
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using OpenIddict.EntityFrameworkCore.Models;    // OpenIddict entity types

namespace M3alam.Infrastructure.Context
{
    public class M3alamDbContext : DbContext
    {
        public M3alamDbContext(DbContextOptions<M3alamDbContext> options)
            : base(options)
        { }

        // Your domain tables
        public DbSet<Message> Messages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Request> Requests { get; set; }

        // If needed for queries, include these but excluded from migrations
        public DbSet<Seeker> Seekers { get; set; }
        public DbSet<Provider> Providers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Helper to exclude identity tables from this context's migrations
            void Exclude<TEntity>() where TEntity : class
                => builder.Entity<TEntity>().Metadata.SetIsTableExcludedFromMigrations(true);

            // ----------------------------------------------------------------
            // 1) Exclude Identity/OpenIddict tables (mapping types to existing tables)
            // ----------------------------------------------------------------
            // ASP.NET Identity
            builder.Entity<ApplicationUser>(b => { b.ToTable("AspNetUsers"); Exclude<ApplicationUser>(); });
            builder.Entity<ApplicationRole>(b => { b.ToTable("AspNetRoles"); Exclude<ApplicationRole>(); });

            builder.Entity<IdentityUserClaim<Guid>>(b => { b.ToTable("AspNetUserClaims"); b.HasKey(x => x.Id); Exclude<IdentityUserClaim<Guid>>(); });
            builder.Entity<IdentityRoleClaim<Guid>>(b => { b.ToTable("AspNetRoleClaims"); b.HasKey(x => x.Id); Exclude<IdentityRoleClaim<Guid>>(); });
            builder.Entity<IdentityUserLogin<Guid>>(b => { b.ToTable("AspNetUserLogins"); b.HasKey(x => new { x.LoginProvider, x.ProviderKey }); Exclude<IdentityUserLogin<Guid>>(); });
            builder.Entity<IdentityUserRole<Guid>>(b => { b.ToTable("AspNetUserRoles"); b.HasKey(x => new { x.UserId, x.RoleId }); Exclude<IdentityUserRole<Guid>>(); });
            builder.Entity<IdentityUserToken<Guid>>(b => { b.ToTable("AspNetUserTokens"); b.HasKey(x => new { x.UserId, x.LoginProvider, x.Name }); Exclude<IdentityUserToken<Guid>>(); });

            // OpenIddict tables
            builder.Entity<OpenIddictEntityFrameworkCoreApplication>(b => { b.ToTable("OpenIddictApplications"); Exclude<OpenIddictEntityFrameworkCoreApplication>(); });
            builder.Entity<OpenIddictEntityFrameworkCoreScope>(b => { b.ToTable("OpenIddictScopes"); Exclude<OpenIddictEntityFrameworkCoreScope>(); });
            builder.Entity<OpenIddictEntityFrameworkCoreAuthorization>(b => { b.ToTable("OpenIddictAuthorizations"); Exclude<OpenIddictEntityFrameworkCoreAuthorization>(); });
            builder.Entity<OpenIddictEntityFrameworkCoreToken>(b => { b.ToTable("OpenIddictTokens"); Exclude<OpenIddictEntityFrameworkCoreToken>(); });

            // Seeker/Provider inheritance mapping (TPH with AspNetUsers)
            builder.Entity<Seeker>(b =>
            {
                b.HasBaseType<ApplicationUser>();
                b.ToTable("Seekers"); // TPT - this creates a separate table
                b.Property(x => x.Latitude);
                b.Property(x => x.Longitude);
                b.Property(x => x.Address).HasMaxLength(300);
            });

            // Provider
            builder.Entity<Provider>(b =>
            {
                b.HasBaseType<ApplicationUser>();
                b.ToTable("Providers"); // TPT - this creates a separate table
                b.Property(x => x.Bio).HasMaxLength(1000);
                b.Property(x => x.Experience);
                b.Property(x => x.Rating);
                b.Property(x => x.ReviewCount);
                b.Property(x => x.CompletedJobs);

                var stringArrayComparer = new ValueComparer<string[]>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)),
                    c => c.ToArray());

                b.Property(x => x.Skills)
                    .HasConversion(
                        v => string.Join(',', v ?? Array.Empty<string>()),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .HasMaxLength(1000)
                    .Metadata.SetValueComparer(stringArrayComparer);

                b.Property(x => x.Availability)
                    .HasConversion(
                        v => string.Join(',', v ?? Array.Empty<string>()),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .HasMaxLength(1000)
                    .Metadata.SetValueComparer(stringArrayComparer);
            });

            // ----------------------------------------------------------------

            // Request
            builder.Entity<Request>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Title).IsRequired().HasMaxLength(200);
                b.Property(x => x.ServiceType).IsRequired().HasMaxLength(100);
                b.Property(x => x.Address).IsRequired().HasMaxLength(300);
                b.Property(x => x.Status).IsRequired();
                b.Property(x => x.PreferredTime).IsRequired();

                b.HasMany(x => x.Messages)
                 .WithOne()
                 .HasForeignKey(x => x.RequestId)
                 .OnDelete(DeleteBehavior.Cascade); // Ensure EF picks up collection
            });

            // Message
            builder.Entity<Message>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Content).IsRequired().HasMaxLength(1000);
                b.Property(x => x.Timestamp).IsRequired();
                b.Property(x => x.RequestId).IsRequired();
                b.Property(x => x.SenderId).IsRequired();
            });

            // Rating
            builder.Entity<Rating>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Score).IsRequired();
                b.Property(x => x.Comment).HasMaxLength(500);
                b.Property(x => x.RequestId).IsRequired();
                b.Property(x => x.ProviderId).IsRequired();
                b.Property(x => x.SeekerId).IsRequired();

                // Relationships to AspNetUsers without multiple cascade paths
                b.HasOne<Provider>()
                 .WithMany(p => p.RatingsReceived)
                 .HasForeignKey(r => r.ProviderId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne<Seeker>()
                 .WithMany(s => s.RatingsGiven)
                 .HasForeignKey(r => r.SeekerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
