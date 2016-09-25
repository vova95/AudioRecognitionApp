using AudioRecognitionApp.BLL.BusinessModels;
using AudioRecognitionApp.BLL.BusinessModels.audio.wavconverter;
using AudioRecognitionApp.BLL.DTO;
using AudioRecognitionApp.DAL.POCO;
using AudioRecognitionApp.DAL.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AudioRecognitionApp.BLL.Services
{
    public class SearchService
    {
        private UnitOfWork Db { get; }

        public SearchService()
        {
            Db = new UnitOfWork(ConfigurationManager.ConnectionStrings["SongsDbConnection"].ConnectionString);
        }

        public IEnumerable<DesiredSongDTO> SearchSong(string songPath)
        {
            Converter converter = new Converter(System.AppDomain.CurrentDomain.BaseDirectory + @"App_Data\temp");
            AudioFingerPrinter fingerPrinter = new AudioFingerPrinter();
            byte[] songBytes;
            if (converter.IsWav(songPath))
            {
                songBytes = converter.ConvertWavFile(songPath);
            }
            else
            {
                converter.Mp3ToWav(songPath);
                songBytes = converter.ConvertWavFile(converter.Mp3Path);
            }

            List<Point> points = fingerPrinter.DetermineKeyPoints(songBytes);

            List<Point> result = FindPointsByHash(points);

            return FindMatches(points, result);
        }

        private List<Point> FindPointsByHash(List<Point> points)
        {
            List<Point> resultPoints = new List<Point>();
            foreach (var point in points)
            {
                resultPoints.AddRange(Db.Points.FindByHash(point.Hash));
            }
            return resultPoints;
        }

        private List<DesiredSongDTO> FindMatches(List<Point> dataCheck, List<Point> resultPoints)
        {
            var groupedResultPoints = resultPoints
                                        .GroupBy(u => u.SongId)
                                        .Select(grp => grp.ToList())
                                        .ToList();

            MatchesFinder finder = new MatchesFinder();
            List<DesiredSongDTO> matchedSongs = new List<DesiredSongDTO>();
            foreach (var listOfPoints in groupedResultPoints)
            {
                string name = Db.Songs.Get(listOfPoints.First().SongId).Name;
                matchedSongs.Add(new DesiredSongDTO
                {
                    Name = name,
                    TotalMatches = finder.GetMatches(dataCheck, listOfPoints)
                });

            }

            return matchedSongs.OrderByDescending(item => item.TotalMatches.TotalMatches).Take(5).ToList();
        }
    }
}