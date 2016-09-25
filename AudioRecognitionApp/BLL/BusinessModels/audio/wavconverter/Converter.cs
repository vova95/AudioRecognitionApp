using NAudio.Wave;
using System.IO;

namespace AudioRecognitionApp.BLL.BusinessModels.audio.wavconverter
{
    public class Converter
    {
        private const int SampleRate = 44100;
        private const int BitsPerSample = 8;
        private const int Channels = 1;
        private readonly string _firstPath;
        private readonly string _secondPath;

        public Converter(string path)
        {
            _firstPath = path + "/temp 1.wav";
            _secondPath = path + "/temp 2.wav";
            Mp3Path = path + "/temp 3.wav";
        }

        public string Mp3Path { get; set; }

        public byte[] ConvertWavFile(string path)
        {
            ConvertWav(path);
            var song = Convert11025WavFile(_firstPath);
            File.Delete(_firstPath);
            File.Delete(_secondPath);
            File.Delete(Mp3Path);
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
            return ReadSong(_secondPath);
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

        public byte[] ConvertMp3ToWav(byte[] mp3File)
        {
            var newFormat = new WaveFormat(SampleRate, BitsPerSample, Channels);
            using (var retMs = new MemoryStream())
            using (var ms = new MemoryStream(mp3File))
            using (var rdr = new Mp3FileReader(ms))
            using (var wtr = new WaveFileWriter(retMs, newFormat))
            {
                rdr.CopyTo(wtr);
                return retMs.ToArray();
            }


        }

        public byte[] ReadSong(string path)
        {
            WaveStream stream = new WaveFileReader(path);
            var buffer = new byte[1024];
            using (var ms = new MemoryStream())
            {
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }
                stream.Close();
                return ms.ToArray();
            }
        }
    }
}
