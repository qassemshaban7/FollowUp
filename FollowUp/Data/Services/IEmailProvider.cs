namespace FollowUp.Data.Services
{
    public interface IEmailProvider
    {
        Task<int> SendMail(int attendId, int tableId, string UserId, string Value, int? minut);
    }
}
