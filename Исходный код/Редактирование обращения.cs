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
    public partial class Form6 : Form
    {
        string conn = "Server=localhost;Database=administration;user=root;password=1111";

        private int appealId;

        public Form6(int id)
        {
            InitializeComponent();

            label8.Text = "Пользователь: " + Program.user;

            appealId = id;

            LoadTopics();
            LoadStatuses();
            LoadAppeal();
        }

        private void LoadAppeal()
        {
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = @"SELECT a.register_date,
                t.topic_name,
                a.description,
                s.name AS status,
                a.response_date

                FROM Appeals a

                JOIN Topics t
                ON a.topic_id = t.id

                JOIN Statuses s
                ON a.status_id = s.id

                WHERE a.id=@id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", appealId);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    dateTimePicker2.Value = Convert.ToDateTime(reader["register_date"]);
                    comboBox1.Text = reader["topic_name"].ToString();
                    textBox2.Text = reader["description"].ToString();
                    comboBox2.Text = reader["status"].ToString();

                    if (reader["response_date"] != DBNull.Value)
                    {
                        dateTimePicker1.Value = Convert.ToDateTime(reader["response_date"]);
                    }
                }
            }
        }

        private void LoadTopics()
        {
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = "SELECT topic_name FROM Topics";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["topic_name"].ToString());
                }
            }
        }

        private void LoadStatuses()
        {
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = "SELECT name FROM Statuses";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    comboBox2.Items.Add(reader["name"].ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите сохранить изменения?", "Подтверждение сохранения", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = @"UPDATE Appeals
                SET register_date=@date,
                topic_id=(SELECT id FROM Topics WHERE topic_name=@topic),
                description=@description,
                status_id=(SELECT id FROM Statuses WHERE name=@status),
                response_date=@response

                WHERE id=@id";

                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@date", dateTimePicker2.Value);
                cmd.Parameters.AddWithValue("@topic", comboBox1.Text);
                cmd.Parameters.AddWithValue("@description", textBox2.Text);
                cmd.Parameters.AddWithValue("@status", comboBox2.Text);
                cmd.Parameters.AddWithValue("@id", appealId);

                if (dateTimePicker1.Checked)
                {
                    cmd.Parameters.AddWithValue("@response", dateTimePicker1.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@response", DBNull.Value);
                }

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Обращение изменено", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
