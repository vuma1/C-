using QuanLyQuanCafe.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace QuanLyQuanCafe.Utils
{
    public static class InvoicePrinter
    {
        // Đường dẫn lưu hóa đơn (Lưu ngay tại thư mục dự án)
        private static readonly string BILL_PATH = "hoadon_in.txt";

        // Phương thức mới - in hóa đơn từ danh sách OrderDetail
        public static void PrintOrderInvoice(string tableName, List<OrderDetail> items, double totalAmount)
        {
            try
            {
                // Sử dụng UTF-8 encoding để hỗ trợ tiếng Việt
                using (StreamWriter writer = new StreamWriter(BILL_PATH, false, Encoding.UTF8))
                {
                    // 1. Header Hóa Đơn (dùng ký tự ASCII để tương thích)
                    writer.WriteLine("+========================================+");
                    writer.WriteLine("|       HOA DON THANH TOAN               |");
                    writer.WriteLine("|       COFFEE SHOP C# WPF               |");
                    writer.WriteLine("+========================================+");

                    // Ngày giờ hiện tại
                    string dateTimeStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    writer.WriteLine($"  Ngay: {dateTimeStr}");
                    writer.WriteLine($"  Ban: {tableName}");
                    writer.WriteLine("+----------------------------------------+");
                    writer.WriteLine(string.Format("  {0,-18} {1,5} {2,12}", "Mon", "SL", "Thanh tien"));
                    writer.WriteLine("------------------------------------------");

                    // 2. Danh sách món
                    foreach (var item in items)
                    {
                        string productName = Truncate(RemoveVietnameseAccents(item.ProductName), 18);
                        string line = string.Format("  {0,-18} {1,5} {2,12:N0}",
                            productName,
                            item.Quantity,
                            item.Total);
                        writer.WriteLine(line);
                    }

                    writer.WriteLine("------------------------------------------");

                    // 3. Tổng tiền
                    writer.WriteLine(string.Format("  TONG CONG:         {0,15:N0} VND", totalAmount));
                    writer.WriteLine("+========================================+");
                    writer.WriteLine("|    Cam on quy khach va hen gap lai!   |");
                    writer.WriteLine("|    Wifi: CoffeeShop / Pass: 12345     |");
                    writer.WriteLine("+========================================+");
                }

                // 4. Tự động mở file hóa đơn (Notepad)
                Process.Start(new ProcessStartInfo
                {
                    FileName = BILL_PATH,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi in hóa đơn: {ex.Message}");
            }
        }

        // Helper: Cắt ngắn tên món nếu quá dài
        private static string Truncate(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            return str.Length > maxLength ? str.Substring(0, maxLength - 2) + ".." : str;
        }

        // Helper: Bỏ dấu tiếng Việt
        private static string RemoveVietnameseAccents(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            
            str = Regex.Replace(str, "[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            str = Regex.Replace(str, "[ÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴ]", "A");
            str = Regex.Replace(str, "[èéẹẻẽêềếệểễ]", "e");
            str = Regex.Replace(str, "[ÈÉẸẺẼÊỀẾỆỂỄ]", "E");
            str = Regex.Replace(str, "[ìíịỉĩ]", "i");
            str = Regex.Replace(str, "[ÌÍỊỈĨ]", "I");
            str = Regex.Replace(str, "[òóọỏõôồốộổỗơờớợởỡ]", "o");
            str = Regex.Replace(str, "[ÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠ]", "O");
            str = Regex.Replace(str, "[ùúụủũưừứựửữ]", "u");
            str = Regex.Replace(str, "[ÙÚỤỦŨƯỪỨỰỬỮ]", "U");
            str = Regex.Replace(str, "[ỳýỵỷỹ]", "y");
            str = Regex.Replace(str, "[ỲÝỴỶỸ]", "Y");
            str = Regex.Replace(str, "đ", "d");
            str = Regex.Replace(str, "Đ", "D");
            
            return str;
        }
    }
}
