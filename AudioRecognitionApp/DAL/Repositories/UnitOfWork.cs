using System;
using System.Data;
using System.Data.SqlClient;

namespace AudioRecognitionApp.DAL.Repositories
{
    public class UnitOfWork
    {
        private IDbConnection _db;
        private PointRepository _pointRepository;
        private SongRepository _songRepository;


        public UnitOfWork(String connectionString)
        {
            _db = new SqlConnection(connectionString);
        }

        public PointRepository Points
        {
            get
            {
                if (_pointRepository == null)
                {
                    _pointRepository = new PointRepository(_db);
                }
                return _pointRepository;
            }

        }

        public SongRepository Songs
        {
            get
            {
                if (_songRepository == null)
                {
                    _songRepository = new SongRepository(_db);
                }
                return _songRepository;
            }
        }
    }
}