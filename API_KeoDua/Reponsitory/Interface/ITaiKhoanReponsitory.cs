using API_KeoDua.Data;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface ITaiKhoanReponsitory
    {
        public int TotalRows { get; set; }

        public Task<bool> IsCheckAccount(string username, string password);
        public Task<string> login(string user, string pass);
        public Task<List<string>> getDataPermission(string userName);
        public Task<List<string>> GetAccountName();
    }
}
