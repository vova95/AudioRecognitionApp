namespace AudioRecognitionApp.BLL.DTO
{
    public class PointDTO
    {
        public int Id { get; set; }
        public long Hash { get; set; }
        public decimal Time { get; set; }
        public int SongId { get; set; }
    }
}