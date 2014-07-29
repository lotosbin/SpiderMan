using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using sharp_net.Mongo;
using Baozou.Entity;
using Ninject;
using MongoDB.Driver.Linq;

namespace SpiderMan.App_Start {
    public class Transaction {
        public void Begin() {
            Timer ms_timer = new Timer(1000 * 60 * 10); //10分钟
            ms_timer.Elapsed += delegate { MatchStatus(); };
            ms_timer.Enabled = true;
            MatchStatus();
        }

        public void MatchStatus() {
            var collection = NinjectWebCommon.Kernel.Get<IMongoRepo<Match>>().Collection;
            var matchs = collection.AsQueryable<Match>().Where(d => d.Status < 2 && d.Time < DateTime.Now);
            if (matchs.Count() != 0) {
                foreach (var match in matchs) {
                    DateTime span;
                    if (match.Type == (int)eMatchType.Tennis) {
                        span = DateTime.Now.Subtract(new TimeSpan(5, 0, 0));
                    } else {
                        span = DateTime.Now.Subtract(new TimeSpan(3, 0, 0));
                    }
                    if (match.Time < span) {
                        match.Status = (int)eMatchStatus.Ago;
                    } else {
                        match.Status = (int)eMatchStatus.Inprocess;
                    }
                    collection.Save(match);
                }
            }
        }
    }
}