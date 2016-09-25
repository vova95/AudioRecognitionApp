using AudioRecognitionApp.BLL.BusinessModels;
using AudioRecognitionApp.BLL.BusinessModels.audio.wavconverter;
using AudioRecognitionApp.BLL.DTO;
using AudioRecognitionApp.DAL.POCO;
using AudioRecognitionApp.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace AudioRecognitionApp.BLL.Services
{
    public class SongService
    {
        private UnitOfWork Db { get; }

        public SongService()
        {
            Db = new UnitOfWork(ConfigurationManager.ConnectionStrings["SongsDbConnection"].ConnectionString);
        }

        public List<SongDTO> GetSongsWithPagination(int page, int numberOfPages)
        {
            List<Song> songs = Db.Songs.GetNextSongs(page, numberOfPages);
            List<SongDTO> resultList = new List<SongDTO>(numberOfPages);
            int n = 0;
            foreach (var song in songs)
            {
                resultList.Add(new SongDTO
                {
                    Id = song.Id,
                    Name = song.Name,
                    RowNumber = page * numberOfPages + 1 + n
                });
                n++;
            }
            return resultList;
        }

        public void DeleteSong(int id)
        {
            Db.Points.DeleteBySongId(id);
            Db.Songs.Delete(id);
        }

        public void CreateSong(string path)
        {
            AudioFingerPrinter fingerPrinter = new AudioFingerPrinter();
            Converter converter = new Converter(System.AppDomain.CurrentDomain.BaseDirectory + @"App_Data\temp");
            byte[] songBytes;
            if (converter.IsWav(path))
            {
                songBytes = converter.ConvertWavFile(path);
            }
            else
            {
                converter.Mp3ToWav(path);
                songBytes = converter.ConvertWavFile(converter.Mp3Path);
            }
            List<Point> points = fingerPrinter.DetermineKeyPoints(songBytes);
            Song insertedSong = Db.Songs.Create(new Song { Name = Path.GetFileNameWithoutExtension(path) });
            foreach (var point in points)
            {
                Db.Points.Create(new Point { Hash = point.Hash, SongId = insertedSong.Id, Time = point.Time });
            }

            File.Delete(path);
        }

        public int CountPages()
        {
            return (int)Math.Ceiling((double)Db.Songs.CountSongs() / 5);
        }

        public SongDTO GetSong(int id)
        {
            Song dbSong = Db.Songs.Get(id);

            SongDTO song = new SongDTO
            {
                Id = dbSong.Id,
                Name = dbSong.Name
            };
            return song;
        }

        public void UpdateSongName(SongDTO song)
        {
            Song dbSong = new Song
            {
                Id = song.Id,
                Name = song.Name
            };
            Db.Songs.Update(dbSong);
        }
    }
}