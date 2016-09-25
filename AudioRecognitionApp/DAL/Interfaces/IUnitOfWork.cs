using AudioRecognitionApp.DAL.POCO;
using System;

namespace AudioRecognitionApp.DAL.Interfaces
{
    interface IUnitOfWork : IDisposable
    {
        IRepository<Point> Points { get; }
        IRepository<Song> Songs { get; }
    }
}
