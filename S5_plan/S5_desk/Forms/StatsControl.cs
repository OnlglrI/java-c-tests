using S5_desk.Services;

namespace S5_desk.Forms;

public class StatsControl : UserControl
{
    private readonly ApiService _api = new ApiService();

    private Label lblTitle;
    private FlowLayoutPanel pnlCards;
    private Panel cardUsers;
    private Panel cardProducts;
    private Panel cardOrders;
    private Panel cardRevenue;
    private Label lblUsers;
    private Label lblProducts;
    private Label lblOrders;
    private Label lblRevenue;
    private DataGridView dgvTopProducts;
    private Button btnRefresh;

    public StatsControl()
    {
        this.Dock = DockStyle.Fill;

        lblTitle = new Label();
        lblTitle.Text = "Статистика";
        lblTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblTitle.Location = new Point(10, 10);
        lblTitle.Size = new Size(200, 30);

        // Cards panel
        pnlCards = new FlowLayoutPanel();
        pnlCards.Location = new Point(10, 50);
        pnlCards.Size = new Size(760, 100);
        pnlCards.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlCards.FlowDirection = FlowDirection.LeftToRight;
        pnlCards.WrapContents = false;

        cardUsers = CreateCard(out lblUsers, "Пользователей: -");
        cardProducts = CreateCard(out lblProducts, "Товаров: -");
        cardOrders = CreateCard(out lblOrders, "Заказов: -");
        cardRevenue = CreateCard(out lblRevenue, "Выручка: - руб.");

        pnlCards.Controls.Add(cardUsers);
        pnlCards.Controls.Add(cardProducts);
        pnlCards.Controls.Add(cardOrders);
        pnlCards.Controls.Add(cardRevenue);

        var lblTop = new Label();
        lblTop.Text = "Топ товаров по заказам:";
        lblTop.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        lblTop.Location = new Point(10, 160);
        lblTop.Size = new Size(250, 22);

        dgvTopProducts = new DataGridView();
        dgvTopProducts.Location = new Point(10, 188);
        dgvTopProducts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvTopProducts.Size = new Size(760, 250);
        dgvTopProducts.ReadOnly = true;
        dgvTopProducts.AllowUserToAddRows = false;
        dgvTopProducts.AllowUserToDeleteRows = false;
        dgvTopProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        btnRefresh = new Button();
        btnRefresh.Text = "Обновить";
        btnRefresh.Dock = DockStyle.Bottom;
        btnRefresh.Height = 36;
        btnRefresh.Click += (s, e) => LoadData();

        this.Controls.Add(lblTitle);
        this.Controls.Add(pnlCards);
        this.Controls.Add(lblTop);
        this.Controls.Add(dgvTopProducts);
        this.Controls.Add(btnRefresh);

        LoadData();
    }

    private Panel CreateCard(out Label label, string initialText)
    {
        var card = new Panel();
        card.Size = new Size(180, 90);
        card.BackColor = Color.White;
        card.BorderStyle = BorderStyle.Fixed3D;
        card.Margin = new Padding(5);

        label = new Label();
        label.Text = initialText;
        label.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleCenter;

        card.Controls.Add(label);
        return card;
    }

    public void LoadData()
    {
        var stats = _api.GetStats();
        if (stats.Count > 0)
        {
            lblUsers.Text = $"Пользователей:\n{stats.GetValueOrDefault("totalUsers", "-")}";
            lblProducts.Text = $"Товаров:\n{stats.GetValueOrDefault("totalProducts", "-")}";
            lblOrders.Text = $"Заказов:\n{stats.GetValueOrDefault("totalOrders", "-")}";
            lblRevenue.Text = $"Выручка:\n{stats.GetValueOrDefault("totalRevenue", "-")} руб.";
        }

        var topProducts = _api.GetTopProducts();
        dgvTopProducts.Columns.Clear();
        dgvTopProducts.Rows.Clear();
        dgvTopProducts.Columns.Add("ProductName", "Товар");
        dgvTopProducts.Columns.Add("OrderCount", "Количество заказов");

        foreach (var item in topProducts)
        {
            string name = item.GetValueOrDefault("productName", "");
            string count = item.GetValueOrDefault("orderCount", "");
            dgvTopProducts.Rows.Add(name, count);
        }
    }
}
