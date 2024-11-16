﻿using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface INhanVienReponsitory
    {
        public int TotalRows { get; set; }

        public Task<List<NhanVien>> GetAllEmployee(string searchString, int startRow, int maxRows);
        public Task AddEmployee(NhanVienTaiKhoan nhanVienTaiKhoan);
        public Task<NhanVienTaiKhoan> GetEmployeeByID(Guid MaNV);
        public Task DeleteEmployee (Guid MaNV);
        public Task UpdateEmployee(Guid MaNV, NhanVien updatedEmployee, TaiKhoan updatedTaiKhoan);
    } 
}