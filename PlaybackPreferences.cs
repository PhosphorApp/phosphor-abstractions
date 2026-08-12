namespace Phosphor.Plugin.Abstractions;

/// <summary>
/// The caller's playback preferences passed to resolve/download. Sources honor what they
/// can and ignore the rest.
/// </summary>
public sealed record PlaybackPreferences
{
    /// <summary>Quality ceiling the source should not exceed when picking a stream.</summary>
    public VideoQuality MaxQuality { get; init; } = VideoQuality.High;

    /// <summary>When true, avoid surround audio tracks in favor of stereo.</summary>
    public bool PreferStereo { get; init; }

    /// <summary>When true, resolve an audio-only stream even for a video item.</summary>
    public bool AudioOnly { get; init; }
}

/// <summary>Coarse quality ceiling, mapped by each source to its own format ladder.</summary>
public enum VideoQuality
{
    Low,
    Medium,
    High,
    Max,
}

/// <summary>
/// Raw downloaded media for the host's disk cache, produced by <see cref="IDownloadable"/>.
/// When the source produced separate tracks, both <see cref="VideoFilePath"/> and
/// <see cref="AudioFilePath"/> are set and the host muxes them; for a muxed/audio-only
/// download only the relevant path is set. Containers are reported so the host can name /
/// remux correctly.
/// </summary>
public sealed record SourceDownload
{
    public string? VideoFilePath { get; init; }
    public string? AudioFilePath { get; init; }
    public string? VideoContainer { get; init; }
    public string? AudioContainer { get; init; }
    public string? Resolution { get; init; }
}

/// <summary>Item metadata fetched on demand (never during search) for enrichment.</summary>
public sealed record SourceMetadata(
    TimeSpan? Duration,
    string? Description,
    IReadOnlyList<ChapterMarker> Chapters,
    DateTimeOffset? PublishedAt = null);

/// <summary>
/// The currently-airing track/segment on a live stream (e.g. a SiriusXM channel's now-playing
/// song), fetched on demand by the host while the stream plays. All members are optional so a
/// source can supply whatever it knows; an all-null instance means "nothing to show right now".
/// </summary>
/// <param name="Title">The track/song title, or the show/episode title for talk content.</param>
/// <param name="Artist">The performing artist(s), or the host/presenter for talk content. Null when unknown.</param>
/// <param name="Album">The album, when known. Null for talk content or when unavailable.</param>
/// <param name="NextChangeUtc">
/// When the source knows it, the wall-clock time the current track is expected to end (so the host
/// can schedule its next poll near the change instead of polling on a fixed short interval). Null
/// when unknown — the host then falls back to its default poll interval.
/// </param>
public sealed record LiveNowPlaying(
    string? Title,
    string? Artist = null,
    string? Album = null,
    DateTimeOffset? NextChangeUtc = null)
{
    /// <summary>True when at least one displayable field is set.</summary>
    public bool HasAny => !string.IsNullOrWhiteSpace(Title)
        || !string.IsNullOrWhiteSpace(Artist)
        || !string.IsNullOrWhiteSpace(Album);
}

/// <summary>
/// One upcoming ("up next" / "coming up") item on a live stream — the single next track/program, or
/// each element of a forward schedule list. All members are optional so a source can supply whatever
/// it knows; an all-null instance means "nothing to show".
/// </summary>
/// <param name="Title">Song title OR program name.</param>
/// <param name="Subtitle">Artist (music) / episode or short description (TV). Null when unknown.</param>
/// <param name="Album">Album, when known. Null otherwise.</param>
/// <param name="StartsUtc">When the item begins (aligns with LiveNowPlaying.NextChangeUtc). Null if unknown.</param>
/// <param name="EndsUtc">When the item ends (slot boundary for a coming-up list). Null if unknown.</param>
public sealed record LiveUpNext(
    string? Title,
    string? Subtitle = null,
    string? Album = null,
    DateTimeOffset? StartsUtc = null,
    DateTimeOffset? EndsUtc = null)
{
    /// <summary>True when at least one displayable field is set.</summary>
    public bool HasAny => !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Subtitle);
}
