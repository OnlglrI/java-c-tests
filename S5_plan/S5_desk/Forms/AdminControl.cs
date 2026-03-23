using S5_desk.Models;
using S5_desk.Services;

namespace S5_desk.Forms;

public class AdminControl : UserControl
{
    private readonly ApiService _api = new ApiService();

    private TabControl tabControl;
    private TabPage tabUsers;
    private TabPage tabOrders;

    // Users tab
    private DataGridView dgvUsers;
    private Button btnToggleActive;
    private Button btnRefreshUsers;

    // Orders tab
    private DataGridView dgvOrders;
    private ComboBox cmbStatusFilter;
    private Button btnChangeStatus;
    private Button btnRefreshOrders;

    private List<Order> _allOrders = new List<Order>();

    public AdminControl()
    {
        this.Dock = DockStyle.Fill;

        tabControl = new TabControl();
        tabControl.Dock = DockStyle.Fill;

        tabUsers = new TabPage("Пользователи");
        tabOrders = new TabPage("Все заказы");

        BuildUsersTab();
        BuildOrdersTab();

        tabControl.TabPages.Add(tabUsers);
        tabControl.TabPages.Add(tabOrders);

        this.Controls.Add(tabControl);

        LoadUsers();
        LoadOrders();
    }

    private void BuildUsersTab()
    {
        dgvUsers = new DataGridView();
        dgvUsers.Location = new Point(5, 5);
        dgvUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvUsers.Size = new Size(tabUsers.Width - 10, tabUsers.Height - 60);
        dgvUsers.ReadOnly = true;
        dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUsers.MultiSelect = false;
        dgvUsers.AllowUserToAddRows = false;
        dgvUsers.AllowUserToDeleteRows = false;
        dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        var pnlUserButtons = new Panel();
        pnlUserButtons.Dock = DockStyle.Bottom;
        pnlUserButtons.Height = 45;

        btnToggleActive = new Button();
        btnToggleActive.Text = "Активировать/Деактивировать";
        btnToggleActive.Location = new Point(5, 8);
        btnToggleActive.Size = new Size(220, 30);
        btnToggleActive.Click += BtnToggleActive_Click;

        btnRefreshUsers = new Button();
        btnRefreshUsers.Text = "Обновить";
        btnRefreshUsers.Location = new Point(235, 8);
        btnRefreshUsers.Size = new Size(110, 30);
        btnRefreshUsers.Click += (s, e) => LoadUsers();

        pnlUserButtons.Controls.Add(btnToggleActive);
        pnlUserButtons.Controls.Add(btnRefreshUsers);

        tabUsers.Controls.Add(dgvUsers);
        tabUsers.Controls.Add(pnlUserButtons);
    }

    private void BuildOrdersTab()
    {
        var lblFilter = new Label();
        lblFilter.Text = "Фильтр по статусу:";
        lblFilter.Location = new Point(5, 10);
        lblFilter.Size = new Size(130, 22);

        cmbStatusFilter = new ComboBox();
        cmbStatusFilter.Location = new Point(140, 7);
        cmbStatusFilter.Size = new Size(160, 25);
        cmbStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatusFilter.Items.AddRange(new string[] { "ALL", "PENDING", "CONFIRMED", "SHIPPED", "DELIVERED", "CANCELLED" });
        cmbStatusFilter.SelectedIndex = 0;
        cmbStatusFilter.SelectedIndexChanged += (s, e) => ApplyOrderFilter();

        dgvOrders = new DataGridView();
        dgvOrders.Location = new Point(5, 38);
        dgvOrders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvOrders.Size = new Size(tabOrders.Width - 10, tabOrders.Height - 95);
        dgvOrders.ReadOnly = true;
        dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvOrders.MultiSelect = false;
        dgvOrders.AllowUserToAddRows = false;
        dgvOrders.AllowUserToDeleteRows = false;
        dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        var pnlOrderButtons = new Panel();
        pnlOrderButtons.Dock = DockStyle.Bottom;
        pnlOrderButtons.Height = 45;

        btnChangeStatus = new Button();
        btnChangeStatus.Text = "Изменить статус";
        btnChangeStatus.Location = new Point(5, 8);
        btnChangeStatus.Size = new Size(150, 30);
        btnChangeStatus.Click += BtnChangeStatus_Click;

        btnRefreshOrders = new Button();
        btnRefreshOrders.Text = "Обновить";
        btnRefreshOrders.Location = new Point(165, 8);
        btnRefreshOrders.Size = new Size(110, 30);
        btnRefreshOrders.Click += (s, e) => LoadOrders();

        pnlOrderButtons.Controls.Add(btnChangeStatus);
        pnlOrderButtons.Controls.Add(btnRefreshOrders);

        tabOrders.Controls.Add(lblFilter);
        tabOrders.Controls.Add(cmbStatusFilter);
        tabOrders.Controls.Add(dgvOrders);
        tabOrders.Controls.Add(pnlOrderButtons);
    }

    private void LoadUsers()
    {
        var users = _api.GetAdminUsers();
        dgvUsers.Columns.Clear();
        dgvUsers.Rows.Clear();

        dgvUsers.Columns.Add("Id", "Id");
        dgvUsers.Columns.Add("Username", "Имя");
        dgvUsers.Columns.Add("Email", "Email");
        dgvUsers.Columns.Add("Role", "Роль");
        dgvUsers.Columns.Add("Active", "Активен");

        dgvUsers.Columns["Id"].Visible = false;

        foreach (var u in users)
        {
            dgvUsers.Rows.Add(u.Id, u.Username, u.Email, u.Role, u.Active ? "Да" : "Нет");
        }
    }

    private void LoadOrders()
    {
        _allOrders = _api.GetAllOrders();
        ApplyOrderFilter();
    }

    private void ApplyOrderFilter()
    {
        dgvOrders.Columns.Clear();
        dgvOrders.Rows.Clear();

        dgvOrders.Columns.Add("Id", "Id");
        dgvOrders.Columns.Add("UserId", "Пользователь Id");
        dgvOrders.Columns.Add("ProductName", "Товар");
        dgvOrders.Columns.Add("Quantity", "Кол-во");
        dgvOrders.Columns.Add("TotalPrice", "Сумма");
        dgvOrders.Columns.Add("Status", "Статус");

        dgvOrders.Columns["Id"].Visible = false;

        string filter = cmbStatusFilter.SelectedItem?.ToString() ?? "ALL";
        var filtered = filter == "ALL"
            ? _allOrders
            : _allOrders.Where(o => o.Status == filter).ToList();

        foreach (var o in filtered)
        {
            dgvOrders.Rows.Add(o.Id, o.UserId, o.ProductName, o.Quantity, o.TotalPrice.ToString("F2"), o.Status);
        }
    }

    private void BtnToggleActive_Click(object? sender, EventArgs e)
    {
        if (dgvUsers.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите пользователя", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        int id = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["Id"].Value);
        _api.ToggleUserActive(id);
        LoadUsers();
    }

    private void BtnChangeStatus_Click(object? sender, EventArgs e)
    {
        if (dgvOrders.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите заказ", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int id = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["Id"].Value);
        string newStatus = ShowInputDialog(
            "Введите новый статус (PENDING, CONFIRMED, SHIPPED, DELIVERED, CANCELLED):",
            "Изменить статус", "CONFIRMED");

        if (!string.IsNullOrWhiteSpace(newStatus))
        {
            _api.UpdateOrderStatus(id, newStatus.Trim().ToUpper());
            LoadOrders();
        }
    }

    private string ShowInputDialog(string prompt, string title, string defaultValue)
    {
        Form inputForm = new Form();
        inputForm.Text = title;
        inputForm.Size = new Size(420, 160);
        inputForm.StartPosition = FormStartPosition.CenterParent;
        inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
        inputForm.MaximizeBox = false;
        inputForm.MinimizeBox = false;

        Label lbl = new Label();
        lbl.Text = prompt;
        lbl.Location = new Point(12, 12);
        lbl.Size = new Size(390, 40);
        lbl.AutoSize = false;

        TextBox txt = new TextBox();
        txt.Text = defaultValue;
        txt.Location = new Point(12, 58);
        txt.Size = new Size(280, 25);

        Button btnOk = new Button();
        btnOk.Text = "OK";
        btnOk.Location = new Point(300, 55);
        btnOk.Size = new Size(90, 28);
        btnOk.DialogResult = DialogResult.OK;

        inputForm.Controls.Add(lbl);
        inputForm.Controls.Add(txt);
        inputForm.Controls.Add(btnOk);
        inputForm.AcceptButton = btnOk;

        return inputForm.ShowDialog() == DialogResult.OK ? txt.Text : "";
    }
}
