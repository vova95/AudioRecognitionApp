using AudioRecognitionApp.DAL.Interfaces;
using AudioRecognitionApp.DAL.POCO;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AudioRecognitionApp.DAL.Repositories
{
    public class SongRepository : IRepository<Song>
    {
        private IDbConnection _db;

        public SongRepository(IDbConnection connection)
        {
            //new SqlConnection(ConfigurationManager.ConnectionStrings["SongsDbConnection"].ConnectionString);
            _db = connection;
        }

        public IEnumerable<Song> GetAll()
        {
            return _db.Query<Song>("Select * From Songs").ToList();
        }

        public Song Get(int id)
        {
            var query = $"Select * from Songs Where Id = {id}";
            return _db.Query<Song>(query, id).Single();
        }

        public Song Create(Song item)
        {
            var query = "Insert Into Songs (Name) Values (@Name) SELECT SCOPE_IDENTITY()";

            var songId = _db.Query<int>(query, item).Single();
            item.Id = songId;
            return item;
        }

        public Song Update(Song song)
        {
            var query = "Update Songs Set Name = @Name Where Id = @Id";
            _db.Execute(query, song);
            return song;
        }

        public Song Delete(int id)
        {
            var query = $"Delete From Songs Where Id = {id}";
            var song = Get(id);
            _db.Execute(query);
            return song;
        }

        public Song FindByName(string name)
        {
            var query = $"Select * from Songs Where Name = {name}";
            return _db.Query<Song>(query).Single();
        }

        public List<Song> GetNextSongs(int page, int numberOfSongs)
        {
            var query = $@"SELECT Id, Name FROM
                        (
                        SELECT Id, Name, ROW_NUMBER() OVER(Order By[Name] DESC) As RowNumber


                        FROM Songs) songs
                        WHERE songs.RowNumber BETWEEN ({page} * {numberOfSongs} + 1) AND(({page} + 1) * {numberOfSongs})

                        ORDER BY[Name] DESC";
            return _db.Query<Song>(query).ToList();
        }

        public int CountSongs()
        {
            return _db.Query<int>("Select Count(Name) From Songs").Single();
        }
    }
}