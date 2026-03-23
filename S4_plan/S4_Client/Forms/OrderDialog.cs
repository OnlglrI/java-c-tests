using S4_Client.Models;
using S4_Client.Services;

namespace S4_Client.Forms;

public class OrderDialog : Form
{
    private readonly ApiService _api;
    private List<Product> _products = new List<Product>();

    private Label lblProduct;
    private ComboBox cmbProduct;
    private Label lblQuantity;
    private NumericUpDown nudQuantity;
    private Label lblTotal;
    private Button btnOrder;
    private Button btnCancel;
    private Label lblError;

    public OrderDialog(ApiService api)
    {
        _api = api;

        this.Text = "Создать заказ";
        this.Size = new Size(360, 240);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        lblProduct = new Label();
        lblProduct.Text = "Товар:";
        lblProduct.Location = new Point(20, 25);
        lblProduct.Size = new Size(90, 20);

        cmbProduct = new ComboBox();
        cmbProduct.Location = new Point(120, 22);
        cmbProduct.Size = new Size(210, 25);
        cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;

        lblQuantity = new Label();
        lblQuantity.Text = "Количество:";
        lblQuantity.Location = new Point(20, 65);
        lblQuantity.Size = new Size(90, 20);

        nudQuantity = new NumericUpDown();
        nudQuantity.Location = new Point(120, 62);
        nudQuantity.Size = new Size(120, 25);
        nudQuantity.Minimum = 1;
        nudQuantity.Maximum = 999;
        nudQuantity.Value = 1;
        nudQuantity.ValueChanged += NudQuantity_ValueChanged;

        lblTotal = new Label();
        lblTotal.Text = "Итого: 0.00 руб.";
        lblTotal.Location = new Point(20, 105);
        lblTotal.Size = new Size(300, 20);
        lblTotal.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

        btnOrder = new Button();
        btnOrder.Text = "Оформить";
        btnOrder.Location = new Point(50, 140);
        btnOrder.Size = new Size(110, 32);
        btnOrder.Click += BtnOrder_Click;

        btnCancel = new Button();
        btnCancel.Text = "Отмена";
        btnCancel.Location = new Point(175, 140);
        btnCancel.Size = new Size(110, 32);
        btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

        lblError = new Label();
        lblError.Text = "";
        lblError.ForeColor = Color.Red;
        lblError.Location = new Point(20, 178);
        lblError.Size = new Size(310, 18);

        this.Controls.Add(lblProduct);
        this.Controls.Add(cmbProduct);
        this.Controls.Add(lblQuantity);
        this.Controls.Add(nudQuantity);
        this.Controls.Add(lblTotal);
        this.Controls.Add(btnOrder);
        this.Controls.Add(btnCancel);
        this.Controls.Add(lblError);

        LoadProducts();
    }

    private void LoadProducts()
    {
        _products = _api.GetProducts();
        cmbProduct.Items.Clear();
        foreach (var p in _products)
        {
            cmbProduct.Items.Add(new ProductItem(p));
        }
        if (cmbProduct.Items.Count > 0)
            cmbProduct.SelectedIndex = 0;
    }

    private void UpdateTotal()
    {
        if (cmbProduct.SelectedItem is ProductItem item)
        {
            decimal total = item.Product.Price * (decimal)nudQuantity.Value;
            lblTotal.Text = $"Итого: {total:F2} руб.";
        }
        else
        {
            lblTotal.Text = "Итого: 0.00 руб.";
        }
    }

    private void CmbProduct_SelectedIndexChanged(object? sender, EventArgs e) => UpdateTotal();
    private void NudQuantity_ValueChanged(object? sender, EventArgs e) => UpdateTotal();

    private void BtnOrder_Click(object? sender, EventArgs e)
    {
        lblError.Text = "";

        if (cmbProduct.SelectedItem is not ProductItem item)
        {
            lblError.Text = "Выберите товар";
            return;
        }

        int productId = item.Product.Id;
        int quantity = (int)nudQuantity.Value;

        var result = _api.CreateOrder(productId, quantity);
        if (result != null)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        else
        {
            lblError.Text = "Ошибка при создании заказа";
        }
    }

    private class ProductItem
    {
        public Product Product { get; }
        public ProductItem(Product p) { Product = p; }
        public override string ToString() => $"{Product.Name} — {Product.Price:F2} руб.";
    }
}
