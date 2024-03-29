using Microsoft.EntityFrameworkCore;
using SGBackend.Models;

namespace SGBackend.Entities;

public enum MediumSource
{
    Spotify
}

[Index(nameof(LinkToMedium), IsUnique = true)]
public class Medium : BaseEntity
{
    public string Title { get; set; }

    public MediumSource MediumSource { get; set; }

    public string LinkToMedium { get; set; }

    public bool ExplicitContent { get; set; }

    public List<Artist> Artists { get; set; }

    public List<MediumImage> Images { get; set; }

    public string AlbumName { get; set; }

    public string ReleaseDate { get; set; }
    
    public double BeatsPerMinute { get; set; }


    public TogetherMediaModel ToTogetherMedia(long listenedSecondsMatch, long listenedSecondsYou, bool hidden)
    {
        var tct = new TogetherMediaModel
        {
            listenedSecondsMatch = listenedSecondsMatch,
            listenedSeconds = listenedSecondsYou,
            hidden = hidden
        };
        SetMediaModel(tct);
        return tct;
    }

    public void SetMediaModel(MediaModel mediaModel)
    {
        mediaModel.albumImages = SortBySize(Images);
        mediaModel.allArtists = Artists.Select(a => a.Name).ToArray();
        mediaModel.explicitFlag = ExplicitContent;
        mediaModel.songTitle = Title;
        mediaModel.linkToMedia = $"spotify:track:{LinkToMedium.Split("/").Last()}";
        mediaModel.albumName = AlbumName;
        mediaModel.releaseDate = ReleaseDate;
        mediaModel.mediumId = Id.ToString();
        mediaModel.bpm = BeatsPerMinute;
    }
    
    public ProfileMediaModel ToProfileMediaModel(long listenedSeconds)
    {
        var profileModel = new ProfileMediaModel
        {
            listenedSeconds = listenedSeconds
        };
        SetMediaModel(profileModel);
        return profileModel;
    }
    

    private static MediumImage[] SortBySize(List<MediumImage> mediumImages)
    {
        return mediumImages.OrderBy(i => i.height).ThenBy(i => i.width).ToArray();
    }

    public ExportMedium ToExportMedium()
    {
        return new ExportMedium
        {
            Artists = Artists.Select(a => a.ToExportArtist()).ToList(),
            Images = Images.Select(i => i.ToExportImage()).ToList(),
            AlbumName = AlbumName,
            ExplicitContent = ExplicitContent,
            Title = Title,
            MediumSource = MediumSource,
            ReleaseDate = ReleaseDate,
            LinkToMedium = LinkToMedium,
            BeatsPerMinute = BeatsPerMinute
        };
    }
}

public class ExportMedium
{
    public string Title { get; set; }

    public MediumSource MediumSource { get; set; }

    public string LinkToMedium { get; set; }

    public bool ExplicitContent { get; set; }

    public List<ExportArtist> Artists { get; set; }

    public List<ExportMediumImage> Images { get; set; }

    public string AlbumName { get; set; }

    public string ReleaseDate { get; set; }
    
    public double BeatsPerMinute { get; set; }

    public Medium ToMedium()
    {
        return new Medium
        {
            LinkToMedium = LinkToMedium,
            Title = Title,
            MediumSource = MediumSource,
            ReleaseDate = ReleaseDate,
            AlbumName = AlbumName,
            ExplicitContent = ExplicitContent,
            Artists = Artists.Select(artist => artist.ToArtist()).ToList(),
            Images = Images.Select(image => image.ToMediumImage()).ToList(),
            BeatsPerMinute = BeatsPerMinute
        };
    }
}