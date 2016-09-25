namespace AudioRecognitionApp.DAL.POCO
{
    public class Point
    {
        public int Id { get; set; }
        public long Hash { get; set; }
        public decimal Time { get; set; }
        public int SongId { get; set; }
    }
}