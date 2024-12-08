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
            const string sqlQuery = @"
                    INSERT INTO tbl_ThongTinGiaoHang (MaThongTin, SoNha, SDT, MaKhachHang, MacDinh, ThanhPho, Phuong, Quan)
                    VALUES (@MaThongTin, @SoNha, @SDT, @MaKhachHang, @MacDinh, N'Thành phố Hồ Chí Minh', @Phuong, @Quan)";

            try
            {
                using var connection = this.thongTinGiaoHangContext.CreateConnection();
                var parameters = new
                {
                    MaThongTin = thongTinGiaoHang.MaThongTin,
                    SoNha = thongTinGiaoHang.SoNha,
                    SDT = thongTinGiaoHang.SDT,
                    maKhachHang = maKhachHang,
                    MacDinh = thongTinGiaoHang.MacDinh,
                    //ThanhPho = thongTinGiaoHang.ThanhPho,
                    Phuong = thongTinGiaoHang.Phuong,
                    Quan = thongTinGiaoHang.Quan,
                };

                int rowsAffected = await connection.ExecuteAsync(sqlQuery, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
                throw new Exception("An error occurred while adding delivery info.", ex);
            }
        }

        public async Task<bool> DeleteInfoDelivery(Guid maKhachHang, Guid maThongTin)
        {
            const string sqlQuery = @"
                DELETE FROM tbl_ThongTinGiaoHang 
                WHERE MaKhachHang = @MaKhachHang AND MaThongTin = @MaThongTin";

            try
            {
                using var connection = this.thongTinGiaoHangContext.CreateConnection();
                var parameters = new
                {
                    MaKhachHang = maKhachHang,
                    MaThongTin = maThongTin
                };

                int rowsAffected = await connection.ExecuteAsync(sqlQuery, parameters);

                return rowsAffected > 0; // Trả về true nếu có ít nhất một bản ghi bị xóa
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
