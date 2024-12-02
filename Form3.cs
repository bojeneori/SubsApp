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

namespace SubsApp
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                listBox1.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (!int.TryParse(textBox3.Text, out int phoneNumber))
            {
                MessageBox.Show("Номер телефона должен содержать только цифры.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            string fio = textBox1.Text;
            string address = textBox2.Text;
            int deliveryTypeId = (int)listBox1.SelectedValue; 


            string connectionString = "server=localhost; database=Subs; user Id =postgres; password =admin ";


            string query = "INSERT INTO public.\"Clients\" (\"FIO\", \"Address\", \"PhoneNumber\", \"DelieveryTypeID\") " +
                           "VALUES (@FIO, @Address, @PhoneNumber, @DelieveryTypeID)";

            try
            {

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();


                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@FIO", fio);
                        command.Parameters.AddWithValue("@Address", address);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@DelieveryTypeID", deliveryTypeId);


                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения клиента: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            string connectionString = "server=localhost; database=Subs; user Id =postgres; password =admin ";
            string query = "SELECT id, \"DelieveryType\" FROM public.\"DeliveryMethod\"";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            listBox1.DataSource = null;
                            listBox1.Items.Clear();


                            var deliveryMethods = new List<DeliveryMethod>();

                            while (reader.Read())
                            {
                                deliveryMethods.Add(new DeliveryMethod
                                {
                                    ID = reader.GetInt32(0), 
                                    DeliveryType = reader.GetString(1) 
                                });
                            }


                            listBox1.DataSource = deliveryMethods;
                            listBox1.DisplayMember = "DeliveryType"; 
                            listBox1.ValueMember = "ID";          
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки способов доставки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public class DeliveryMethod
        {
            public int ID { get; set; }
            public string DeliveryType { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
