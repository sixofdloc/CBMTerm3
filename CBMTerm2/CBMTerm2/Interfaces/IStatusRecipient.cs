using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBMTerm2.Interfaces
{
    public interface IStatusRecipient
    {
        void SetStatus(string s, int bytestransferred, int bytestogo);
    }
}
