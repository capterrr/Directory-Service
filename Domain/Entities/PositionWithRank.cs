using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class PositionWithRank
{
    public Position Position { get; }
    public Rank Rank { get; }

    public PositionWithRank(Position position, Rank rank)
    {
        Position = position ?? throw new ArgumentNullException(nameof(position));
        Rank = rank ?? throw new ArgumentNullException(nameof(rank));
    }

    public PositionWithRank WithPosition(Position position)
    {
        return new PositionWithRank(position, Rank);
    }

    public PositionWithRank WithRank(Rank rank)
    {
        return new PositionWithRank(Position, rank);
    }
}