using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVideoExtractFaces.Model
{
    public class Image
    {
        public byte[] Data { get; set; }
        public string Description { get; set; }

        public Image(byte[] data, string description)
        {
            Data = data;
            Description = description;
        }
    }

}
