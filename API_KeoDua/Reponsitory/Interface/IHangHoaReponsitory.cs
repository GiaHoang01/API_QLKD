﻿using API_KeoDua.Data;
using API_KeoDua.DataView;

namespace API_KeoDua.Reponsitory.Interface
{
    public interface IHangHoaReponsitory
    {
        public int TotalRows { get; set; }
        public Task<List<HangHoa>> GetAllProduct(string searchString, int startRow, int maxRows);
        public Task AddProduct(HangHoa newProduct, decimal giaBan);
    }
}
