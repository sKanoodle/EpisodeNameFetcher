using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpisodeNameFetcher
{
    [Flags]
    public enum ParsingMode
    {
        Debug = 1,
        Wikipedia = 2,
        TheTVDB = 4,
        MovieTitle = 8,

    }
}
