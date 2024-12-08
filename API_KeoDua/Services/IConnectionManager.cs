namespace API_KeoDua.Services
{
    public interface IConnectionManager
    {
        string ConnectionString { get; set; }
        void SetConnectionString(string user, string pass);
        void ClearConnectionString();
    }
}
