using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_64130005.Models
{
    public class GioHang_64130005
    {
        Project_64130005Entities1 db = new Project_64130005Entities1();
        public string MaSach { set; get; }
        public string TenSach { set; get; }
        public string AnhSach { set; get; }
        public int DonGia { set; get; }
        public int SoLuong { set; get; }
        public string TheLoai { set; get; }
        public int ThanhTien
        {
            get { return SoLuong * DonGia; }
        }
        public GioHang_64130005(string MaSach)
        {
            this.MaSach = MaSach;
            Sach sach = db.Saches.Single(n => n.MaSach == this.MaSach);
            TenSach = sach.TenSach;
            AnhSach = sach.AnhSach;
            DonGia = int.Parse(sach.DonGia.ToString());
            SoLuong = 1;
            TheLoai = sach.TheLoai.TenLoai;
        }
    }
}