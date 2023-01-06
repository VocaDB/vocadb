#nullable disable

namespace VocaDb.Model.Domain.Albums;

/// <summary>
/// Represents a combination of disc number and track number,
/// uniquely identifying a track position in an album.
/// </summary>
public readonly struct TrackIndex
{
	public static TrackIndex Empty => new TrackIndex(0, 0);

	public TrackIndex(int discNum, int trackNum)
		: this()
	{
		DiscNumber = discNum;
		TrackNumber = trackNum;
	}

	public int DiscNumber { get; }

	public int TrackNumber { get; }

#nullable enable
	public bool Equals(TrackIndex obj)
	{
		return DiscNumber == obj.DiscNumber && TrackNumber == obj.TrackNumber;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		return obj is TrackIndex && Equals((TrackIndex)obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			return (TrackNumber * 397) ^ DiscNumber;
		}
	}

	public override string ToString() => $"{DiscNumber}.{TrackNumber}";
#nullable disable

	public static bool operator ==(TrackIndex left, TrackIndex right) => left.Equals(right);

	public static bool operator !=(TrackIndex left, TrackIndex right) => !left.Equals(right);
}
