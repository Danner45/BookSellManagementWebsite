using Project_64130005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_64130005.Utils
{
    public class ProductSort
    {
        private static IProductSort tempSort;

        //Đoạn code sắp xếp sp theo thứ tự tăng dần Ascending
        public static IEnumerable<Sach> Ascending(IEnumerable<Sach> sachs)
        {
            //TempSort lưu trự một đối tượng
            tempSort = new SortAscending();
            return tempSort.Sort(sachs);
        }

        //Đoạn code sắp xếp sp theo thứ tự giảm dần Descending
        public static IEnumerable<Sach> Descending(IEnumerable<Sach> sachs)
        {
            tempSort = new SortDescending();
            return tempSort.Sort(sachs);
        }

        //Sắp xếp ds sản phẩm
        public interface IProductSort
        {
            IEnumerable<Sach> Sort(IEnumerable<Sach> sachs);
        }

        //SortAscending sắp xếp danh sách sản phẩm theo giá đơn vị tăng dần.
        public class SortAscending : IProductSort
        {
            public IEnumerable<Sach> Sort(IEnumerable<Sach> sachs)
            {
                return sachs.OrderBy(p => p.DonGia);
            }
        }
        //SortDescending sắp xếp danh sách sản phẩm theo giá đơn vị giảm dần.
        public class SortDescending : IProductSort
        {
            public IEnumerable<Sach> Sort(IEnumerable<Sach> sachs)
            {
                return sachs.OrderByDescending(p => p.DonGia);
            }
        }
    }
}