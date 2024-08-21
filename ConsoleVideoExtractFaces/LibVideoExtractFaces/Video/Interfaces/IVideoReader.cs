using LibVideoExtractFaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVideoExtractFaces.Video.Interfaces
{
    public interface IVideoReader
    {
        IEnumerable<Frame> ReadFrames(string videoPath);
    }

}
