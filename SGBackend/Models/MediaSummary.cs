﻿using SGBackend.Entities;

namespace SGBackend.Models;

public class MediaSummary
{
    public string mediumId { get; set; }
    
    public string songTitle { get; set; }

    public string[] allArtists { get; set; }

    public bool explicitFlag { get; set; }
    
    public bool hidden { get; set; }

    public long? listenedSeconds { get; set; }

    public long? listenedSecondsYou { get; set; }

    public long? listenedSecondsMatch { get; set; }

    public MediumImage[] albumImages { get; set; }

    public string linkToMedia { get; set; }

    public string albumName { get; set; }

    public string releaseDate { get; set; }
    
    public string? hiddenOrigin { get; set; }
}