using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace SubsApp
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                string name = textBox1.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Введите название.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                string address = textBox2.Text.Trim();
                if (string.IsNullOrEmpty(address))
                {
                    MessageBox.Show("Введите адресс.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (maskedTextBox1.Text == null)
                {
                    MessageBox.Show("Введите контактный номер", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                string email = textBox3.Text.Trim();
                if (string.IsNullOrEmpty(email))
                {
                    MessageBox.Show("Введите электронный почтовый адрес", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; database=Subs; user Id=postgres; password=admin"))
                {
                    connection.Open();

                    string query = "INSERT INTO public.\"Publishers\" (\"Name\", \"Address\", \"Contact\", \"Email\") " +
                                   "VALUES (@Name, @Address, @Contact, @Email)";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Address", address);
                        command.Parameters.AddWithValue("@Contact", maskedTextBox1.Text);
                        command.Parameters.AddWithValue("@Email", email);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Новый издатель добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения издателя: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
