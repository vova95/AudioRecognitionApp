using System.Collections.Generic;

namespace AudioRecognitionApp.BLL.DTO
{
    public class BaseInformationDTO
    {
        public int NumberOfPages { get; set; }
        public List<SongDTO> Songs { get; set; }
    }
}