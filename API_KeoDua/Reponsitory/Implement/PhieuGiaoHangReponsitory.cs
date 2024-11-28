using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Transactions;

namespace API_KeoDua.Reponsitory.Implement
{
    public class PhieuGiaoHangReponsitory : IPhieuGiaoHangReponsitory
    {
        private readonly PhieuGiaoHangContext phieuGiaoHangContext;
        private readonly CT_PhieuNhapContext ct_PhieuNhapContext;

        public PhieuGiaoHangReponsitory(PhieuGiaoHangContext phieuGiaoHangContext, CT_PhieuNhapContext ct_PhieuNhapContext)
        {
            this.phieuGiaoHangContext = phieuGiaoHangContext;
            this.ct_PhieuNhapContext = ct_PhieuNhapContext;
        }

        public int TotalRows { get; set; }

        public async Task<List<dynamic>> GetAllShippingNote(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRow)
        {
            try
            {
                var sqlWhere = new StringBuilder();
                sqlWhere.Append(@" WHERE (DATEDIFF(DAY, @FromDate, NgayGiao) >= 0 OR DATEDIFF(DAY, NgayTao, @ToDate) <= 0)");

                var param = new DynamicParameters();
                param.Add("@FromDate", fromDate);
                param.Add("@ToDate", toDate);

                // Lọc theo từ khóa tìm kiếm (Tên khách hàng hoặc Tên nhân viên)
                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" AND (LTRIM(RTRIM(n.TenNV)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString ");
                    sqlWhere.Append(" OR LTRIM(RTRIM(k.TenKhachHang)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString)");
                    param.Add("@SearchString", $"%{searchString}%");
                }

                // Phân trang
                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRow);

                // Tạo câu truy vấn với các điều kiện WHERE và phân trang
                string sqlQuery = $@"
            SELECT COUNT(1) 
            FROM tbl_PhieuGiaoHang p
            JOIN tbl_HoaDonBanHang h ON p.MaHoaDon = h.MaHoaDon
            JOIN tbl_KhachHang k ON h.MaKhachHang = k.MaKhachHang
            LEFT JOIN tbl_NhanVien n ON p.MaNV = n.MaNV
            {sqlWhere};

            SELECT p.*, k.TenKhachHang, n.TenNV 
            FROM tbl_PhieuGiaoHang p
            JOIN tbl_HoaDonBanHang h ON p.MaHoaDon = h.MaHoaDon
            JOIN tbl_KhachHang k ON h.MaKhachHang = k.MaKhachHang
            LEFT JOIN tbl_NhanVien n ON p.MaNV = n.MaNV
            {sqlWhere}
            ORDER BY p.NgayTao ASC
            OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";

                using (var connection = this.phieuGiaoHangContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        // Lấy tổng số hàng từ truy vấn đầu tiên
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();

                        // Lấy danh sách phiếu giao hàng từ truy vấn thứ hai, trả về dynamic
                        var result = await multi.ReadAsync<dynamic>();

                        // Duyệt qua kết quả và ánh xạ vào đối tượng dynamic
                        foreach (var item in result)
                        {
                            string tenKhachHang = item.TenKhachHang;
                            string tenNV = item.TenNV;
                        }
                        return result.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching shipping notes", ex);
            }
        }

        public async Task<List<dynamic>> GetAllShippingNoteNewCreated(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRow)
        {
            try
            {
                var sqlWhere = new StringBuilder();
                sqlWhere.Append(@" WHERE (DATEDIFF(DAY, @FromDate, NgayGiao) >= 0 OR DATEDIFF(DAY, NgayTao, @ToDate) <= 0)");

                // Lọc theo trạng thái "Mới tạo"
                sqlWhere.Append(" AND p.TrangThai = N'Mới tạo'");

                var param = new DynamicParameters();
                param.Add("@FromDate", fromDate);
                param.Add("@ToDate", toDate);

                // Lọc theo từ khóa tìm kiếm (Tên khách hàng)
                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" AND (LTRIM(RTRIM(k.TenKhachHang)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString)");
                    param.Add("@SearchString", $"%{searchString}%");
                }

                // Phân trang
                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRow);

                // Tạo câu truy vấn với các điều kiện WHERE và phân trang
                string sqlQuery = $@"
    SELECT COUNT(1) 
    FROM tbl_PhieuGiaoHang p
    JOIN tbl_HoaDonBanHang h ON p.MaHoaDon = h.MaHoaDon
    JOIN tbl_KhachHang k ON h.MaKhachHang = k.MaKhachHang
    LEFT JOIN tbl_NhanVien n ON p.MaNV = n.MaNV
    {sqlWhere};

    SELECT p.*, k.TenKhachHang
    FROM tbl_PhieuGiaoHang p
    JOIN tbl_HoaDonBanHang h ON p.MaHoaDon = h.MaHoaDon
    JOIN tbl_KhachHang k ON h.MaKhachHang = k.MaKhachHang
    LEFT JOIN tbl_NhanVien n ON p.MaNV = n.MaNV
    {sqlWhere}
    ORDER BY p.NgayTao ASC
    OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";

                using (var connection = this.phieuGiaoHangContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        // Lấy tổng số hàng từ truy vấn đầu tiên
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();

                        // Lấy danh sách phiếu giao hàng từ truy vấn thứ hai, trả về dynamic
                        var result = await multi.ReadAsync<dynamic>();

                        // Duyệt qua kết quả và ánh xạ vào đối tượng dynamic (Không cần ánh xạ TenNV nữa)
                        foreach (var item in result)
                        {
                            string tenKhachHang = item.TenKhachHang;
                            // Không cần ánh xạ TenNV
                        }

                        return result.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching shipping notes", ex);
            }
        }

        public async Task<dynamic> GetAllShippingNoteByID(Guid? maPhieuGiao)
        {
            try
            {
                var sqlWhere = new StringBuilder();
                sqlWhere.Append(" WHERE p.MaPhieuGiao = @MaPhieuGiao");

                var param = new DynamicParameters();
                param.Add("@MaPhieuGiao", maPhieuGiao);

                string sqlQuery = $@"
                                    SELECT 
                                        p.*, 
                                        n.TenNV, n.MaNV,
                                        h.MaHoaDon, h.NgayBan, h.TongTriGia, h.GhiChu, h.MaHinhThuc,
                                        k.MaKhachHang, k.TenKhachHang, k.MaLoaiKH,
                                        t.MaThongTin, t.SoNha, t.SDT, t.MaPhuong, t.MaQuan, t.ThanhPho, t.DiaChi, t.MacDinh
                                    FROM tbl_PhieuGiaoHang p
                                    LEFT JOIN tbl_HoaDonBanHang h ON p.MaHoaDon = h.MaHoaDon
                                    LEFT JOIN tbl_KhachHang k ON h.MaKhachHang = k.MaKhachHang
                                    LEFT JOIN tbl_NhanVien n ON p.MaNV = n.MaNV
                                    LEFT JOIN tbl_ThongTinGiaoHang t ON k.MaKhachHang = t.MaKhachHang
                                    {sqlWhere} ORDER BY t.MacDinh DESC";

                using (var connection = this.phieuGiaoHangContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        var result = await multi.ReadAsync<dynamic>();

                        // Nhóm thông tin giao hàng vào một đối tượng duy nhất
                        var groupedResult = result
                            .GroupBy(r => r.MaPhieuGiao)
                            .Select(g => new
                            {
                                MaPhieuGiao = g.First().MaPhieuGiao,
                                NgayTao = g.First().NgayTao,
                                NgayGiao = g.First().NgayGiao,
                                TrangThai = g.First().TrangThai,
                                MaNV = g.First().MaNV,
                                TenNV = g.First().TenNV,
                                MaHoaDon = g.First().MaHoaDon,
                                NgayBan = g.First().NgayBan,
                                TongTriGia = g.First().TongTriGia,
                                MaHinhThuc = g.First().MaHinhThuc,
                                GhiChu = g.First().GhiChu,
                                MaKhachHang = g.First().MaKhachHang,
                                TenKhachHang = g.First().TenKhachHang,
                                SDT = g.First().SDT,
                                ThongTinGiaoHang = g.Select(i => new
                                {
                                    i.MaThongTin,
                                    i.SDT,
                                    i.DiaChi,
                                    i.MacDinh,
                                }).ToList() // Mảng thông tin giao hàng
                            })
                            .FirstOrDefault(); // Lấy một kết quả duy nhất

                        return groupedResult; // Trả về phieuGiaoHang duy nhất
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching shipping note by ID", ex);
            }
        }

        public async Task<bool> UpdateShippingNote(PhieuGiaoHang phieuGiaoHang)
        {
            try
            {
                this.phieuGiaoHangContext.Entry(phieuGiaoHang).State = EntityState.Modified;
                await this.phieuGiaoHangContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<bool> AddShippingNote(PhieuGiaoHang phieuGiaoHang)
        {
            try
            {
                await this.phieuGiaoHangContext.tbl_PhieuGiaoHang.AddAsync(phieuGiaoHang);
                await this.phieuGiaoHangContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<bool> DeleteShippingNote(Guid maPhieuGiao)
        //{
        //    try
        //    {
        //        var phieuGiaoHang = await this.phieuGiaoHangContext.tbl_PhieuGiaoHang.FirstOrDefaultAsync(x => x.MaPhieuGiao == maPhieuGiao);
        //        if(phieuGiaoHang == null) { return false; }
        //        this.phieuGiaoHangContext.tbl_PhieuGiaoHang.Remove(phieuGiaoHang);
        //        await this.phieuGiaoHangContext.SaveChangesAsync();
        //        return true;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<bool> DeleteShippingNote(Guid maPhieuGiao)
        {
            try
            {
                var sqlQuery = "DELETE FROM tbl_PhieuGiaoHang WHERE MaPhieuGiao = @maPhieuGiao";
                var result = await this.phieuGiaoHangContext.Database.ExecuteSqlRawAsync(sqlQuery, new SqlParameter("@maPhieuGiao", maPhieuGiao));

                // Kiểm tra số lượng bản ghi bị xóa
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }

}
