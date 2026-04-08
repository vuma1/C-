namespace QuanLyQuanCafe.Models
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public Table(int id, string name, string status)
        {
            Id = id;
            Name = name;
            Status = status;
        }

        public override string ToString() => Name; // Để hiển thị tên trên nút
    }
}
