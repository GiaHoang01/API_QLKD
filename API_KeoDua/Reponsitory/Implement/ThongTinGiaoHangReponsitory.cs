using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using Microsoft.AspNetCore.Mvc;
using API_KeoDua.Reponsitory.Interface;
using API_KeoDua.Data;
using System.Transactions;

namespace API_KeoDua.Reponsitory.Implement
{
    public class ThongTinGiaoHangReponsitory:IThongTinGiaoHangReponsitory
    {
        private readonly ThongTinGiaoHangContext thongTinGiaoHangContext;
        public ThongTinGiaoHangReponsitory(ThongTinGiaoHangContext thongTinGiaoHangContext)
        {
            this.thongTinGiaoHangContext = thongTinGiaoHangContext;
        }

        public async Task<List<ThongTinGiaoHang>> GetAllInfoDelivery(Guid customerId)
        {
            const string sqlQuery = @"
                    SELECT * 
                    FROM tbl_ThongTinGiaoHang WITH (NOLOCK)
                    WHERE MaKhachHang = @MaKhachHang
                    ORDER BY MacDinh";

            try
            {
                using var connection = this.thongTinGiaoHangContext.CreateConnection();
                var param = new { MaKhachHang = customerId };

                var deliveryInfoList = await connection.QueryAsync<ThongTinGiaoHang>(sqlQuery, param);
                return deliveryInfoList.ToList();
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý lỗi chung nếu cần
                throw new Exception($"An error occurred while retrieving delivery info for customer {customerId}.", ex);
            }
        }

        public async Task<bool> AddInfoDelivery(Guid maKhachHang, ThongTinGiaoHang thongTinGiaoHang)
        {
            const string checkQuery = @"
        SELECT COUNT(1) 
        FROM tbl_ThongTinGiaoHang 
        WHERE MaKhachHang = @MaKhachHang";

            const string sqlQuery = @"
        INSERT INTO tbl_ThongTinGiaoHang (MaThongTin, SoNha, SDT, MaKhachHang, MacDinh, ThanhPho, Phuong, Quan)
        VALUES (@MaThongTin, @SoNha, @SDT, @MaKhachHang, @MacDinh, N'Thành phố Hồ Chí Minh', @Phuong, @Quan)";

            try
            {
                using var connection = this.thongTinGiaoHangContext.CreateConnection();

                // Kiểm tra nếu MaKhachHang đã tồn tại
                bool exists = await connection.ExecuteScalarAsync<bool>(checkQuery, new { MaKhachHang = maKhachHang });

                // Gán giá trị cho MacDinh
                thongTinGiaoHang.MacDinh = exists ? false : true;

                var parameters = new
                {
                    MaThongTin = thongTinGiaoHang.MaThongTin,
                    SoNha = thongTinGiaoHang.SoNha,
                    SDT = thongTinGiaoHang.SDT,
                    MaKhachHang = maKhachHang,
                    MacDinh = thongTinGiaoHang.MacDinh,
                    Phuong = thongTinGiaoHang.Phuong,
                    Quan = thongTinGiaoHang.Quan,
                };

                // Thực hiện lệnh INSERT
                int rowsAffected = await connection.ExecuteAsync(sqlQuery, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding delivery info.", ex);
            }
        }


        public async Task<bool> DeleteInfoDelivery(Guid maKhachHang, Guid maThongTin)
        {
            const string checkCountQuery = @"
        SELECT COUNT(1) 
        FROM tbl_ThongTinGiaoHang 
        WHERE MaKhachHang = @MaKhachHang";

            const string checkDefaultQuery = @"
        SELECT MacDinh 
        FROM tbl_ThongTinGiaoHang 
        WHERE MaThongTin = @MaThongTin AND MaKhachHang = @MaKhachHang";

            const string updateDefaultQuery = @"
        UPDATE tbl_ThongTinGiaoHang
        SET MacDinh = 1
        WHERE MaKhachHang = @MaKhachHang AND MaThongTin != @MaThongTin
        AND MacDinh = 0";

            const string deleteQuery = @"
        DELETE FROM tbl_ThongTinGiaoHang 
        WHERE MaKhachHang = @MaKhachHang AND MaThongTin = @MaThongTin";

            try
            {
                using var connection = this.thongTinGiaoHangContext.CreateConnection();

                // Kiểm tra số lượng dòng dữ liệu của khách hàng
                int count = await connection.ExecuteScalarAsync<int>(checkCountQuery, new { MaKhachHang = maKhachHang });

                // Nếu khách hàng chỉ có 1 dòng, xóa bình thường
                if (count == 1)
                {
                    int rowsDeleted = await connection.ExecuteAsync(deleteQuery, new { MaKhachHang = maKhachHang, MaThongTin = maThongTin });
                    return rowsDeleted > 0;
                }

                // Kiểm tra giá trị MacDinh của dòng bị xóa
                int macDinh = await connection.ExecuteScalarAsync<int>(checkDefaultQuery, new { MaKhachHang = maKhachHang, MaThongTin = maThongTin });

                // Nếu MacDinh = 1 và khách hàng có từ 2 dòng trở lên
                if (macDinh == 1)
                {
                    // Cập nhật dòng khác thành MacDinh = 1
                    int rowsUpdated = await connection.ExecuteAsync(updateDefaultQuery, new { MaKhachHang = maKhachHang, MaThongTin = maThongTin });

                    // Nếu không có dòng nào được cập nhật (trường hợp này không nên xảy ra), trả về false
                    if (rowsUpdated == 0)
                    {
                        return false;
                    }
                }

                // Xóa dòng bị chỉ định
                int rowsAffected = await connection.ExecuteAsync(deleteQuery, new { MaKhachHang = maKhachHang, MaThongTin = maThongTin });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting delivery info.", ex);
            }
        }


        public async Task<bool> UpdateInfoDelivery(Guid maKhachHang, Guid maThongTin, ThongTinGiaoHang thongTinGiaoHang)
        {
            const string sqlQuery = @"
        UPDATE tbl_ThongTinGiaoHang
        SET 
            SoNha = @SoNha,
            SDT = @SDT,
            MacDinh = @MacDinh,
            ThanhPho = @ThanhPho,
            Phuong = @Phuong,
            Quan = @Quan
        WHERE 
            MaKhachHang = @MaKhachHang AND MaThongTin = @MaThongTin";

            try
            {
                using var connection = this.thongTinGiaoHangContext.CreateConnection();
                var parameters = new
                {
                    MaKhachHang = maKhachHang,
                    MaThongTin = maThongTin,
                    SoNha = thongTinGiaoHang.SoNha,
                    SDT = thongTinGiaoHang.SDT,
                    MacDinh = thongTinGiaoHang.MacDinh,
                    ThanhPho = thongTinGiaoHang.ThanhPho,
                    Phuong = thongTinGiaoHang.Phuong,
                    Quan = thongTinGiaoHang.Quan
                };

                int rowsAffected = await connection.ExecuteAsync(sqlQuery, parameters);

                return rowsAffected > 0; // Trả về true nếu có ít nhất một bản ghi được cập nhật
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"SQL error while updating delivery info for customer {maKhachHang} and info {maThongTin}: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating delivery info.", ex);
            }
        }


    }
}
