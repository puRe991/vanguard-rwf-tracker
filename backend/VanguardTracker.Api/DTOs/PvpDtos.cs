namespace VanguardTracker.Api.DTOs;

public record PvpLadderEntryDto(
    int Rank,
    Guid Id,
    string Name,
    string Region,
    string Bracket, // "2v2" | "3v3" | "rbg" | "solo-shuffle"
    int Rating,
    string Tier, // "Combatant" | "Challenger" | "Rival" | "Duelist" | "Elite" | "Gladiator"
    List<string> Players,
    DateTimeOffset UpdatedAt
);
