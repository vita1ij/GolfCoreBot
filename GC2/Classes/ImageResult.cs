using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GC2.Classes
{
    public class ImageResult
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public Stream Stream { get; set; }
        public string ReferenceName { get; set; }
    }
}
