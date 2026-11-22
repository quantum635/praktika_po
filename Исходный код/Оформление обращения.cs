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
    public partial class Form3 : Form
    {
        string conn = "Server=localhost;Database=administration;user=root;password=1111";
        Dictionary<string, int> topics = new Dictionary<string, int>();
        int selectedExecutorId;

        public Form3()
        {
            InitializeComponent();

            LoadTopics();

            label2.Text = DateTime.Now.ToString("dd.MM.yyyy");
            label1.Text = "Регистрационный номер: " + GetNextNumber().ToString();

            label8.Text = "Пользователь: " + Program.user;
        }

        private void LoadTopics()
        {
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = @"SELECT t.id,
                t.topic_name,
                CONCAT(e.surname,' ',e.first_name,' ',e.patronymic) AS executor,
                e.id AS executor_id

                FROM Topics t
                
                JOIN Executors e
                ON t.executor_id = e.id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string topic = reader["topic_name"].ToString();
                    comboBox1.Items.Add(topic);
                    topics.Add(topic, Convert.ToInt32(reader["executor_id"]));
                }
            }
        }

        private int GetNextNumber()
        {
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = "SELECT IFNULL(MAX(id),0)+1 FROM Appeals";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private int GetTopicId()
        {
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = "SELECT id FROM Topics WHERE topic_name=@name";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", comboBox1.Text);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private DateTime CalculateDeadline(DateTime date)
        {
            DateTime deadline = date.AddDays(30);

            while (deadline.DayOfWeek == DayOfWeek.Saturday || deadline.DayOfWeek == DayOfWeek.Sunday)
            {
                deadline = deadline.AddDays(-1);
            }

            return deadline;
        }

        private int GetApplicantId()
        {
            string[] fio = textBox1.Text.Trim().Split(' ');

            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string checkQuery = @"SELECT id FROM Applicants
                WHERE surname=@surname
                AND first_name=@first
                AND patronymic=@patronymic";

                MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);

                checkCmd.Parameters.AddWithValue("@surname", fio[0]);
                checkCmd.Parameters.AddWithValue("@first", fio[1]);
                checkCmd.Parameters.AddWithValue("@patronymic", fio[2]);

                object result = checkCmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }

                string insertQuery = @"INSERT INTO Applicants(surname, first_name, patronymic, address)
                VALUES(@surname, @first, @patronymic, @address);
                
                SELECT LAST_INSERT_ID();";

                MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);

                insertCmd.Parameters.AddWithValue("@surname", fio[0]);
                insertCmd.Parameters.AddWithValue("@first", fio[1]);
                insertCmd.Parameters.AddWithValue("@patronymic", fio[2]);
                insertCmd.Parameters.AddWithValue("@address", textBox3.Text);

                return Convert.ToInt32(insertCmd.ExecuteScalar());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string topic = comboBox1.Text;

            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();

                string query = @"SELECT CONCAT(surname, ' ', first_name,' ', patronymic)
                FROM Executors WHERE id=@id";

                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@id", topics[topic]);

                label6.Text = "Исполнитель: " + cmd.ExecuteScalar().ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(Program.manager_admin_appeal == 1)
            {
                this.Hide();
                Form4 appeals = new Form4();
                appeals.Show();
            }
            else
            {
                this.Hide();
                Form2 main = new Form2();
                main.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите тему обращения!");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите ФИО!");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Введите содержание обращения!");
                return;
            }

            DialogResult result = MessageBox.Show("Вы действительно хотите создать обращение?", "Подтверждение создания", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                using (MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    string query = @"INSERT INTO Appeals(register_date, applicant_id, topic_id, description, status_id, deadline)
                    VALUES(@date, @applicant, @topic, @description, 1, @deadline)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@applicant", GetApplicantId());
                    cmd.Parameters.AddWithValue("@topic", GetTopicId());
                    cmd.Parameters.AddWithValue("@description", textBox2.Text);
                    cmd.Parameters.AddWithValue("@deadline", CalculateDeadline(DateTime.Now));
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Обращение зарегистрировано");

                if (Program.manager_admin_appeal == 1)
                {
                    this.Hide();
                    Form4 appeals = new Form4();
                    appeals.Show();
                }
                else
                {
                    this.Hide();
                    Form2 main = new Form2();
                    main.Show();
                }
            }
            else
            {
                if (Program.manager_admin_appeal == 1)
                {
                    this.Hide();
                    Form4 appeals = new Form4();
                    appeals.Show();
                }
                else
                {
                    this.Hide();
                    Form2 main = new Form2();
                    main.Show();
                }
            }
        }
    }
}
