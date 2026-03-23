using S6_desk.Styles;

namespace S6_desk.Forms;

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
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;

    public MainForm()
    {
        this.Text = "Магазин";
        this.Size = new Size(900, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(700, 450);

        AppTheme.Apply(this);

        // Status strip
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        lblStatus.Text = "Готово";
        statusStrip.Items.Add(lblStatus);
        statusStrip.Dock = DockStyle.Bottom;

        // Sidebar
        pnlSidebar = new Panel();
        pnlSidebar.Width = 140;
        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.BackColor = AppTheme.SidebarColor;

        lblUser = new Label();
        lblUser.Text = $"Привет,\n{AppState.Username}";
        lblUser.ForeColor = Color.White;
        lblUser.Location = new Point(5, 15);
        lblUser.Size = new Size(130, 45);
        lblUser.TextAlign = ContentAlignment.MiddleCenter;
        lblUser.Font = AppTheme.DefaultFont;

        btnProducts = new Button();
        btnProducts.Text = "Товары";
        btnProducts.Location = new Point(10, 70);
        btnProducts.Size = new Size(120, 36);
        AppTheme.StyleButton(btnProducts);
        btnProducts.Click += (s, e) => ShowProducts();

        btnOrders = new Button();
        btnOrders.Text = "Заказы";
        btnOrders.Location = new Point(10, 115);
        btnOrders.Size = new Size(120, 36);
        AppTheme.StyleButton(btnOrders);
        btnOrders.Click += (s, e) => ShowOrders();

        bool isAdmin = AppState.Role == "ADMIN";

        btnAdmin = new Button();
        btnAdmin.Text = "Администратор";
        btnAdmin.Location = new Point(10, 160);
        btnAdmin.Size = new Size(120, 36);
        AppTheme.StyleButton(btnAdmin);
        btnAdmin.Visible = isAdmin;
        btnAdmin.Click += (s, e) => ShowAdmin();

        btnStats = new Button();
        btnStats.Text = "Статистика";
        btnStats.Location = new Point(10, 205);
        btnStats.Size = new Size(120, 36);
        AppTheme.StyleButton(btnStats);
        btnStats.Visible = isAdmin;
        btnStats.Click += (s, e) => ShowStats();

        btnLogout = new Button();
        btnLogout.Text = "Выйти";
        btnLogout.Dock = DockStyle.Bottom;
        btnLogout.Height = 36;
        AppTheme.StyleButton(btnLogout, false);
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
        pnlContent.BackColor = AppTheme.BackgroundColor;

        this.Controls.Add(pnlContent);
        this.Controls.Add(pnlSidebar);
        this.Controls.Add(statusStrip);

        ShowProducts();
    }

    public void SetStatus(string message)
    {
        lblStatus.Text = message;
    }

    private void ShowProducts()
    {
        pnlContent.Controls.Clear();
        var ctrl = new ProductsControl(this);
        ctrl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(ctrl);
    }

    private void ShowOrders()
    {
        pnlContent.Controls.Clear();
        var ctrl = new OrdersControl(this);
        ctrl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(ctrl);
    }

    private void ShowAdmin()
    {
        pnlContent.Controls.Clear();
        var ctrl = new AdminControl(this);
        ctrl.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(ctrl);
    }

    private void ShowStats()
    {
        pnlContent.Controls.Clear();
        var ctrl = new StatsControl(this);
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
