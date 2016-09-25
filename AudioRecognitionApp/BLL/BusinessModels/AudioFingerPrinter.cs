using AudioRecognitionApp.alg.math;
using AudioRecognitionApp.BLL.BusinessModels.math;
using AudioRecognitionApp.DAL.POCO;
using System;
using System.Collections.Generic;

namespace AudioRecognitionApp.BLL.BusinessModels
{
    public class AudioFingerPrinter
    {
        private const int SampleRate = 44100;
        private const int ChunkSize = 1024;
        private const int UpperLimit = 500;
        private const int LowerLimit = 5;
        private const int FuzFactor = 2;

        private static readonly int[] Range = { 25, 40, 80, 120, 180, 240, 280, 320, 360, 420, 500 };

        private double[][] _highscores;

        private long[][] _points;

        public int GetIndex(int freq)
        {
            int i = 0;
            while (Range[i] < freq)
                i++;
            return i;
        }

        private Complex[][] TransformToFft(byte[] audio)
        {
            int totalSize = audio.Length;

            int amountPossible = totalSize / ChunkSize;

            //When turning into frequency domain we'll need complex numbers:
            Complex[][] results = new Complex[amountPossible][];

            //For all the chunks:
            for (int times = 0; times < amountPossible; times++)
            {
                Complex[] complex = new Complex[ChunkSize];
                for (int i = 0; i < ChunkSize; i++)
                {
                    //Put the time domain data into a complex number with imaginary part as 0:
                    complex[i] = new Complex(audio[(times * ChunkSize) + i], 0);
                }
                //Perform FFT analysis on the chunk:
                results[times] = FFT.fft(hamming(complex));
            }

            return results;
        }

        public long HashLowPeaks(long p1, long p2, long p3, long p4)
        {
            return (p4 - p4 % FuzFactor)
                    * 10000000 + (p3 - p3 % FuzFactor) * 10000 + (p2 - p2 % FuzFactor) * 100
                    + (p1 - p1 % FuzFactor);
        }

        private long HashHighPeaks(long p1, long p2, long p3)
        {
            return (p3 - p3 % FuzFactor) * 1000000 + (p2 - p2 % FuzFactor) * 1000
                    + (p1 - p1 % FuzFactor);
        }

        public List<Point> DetermineKeyPoints(byte[] byteArray)
        {
            decimal timeForOneHash = 0.1m; //Math.Round(((double)byteArray.Length / 11025) / ((double)byteArray.Length / CHUNK_SIZE), 3);
            Complex[][] results = TransformToFft(byteArray);
            List<Point> hashes = new List<Point>();
            _highscores = new double[results.Length][];
            for (int i = 0; i < results.Length; i++)
            {
                _highscores[i] = new double[11];
                for (int j = 0; j < 11; j++)
                {
                    _highscores[i][j] = 0;
                }
            }

            _points = new long[results.Length][];
            for (var i = 0; i < results.Length; i++)
            {
                _points[i] = new long[11];
                for (var j = 0; j < 11; j++)
                {
                    _points[i][j] = 0;
                }
            }

            for (var t = 0; t < results.Length; t++)
            {
                for (var freq = LowerLimit; freq < UpperLimit - 1; freq++)
                {
                    // Get the magnitude:
                    var mag = Math.Log(results[t][freq].abs() + 1);

                    // Find out which range we are in:
                    var index = GetIndex(freq);

                    // Save the highest magnitude and corresponding frequency:
                    if (mag > _highscores[t][index])
                    {
                        _highscores[t][index] = mag;

                        _points[t][index] = freq;
                    }
                }

                var h1 = HashLowPeaks(_points[t][0], _points[t][1], _points[t][2],
                        _points[t][3]);
                var h2 = HashHighPeaks(_points[t][4], _points[t][5], _points[t][6]);
                var h3 = HashHighPeaks(_points[t][7], _points[t][8], _points[t][9]);

                hashes.Add(new Point { Time = Math.Round(timeForOneHash * (t + 1), 2), Hash = h1, SongId = 1 });
                hashes.Add(new Point { Time = Math.Round(timeForOneHash * (t + 1), 2), Hash = h2, SongId = 1 });
                hashes.Add(new Point { Time = Math.Round(timeForOneHash * (t + 1), 2), Hash = h3, SongId = 1 });

            }
            return hashes;
        }

        private Complex[] hamming(Complex[] iwv)
        {
            int N = iwv.Length;

            // iwv[i].Re = real number (raw wave data) 
            // iwv[i].Im = imaginary number (0 since it hasn't gone through FFT yet)
            for (int n = 0; n < N; n++)
                iwv[n].re *= 0.54 - 0.46 * Math.Cos((2 * Math.PI * n) / (N - 1));

            return iwv;
        }
    }
}
