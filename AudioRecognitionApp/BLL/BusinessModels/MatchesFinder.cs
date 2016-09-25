using AudioRecognitionApp.BLL.DTO;
using AudioRecognitionApp.DAL.POCO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioRecognitionApp.BLL.BusinessModels
{
    public class MatchesFinder
    {
        public TotalMatchesDTO GetMatches(List<Point> micro, List<Point> song)
        {
            Lookup<long, decimal> hashes = song.ToLookup(item => item.Hash, item => item.Time) as Lookup<long, decimal>;
            int count = 0;
            List<Point> d = new List<Point>();
            List<double> t = new List<double>();
            foreach (Point m in micro)
            {
                if (hashes.Contains(m.Hash))
                {
                    d.Add(m);
                    foreach (var item in hashes[m.Hash])
                    {
                        count++;
                        t.Add(Math.Floor((double)(item - m.Time)));
                    }
                }

            }
            var z = t.GroupBy(x => x).OrderByDescending(grp => grp.Count());
            double absoluteTime = GetAbsoluteTime(t);
            var y = z.Select(x => new MatchTime { Time = x.Key, Count = x.Count() }).Select(x => x).Where(x => (x.Time > (absoluteTime - 1.8) && x.Time < (absoluteTime + 1.8)));
            int matches = 0;
            y.ToList().ForEach(x => { if (x.Time > 0) matches += x.Count; });
            return new TotalMatchesDTO { AbsoluteTime = convertToMinutes((int)absoluteTime), TotalMatches = matches };
        }

        private double GetAbsoluteTime(List<double> list)
        {
            return list.GroupBy(x => x).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
        }

        private string convertToMinutes(int time)
        {
            var span = new TimeSpan(0, 0, time);
            var newTime = string.Format("{0}:{1:00}", (int)span.TotalMinutes, span.Seconds);
            return newTime;
        }

        private struct MatchTime
        {
            public double Time;
            public int Count;
        }
    }
}
