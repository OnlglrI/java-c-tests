using S4_Client.Models;
using S4_Client.Services;

namespace S4_Client.Forms;

public class ProductsControl : UserControl
{
    private readonly ApiService _api = new ApiService();

    private Label lblTitle;
    private DataGridView dgvProducts;
    private Panel pnlButtons;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnRefresh;

    public ProductsControl()
    {
        this.Dock = DockStyle.Fill;

        lblTitle = new Label();
        lblTitle.Text = "Товары";
        lblTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblTitle.Location = new Point(10, 10);
        lblTitle.Size = new Size(200, 30);

        dgvProducts = new DataGridView();
        dgvProducts.Location = new Point(10, 50);
        dgvProducts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvProducts.Size = new Size(this.Width - 20, this.Height - 110);
        dgvProducts.ReadOnly = true;
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProducts.MultiSelect = false;
        dgvProducts.AllowUserToAddRows = false;
        dgvProducts.AllowUserToDeleteRows = false;
        dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

        pnlButtons = new Panel();
        pnlButtons.Dock = DockStyle.Bottom;
        pnlButtons.Height = 50;
        pnlButtons.Padding = new Padding(5);

        btnAdd = new Button();
        btnAdd.Text = "Добавить";
        btnAdd.Location = new Point(10, 10);
        btnAdd.Size = new Size(110, 30);
        btnAdd.Click += BtnAdd_Click;

        btnEdit = new Button();
        btnEdit.Text = "Редактировать";
        btnEdit.Location = new Point(130, 10);
        btnEdit.Size = new Size(130, 30);
        btnEdit.Click += BtnEdit_Click;

        btnDelete = new Button();
        btnDelete.Text = "Удалить";
        btnDelete.Location = new Point(270, 10);
        btnDelete.Size = new Size(110, 30);
        btnDelete.Click += BtnDelete_Click;

        btnRefresh = new Button();
        btnRefresh.Text = "Обновить";
        btnRefresh.Location = new Point(390, 10);
        btnRefresh.Size = new Size(110, 30);
        btnRefresh.Click += (s, e) => LoadData();

        bool isAdmin = AppState.Role == "ADMIN";
        btnAdd.Visible = isAdmin;
        btnEdit.Visible = isAdmin;
        btnDelete.Visible = isAdmin;

        pnlButtons.Controls.Add(btnAdd);
        pnlButtons.Controls.Add(btnEdit);
        pnlButtons.Controls.Add(btnDelete);
        pnlButtons.Controls.Add(btnRefresh);

        this.Controls.Add(lblTitle);
        this.Controls.Add(dgvProducts);
        this.Controls.Add(pnlButtons);

        LoadData();
    }

    public void LoadData()
    {
        var products = _api.GetProducts();
        dgvProducts.Columns.Clear();
        dgvProducts.Rows.Clear();

        dgvProducts.Columns.Add("Id", "Id");
        dgvProducts.Columns.Add("Name", "Название");
        dgvProducts.Columns.Add("Description", "Описание");
        dgvProducts.Columns.Add("Price", "Цена");
        dgvProducts.Columns.Add("Stock", "Остаток");

        dgvProducts.Columns["Id"].Visible = false;

        foreach (var p in products)
        {
            dgvProducts.Rows.Add(p.Id, p.Name, p.Description, p.Price.ToString("F2"), p.Stock);
        }
    }

    private Product? GetSelectedProduct()
    {
        if (dgvProducts.SelectedRows.Count == 0) return null;
        var row = dgvProducts.SelectedRows[0];
        return new Product
        {
            Id = Convert.ToInt32(row.Cells["Id"].Value),
            Name = row.Cells["Name"].Value?.ToString() ?? "",
            Description = row.Cells["Description"].Value?.ToString() ?? "",
            Price = decimal.TryParse(row.Cells["Price"].Value?.ToString(), out var price) ? price : 0,
            Stock = Convert.ToInt32(row.Cells["Stock"].Value)
        };
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var dialog = new ProductDialog(null);
        if (dialog.ShowDialog() == DialogResult.OK && dialog.Result != null)
        {
            _api.CreateProduct(dialog.Result);
            LoadData();
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        var product = GetSelectedProduct();
        if (product == null)
        {
            MessageBox.Show("Выберите товар для редактирования", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var dialog = new ProductDialog(product);
        if (dialog.ShowDialog() == DialogResult.OK && dialog.Result != null)
        {
            dialog.Result.Id = product.Id;
            _api.UpdateProduct(dialog.Result);
            LoadData();
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        var product = GetSelectedProduct();
        if (product == null)
        {
            MessageBox.Show("Выберите товар для удаления", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show($"Удалить товар \"{product.Name}\"?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm == DialogResult.Yes)
        {
            _api.DeleteProduct(product.Id);
            LoadData();
        }
    }
}
