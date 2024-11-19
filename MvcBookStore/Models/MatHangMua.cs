
using MvcBookStore.Models;
using System.Linq;
public class MatHangMua
{
    QLBANSACH2Entities2 db = new QLBANSACH2Entities2();
    public int Masach { get; set; }
    public string TenSach { get; set; }
    public string AnhBia { get; set; }
    public double DonGia { get; set; }
    public int SoLuong { get; set; }
    //Tỉnh thành tiền = DongGia * SoLuong
    public double ThanhTien()
    {
        return SoLuong * DonGia;
    }
    public MatHangMua(int MaSach)
    {
        this.Masach = MaSach;
        //Tìm sách trong CSDL có mã id cần và gán cho mặt hàng được mua
        var sach = db.SACHes.Single(s => s.Masach == this.Masach);
        this.TenSach = sach.Tensach;
        this.AnhBia = sach.Hinhminhoa;
        this.DonGia = double.Parse(sach.Donggia.ToString());
        this.SoLuong = 1; //Số lượng mua ban đầu của một mặt hàng là 1 (cho lần click đầu)
    }
}