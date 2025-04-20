namespace Coterie.Api.Models.ErrorHandler
{
    public abstract class BaseSuccessResponse
    {
        public bool IsSuccessful { get; } = true;
        public string TransactionId { get; } = Guid.NewGuid().ToString();
    }
}
