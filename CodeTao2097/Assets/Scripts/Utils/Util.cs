using System.Collections.Generic;

namespace CodeTao
{
    public static class Util
    {
        public static bool IsTagIncluded(string tag, List<ETag> tags)
        {
            foreach (var t in tags)
            {
                if (tag == t.ToString())
                {
                    return true;
                }
            }

            return false;
        }
    }
}