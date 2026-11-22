using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ПрактикаРМПО
{
    public partial class Form4 : Form
    {
        string conn = "Server=localhost;Database=administration;user=root;password=1111";

        public Form4()
        {
            InitializeComponent();

            label1.Text = "Пользователь:" + Program.user;

            LoadAppeals();

            if (Program.role == 2)
            {
                button3.Visible = false;
            }
        }

        private void LoadAppeals()
        {
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = @"SELECT a.id AS 'Регистрационный номер',
                a.register_date AS 'Дата регистрации',
                CONCAT(ap.surname, ' ', ap.first_name, ' ', ap.patronymic) AS 'ФИО заявителя',
                t.topic_name AS 'Тема обращения',
                s.name AS 'Статус',
                a.deadline AS 'Контрольный срок'

                FROM Appeals a

                JOIN Applicants ap
                ON a.applicant_id = ap.id

                JOIN Topics t
                ON a.topic_id = t.id

                JOIN Statuses s
                ON a.status_id = s.id

                ORDER BY a.id ASC";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                HighlightDeadlines();
            }
        }

        private void HighlightDeadlines()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                DateTime deadline = Convert.ToDateTime(
                    row.Cells["Контрольный срок"].Value);

                int days = (deadline.Date - DateTime.Today).Days;

                if (days < 0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                }
                else if (days <= 3)
                {
                    row.DefaultCellStyle.BackColor = Color.Khaki;
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int id = Convert.ToInt32(
                dataGridView1.Rows[e.RowIndex]
                .Cells["Регистрационный номер"].Value);

            Form5 form = new Form5(id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadAppeals();
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            HighlightDeadlines();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.manager_admin_appeal = 1;

            this.Hide();
            Form3 appeal = new Form3();
            appeal.Show();

            appeal.Text = "Добавление обращения";
            appeal.Controls["button1"].Text = "Добавить";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 login = new Form1();
            login.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите обращение для редактирования!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Регистрационный номер"].Value);

            Form6 edit = new Form6(id);
            if (edit.ShowDialog() == DialogResult.OK)
            {
                LoadAppeals();
            }
        }
    }
}