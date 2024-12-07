using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Policy;
using System.Text;
namespace API_KeoDua.Reponsitory.Implement
{
    public class NhomQuyenRepository : INhomQuyenRepository
    {
        private readonly NhomQuyenContext nhomQuyenContext;
        private readonly QuyenContext quyenContext;

        public NhomQuyenRepository(NhomQuyenContext nhomQuyenContext, QuyenContext quyenContext)
        {
            this.nhomQuyenContext = nhomQuyenContext;
            this.quyenContext = quyenContext;
        }

        public int TotalRows { get; set; }

        public async Task<List<NhomQuyen>> quickSearchNhomQuyen(string searchString)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var sqlWhere = new StringBuilder();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" AND (MaNhomQuyen like @SearchString ESCAPE '\\' OR (TenNhomQuyen) like @SearchString ESCAPE '\\')");
                    param.Add("SearchString", "%" + searchString + "%");
                }

                string sqlQuery = @"SELECT * FROM tbl_NhomQuyen WITH (NOLOCK) where MaNhomQuyen!='NQ00000005' " + sqlWhere;

                using (var connection = this.nhomQuyenContext.CreateConnection())
                {
                    var resultData = (await connection.QueryAsync<NhomQuyen>(sqlQuery, param)).ToList();
                    return resultData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<NhomQuyen>> GetNhomQuyen()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                string sqlQuery;

                // Kiểm tra nếu tenNV là null hoặc rỗng
                sqlQuery = @" SELECT MaNhomQuyen, TenNhomQuyen FROM tbl_NhomQuyen nq WITH (NOLOCK)";

                using (var connection = this.nhomQuyenContext.CreateConnection())
                {
                    var resultData = (await connection.QueryAsync<NhomQuyen>(sqlQuery, param)).ToList();
                    return resultData;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Có lỗi xảy ra khi lấy dữ liệu nhóm quyền.", ex);
            }
        }

        public async Task<List<Quyen>> GetQuyenByTenNhomQuyen(string TenNQ)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                string sqlQuery;


                if (string.IsNullOrEmpty(TenNQ))
                {
                    // Nếu TenNQ trống thì select tất cả các quyền
                    sqlQuery = @"
                SELECT q.MaQuyen, q.TenQuyen
                FROM tbl_Quyen q";
                }
                else
                {
                    // Nếu TenNQ có giá trị, select quyền theo tên nhóm quyền
                    sqlQuery = @"
                SELECT q.MaQuyen, q.TenQuyen
                FROM tbl_Quyen q
                JOIN tbl_CapQuyen cq ON q.MaQuyen = cq.MaQuyen
                JOIN tbl_NhomQuyen nq ON cq.MaNhomQuyen = nq.MaNhomQuyen
                WHERE nq.TenNhomQuyen = @TenNQ";
                    param.Add("@TenNQ", TenNQ);  // Thêm tham số TenNQ vào DynamicParameters
                }


                using (var connection = this.quyenContext.CreateConnection())
                {
                    var resultData = (await connection.QueryAsync<Quyen>(sqlQuery, param)).ToList();
                    return resultData;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Có lỗi xảy ra khi lấy danh sách quyền.", ex);
            }
        }

        public async Task UpdateRole(string tenTaiKhoan, string tenNhomQuyen)
        {
            try
            {
                // Tạo đối tượng DynamicParameters để truyền tham số cho stored procedure
                DynamicParameters param = new DynamicParameters();
                param.Add("@RoleNames", tenNhomQuyen);  // Thêm tham số @RoleNames
                param.Add("@UserName", tenTaiKhoan);    // Thêm tham số @UserName

                string sqlQuery = "[dbo].[ManagePermissionsBasedOnRoles]";  // Stored procedure cần gọi

                // Sử dụng Dapper để thực thi stored procedure
                using (var connection = this.quyenContext.CreateConnection())
                {
                    // Thực thi stored procedure với các tham số đã truyền vào
                    await connection.ExecuteAsync(sqlQuery, param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                throw new InvalidOperationException("Có lỗi xảy ra khi thực thi stored procedure.", ex);
            }
        }

    }
}
