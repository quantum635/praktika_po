using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ПрактикаРМПО
{
    public partial class Form5 : Form
    {
        string conn = "Server=localhost;Database=administration;user=root;password=1111";
        private int appealId;

        public Form5(int appealId)
        {
            InitializeComponent();

            this.appealId = appealId;

            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = @"SELECT a.id,
                a.register_date,
                CONCAT(ap.surname,' ',ap.first_name,' ',ap.patronymic) AS applicant,
                ap.address,
                t.topic_name,
                CONCAT(e.surname,' ',e.first_name,' ',e.patronymic) AS executor,
                a.description,
                s.name AS status,
                a.deadline,
                a.response_date

                FROM Appeals a

                JOIN Applicants ap
                ON a.applicant_id = ap.id

                JOIN Topics t
                ON a.topic_id = t.id

                JOIN Executors e
                ON t.executor_id = e.id

                JOIN Statuses s
                ON a.status_id = s.id

                WHERE a.id = @id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", appealId);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    label1.Text += reader["id"];
                    label2.Text += Convert.ToDateTime(reader["register_date"]).ToString("dd.MM.yyyy");
                    label3.Text += reader["applicant"].ToString();
                    label7.Text += reader["address"].ToString();
                    label4.Text += reader["topic_name"].ToString();
                    label5.Text += reader["description"].ToString();
                    label6.Text += reader["executor"].ToString();
                    label9.Text += reader["status"].ToString();

                    if (reader["response_date"] == DBNull.Value)
                        label10.Text += "—";
                    else
                        label10.Text += Convert.ToDateTime(reader["response_date"]).ToString("dd.MM.yyyy");

                    label11.Text += Convert.ToDateTime(reader["deadline"]).ToString("dd.MM.yyyy");

                    DateTime deadline = Convert.ToDateTime(reader["deadline"]);

                    if (deadline.Date < DateTime.Today)
                    {
                        label11.ForeColor = Color.Red;
                    }
                    else if ((deadline.Date - DateTime.Today).Days <= 3)
                    {
                        label11.ForeColor = Color.DarkOrange;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите удалить обращение?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();
                    string query = "DELETE FROM Appeals WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", appealId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Обращение удалено", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}