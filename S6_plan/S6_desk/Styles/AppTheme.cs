namespace S6_desk.Styles;
public static class AppTheme
{
    public static readonly Color PrimaryColor = Color.FromArgb(37, 99, 235);
    public static readonly Color DangerColor = Color.FromArgb(220, 38, 38);
    public static readonly Color SidebarColor = Color.FromArgb(30, 30, 35);
    public static readonly Color BackgroundColor = Color.FromArgb(248, 250, 252);
    public static readonly Font DefaultFont = new Font("Segoe UI", 9f);
    public static readonly Font TitleFont = new Font("Segoe UI", 14f, FontStyle.Bold);

    public static void StyleButton(Button btn, bool isPrimary = true)
    {
        btn.BackColor = isPrimary ? PrimaryColor : DangerColor;
        btn.ForeColor = Color.White;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font = DefaultFont;
        btn.Cursor = Cursors.Hand;
    }

    public static void StyleDataGrid(DataGridView grid)
    {
        grid.BackgroundColor = BackgroundColor;
        grid.BorderStyle = BorderStyle.None;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
        grid.DefaultCellStyle.Font = DefaultFont;
        grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        grid.EnableHeadersVisualStyles = false;
    }

    public static void Apply(Form form)
    {
        form.BackColor = BackgroundColor;
        form.Font = DefaultFont;
    }
}
