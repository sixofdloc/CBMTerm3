﻿namespace CBMTerm3.Classes
{
    public static class Extensions
    {
        public static bool MatchesAt(this byte[] tosearch, byte[] tofind, int start, int len)
        {
            if (start + len > tosearch.Length) return false;
            bool b = true;
            for (int i = 0; i < len; i++)
            {
                if (tosearch[start + i] != tofind[i]) b = false;
            }
            return b;
        }

    }
}
