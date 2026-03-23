using S4_Client.Services;

namespace S4_Client.Forms;

public class LoginForm : Form
{
    private readonly ApiService _api = new ApiService();

    private Label lblTitle;
    private Label lblUsername;
    private TextBox txtUsername;
    private Label lblPassword;
    private TextBox txtPassword;
    private Button btnLogin;
    private Button btnRegister;
    private Label lblError;

    public LoginForm()
    {
        this.Text = "Вход";
        this.Size = new Size(400, 280);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        lblTitle = new Label();
        lblTitle.Text = "Магазин";
        lblTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblTitle.Location = new Point(0, 15);
        lblTitle.Size = new Size(384, 40);

        lblUsername = new Label();
        lblUsername.Text = "Имя пользователя:";
        lblUsername.Location = new Point(40, 70);
        lblUsername.Size = new Size(150, 20);

        txtUsername = new TextBox();
        txtUsername.Location = new Point(40, 92);
        txtUsername.Size = new Size(300, 25);

        lblPassword = new Label();
        lblPassword.Text = "Пароль:";
        lblPassword.Location = new Point(40, 125);
        lblPassword.Size = new Size(150, 20);

        txtPassword = new TextBox();
        txtPassword.Location = new Point(40, 147);
        txtPassword.Size = new Size(300, 25);
        txtPassword.PasswordChar = '*';

        btnLogin = new Button();
        btnLogin.Text = "Войти";
        btnLogin.Location = new Point(40, 185);
        btnLogin.Size = new Size(140, 32);
        btnLogin.Click += BtnLogin_Click;

        btnRegister = new Button();
        btnRegister.Text = "Регистрация";
        btnRegister.Location = new Point(200, 185);
        btnRegister.Size = new Size(140, 32);
        btnRegister.Click += BtnRegister_Click;

        lblError = new Label();
        lblError.Text = "";
        lblError.ForeColor = Color.Red;
        lblError.Location = new Point(40, 225);
        lblError.Size = new Size(300, 20);

        this.Controls.Add(lblTitle);
        this.Controls.Add(lblUsername);
        this.Controls.Add(txtUsername);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
        this.Controls.Add(btnRegister);
        this.Controls.Add(lblError);
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        lblError.Text = "";
        if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            lblError.Text = "Заполните все поля";
            return;
        }

        var result = _api.Login(txtUsername.Text.Trim(), txtPassword.Text);
        if (result != null && !string.IsNullOrEmpty(result.Token))
        {
            AppState.Token = result.Token;
            AppState.Username = result.Username;
            AppState.Role = result.Role;
            var mainForm = new MainForm();
            mainForm.Show();
            this.Close();
        }
        else
        {
            lblError.Text = "Неверное имя пользователя или пароль";
        }
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        var registerForm = new RegisterForm();
        registerForm.ShowDialog(this);
    }
}
