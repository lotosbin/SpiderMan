using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using sharp_net.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpiderMan.Models {
    public enum eVideoRegion {
        Chinese = 1,
        JapanKorea = 2,
        EuropeAmerica = 3,
        Other = 4
    }

    public class AdianboVideo : MEntity {
        public bool IsTeleplay { get; set; }

        public string ImdbId { get; set; }
        public int Imdb { get; set; }
        public string DoubanId { get; set; }
        public int Douban { get; set; }

        public string EnglishName { get; set; }
        public string ChinsesName { get; set; }
        public string Intro { get; set; }
        public DateTime ShowDate { get; set; }
        public DateTime CloseDate { get; set; } //for TV
        public int Region { get; set; } //eVideoRegion
        public string Nation { get; set; }

        public int CurrentQuater { get; set; } //for TV
        public int CurrentSet { get; set; } //for TV

        public string[] Writer { get; set; }
        public string[] Director { get; set; }
        public string[] Actor { get; set; }

        public bool HasHD { get; set; } //for movies
        
        public IList<string> Comments { get; set; }

        public static UpdateBuilder<AdianboVideo> UpdateBuilder(AdianboVideo data) {
            var update = new UpdateBuilder<AdianboVideo>();
            if (data.Imdb > 0)
                update = Update<AdianboVideo>.Set(d => d.Imdb, data.Imdb);
            if (!string.IsNullOrEmpty(data.ImdbId))
                update = update.Set(d => d.ImdbId, data.ImdbId);
            if (data.Douban > 0)
                update = Update<AdianboVideo>.Set(d => d.Douban, data.Douban);
            if (!string.IsNullOrEmpty(data.DoubanId))
                update = update.Set(d => d.DoubanId, data.DoubanId);
            if (data.IsTeleplay)
                update = update.Set(d => d.IsTeleplay, true);
            if (!string.IsNullOrEmpty(data.ChinsesName))
                update = update.Set(d => d.ChinsesName, data.ChinsesName);
            if (string.IsNullOrEmpty(data.EnglishName))
                update = update.Set(d => d.EnglishName, data.EnglishName);
            if (!string.IsNullOrEmpty(data.Intro))
                update = update.Set(d => d.Intro, data.Intro);
            if (data.ShowDate != null)
                update = update.Set(d => d.ShowDate, data.ShowDate);
            if (data.CloseDate != null)
                update = update.Set(d => d.CloseDate, data.CloseDate);
            if (data.Region != 0)
                update = update.Set(d => d.Region, data.Region);
            if (!string.IsNullOrEmpty(data.Nation))
                update = update.Set(d => d.Nation, data.Nation);
            if (data.CurrentQuater != 0)
                update = update.Set(d => d.CurrentQuater, data.CurrentQuater);
            if (data.CurrentSet != 0)
                update = update.Set(d => d.CurrentSet, data.CurrentSet);
            if (data.Writer.Length > 0)
                update = update.Set(d => d.Writer, data.Writer);
            if (data.Director.Length > 0)
                update = update.Set(d => d.Director, data.Director);
            if (data.Actor.Length > 0)
                update = update.Set(d => d.Actor, data.Actor);
            if (data.HasHD)
                update = update.Set(d => d.HasHD, true);
            if (data.Comments.Count > 0)
                update = update.Set(d => d.Comments, data.Comments);
            return update;
        }
    }

    public class VideoSource : MEntity {
        public bool IsTeleplay { get; set; }
        public string Name { get; set; }
        public string ImdbId { get; set; }
        public string SourceSite { get; set; }
        public string SourceLink { get; set; }
        public string ProviderId { get; set; }
        public IList<VideoLink> Links { get; set; }
    }

    public class VideoLink {
        public string FileName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Format { get; set; }
        public string Size { get; set; }
        public string SubtitleLink { get; set; }

        public int Quater { get; set; } //for TV
        public int Set { get; set; } //for TV

        public string EMule { get; set; } //包含旋风下载、旋风云播放、迅雷云播放
        public string Xunlei { get; set; }
        public string XunleiPan { get; set; }
        public string XuanfengPan { get; set; }
        public string Online { get; set; }
    }
}