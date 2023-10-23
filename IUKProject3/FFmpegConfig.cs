using System;

namespace FFMpegWrapper
{
    public class FFmpegConfig
    {
        public String _path = null;
        public bool _useShellExecute = false;
        public bool _createAWindow = false;

        public FFmpegConfig(String path, bool useShellExecute, bool createAWindow )
        {
            _path = path;
            _useShellExecute = useShellExecute;
            _createAWindow = createAWindow;
        }
    }
}