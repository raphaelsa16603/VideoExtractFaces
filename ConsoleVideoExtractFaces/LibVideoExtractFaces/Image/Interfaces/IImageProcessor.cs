using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVideoExtractFaces.Model;

namespace LibVideoExtractFaces.Image.Interfaces
{
    public interface IImageProcessor
    {
        IEnumerable<LibVideoExtractFaces.Model.Image> ExtractFaces(IEnumerable<Frame> frames);
        IEnumerable<LibVideoExtractFaces.Model.Image> ExtractFullBodies(IEnumerable<Frame> frames);
    }

}
