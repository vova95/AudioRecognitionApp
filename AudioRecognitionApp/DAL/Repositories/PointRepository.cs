using AudioRecognitionApp.DAL.Interfaces;
using AudioRecognitionApp.DAL.POCO;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AudioRecognitionApp.DAL.Repositories
{
    public class PointRepository : IRepository<Point>
    {
        private IDbConnection _db;

        public PointRepository(IDbConnection connection)
        {
            _db = connection;
        }

        public IEnumerable<Point> GetAll()
        {
            return _db.Query<Point>("Select * From Points");
        }

        public Point Get(int id)
        {
            var query = $"Select * From Points Where Id = {id}";
            var point = _db.Query<Point>(query).Single();
            return point;
        }

        public Point Create(Point item)
        {
            var query = "Insert Into Points (Hash, Time, SongId) " +
                        "Values (@Hash, @Time, @SongId) SELECT SCOPE_IDENTITY()";

            var pointId = _db.Query<int>(query, item).Single();
            item.Id = pointId;
            return item;
        }

        public Point Update(Point item)
        {
            var query = "Update Points Set Hash = @Hash, Time = @Time, SongId = @SongId Where Id = @Id";
            _db.Execute(query, item);
            return item;
        }

        public Point Delete(int id)
        {
            var query = $"Delete Points Where Id = {id}";
            var point = Get(id);
            _db.Execute(query);
            return point;
        }

        public void DeleteBySongId(int songId)
        {
            var query = $"Delete Points Where SongId = {songId}";
            _db.Execute(query);
        }
        public List<Point> FindByHash(long hash)
        {
            var query = $"Select * From Points Where Hash = {hash}";
            return _db.Query<Point>(query, hash).ToList();
        }

        public List<Point> FindByListOfHashes(long[] hashes)
        {
            var query = "Select * From Points Where Hashes In @Ids";
            return _db.Query<Point>(query, new { Ids = hashes }).ToList();
        }
    }
}