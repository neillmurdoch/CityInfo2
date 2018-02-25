namespace CityInfo.API
{
    public interface IMailService
    {
        void Send(string subject, string message);
    }
}