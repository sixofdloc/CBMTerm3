﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBMTerm2.Interfaces
{
    public interface IDataSender
    {
        void Send(byte[] data);
        void Send(string data);
        void Send(byte data);
        void Send(byte[] data, int count);

    }
}