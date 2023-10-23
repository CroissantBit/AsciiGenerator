using System;
using FFMpegWrapper;



namespace ConsoleApp5
{
    class Program
    {
        static string path;
        static int pixelInterval = 8;
        static double brightnessMultiplier = 1;

        static void Main()
        {
            Console.Title = "ASCII Writer";
            FFmpegConfig config = new("./bin/ffmpeg.exe", false, true);

            using (FFmpegWrapper ffmpegwrapper = new(config))
            {
                string res = ffmpegwrapper.customCommandTest(" -i  ./Toxic.mp4 -s 640x360 -pix_fmt rgba -f image2pipe -vcodec png -");
            }
        }
    }
}