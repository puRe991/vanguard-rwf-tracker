using Microsoft.EntityFrameworkCore;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Data;

public class VanguardDbContext(DbContextOptions<VanguardDbContext> options) : DbContext(options)
{
    public DbSet<Expansion> Expansions => Set<Expansion>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Raid> Raids => Set<Raid>();
    public DbSet<Boss> Bosses => Set<Boss>();
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<Kill> Kills => Set<Kill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Season>()
            .HasOne(s => s.Expansion)
            .WithMany(e => e.Seasons)
            .HasForeignKey(s => s.ExpansionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Raid>()
            .HasOne(r => r.Season)
            .WithMany(s => s.Raids)
            .HasForeignKey(r => r.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Boss>()
            .HasOne(b => b.Raid)
            .WithMany(r => r.Bosses)
            .HasForeignKey(b => b.RaidId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Boss>()
            .HasIndex(b => new { b.RaidId, b.Order })
            .IsUnique();

        modelBuilder.Entity<Kill>()
            .HasOne(k => k.Boss)
            .WithMany(b => b.Kills)
            .HasForeignKey(k => k.BossId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Kill>()
            .HasOne(k => k.Guild)
            .WithMany(g => g.Kills)
            .HasForeignKey(k => k.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Kill>()
            .HasIndex(k => new { k.BossId, k.GuildId });

        modelBuilder.Entity<Guild>()
            .HasIndex(g => g.Name);
    }
}
