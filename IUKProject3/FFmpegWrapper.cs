using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace FFMpegWrapper
{
    public class FFmpegWrapper : IDisposable
    {
        protected FFmpegConfig _config = null;
        private bool _disposed = false;
        private Process _process;

        public FFmpegWrapper(FFmpegConfig config)
        {
            _config = config;
            startFFmpeg();
        }

        public void startFFmpeg()
        {
            _process = new Process();
            _process.StartInfo.UseShellExecute = _config._useShellExecute;
            _process.StartInfo.FileName = _config._path;
            _process.StartInfo.CreateNoWindow = !_config._createAWindow;
            _process.StartInfo.RedirectStandardOutput = true;
        }


        public void Dispose()
        {
            
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _process.Dispose();
                }

                _disposed = true;
            }
        }
        
        public string customCommandTest(String command)
        {
            MemoryStream copyStream = new MemoryStream();
            MemoryStream inputStream = new MemoryStream();
            try
            {
                string result = String.Empty;

                _process.StartInfo.Arguments = command;
                _process.Start();
                _process.StandardOutput.BaseStream.CopyTo(inputStream);
                _process.WaitForExit();
                result = _process.StandardOutput.ReadToEnd();
                inputStream.Position = 0;
                bool headerFound = false;

                while (inputStream.Position < inputStream.Length)
                {
                    if (!headerFound)
                    {
                        byte[] chunkHeader = new byte[8];
                        inputStream.Read(chunkHeader, 0, 8);

                        headerFound = true;
                        copyStream.Write(chunkHeader, 0, 8);
                    }

                    byte[] chunk = new byte[4];
                    inputStream.Read(chunk, 0, 4);
                    copyStream.Write(chunk, 0, 4);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(chunk);
                    }

                    int i = BitConverter.ToInt32(chunk, 0);

                    chunk = new byte[4];
                    inputStream.Read(chunk, 0, 4);
                    copyStream.Write(chunk, 0, 4);

                    var str = Encoding.ASCII.GetChars(chunk);
                    byte[] tempByteArray = new byte[i + 4];
                    inputStream.Read(tempByteArray, 0, i + 4);

                    copyStream.Write(tempByteArray, 0, i + 4);
                    if (new string(str) == "IEND")
                    {

                        safeAsPng(copyStream);
                        copyStream = new MemoryStream();
                        headerFound = false;
                    }

                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                copyStream.Dispose();
                inputStream.Dispose();
            }

            return String.Empty;
        }
        
        public static void safeAsPng(Stream stream)
        {
            // Create a Bitmap object from the byte array
            using (Bitmap image = new Bitmap(stream))
            {
                Console.Clear();
                ConvertToText(image);
            }


            //image.Save(Guid.NewGuid().ToString() + "plswork23.png", ImageFormat.Png);
        }

        static void ConvertToText(Bitmap bitmap)
        {
            int pixelInterval = 8;
            double brightnessMultiplier = 1;
            using (Bitmap bmp = bitmap)
            {
                string WrittenLine = "";
                for (int y = 0; y < bmp.Size.Height - (bmp.Size.Height % pixelInterval); y += pixelInterval)
                {
                    for (int x = 0; x < bmp.Size.Width; x++)
                    {
                        if (x % pixelInterval == 0 ||
                            x % pixelInterval == 1) // The character height-width-ratio is approximately 2/1
                        {
                            WrittenLine +=
                                GetSymbolFromBrightness(bmp.GetPixel(x, y).GetBrightness() * brightnessMultiplier);
                        }
                    }
                
                    Console.WriteLine(WrittenLine);
                    WrittenLine = "";
                }
            }
            Thread.Sleep(42);
        }

        static string GetSymbolFromBrightness(double brightness)
        {
            switch ((int)(brightness * 10))
            {
                case 0:
                    return "@";
                case 1:
                    return "$";
                case 2:
                    return "#";
                case 3:
                    return "*";
                case 4:
                    return "!";
                case 5:
                    return "+";
                case 6:
                    return ":";
                case 7:
                    return "~";
                case 8:
                    return "-";
                case 9:
                    return ".";
                default:
                    return " ";
            }
        }
    }
}