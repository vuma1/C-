using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuanLyQuanCafe.ViewModels
{
    public class TableViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        private string _tableName = "";
        private Table? _selectedTable;

        public ObservableCollection<Table> Tables { get; } = new();

        public string TableName
        {
            get => _tableName;
            set => SetProperty(ref _tableName, value);
        }

        public Table? SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (SetProperty(ref _selectedTable, value) && value != null)
                {
                    TableName = value.Name;
                }
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        public TableViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadTables());
            AddCommand = new RelayCommand(_ => AddTable());
            UpdateCommand = new RelayCommand(_ => UpdateTable(), _ => SelectedTable != null);
            DeleteCommand = new RelayCommand(_ => DeleteTable(), _ => SelectedTable != null);
            ClearCommand = new RelayCommand(_ => ClearForm());

            LoadTables();
        }

        private void LoadTables()
        {
            Tables.Clear();
            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "SELECT * FROM tables";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Tables.Add(new Table(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetString("status")));
            }
        }

        private void AddTable()
        {
            if (string.IsNullOrEmpty(TableName))
            {
                MessageBox.Show("Vui lòng nhập tên bàn!", "Thông báo");
                return;
            }

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "INSERT INTO tables (name, status) VALUES (@name, 'Trống')";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", TableName);
            cmd.ExecuteNonQuery();

            LoadTables();
            ClearForm();
            MessageBox.Show("Thêm thành công!", "Thông báo");
        }

        private void UpdateTable()
        {
            if (SelectedTable == null) return;

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "UPDATE tables SET name = @name WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", TableName);
            cmd.Parameters.AddWithValue("@id", SelectedTable.Id);
            cmd.ExecuteNonQuery();

            LoadTables();
            ClearForm();
            MessageBox.Show("Cập nhật thành công!", "Thông báo");
        }

        private void DeleteTable()
        {
            if (SelectedTable == null) return;

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "DELETE FROM tables WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", SelectedTable.Id);
            cmd.ExecuteNonQuery();

            LoadTables();
            ClearForm();
        }

        public void UpdateTableStatus(int id, string status)
        {
            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "UPDATE tables SET status = @status WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            LoadTables();
        }

        private void ClearForm()
        {
            TableName = "";
            SelectedTable = null;
        }
    }
}
