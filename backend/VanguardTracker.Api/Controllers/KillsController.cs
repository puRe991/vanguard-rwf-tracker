using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanguardTracker.Api.Data;
using VanguardTracker.Api.DTOs;
using VanguardTracker.Api.Models;

namespace VanguardTracker.Api.Controllers;

[ApiController]
[Route("api/kills")]
public class KillsController(VanguardDbContext db) : ControllerBase
{
    /// <summary>
    /// Community-Beitrag für historische Kills (Classic bis WotLK ohne API-Abdeckung).
    /// Landet als unbestätigt und durchläuft den Moderations-Workflow vor Veröffentlichung.
    /// </summary>
    [HttpPost("submit")]
    [Authorize]
    public async Task<ActionResult<Guid>> Submit(SubmitKillRequest request, CancellationToken ct)
    {
        var kill = new Kill
        {
            Id = Guid.NewGuid(),
            BossId = request.BossId,
            GuildId = request.GuildId,
            Timestamp = request.Timestamp,
            PullCount = request.PullCount,
            SourceUrl = request.SourceUrl,
            Status = KillStatus.Unconfirmed,
        };

        db.Kills.Add(kill);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(Submit), new { id = kill.Id }, kill.Id);
    }
}
