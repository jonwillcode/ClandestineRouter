using ClandestineRouter.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClandestineRouter.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<BehaviorType> BehaviorTypes { get; set; }
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<EncounterType> EncounterTypes { get; set; }
        public DbSet<InboundContent> InboundContents { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<PersonaAssociation> PersonaAssociations { get; set; }
        public DbSet<SocialMediaAccount> SocialMediaAccounts { get; set; }
        public DbSet<SocialMediaAccountLink> SocialMediaAccountLinks { get; set; }
        public DbSet<SocialMediaApp> SocialMediaApps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureLookupEntity<SocialMediaApp>(builder);
            ConfigureLookupEntity<EncounterType>(builder);
            ConfigureLookupEntity<BehaviorType>(builder);

            builder.Entity<Encounter>()
                .HasMany(e => e.BeginBehaviorType)
                .WithMany(b => b.EncountersBegin)
                .UsingEntity(j => j.ToTable("EncounterBeginBehaviorType"));

            builder.Entity<Encounter>()
                .HasMany(e => e.EndBehaviorType)
                .WithMany(b => b.EncountersEnd)
                .UsingEntity(j => j.ToTable("EncounterEndBehaviorType"));

            builder.Entity<Encounter>()
                .HasMany(e => e.BeginBehaviorType)
                .WithMany(b => b.EncountersBegin)
                .UsingEntity(j => j.ToTable("EncounterBeginBehaviorType"));

            builder.Entity<Encounter>()
                .HasMany(e => e.EndBehaviorType)
                .WithMany(b => b.EncountersEnd)
                .UsingEntity(j => j.ToTable("EncounterEndBehaviorType"));

            // Configure PersonaAssociation as the join table
            builder.Entity<PersonaAssociation>()
                .HasKey(pa => new { pa.BasePersonaId, pa.AssociatePersonaId }); // Composite primary key

            builder.Entity<PersonaAssociation>()
                .HasOne(pa => pa.BasePersona)
                .WithMany()
                .HasForeignKey(pa => pa.BasePersonaId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PersonaAssociation>()
                .HasOne(pa => pa.AssociatePersona)
                .WithMany()
                .HasForeignKey(pa => pa.AssociatePersonaId)
                .OnDelete(DeleteBehavior.NoAction);

            // Ensure unique combination (this is automatically enforced by the composite key above)
            builder.Entity<PersonaAssociation>()
                .HasIndex(pa => new { pa.BasePersonaId, pa.AssociatePersonaId })
                .IsUnique();
        }

        private static void ConfigureLookupEntity<T>(ModelBuilder builder) where T : BaseLookupModel
        {
            builder.Entity<T>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(256);
                entity.HasIndex(e => e.Name)
                      .IsUnique();
            });
        }
    }
}
