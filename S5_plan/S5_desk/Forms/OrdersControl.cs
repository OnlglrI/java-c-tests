using S5_desk.Models;
using S5_desk.Services;

namespace S5_desk.Forms;

public class OrdersControl : UserControl
{
    private readonly ApiService _api = new ApiService();

    private Label lblTitle;
    private DataGridView dgvOrders;
    private Panel pnlButtons;
    private Button btnCreate;
    private Button btnDelete;
    private Button btnRefresh;

    public OrdersControl()
    {
        this.Dock = DockStyle.Fill;

        lblTitle = new Label();
        lblTitle.Text = "Заказы";
        lblTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblTitle.Location = new Point(10, 10);
        lblTitle.Size = new Size(200, 30);

        dgvOrders = new DataGridView();
        dgvOrders.Location = new Point(10, 50);
        dgvOrders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvOrders.Size = new Size(this.Width - 20, this.Height - 110);
        dgvOrders.ReadOnly = true;
        dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvOrders.MultiSelect = false;
        dgvOrders.AllowUserToAddRows = false;
        dgvOrders.AllowUserToDeleteRows = false;
        dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        pnlButtons = new Panel();
        pnlButtons.Dock = DockStyle.Bottom;
        pnlButtons.Height = 50;
        pnlButtons.Padding = new Padding(5);

        btnCreate = new Button();
        btnCreate.Text = "Создать заказ";
        btnCreate.Location = new Point(10, 10);
        btnCreate.Size = new Size(130, 30);
        btnCreate.Click += BtnCreate_Click;

        btnDelete = new Button();
        btnDelete.Text = "Удалить";
        btnDelete.Location = new Point(150, 10);
        btnDelete.Size = new Size(110, 30);
        btnDelete.Click += BtnDelete_Click;

        btnRefresh = new Button();
        btnRefresh.Text = "Обновить";
        btnRefresh.Location = new Point(270, 10);
        btnRefresh.Size = new Size(110, 30);
        btnRefresh.Click += (s, e) => LoadData();

        pnlButtons.Controls.Add(btnCreate);
        pnlButtons.Controls.Add(btnDelete);
        pnlButtons.Controls.Add(btnRefresh);

        this.Controls.Add(lblTitle);
        this.Controls.Add(dgvOrders);
        this.Controls.Add(pnlButtons);

        LoadData();
    }

    public void LoadData()
    {
        var orders = _api.GetOrders();
        dgvOrders.Columns.Clear();
        dgvOrders.Rows.Clear();

        dgvOrders.Columns.Add("Id", "Id");
        dgvOrders.Columns.Add("ProductName", "Товар");
        dgvOrders.Columns.Add("Quantity", "Количество");
        dgvOrders.Columns.Add("TotalPrice", "Сумма");
        dgvOrders.Columns.Add("Status", "Статус");

        dgvOrders.Columns["Id"].Visible = false;

        foreach (var o in orders)
        {
            dgvOrders.Rows.Add(o.Id, o.ProductName, o.Quantity, o.TotalPrice.ToString("F2"), o.Status);
        }
    }

    private int GetSelectedOrderId()
    {
        if (dgvOrders.SelectedRows.Count == 0) return -1;
        return Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["Id"].Value);
    }

    private void BtnCreate_Click(object? sender, EventArgs e)
    {
        var dialog = new OrderDialog(_api);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            LoadData();
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        int id = GetSelectedOrderId();
        if (id == -1)
        {
            MessageBox.Show("Выберите заказ для удаления", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show("Удалить выбранный заказ?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm == DialogResult.Yes)
        {
            _api.DeleteOrder(id);
            LoadData();
        }
    }
}
