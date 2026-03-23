using S4_Client.Models;

namespace S4_Client.Forms;

public class ProductDialog : Form
{
    public Product? Result { get; private set; }

    private Label lblName;
    private TextBox txtName;
    private Label lblDescription;
    private TextBox txtDescription;
    private Label lblPrice;
    private TextBox txtPrice;
    private Label lblStock;
    private TextBox txtStock;
    private Button btnSave;
    private Button btnCancel;
    private Label lblError;

    public ProductDialog(Product? product)
    {
        this.Text = product == null ? "Добавить товар" : "Редактировать товар";
        this.Size = new Size(380, 300);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        lblName = new Label();
        lblName.Text = "Название:";
        lblName.Location = new Point(20, 20);
        lblName.Size = new Size(100, 20);

        txtName = new TextBox();
        txtName.Location = new Point(130, 18);
        txtName.Size = new Size(220, 25);
        txtName.Text = product?.Name ?? "";

        lblDescription = new Label();
        lblDescription.Text = "Описание:";
        lblDescription.Location = new Point(20, 55);
        lblDescription.Size = new Size(100, 20);

        txtDescription = new TextBox();
        txtDescription.Location = new Point(130, 53);
        txtDescription.Size = new Size(220, 25);
        txtDescription.Text = product?.Description ?? "";

        lblPrice = new Label();
        lblPrice.Text = "Цена:";
        lblPrice.Location = new Point(20, 90);
        lblPrice.Size = new Size(100, 20);

        txtPrice = new TextBox();
        txtPrice.Location = new Point(130, 88);
        txtPrice.Size = new Size(220, 25);
        txtPrice.Text = product?.Price.ToString("F2") ?? "";

        lblStock = new Label();
        lblStock.Text = "Остаток:";
        lblStock.Location = new Point(20, 125);
        lblStock.Size = new Size(100, 20);

        txtStock = new TextBox();
        txtStock.Location = new Point(130, 123);
        txtStock.Size = new Size(220, 25);
        txtStock.Text = product?.Stock.ToString() ?? "";

        btnSave = new Button();
        btnSave.Text = "Сохранить";
        btnSave.Location = new Point(60, 175);
        btnSave.Size = new Size(110, 32);
        btnSave.Click += BtnSave_Click;

        btnCancel = new Button();
        btnCancel.Text = "Отмена";
        btnCancel.Location = new Point(185, 175);
        btnCancel.Size = new Size(110, 32);
        btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

        lblError = new Label();
        lblError.Text = "";
        lblError.ForeColor = Color.Red;
        lblError.Location = new Point(20, 215);
        lblError.Size = new Size(330, 20);

        this.Controls.Add(lblName);
        this.Controls.Add(txtName);
        this.Controls.Add(lblDescription);
        this.Controls.Add(txtDescription);
        this.Controls.Add(lblPrice);
        this.Controls.Add(txtPrice);
        this.Controls.Add(lblStock);
        this.Controls.Add(txtStock);
        this.Controls.Add(btnSave);
        this.Controls.Add(btnCancel);
        this.Controls.Add(lblError);
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        lblError.Text = "";

        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            lblError.Text = "Введите название товара";
            return;
        }

        if (!decimal.TryParse(txtPrice.Text.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal price) || price < 0)
        {
            lblError.Text = "Некорректная цена";
            return;
        }

        if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
        {
            lblError.Text = "Некорректный остаток";
            return;
        }

        Result = new Product
        {
            Name = txtName.Text.Trim(),
            Description = txtDescription.Text.Trim(),
            Price = price,
            Stock = stock
        };

        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}
