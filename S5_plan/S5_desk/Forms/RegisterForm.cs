using S5_desk.Services;

namespace S5_desk.Forms;

public class RegisterForm : Form
{
    private readonly ApiService _api = new ApiService();

    private Label lblTitle;
    private Label lblUsername;
    private TextBox txtUsername;
    private Label lblEmail;
    private TextBox txtEmail;
    private Label lblPassword;
    private TextBox txtPassword;
    private Label lblConfirm;
    private TextBox txtConfirm;
    private Button btnRegister;
    private Button btnBack;
    private Label lblError;

    public RegisterForm()
    {
        this.Text = "Регистрация";
        this.Size = new Size(400, 320);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        lblTitle = new Label();
        lblTitle.Text = "Регистрация";
        lblTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblTitle.Location = new Point(0, 10);
        lblTitle.Size = new Size(384, 30);

        lblUsername = new Label();
        lblUsername.Text = "Имя пользователя:";
        lblUsername.Location = new Point(40, 52);
        lblUsername.Size = new Size(150, 18);

        txtUsername = new TextBox();
        txtUsername.Location = new Point(40, 72);
        txtUsername.Size = new Size(300, 25);

        lblEmail = new Label();
        lblEmail.Text = "Email:";
        lblEmail.Location = new Point(40, 105);
        lblEmail.Size = new Size(150, 18);

        txtEmail = new TextBox();
        txtEmail.Location = new Point(40, 125);
        txtEmail.Size = new Size(300, 25);

        lblPassword = new Label();
        lblPassword.Text = "Пароль:";
        lblPassword.Location = new Point(40, 158);
        lblPassword.Size = new Size(150, 18);

        txtPassword = new TextBox();
        txtPassword.Location = new Point(40, 178);
        txtPassword.Size = new Size(300, 25);
        txtPassword.PasswordChar = '*';

        lblConfirm = new Label();
        lblConfirm.Text = "Подтвердите пароль:";
        lblConfirm.Location = new Point(40, 211);
        lblConfirm.Size = new Size(150, 18);

        txtConfirm = new TextBox();
        txtConfirm.Location = new Point(40, 231);
        txtConfirm.Size = new Size(300, 25);
        txtConfirm.PasswordChar = '*';

        btnRegister = new Button();
        btnRegister.Text = "Зарегистрироваться";
        btnRegister.Location = new Point(40, 265);
        btnRegister.Size = new Size(160, 32);
        btnRegister.Click += BtnRegister_Click;

        btnBack = new Button();
        btnBack.Text = "Назад";
        btnBack.Location = new Point(215, 265);
        btnBack.Size = new Size(120, 32);
        btnBack.Click += (s, e) => this.Close();

        lblError = new Label();
        lblError.Text = "";
        lblError.ForeColor = Color.Red;
        lblError.Location = new Point(40, 302);
        lblError.Size = new Size(300, 18);

        this.Controls.Add(lblTitle);
        this.Controls.Add(lblUsername);
        this.Controls.Add(txtUsername);
        this.Controls.Add(lblEmail);
        this.Controls.Add(txtEmail);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(lblConfirm);
        this.Controls.Add(txtConfirm);
        this.Controls.Add(btnRegister);
        this.Controls.Add(btnBack);
        this.Controls.Add(lblError);
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        lblError.Text = "";

        if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
            string.IsNullOrWhiteSpace(txtEmail.Text) ||
            string.IsNullOrWhiteSpace(txtPassword.Text) ||
            string.IsNullOrWhiteSpace(txtConfirm.Text))
        {
            lblError.Text = "Заполните все поля";
            return;
        }

        if (txtPassword.Text != txtConfirm.Text)
        {
            lblError.Text = "Пароли не совпадают";
            return;
        }

        var result = _api.Register(txtUsername.Text.Trim(), txtEmail.Text.Trim(), txtPassword.Text);
        if (result != null && !string.IsNullOrEmpty(result.Token))
        {
            MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
        else
        {
            lblError.Text = "Ошибка регистрации. Попробуйте другое имя или email.";
        }
    }
}
