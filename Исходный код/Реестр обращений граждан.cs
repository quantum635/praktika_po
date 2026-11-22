using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ПрактикаРМПО
{
    public partial class Form2 : Form
    {
        string conn = "Server=localhost;Database=administration;user=root;password=1111";
        DataTable catalogTable;
        int ascending = -1;

        public Form2()
        {
            InitializeComponent();

            LoadCatalog();

            label1.Text = "Пользователь: " + Program.user;

            comboBox1.Items.Add("Все категории");

            foreach (DataRow row in catalogTable.Rows)
            {
                string section = row["Раздел"].ToString();

                if (!comboBox1.Items.Contains(section))
                    comboBox1.Items.Add(section);
            }

            comboBox1.SelectedIndex = 0;

            switch (Program.role)
            {
                case 1:
                    UserOff();
                    break;
                case 2:
                    ManagerOff();
                    break;
            }
        }

        private void LoadCatalog()
        {
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = @"SELECT s.name as 'Раздел',
                t.topic_name as 'Тема обращения',
                d.name as 'Ответственный отдел',
                CONCAT(e.surname, ' ', e.first_name, ' ', e.patronymic) as 'Исполнитель',
                t.deadline_days as 'Нормативный срок рассмотрения',
                (
                SELECT COUNT(*) FROM Appeals a
                JOIN Topics t2 ON a.topic_id = t2.id
                WHERE t2.executor_id = t.executor_id AND a.status_id = 2
                ) AS 'Текущая нагрузка',
                t.image_path as 'Путь'
                
                FROM Topics t

                JOIN Sections s
                ON t.section_id = s.id

                JOIN Departments d
                ON t.department_id = d.id

                JOIN Executors e
                ON t.executor_id = e.id";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dt.Columns.Add("Изображение", typeof(Image));
                foreach (DataRow row in dt.Rows)
                {
                    string path = row["Путь"].ToString();
                    row["Изображение"] = LoadImage(path);
                }

                catalogTable = dt;
                dataGridView1.DataSource = catalogTable;

                dataGridView1.Columns["Изображение"].DisplayIndex = 0;
                dataGridView1.Columns["Путь"].Visible = false;
            }
        }

        private Image LoadImage(string path)
        {
            string fullPath = Path.Combine(Application.StartupPath, path);

            if (File.Exists(fullPath))
            {
                return Image.FromFile(fullPath);
            }

            return Properties.Resources.no_image;
        }

        private void UpdateCounter()
        {
            int shown = catalogTable.DefaultView.Count;
            int all = catalogTable.Rows.Count;

            label2.Text = $"Показано {shown} из {all} записей";
        }

        private void ApplyFilter()
        {
            List<string> filters = new List<string>();

            string search = textBox1.Text.Trim();
            search = search.Replace("'", "''");

            //Поиск
            if (search != "")
            {
                filters.Add(
                    $"([Раздел] LIKE '%{search}%' OR " +
                    $"[Тема обращения] LIKE '%{search}%' OR " +
                    $"[Ответственный отдел] LIKE '%{search}%' OR " +
                    $"[Исполнитель] LIKE '%{search}%')");
            }

            //Фильтр
            if (comboBox1.Text != "Все категории")
            {
                string category = comboBox1.Text.Replace("'", "''");

                filters.Add(
                    $"[Раздел] = '{category}'");
            }

            catalogTable.DefaultView.RowFilter =
                string.Join(" AND ", filters);

            UpdateCounter();
        }

        public void GuestOff()
        {
            label2.Visible = false; label3.Visible = false; label4.Visible = false;
            button2.Visible = false;
            textBox1.Visible = false;
            comboBox1.Visible = false;
            button3.Visible = false;
            button4.Visible = false;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        public void UserOff()
        {
            button4.Visible = false;
        }

        public void ManagerOff()
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 login = new Form1();
            login.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (ascending)
            {
                case -1:
                    catalogTable.DefaultView.Sort = "Текущая нагрузка ASC";
                    button2.Text = "↑Нагрузка";
                    ascending = 1;
                    break;
                case 1:
                    catalogTable.DefaultView.Sort = "Текущая нагрузка DESC";
                    button2.Text = "↓Нагрузка";
                    ascending = 2;
                    break;
                case 2:
                    catalogTable.DefaultView.Sort = "";
                    button2.Text = "Без сортировки";
                    ascending = -1;
                    break;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 appeal = new Form3();
            appeal.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 appeals = new Form4();
            appeals.Show();
        }
    }
}