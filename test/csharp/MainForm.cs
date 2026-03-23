using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TestApp;

public class MainForm : Form
{
    private readonly HttpClient _http = new();
    private Label _lblResult = new();
    private Button _btnTest = new();
    private TextBox _txtUrl = new();

    public MainForm()
    {
        Text = "Import Test";
        Width = 500;
        Height = 250;

        _txtUrl.Text = "http://localhost:9090/api/hello";
        _txtUrl.Width = 350;
        _txtUrl.Location = new System.Drawing.Point(20, 20);

        _btnTest.Text = "Test GET";
        _btnTest.Location = new System.Drawing.Point(380, 18);
        _btnTest.Click += async (s, e) => await OnTest();

        _lblResult.Location = new System.Drawing.Point(20, 70);
        _lblResult.Size = new System.Drawing.Size(440, 100);
        _lblResult.Text = "нажми Test GET";

        Controls.Add(_txtUrl);
        Controls.Add(_btnTest);
        Controls.Add(_lblResult);
    }

    private async Task OnTest()
    {
        try
        {
            var response = await _http.GetStringAsync(_txtUrl.Text);
            var obj = JsonConvert.DeserializeObject(response);
            _lblResult.Text = JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        catch (Exception ex)
        {
            _lblResult.Text = "Ошибка: " + ex.Message;
        }
    }

    private void SetAuthHeader(string token)
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}
