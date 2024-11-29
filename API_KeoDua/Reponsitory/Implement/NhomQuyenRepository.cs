using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<NhomQuyen>> GetNhomQuyenByTenNV(string tenNV)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                string sqlQuery;

                // Kiểm tra nếu tenNV là null hoặc rỗng
                if (string.IsNullOrEmpty(tenNV))
                {
                    sqlQuery = @" SELECT MaNhomQuyen, TenNhomQuyen FROM tbl_NhomQuyen nq WITH (NOLOCK)";
                }
                else
                {
                    sqlQuery = @"SELECT nq.MaNhomQuyen,  nq.TenNhomQuyen
                    FROM tbl_NhomQuyen nq
                    JOIN tbl_TaiKhoan tk ON tk.MaNhomQuyen = nq.MaNhomQuyen
                    JOIN tbl_NhanVien nv ON tk.TenTaiKhoan = nv.TenTaiKhoan
                    WHERE nv.TenNV = @TenNV";
                    param.Add("@TenNV", tenNV);
                }

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

        public async Task<List<Quyen>> GetQuyenByTenNV(string tenNV)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                string sqlQuery;

                // Nếu TenNV là null hoặc rỗng, lấy tất cả quyền
                if (string.IsNullOrEmpty(tenNV))
                {
                    sqlQuery = @"SELECT MaQuyen, TenQuyen FROM tbl_Quyen q WITH (NOLOCK)";
                }
                else
                {
                    // Truy vấn lấy quyền theo tên nhân viên (TenNV)
                    sqlQuery = @"
                    SELECT q.MaQuyen, q.TenQuyen
                    FROM tbl_Quyen q
                    JOIN tbl_CapQuyen cq ON q.MaQuyen = cq.MaQuyen
                    JOIN tbl_NhomQuyen nq ON cq.MaNhomQuyen = nq.MaNhomQuyen
                    JOIN tbl_TaiKhoan tk ON nq.MaNhomQuyen = tk.MaNhomQuyen
                    JOIN tbl_NhanVien nv ON tk.TenTaiKhoan = nv.TenTaiKhoan
                    WHERE nv.TenNV = @TenNV";

                    param.Add("@TenNV", tenNV);
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

    }
}
