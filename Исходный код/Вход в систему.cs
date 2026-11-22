using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace ПрактикаРМПО
{
    public partial class Form1 : Form
    {
        string conn = "Server=localhost;Database=administration;user=root;password=1111";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Поля логин и пароль должны быть заполнены!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                using(MySqlConnection connection = new MySqlConnection(conn))
                {
                    connection.Open();

                    string query = "SELECT login, password_hash, salt, surname, first_name, patronymic, role_id FROM users WHERE login=@login";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@login", textBox1.Text);
                    
                    MySqlDataReader reader = cmd.ExecuteReader();

                    bool UserExists = false;
                    string password_hash = null;
                    string salt = null;
                    string fio = null;
                    int role = 0;

                    if (reader.Read())
                    {
                        UserExists = true;
                        password_hash = reader["password_hash"].ToString();
                        salt = reader["salt"].ToString();
                        fio = $"{reader["surname"].ToString()} {reader["first_name"].ToString()} {reader["patronymic"].ToString()}";
                        role = Convert.ToInt32(reader["role_id"]);
                    }

                    reader.Close();

                    if(!UserExists)
                    {
                        MessageBox.Show("Неверный логин.", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(textBox2.Text + salt))).ToLower() == password_hash)
                    {
                        Program.user = fio;
                        Program.role = role;

                        this.Hide();
                        Form2 main = new Form2();
                        main.Show();
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль.", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Form2 main = new Form2();
            main.Show();
            main.GuestOff();
        }
    }
}