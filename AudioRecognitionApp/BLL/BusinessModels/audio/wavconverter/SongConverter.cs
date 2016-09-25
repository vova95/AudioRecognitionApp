using NAudio.Wave;
using System.IO;

namespace AudioRecognitionApp.BLL.BusinessModels.audio.wavconverter
{
    public class SongConverter
    {
        private readonly string _firstPath;
        private readonly string _secondPath;
        public readonly string Mp3Path;
        private const int SampleRate = 44100;
        private const int BitsPerSample = 8;
        private const int Channels = 1;

        public SongConverter()
        {
            _firstPath = @"D:\Projects\AudioRecognitionApp\AudioRecognitionApp\App_Data\temp";
            _firstPath += "\\Temp  Song 1.wav";
            _secondPath = @"D:\Projects\AudioRecognitionApp\AudioRecognitionApp\App_Data\temp";
            _secondPath += "\\Temp Song 2.wav";
            Mp3Path = @"D:\Projects\AudioRecognitionApp\AudioRecognitionApp\App_Data\temp";
            Mp3Path += "\\Temp Mp3 Song 2.mp3";
        }

        public byte[] ConvertWavFile(string path)
        {
            ConvertWav(path);
            WaveStream stream = new WaveFileReader(_firstPath);
            byte[] buffer = new byte[1024];
            int bytesRead;
            MemoryStream ms = new MemoryStream();

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, bytesRead);
            }
            stream.Close();
            var song = Convert11025WavFile(_firstPath);
            return song;
        }

        public bool IsWav(string songPath)
        {
            var extension = Path.GetExtension(songPath);
            return extension != null && extension.Equals(".wav");
        }

        public void Mp3ToWav(string mp3FilePath)
        {
            using (Mp3FileReader reader = new Mp3FileReader(mp3FilePath))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    WaveFileWriter.CreateWaveFile(Mp3Path, pcmStream);
                }
            }
        }

        private byte[] Convert11025WavFile(string path)
        {
            Convert11025Wav(path);
            WaveStream stream = new WaveFileReader(_secondPath);
            byte[] buffer = new byte[1024];
            int bytesRead;
            MemoryStream ms = new MemoryStream();

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                //n++;
                //if (n > 1000)
                //{
                //    break;
                //}
                ms.Write(buffer, 0, bytesRead);
            }
            byte[] song = ms.ToArray();
            stream.Close();
            return song;
            //File.Delete(@"C:\Users\admin\Documents\Visual Studio 2015\Projects\AudioRecognitionService\AudioRecognitionService\temp.wav");            
        }

        public void ConvertWav(string path)
        {
            var newFormat = new WaveFormat(SampleRate, BitsPerSample, Channels);
            using (WaveStream stream = new WaveFileReader(path))
            {
                int b = stream.WaveFormat.Channels;
                int a = stream.WaveFormat.BitsPerSample;
                int c = stream.WaveFormat.AverageBytesPerSecond;
                using (var conversionStream = new WaveFormatConversionStream(newFormat, stream))
                {
                    WaveFileWriter.CreateWaveFile(_firstPath, conversionStream);
                }
            }
        }

        private void Convert11025Wav(string path)
        {
            var newFormat = new WaveFormat(11025, BitsPerSample, Channels);
            using (WaveStream stream = new WaveFileReader(path))
            {
                int b = stream.WaveFormat.Channels;
                int a = stream.WaveFormat.BitsPerSample;
                int c = stream.WaveFormat.AverageBytesPerSecond;
                using (var conversionStream = new WaveFormatConversionStream(newFormat, stream))
                {
                    WaveFileWriter.CreateWaveFile(_secondPath, conversionStream);
                }
            }
        }
    }
}
