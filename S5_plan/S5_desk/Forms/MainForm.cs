namespace S5_desk.Forms;

public class MainForm : Form
{
    private Panel pnlSidebar;
    private Panel pnlContent;
    private Label lblUser;
    private Button btnProducts;
    private Button btnOrders;
    private Button btnAdmin;
    private Button btnStats;
    private Button btnLogout;

    public MainForm()
    {
        this.Text = "Магазин";
        this.Size = new Size(900, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(700, 450);

        // Sidebar
        pnlSidebar = new Panel();
        pnlSidebar.Width = 140;
        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.BackColor = Color.FromArgb(45, 45, 48);

        lblUser = new Label();
        lblUser.Text = $"Привет,\n{AppState.Username}";
        lblUser.ForeColor = Color.White;
        lblUser.Location = new Point(5, 15);
        lblUser.Size = new Size(130, 45);
        lblUser.TextAlign = ContentAlignment.MiddleCenter;
        lblUser.Font = new Font("Segoe UI", 9f);

        btnProducts = new Button();
        btnProducts.Text = "Товары";
        btnProducts.Location = new Point(10, 70);
        btnProducts.Size = new Size(120, 36);
        btnProducts.FlatStyle = FlatStyle.Flat;
        btnProducts.ForeColor = Color.White;
        btnProducts.BackColor = Color.FromArgb(62, 62, 66);
        btnProducts.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnProducts.Click += (s, e) => ShowProducts();

        btnOrders = new Button();
        btnOrders.Text = "Заказы";
        btnOrders.Location = new Point(10, 115);
        btnOrders.Size = new Size(120, 36);
        btnOrders.FlatStyle = FlatStyle.Flat;
        btnOrders.ForeColor = Color.White;
        btnOrders.BackColor = Color.FromArgb(62, 62, 66);
        btnOrders.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnOrders.Click += (s, e) => ShowOrders();

        bool isAdmin = AppState.Role == "ADMIN";

        btnAdmin = new Button();
        btnAdmin.Text = "Администратор";
        btnAdmin.Location = new Point(10, 160);
        btnAdmin.Size = new Size(120, 36);
        btnAdmin.FlatStyle = FlatStyle.Flat;
        btnAdmin.ForeColor = Color.White;
        btnAdmin.BackColor = Color.FromArgb(62, 62, 66);
        btnAdmin.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnAdmin.Visible = isAdmin;
        btnAdmin.Click += (s, e) => ShowAdmin();

        btnStats = new Button();
        btnStats.Text = "Статистика";
        btnStats.Location = new Point(10, 205);
        btnStats.Size = new Size(120, 36);
        btnStats.FlatStyle = FlatStyle.Flat;
        btnStats.ForeColor = Color.White;
        btnStats.BackColor = Color.FromArgb(62, 62, 66);
        btnStats.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnStats.Visible = isAdmin;
        btnStats.Click += (s, e) => ShowStats();

        btnLogout = new Button();
        btnLogout.Text = "Выйти";
        btnLogout.Dock = DockStyle.Bottom;
        btnLogout.Height = 36;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.ForeColor = Color.White;
        btnLogout.BackColor = Color.FromArgb(80, 40, 40);
        btnLogout.FlatAppearance.BorderColor = Color.FromArgb(100, 50, 50);
        btnLogout.Click += BtnLogout_Click;

        pnlSidebar.Controls.Add(lblUser);
        pnlSidebar.Controls.Add(btnProducts);
        pnlSidebar.Controls.Add(btnOrders);
        pnlSidebar.Controls.Add(btnAdmin);
        pnlSidebar.Controls.Add(btnStats);
        pnlSidebar.Controls.Add(btnLogout);

        // Content area
        pnlContent = new Panel();
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.BackColor = Color.White;

        this.Controls.Add(pnlContent);
        this.Controls.Add(pnlSidebar);

        ShowProducts();
    }

    private void ShowProducts()
    {
        pnlContent.Controls.Clear();
        var ctrl = new ProductsControl();
        ctrl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(ctrl);
    }

    private void ShowOrders()
    {
        pnlContent.Controls.Clear();
        var ctrl = new OrdersControl();
        ctrl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(ctrl);
    }

    private void ShowAdmin()
    {
        pnlContent.Controls.Clear();
        var ctrl = new AdminControl();
        ctrl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(ctrl);
    }

    private void ShowStats()
    {
        pnlContent.Controls.Clear();
        var ctrl = new StatsControl();
        ctrl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(ctrl);
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        AppState.Token = null;
        AppState.Username = null;
        AppState.Role = null;
        var loginForm = new LoginForm();
        loginForm.Show();
        this.Close();
    }
}
