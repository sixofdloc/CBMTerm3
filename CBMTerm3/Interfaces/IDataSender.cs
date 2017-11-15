namespace CBMTerm3.Interfaces
{
    public interface IDataSender
    {
        void Send(byte[] data);
        void Send(string data);
        void Send(byte data);
        void Send(byte[] data, int count);

    }
}
