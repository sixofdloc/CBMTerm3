﻿namespace FileTransferProtocols.Interfaces
{
    public interface IDataSender
    {
        void Send(byte[] data);
        void Send(string data);
        void Send(byte data);
        void Send(byte[] data, int count);
        void Send(byte[] data, int count, int delay);

    }
}
